using Microsoft.AspNetCore.Identity;

namespace HomePal.Api.Factories;

public class LocalizedIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DuplicateEmail(string email)
        => new() { Code = nameof(DuplicateEmail), Description = "Identity.DuplicateEmail" };

    public override IdentityError DuplicateUserName(string userName)
        => new() { Code = nameof(DuplicateUserName), Description = "Identity.DuplicateUserName" };

    public override IdentityError PasswordTooShort(int length)
        => new() { Code = nameof(PasswordTooShort), Description = "Identity.PasswordTooShort" };

    public override IdentityError PasswordRequiresNonAlphanumeric()
        => new() { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Identity.PasswordRequiresNonAlphanumeric" };

    public override IdentityError PasswordRequiresDigit()
        => new() { Code = nameof(PasswordRequiresDigit), Description = "Identity.PasswordRequiresDigit" };

    public override IdentityError PasswordRequiresLower()
        => new() { Code = nameof(PasswordRequiresLower), Description = "Identity.PasswordRequiresLower" };

    public override IdentityError PasswordRequiresUpper()
        => new() { Code = nameof(PasswordRequiresUpper), Description = "Identity.PasswordRequiresUpper" };

    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        => new() { Code = nameof(PasswordRequiresUniqueChars), Description = "Identity.PasswordRequiresUniqueChars" };

    public override IdentityError InvalidToken()
        => new() { Code = nameof(InvalidToken), Description = "Identity.InvalidToken" };

    public override IdentityError PasswordMismatch()
        => new() { Code = nameof(PasswordMismatch), Description = "Identity.PasswordMismatch" };

    public override IdentityError UserAlreadyHasPassword()
        => new() { Code = nameof(UserAlreadyHasPassword), Description = "Identity.UserAlreadyHasPassword" };

    public override IdentityError UserLockoutNotEnabled()
        => new() { Code = nameof(UserLockoutNotEnabled), Description = "Identity.UserLockoutNotEnabled" };

    public override IdentityError RecoveryCodeRedemptionFailed()
        => new() { Code = nameof(RecoveryCodeRedemptionFailed), Description = "Identity.RecoveryCodeRedemptionFailed" };

    public override IdentityError ConcurrencyFailure()
        => new() { Code = nameof(ConcurrencyFailure), Description = "Identity.ConcurrencyFailure" };

    public override IdentityError DefaultError()
        => new() { Code = nameof(DefaultError), Description = "Identity.DefaultError" };
}
