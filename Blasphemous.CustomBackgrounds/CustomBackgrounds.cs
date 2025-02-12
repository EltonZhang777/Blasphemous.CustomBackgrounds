using Blasphemous.CustomBackgrounds.Components.Backgrounds;
using Blasphemous.ModdingAPI;
using Newtonsoft.Json;
using System.IO;

namespace Blasphemous.CustomBackgrounds;

public class CustomBackgrounds : BlasMod
{
    private int _backgroundIndex;

    internal int BackgroundIndex
    {
        get => _backgroundIndex;
        set
        {
            int numTotalBackgrounds = BackgroundRegister.Total + 4;
            if (value < 0)
                value = numTotalBackgrounds - 1;
            if (value >= numTotalBackgrounds)
                value = 0;
            _backgroundIndex = value;
        }
    }
    internal int ModBackgroundIndex
    {
        get
        {
            if (!IsDisplayingModBackground)
                throw new System.Exception($"Failed attempt to call mod background by index when displaying vanilla background!");

            return BackgroundIndex - 4;
        }
    }

    internal bool IsDisplayingModBackground
    {
        get
        {
            return !(BackgroundIndex >= 0 && BackgroundIndex <= 3);
        }
    }

    internal bool IsDisplayingVanillaBackground
    {
        get => !IsDisplayingModBackground;
    }

    internal CustomBackgrounds() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    protected override void OnInitialize()
    {
        // initialize config
        Config cfg = ConfigHandler.Load<Config>();
        ConfigHandler.Save(cfg);
    }

    protected override void OnRegisterServices(ModServiceProvider provider)
    {
#if DEBUG
        provider.RegisterBackground(new Background(FileHandler, "test_background_static.json"));
        provider.RegisterBackground(new Background(FileHandler, "test_background_animated.json"));
#endif
    }

    protected override void OnAllInitialized()
    {
#if DEBUG
        File.WriteAllText(
            Path.Combine(FileHandler.ContentFolder, @"test_background_static_output.json"),
            JsonConvert.SerializeObject(BackgroundRegister.AtIndex(0).backgroundInfo, Formatting.Indented));
#endif
    }
}
