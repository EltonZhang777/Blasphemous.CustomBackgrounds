using Blasphemous.ModdingAPI;

namespace Blasphemous.CustomBackgrounds;

public class CustomBackgrounds : BlasMod
{
    internal CustomBackgrounds() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    protected override void OnInitialize()
    {
        // initialize config
        Config cfg = ConfigHandler.Load<Config>();
        ConfigHandler.Save(cfg);
    }

    protected override void OnRegisterServices(ModServiceProvider provider)
    {

    }

    protected override void OnAllInitialized()
    {

    }
}
