using Blasphemous.CheatConsole;
using Blasphemous.CustomBackgrounds.Commands;
using Blasphemous.CustomBackgrounds.Components.Backgrounds;
using Blasphemous.CustomBackgrounds.Events;
using Blasphemous.CustomBackgrounds.Extensions;
using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Helpers;
using Framework.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blasphemous.CustomBackgrounds;

public class CustomBackgrounds : BlasMod
{
    internal Config config;
    internal BackgroundSaveData backgroundSaveData = new();
    private int _mainMenuBgIndex;
    private static readonly string _saveFileName = @"BackgroundSaveData.json";

    internal EventHandler EventHandler { get; } = new();
    internal List<BaseBackground> UnlockedBackgrounds => BackgroundRegister.Backgrounds.SelectUnlocked(true).ToList();
    internal List<MainMenuBackground> UnlockedMainMenuBackgrounds => BackgroundRegister.MainMenuBackgrounds.SelectUnlocked(true).ToList();
    internal int MainMenuBgIndex
    {
        get => _mainMenuBgIndex;
        set
        {
            int numTotalBackgrounds = UnlockedMainMenuBackgrounds.Count + 4;
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

            _mainMenuBgIndex = value;
        }
    }
    internal int ModMainMenuBgIndex
    {
        get
        {
            if (!IsDisplayingModMainMenuBg)
                throw new System.Exception($"Failed attempt to call mod background by index when displaying vanilla background!");

            return MainMenuBgIndex - 4;
        }
    }
    internal bool IsDisplayingModMainMenuBg
    {
        get
        {
            return !(MainMenuBgIndex >= 0 && MainMenuBgIndex <= 3);
        }
    }
    internal bool IsDisplayingVanillaMainMenuBg
    {
        get => !IsDisplayingModMainMenuBg;
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
        provider.RegisterBackground(new MainMenuBackground(FileHandler, "test_background_static.json"));
        provider.RegisterBackground(new MainMenuBackground(FileHandler, "test_background_animated.json"));
        provider.RegisterBackground(new DeathBackground(FileHandler, "test_death.json"));
        provider.RegisterBackground(new LoadingBackground(FileHandler, "test_loading.json"));
#endif
        provider.RegisterCommand(new BackgroundCommand());
    }

    protected override void OnAllInitialized()
    {
        // read save data from file
        LoadBackgroundSave();

        // show pop-up for auto-acquired backgrounds that aren't unlocked yet
        foreach (BaseBackground background in BackgroundRegister.Backgrounds.Where(x => x.info.acquisitionType == BaseBackgroundInfo.AcquisitionType.OnInitialize))
        {
            background.SetUnlocked(true);
        }

        SaveBackgroundSave();
#if DEBUG
        StringBuilder sb = new();
        sb.AppendLine($"All backgrounds in register:");
        for (int i = 0; i < BackgroundRegister.Total; i++)
        {
            sb.AppendLine($"#{i}: {BackgroundRegister.AtIndex(i).info.name}");
            sb.AppendLine($"\n  [unlocked?: {BackgroundRegister.AtIndex(i).isUnlocked}]");
            sb.AppendLine($"\n  [type: {BackgroundRegister.AtIndex(i).GetType()}]");
        }
        ModLog.Info(sb);
#endif
    }

    protected override void OnLevelLoaded(string oldLevel, string newLevel)
    {
        if (SceneHelper.GameSceneLoaded)
        {
            // if displaying a mod background, store the currently displayed background's name
            backgroundSaveData.currentModMainMenuBg = IsDisplayingModMainMenuBg
                ? UnlockedMainMenuBackgrounds[ModMainMenuBgIndex].info.name
                : "";
#if DEBUG
            // set flags of debug backgrounds to true
            Core.Events.SetFlag(BackgroundRegister.DeathBackgrounds.First(x => x.info.name == "test_death").activeFlag, true);
            Core.Events.SetFlag(BackgroundRegister.LoadingBackgrounds.First(x => x.info.name == "test_loading").activeFlag, true);
#endif
        }
        else if (SceneHelper.MenuSceneLoaded)
        {
            if (backgroundSaveData.currentIsModMainMenuBg)
            {
                // restore displayed mod background according to name (because index may be changed by newly unlocked backgrounds)
                if (!BackgroundRegister.Exists(backgroundSaveData.currentModMainMenuBg))
                {
                    ModLog.Warn($"Saved background does not exist! Defaulting background to `Blasphemous`.");
                    //BackgroundIndex = 0;
                }
                else
                {
                    int index = UnlockedMainMenuBackgrounds.IndexOf(UnlockedMainMenuBackgrounds.First(x => x.info.name == backgroundSaveData.currentModMainMenuBg));
                    UnlockedMainMenuBackgrounds[ModMainMenuBgIndex].GameObj.SetActive(false);
                    MainMenuBgIndex = index + 4;
                    UnlockedMainMenuBackgrounds[ModMainMenuBgIndex].GameObj.SetActive(true);
                }
            }
        }
    }

    protected override void OnDispose()
    {
        backgroundSaveData.currentModMainMenuBg = IsDisplayingModMainMenuBg
            ? UnlockedMainMenuBackgrounds[ModMainMenuBgIndex].info.name
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
