using Microsoft.AspNetCore.Identity;

namespace Cultivation.Shared.Helper;

public class PasswordHelper
{
    public static string HashPassword(string password)
    {
        var passwordHasher = new PasswordHasher<object>();
        return passwordHasher.HashPassword(null, password);
    }

    public static bool CheckPasswordPolicy(string password, PasswordCriteria opts = null)
    {
        opts ??= new PasswordCriteria();

        if (password.Length < opts.RequiredLength)
            return false;

        int digitCount = 0, lowerCount = 0, upperCount = 0, symbolCount = 0;
        var set = new HashSet<char>();

        foreach (var character in password)
        {
            if (char.IsDigit(character))
                digitCount++;
            else if (char.IsUpper(character))
                upperCount++;
            else if (char.IsLower(character))
                lowerCount++;
            else
                symbolCount++;

            set.Add(character);
        }
        return set.Count >= opts.RequiredUniqueChars
            && (!opts.RequireDigit || digitCount > 0)
            && (!opts.RequireLowercase || lowerCount > 0)
            && (!opts.RequireUppercase || upperCount > 0)
            && (!opts.RequireNonAlphanumeric || symbolCount > 0);
    }
}
