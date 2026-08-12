namespace HomePal.Domain.Entities;

public class Household
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Governorate { get; set; }
    public string? City { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Pantry? Pantry { get; set; }
    public ShoppingList? ShoppingList { get; set; }
    public ICollection<HouseholdMember> Members { get; set; } = new List<HouseholdMember>();
    public ICollection<HouseholdInvitation> Invitations { get; set; } = new List<HouseholdInvitation>();
    public ICollection<HouseholdMonthlyBudget> Budgets { get; set; } = new List<HouseholdMonthlyBudget>();
    public ICollection<HouseholdExpense> Expenses { get; set; } = new List<HouseholdExpense>();
}
