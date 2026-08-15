using HomePal.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HomePal.Persistence.Interceptors;

public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            ApplyAuditAndSoftDeleteInfo(eventData.Context, isAsync: false).GetAwaiter().GetResult();
        }
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            await ApplyAuditAndSoftDeleteInfo(eventData.Context, isAsync: true, cancellationToken);
        }
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static async Task ApplyAuditAndSoftDeleteInfo(
        DbContext context,
        bool isAsync,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var deletedAuditableEntries = context.ChangeTracker.Entries<BaseAuditableEntity>()
            .Where(e => e.State == EntityState.Deleted)
            .ToList();

        if (deletedAuditableEntries.Count > 0)
        {
            var processedEntries = new HashSet<EntityEntry>();
            var queue = new Queue<EntityEntry<BaseAuditableEntity>>(deletedAuditableEntries);

            while (queue.Count > 0)
            {
                var entry = queue.Dequeue();
                if (!processedEntries.Add(entry))
                    continue;

                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = now;

                // Process referencing foreign keys for both tracked & untracked database entities
                foreach (var fk in entry.Metadata.GetReferencingForeignKeys())
                {
                    if (fk.DeclaringEntityType.IsOwned())
                        continue;

                    var principalProperties = fk.PrincipalKey.Properties;
                    var principalValues = principalProperties.Select(p => entry.Property(p.Name).CurrentValue).ToArray();
                    if (principalValues.Any(v => v == null))
                        continue;

                    var principalId = principalValues[0];
                    var dependentTableName = fk.DeclaringEntityType.GetTableName();
                    var dependentSchema = fk.DeclaringEntityType.GetSchema() ?? "dbo";
                    if (string.IsNullOrWhiteSpace(dependentTableName))
                        continue;

                    var fkProperty = fk.Properties.First();
                    var fkColumnName = fkProperty.GetColumnName(StoreObjectIdentifier.Table(dependentTableName, dependentSchema)) 
                        ?? fkProperty.Name;

                    var isDependentAuditable = typeof(BaseAuditableEntity).IsAssignableFrom(fk.DeclaringEntityType.ClrType);

                    switch (fk.DeleteBehavior)
                    {
                        case DeleteBehavior.Restrict:
                        case DeleteBehavior.ClientNoAction:
                        case DeleteBehavior.NoAction:
                            var hasTrackedActive = context.ChangeTracker.Entries()
                                .Where(d => fk.DeclaringEntityType.IsAssignableFrom(d.Metadata) && d.State != EntityState.Deleted && (d.Entity is not BaseAuditableEntity a || !a.IsDeleted))
                                .Any(d => Equals(d.Property(fkProperty.Name).CurrentValue, principalId));

                            if (!hasTrackedActive)
                            {
                                var checkSql = isDependentAuditable
                                    ? $"SELECT TOP 1 1 AS Value FROM [{dependentSchema}].[{dependentTableName}] WHERE [{fkColumnName}] = {{0}} AND [IsDeleted] = 0"
                                    : $"SELECT TOP 1 1 AS Value FROM [{dependentSchema}].[{dependentTableName}] WHERE [{fkColumnName}] = {{0}}";

                                var query = context.Database.SqlQueryRaw<int>(checkSql, principalId!);
                                var count = isAsync ? (await query.ToListAsync(cancellationToken)).Count : query.ToList().Count;
                                if (count > 0)
                                    hasTrackedActive = true;
                            }

                            if (hasTrackedActive)
                            {
                                throw new InvalidOperationException(
                                    $"The entity of type '{entry.Metadata.DisplayName()}' cannot be deleted because it is referenced by an active '{fk.DeclaringEntityType.DisplayName()}' with a '{fk.DeleteBehavior}' delete behavior.");
                            }
                            break;

                        case DeleteBehavior.Cascade:
                        case DeleteBehavior.ClientCascade:
                            if (isDependentAuditable)
                            {
                                // Direct database bulk update for untracked rows
                                var cascadeSql = $"UPDATE [{dependentSchema}].[{dependentTableName}] SET [IsDeleted] = 1, [DeletedAt] = {{0}} WHERE [{fkColumnName}] = {{1}} AND [IsDeleted] = 0";
                                if (isAsync)
                                    await context.Database.ExecuteSqlRawAsync(cascadeSql, new object[] { now, principalId! }, cancellationToken);
                                else
                                    context.Database.ExecuteSqlRaw(cascadeSql, now, principalId!);
                            }

                            // Sync any in-memory tracked dependents
                            var trackedCascadeDependents = context.ChangeTracker.Entries()
                                .Where(d => fk.DeclaringEntityType.IsAssignableFrom(d.Metadata) && d.State != EntityState.Detached)
                                .Where(d => Equals(d.Property(fkProperty.Name).CurrentValue, principalId))
                                .ToList();

                            foreach (var dep in trackedCascadeDependents)
                            {
                                if (dep.Entity is BaseAuditableEntity audDep && !audDep.IsDeleted)
                                {
                                    if (dep is EntityEntry<BaseAuditableEntity> typedDep)
                                    {
                                        queue.Enqueue(typedDep);
                                    }
                                    else
                                    {
                                        dep.State = EntityState.Modified;
                                        audDep.IsDeleted = true;
                                        audDep.DeletedAt = now;
                                    }
                                }
                            }
                            break;

                        case DeleteBehavior.SetNull:
                        case DeleteBehavior.ClientSetNull:
                            if (fkProperty.IsNullable)
                            {
                                // Direct database bulk update to nullify foreign keys on untracked rows
                                var setNullSql = $"UPDATE [{dependentSchema}].[{dependentTableName}] SET [{fkColumnName}] = NULL WHERE [{fkColumnName}] = {{0}}";
                                if (isAsync)
                                    await context.Database.ExecuteSqlRawAsync(setNullSql, new object[] { principalId! }, cancellationToken);
                                else
                                    context.Database.ExecuteSqlRaw(setNullSql, principalId!);

                                // Sync in-memory tracked dependents
                                var trackedSetNullDependents = context.ChangeTracker.Entries()
                                    .Where(d => fk.DeclaringEntityType.IsAssignableFrom(d.Metadata) && d.State != EntityState.Detached)
                                    .Where(d => Equals(d.Property(fkProperty.Name).CurrentValue, principalId))
                                    .ToList();

                                foreach (var dep in trackedSetNullDependents)
                                {
                                    if (dep.State != EntityState.Deleted && (dep.Entity is not BaseAuditableEntity aud || !aud.IsDeleted))
                                    {
                                        dep.Property(fkProperty.Name).CurrentValue = null;
                                    }
                                }
                            }
                            break;
                    }
                }
            }

            // Reset owned entities (such as JSON columns / LocalizedItem collections)
            // that EF Core marked as Deleted during cascade deletion back to Unchanged.
            foreach (var ownedEntry in context.ChangeTracker.Entries().Where(e => e.Metadata.IsOwned() && e.State == EntityState.Deleted))
            {
                ownedEntry.State = EntityState.Unchanged;
            }
        }

        // Standard CreatedAt / UpdatedAt audit timestamps
        foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.IsDeleted = false;
                    break;
                case EntityState.Modified:
                    if (!entry.Entity.IsDeleted)
                    {
                        entry.Entity.UpdatedAt = now;
                    }
                    break;
            }
        }
    }
}
