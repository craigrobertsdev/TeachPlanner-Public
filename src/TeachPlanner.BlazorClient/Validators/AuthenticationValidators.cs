using System.Globalization;
using System.Text.RegularExpressions;

namespace TeachPlanner.BlazorClient.Validators;

public static class AuthenticationValidators
{
    public static bool IsValidPassword(string password)
    {
        bool hasNonAlpha = false;
        bool hasNumeral = false;
        bool hasLower = false;
        bool hasUpper = false;

        foreach (var c in password.ToCharArray())
        {
            if (Char.IsDigit(c))
            {
                hasNumeral = true;
            }

            if (Char.IsLower(c))
            {
                hasLower = true;
            }

            if (Char.IsUpper(c))
            {
                hasUpper = true;
            }

            if (!Char.IsPunctuation(c))
            {
                hasNonAlpha = true;
            }
        }

        return hasNonAlpha && hasNumeral && hasLower && hasUpper;
    }

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            // Normalise the domain
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

            // Examines the domain part of the email and normalises it
            string DomainMapper(Match match)
            {
                // Use IdnMapping class to convert Unicode domain names
                var idn = new IdnMapping();

                // Pull out and process domain name (throws ArgumentException on invalid)
                string domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

}
