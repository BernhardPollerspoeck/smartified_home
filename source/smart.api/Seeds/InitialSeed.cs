using asp.net.core.helper.core.Seed;
using smart.database;
using BCryptNet = BCrypt.Net.BCrypt;

namespace smart.api.Seeds;

public class InitialSeed : BaseSeed<SmartContext>
{

    protected override string Key => nameof(InitialSeed);

    public override int Order => 1;

    protected override bool PerformSeed(IServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SmartContext>();

            var newUser = new User
            {
                Username = "admin",
                PasswordHash = BCryptNet.HashPassword("Admin1!"),
                IsAdmin = true,
            };
            db.Users.Add(newUser);
            db.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }
}