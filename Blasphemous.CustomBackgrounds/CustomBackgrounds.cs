using Blasphemous.CheatConsole;
using Blasphemous.CustomBackgrounds.Commands;
using Blasphemous.CustomBackgrounds.Components.Backgrounds;
using Blasphemous.CustomBackgrounds.Events;
using Blasphemous.CustomBackgrounds.Extensions;
using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blasphemous.CustomBackgrounds;

public class CustomBackgrounds : BlasMod
{
    internal Config config;
    internal BackgroundSaveData backgroundSaveData = new();
    private int _backgroundIndex;
    private static readonly string _saveFileName = @"BackgroundSaveData.json";

    internal EventHandler EventHandler { get; } = new();
    internal List<Background> UnlockedBackgrounds => BackgroundRegister.Backgrounds.Where(x => x.isUnlocked == true).ToList();
    internal int BackgroundIndex
    {
        get => _backgroundIndex;
        set
        {
            int numTotalBackgrounds = UnlockedBackgrounds.Count + 4;
            if (value < 0)
            {
                do
                {
                    value += numTotalBackgrounds;
                } while (value < 0);
            }
            else if (value >= numTotalBackgrounds)
            {
                do
                {
                    value -= numTotalBackgrounds;
                } while (value >= numTotalBackgrounds);
            }

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
        config = ConfigHandler.Load<Config>();
        LocalizationHandler.RegisterDefaultLanguage("en");
    }

    protected override void OnRegisterServices(ModServiceProvider provider)
    {
#if DEBUG
        provider.RegisterBackground(new Background(FileHandler, "test_background_static.json"));
        provider.RegisterBackground(new Background(FileHandler, "test_background_animated.json"));
#endif
        provider.RegisterCommand(new BackgroundCommand());
    }

    protected override void OnAllInitialized()
    {
        // read save data from file
        LoadBackgroundSave();

        // show pop-up for auto-acquired backgrounds that aren't unlocked yet
        foreach (Background background in BackgroundRegister.Backgrounds.Where(x => x.info.acquisitionType == BackgroundInfo.AcquisitionType.OnInitialize))
        {
            background.SetUnlocked(true);
        }

        SaveBackgroundSave();
    }

    protected override void OnLevelLoaded(string oldLevel, string newLevel)
    {
        if (SceneHelper.GameSceneLoaded)
        {
            // if displaying a mod background, store the currently displayed background's name
            backgroundSaveData.currentModBackground = IsDisplayingModBackground
                ? UnlockedBackgrounds[ModBackgroundIndex].info.name
                : "";
        }
        else if (SceneHelper.MenuSceneLoaded)
        {
            if (backgroundSaveData.currentIsModBackground)
            {
                // restore displayed mod background according to name (because index may be changed by newly unlocked backgrounds)
                if (!BackgroundRegister.Exists(backgroundSaveData.currentModBackground))
                {
                    ModLog.Warn($"Saved background does not exist! Defaulting background to `Blasphemous`.");
                    //BackgroundIndex = 0;
                }
                else
                {
                    int index = UnlockedBackgrounds.IndexOf(UnlockedBackgrounds.First(x => x.info.name == backgroundSaveData.currentModBackground));
                    UnlockedBackgrounds[ModBackgroundIndex].GameObj.SetActive(false);
                    BackgroundIndex = index + 4;
                    UnlockedBackgrounds[ModBackgroundIndex].GameObj.SetActive(true);
                }
            }
#if DEBUG
            StringBuilder sb = new();
            sb.AppendLine($"All backgrounds in register:");
            for (int i = 0; i < BackgroundRegister.Total; i++)
            {
                sb.AppendLine($"#{i}: {BackgroundRegister.AtIndex(i).info.name} [unlocked?: {BackgroundRegister.AtIndex(i).isUnlocked}]");
            }
            sb.AppendLine($"\nAll unlocked backgrounds:");
            for (int i = 0; i < UnlockedBackgrounds.Count; i++)
            {
                sb.AppendLine($"#{i}: {UnlockedBackgrounds[i].info.name}");
            }
            ModLog.Info(sb);
#endif
        }
    }

    protected override void OnDispose()
    {
        backgroundSaveData.currentModBackground = IsDisplayingModBackground
            ? UnlockedBackgrounds[ModBackgroundIndex].info.name
            : "";
        SaveBackgroundSave();
        ConfigHandler.Save(config);
    }

    internal void LoadBackgroundSave()
    {
        if (!FileHandler.LoadContentAsJson(_saveFileName, out backgroundSaveData))
        {
            ModLog.Warn($"`{_saveFileName}` does not exist! Initializing.");
            backgroundSaveData = new();
            SaveBackgroundSave();
            return;
        }

        bool hasNameNotFound = false;
        foreach (string name in backgroundSaveData.unlockedBackgrounds)
        {
            ModLog.Warn($"background name: {name}");
            if (BackgroundRegister.AtName(name) == null)
            {
                ModLog.Error($"Background of name {name} not found in Background Register! Failed to load it.");
                hasNameNotFound = true;
            }
            else
            {
                BackgroundRegister.AtName(name)?.SetUnlocked(true, false);
            }
        }

        if (hasNameNotFound)
        {
            SaveBackgroundSave();
        }
    }

    internal void SaveBackgroundSave()
    {
        backgroundSaveData.unlockedBackgrounds = UnlockedBackgrounds.Select(x => x.info.name).ToList();
        FileHandler.WriteJsonToContent(_saveFileName, backgroundSaveData);
    }
}
