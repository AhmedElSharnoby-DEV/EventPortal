using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using Umbraco.Cms.Core.Security;

namespace EventPortal.Validators
{
    public class CustomPasswordValidator : IPasswordValidator<MemberIdentityUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<MemberIdentityUser> manager, MemberIdentityUser user, string? password)
        {
            List<IdentityError> errors = new List<IdentityError>();

            if (password is null || password.Length < 8)
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordLength",
                    Description = "Password must be at least 8 characters long."
                });
            }

            if (!Regex.IsMatch(password!, @"[A-Z]"))
            {
                errors.Add(new IdentityError
                {
                    Code = "UppercaseRequired",
                    Description = "Password must contain at least one uppercase letter."
                });
            }

            if (!Regex.IsMatch(password!, @"[a-z]"))
            {
                errors.Add(new IdentityError
                {
                    Code = "LowercaseRequired",
                    Description = "Password must contain at least one lowercase letter."
                });
            }

            if (!Regex.IsMatch(password!, @"\d"))
            {
                errors.Add(new IdentityError
                {
                    Code = "NumberRequired",
                    Description = "Password must contain at least one number."
                });
            }

            if (!Regex.IsMatch(password!, @"[!@#$%^&*(),.?""{}|<>]"))
            {
                errors.Add(new IdentityError
                {
                    Code = "SpecialCharacterRequired",
                    Description = "Password must contain at least one special character (e.g., !@#$%^&*)."
                });
            }
            return Task.FromResult(errors.Any()
                ? IdentityResult.Failed(errors.ToArray())
                : IdentityResult.Success);
        }
    }
}
