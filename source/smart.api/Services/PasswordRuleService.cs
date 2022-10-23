namespace smart.api.Services;

public sealed class PasswordRuleService : IPasswordRuleService
{
    public bool IsValidPassword(string? password)
    {
        if (password is null)
        {
            return false;
        }
        var chars = password.ToCharArray();
        return chars.Any(c => char.IsUpper(c))          //uppercase
             && chars.Any(c => char.IsLower(c))         //lowercase
             && chars.Any(c => char.IsNumber(c))        //numerical
             && chars.Any(c => char.IsLetter(c))        //letter
             && chars.Length >= 6;                      //length
    }
}
