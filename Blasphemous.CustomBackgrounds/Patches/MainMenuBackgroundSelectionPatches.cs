using Blasphemous.CustomBackgrounds.Components.Backgrounds;
using Blasphemous.ModdingAPI;
using Framework.Managers;
using Gameplay.UI.Others.MenuLogic;
using HarmonyLib;
using System.Linq;
using UnityEngine.UI;

namespace Blasphemous.CustomBackgrounds.Patches;

/// <summary>
/// Makes background selection able to scroll to modded backgrounds.
/// </summary>
[HarmonyPatch(typeof(NewMainMenu), "ProcessMoveInput")]
class NewMainMenu_ProcessMoveInput_IndexExtension_Patch
{
    public static bool Prefix(
        bool movingRight,
        NewMainMenu __instance,
        string ___soundOnMove,
        Text ___backgroundLabel)
    {
        // disable all mod backgrounds
        BackgroundRegister.Backgrounds.ToList().ForEach(x => x.GameObj.SetActive(false));

        // if the index after moving will be pointing to a vanilla background, return to executing original method instead
        Main.CustomBackgrounds.MainMenuBgIndex += movingRight ? 1 : -1;
        if (Main.CustomBackgrounds.IsDisplayingVanillaMainMenuBg)
        {
            // special fix for border cases
            if (Main.CustomBackgrounds.MainMenuBgIndex == 3 && __instance.bgIndex == 3)
                __instance.bgIndex = 4;
            if (Main.CustomBackgrounds.MainMenuBgIndex == 0 && __instance.bgIndex == 0)
                __instance.bgIndex = -1;
            SetVanillaBackgroundActive(true);
            return true;
        }

        // play GUI audio
        if (___soundOnMove != string.Empty)
        {
            Core.Audio.PlayOneShot(___soundOnMove, default);
        }

        // enable mod background and disable vanilla background
        SetVanillaBackgroundActive(false);
        Main.CustomBackgrounds.UnlockedMainMenuBackgrounds[Main.CustomBackgrounds.ModMainMenuBgIndex].GameObj.SetActive(true);
        Main.CustomBackgrounds.UnlockedMainMenuBackgrounds[Main.CustomBackgrounds.ModMainMenuBgIndex].SetGameObjectLayer();

        // update background selection's displayed label text
        ___backgroundLabel.text = Main.CustomBackgrounds.UnlockedMainMenuBackgrounds[Main.CustomBackgrounds.ModMainMenuBgIndex].ColoredLocalizedName;

        return false;

        void SetVanillaBackgroundActive(bool active)
        {
            __instance.transform.Find("Menu/StaticBackground").gameObject.SetActive(active);
            __instance.transform.Find("Menu/AnimatedBackgroundRoot").gameObject.SetActive(active);
        }
    }
}

/// <summary>
/// If config initialized persistent mod background, show it. 
/// If not, update this mod's background index to vanilla index.
/// </summary>
[HarmonyPatch(typeof(NewMainMenu), "Awake")]
class NewMainMenu_Awake_UpdateBackgroundIndexToModIndex_Patch
{
    public static bool hasExecuted = false;

    public static void Postfix(NewMainMenu __instance)
    {
        if (hasExecuted)
            return;

        hasExecuted = true;
        // if background name in save isn't valid, read from vanilla.
        bool shouldReadVanilla = (!Main.CustomBackgrounds.backgroundSaveData.currentIsModMainMenuBg) || !BackgroundRegister.Exists(Main.CustomBackgrounds.backgroundSaveData.currentModMainMenuBg, true);
        if (shouldReadVanilla)
        {
            Main.CustomBackgrounds.MainMenuBgIndex = __instance.bgIndex;
        }
        else
        {
            Main.CustomBackgrounds.MainMenuBgIndex = 4 + Main.CustomBackgrounds.UnlockedMainMenuBackgrounds.IndexOf(Main.CustomBackgrounds.UnlockedMainMenuBackgrounds.First(x => x.info.name == Main.CustomBackgrounds.backgroundSaveData.currentModMainMenuBg));
#if DEBUG
            ModLog.Warn($"Applying mod background on startup!");
            ModLog.Warn($"Main.CustomBackgrounds.BackgroundIndex: {Main.CustomBackgrounds.MainMenuBgIndex}");
#endif
            // enable mod background and disable vanilla background
            SetVanillaBackgroundActive(false);
            Main.CustomBackgrounds.UnlockedMainMenuBackgrounds[Main.CustomBackgrounds.ModMainMenuBgIndex].GameObj.SetActive(true);
            Main.CustomBackgrounds.UnlockedMainMenuBackgrounds[Main.CustomBackgrounds.ModMainMenuBgIndex].SetGameObjectLayer();
        }

        void SetVanillaBackgroundActive(bool active)
        {
            __instance.transform.Find("Menu/StaticBackground").gameObject.SetActive(active);
            __instance.transform.Find("Menu/AnimatedBackgroundRoot").gameObject.SetActive(active);
        }
    }
}

/// <summary>
/// Display modded background name at background selection tab
/// </summary>
[HarmonyPatch(typeof(NewMainMenu), "UpdateBackgroundLabelText")]
class NewMainMenu_UpdateBackgroundLabelText_UpdateModText_Patch
{
    public static bool Prefix(Text ___backgroundLabel)
    {
        if (Main.CustomBackgrounds.IsDisplayingVanillaMainMenuBg)
            return true;

        ___backgroundLabel.text = Main.CustomBackgrounds.UnlockedMainMenuBackgrounds[Main.CustomBackgrounds.ModMainMenuBgIndex].ColoredLocalizedName;
        return false;
    }
}
