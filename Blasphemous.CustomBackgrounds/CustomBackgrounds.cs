using Blasphemous.CheatConsole;
using Blasphemous.CustomBackgrounds.Commands;
using Blasphemous.CustomBackgrounds.Components.Backgrounds;
using Blasphemous.CustomBackgrounds.Components.Map;
using Blasphemous.CustomBackgrounds.Events;
using Blasphemous.CustomBackgrounds.Extensions;
using Blasphemous.CustomBackgrounds.Persistence;
using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Helpers;
using Blasphemous.ModdingAPI.Persistence;
using Framework.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blasphemous.CustomBackgrounds;

/// <summary>
/// Master class for CustomBackgrounds mod
/// </summary>
public class CustomBackgrounds : BlasMod, IPersistentMod
{
    internal Config config;
    internal GlobalSaveData globalSaveData = new();
    private int _mainMenuBgIndex;
    private static readonly string _saveFileName = @"BackgroundSaveData.json";

    public string PersistentID => ModInfo.MOD_ID;
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
        provider.RegisterCommand(new MapMarkCommand());
        provider.RegisterCommand(new MapMarkCommand());

#if DEBUG
        provider.RegisterBackground(new MainMenuBackground(FileHandler, "test_background_static.json"));
        provider.RegisterBackground(new MainMenuBackground(FileHandler, "test_background_animated.json"));
        provider.RegisterBackground(new DeathBackground(FileHandler, "test_death.json"));
        provider.RegisterBackground(new ArcadeDeathBackground(FileHandler, "test_death_demake.json"));
        provider.RegisterBackground(new LoadingBackground(FileHandler, "test_loading.json"));
        provider.RegisterBackground(new VictoryBackground(FileHandler, "test_victory_regular_boss.json"));
        provider.RegisterBackground(new VictoryBackground(FileHandler, "test_victory_final_boss.json"));
        provider.RegisterBackground(new ArcadeIntroBackground(FileHandler, "test_minigame_intro.json"));

        provider.RegisterMapMark(new ModMapMark(FileHandler, "test_mod_map_mark.json"));
#endif
    }

    protected override void OnAllInitialized()
    {
        // read save data from file
        LoadGlobalSave();

        // show pop-up for auto-acquired backgrounds that aren't unlocked yet
        foreach (BaseBackground background in BackgroundRegister.Backgrounds.Where(x => x.info.acquisitionType == BaseBackgroundInfo.AcquisitionType.OnInitialize))
        {
            background.SetUnlocked(true);
        }

        SaveGlobalSave();
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
            globalSaveData.currentModMainMenuBg = IsDisplayingModMainMenuBg
                ? UnlockedMainMenuBackgrounds[ModMainMenuBgIndex].info.name
                : "";
#if DEBUG
            // set flags of debug backgrounds to true
            Core.Events.SetFlag("custom_backgrounds_mod_debug", true);
#endif
        }
        else if (SceneHelper.MenuSceneLoaded)
        {
            if (globalSaveData.currentIsModMainMenuBg)
            {
                // restore displayed mod background according to name (because index may be changed by newly unlocked backgrounds)
                if (!BackgroundRegister.Exists(globalSaveData.currentModMainMenuBg))
                {
                    ModLog.Warn($"Saved background does not exist! Defaulting background to `Blasphemous`.");
                    //BackgroundIndex = 0;
                }
                else
                {
                    int index = UnlockedMainMenuBackgrounds.IndexOf(UnlockedMainMenuBackgrounds.First(x => x.info.name == globalSaveData.currentModMainMenuBg));
                    UnlockedMainMenuBackgrounds[ModMainMenuBgIndex].GameObj.SetActive(false);
                    MainMenuBgIndex = index + 4;
                    UnlockedMainMenuBackgrounds[ModMainMenuBgIndex].GameObj.SetActive(true);
                }
            }
        }
    }

    protected override void OnDispose()
    {
        globalSaveData.currentModMainMenuBg = IsDisplayingModMainMenuBg
            ? UnlockedMainMenuBackgrounds[ModMainMenuBgIndex].info.name
            : "";
        SaveGlobalSave();
        ConfigHandler.Save(config);
    }

    internal void LoadGlobalSave()
    {
        if (!FileHandler.LoadContentAsJson(_saveFileName, out globalSaveData))
        {
            ModLog.Warn($"`{_saveFileName}` does not exist! Initializing.");
            globalSaveData = new();
            SaveGlobalSave();
            return;
        }

        bool hasNameNotFound = false;
        foreach (string name in globalSaveData.unlockedBackgrounds)
        {
            if (BackgroundRegister.AtName(name) == null)
            {
                ModLog.Error($"Background of name `{name}` not found in Background Register! Failed to load it.");
                hasNameNotFound = true;
            }
            else
            {
                BackgroundRegister.AtName(name)?.SetUnlocked(true, false);
            }
        }

        if (hasNameNotFound)
        {
            SaveGlobalSave();
        }
    }

    internal void SaveGlobalSave()
    {
        globalSaveData.unlockedBackgrounds = UnlockedBackgrounds.Select(x => x.info.name).ToList();
        FileHandler.WriteJsonToContent(_saveFileName, globalSaveData);
    }

    public SaveData SaveGame()
    {
        SlotSaveData result = new();
        result.modMapMarks = ModMapManager.GetCurrentMapMarksSaveData();
        return result;
    }

    public void LoadGame(SaveData data)
    {
        SlotSaveData slotData = data as SlotSaveData;
        List<ModMapMarkSaveData> markSaveData = (List<ModMapMarkSaveData>)slotData.modMapMarks.MemberwiseClone();
        ModMapManager.LoadMapMarksSaveData(markSaveData);
#if DEBUG
        ModLog.Warn($"Loaded slot save data!");
        ModLog.Warn($"ModMapManager.modMapMarkInstances is null?: {ModMapManager.modMapMarkInstances == null}");
        ModLog.Warn($"slotData.modMapMarks is null?: {slotData.modMapMarks == null}");
        ModLog.Warn($"Mod map marks on current save:");
        foreach (ModMapMark modMapMark in ModMapManager.modMapMarkInstances)
        {
            ModLog.Warn($"  `{modMapMark.info.id}` at {modMapMark.cellKey}");
        }
#endif
    }

    public void ResetGame()
    {
        ModMapManager.ClearAllMapMarks();
#if DEBUG
        ModLog.Warn($"reset slot save data!");
#endif
    }
}
