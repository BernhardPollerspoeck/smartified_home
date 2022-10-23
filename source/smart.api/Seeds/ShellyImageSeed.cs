using asp.net.core.helper.core.Seed;
using smart.database;

namespace smart.api.Seeds;

public class ShellyImageSeed : BaseSeed<SmartContext>
{

    protected override string Key => nameof(ShellyImageSeed);

    public override int Order => 2;

    protected override bool PerformSeed(IServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SmartContext>();

            var info = new ImageInfo
            {
                ElementType = EElementType.Shelly1,
                Name = "bpoller/ShellyHandler",
                Tag = "v1"
            };
            db.ImageInfos.Add(info);
            db.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }

}
