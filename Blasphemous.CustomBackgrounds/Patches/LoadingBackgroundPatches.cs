using Blasphemous.CustomBackgrounds.Components.Backgrounds;
using Blasphemous.ModdingAPI;
using Framework.Managers;
using Gameplay.UI;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace Blasphemous.CustomBackgrounds.Patches;

/// <summary>
/// Show mod loading screen
/// </summary>
[HarmonyPatch(typeof(UIController), "ShowLoad")]
class UIController_ShowLoad_ShowModLoadingBackground_Patch
{
    public static void Prefix(
        UIController __instance,
        bool show)
    {
        if (Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.DEMAKE))
        {
            // can probably add support for mini-game loading screen later.
            return;
        }

        // main-game loading screen
        if (!show) // loading screen is being deactivated by game
        {
            // disable all mod loading backgrounds on close, re-enabling vanilla
            BackgroundRegister.LoadingBackgrounds.SelectUnlocked(true).ToList().ForEach(x => x.SetActive(false));
            PatchController.SetVanillaCounterpartActive<LoadingBackground>(true);
            return;
        }
        else // loading screen is being activated by game
        {
            List<LoadingBackground> activeBackgrounds = BackgroundRegister.LoadingBackgrounds.SelectUnlocked(true).ToList().Where(x => Core.Events.GetFlag(x.info.activeFlag)).ToList();

            if (activeBackgrounds.Count == 0)
                return;

            if (activeBackgrounds.Count > 1)
            {
                ModLog.Warn($"More than one LoadingBackgrounds are active simultaneously!");
            }

            // enable mod backgrounds whose corresponding activeFlag is true
            PatchController.SetVanillaCounterpartActive<LoadingBackground>(false);
            activeBackgrounds.ForEach(x => x.SetActive(true));
        }

    }
}