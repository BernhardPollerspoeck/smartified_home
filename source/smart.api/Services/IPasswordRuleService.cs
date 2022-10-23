namespace smart.api.Services;

public interface IPasswordRuleService
{
    bool IsValidPassword(string? password);

}
