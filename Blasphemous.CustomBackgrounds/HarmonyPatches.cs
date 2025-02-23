﻿using Blasphemous.CustomBackgrounds.Components.Backgrounds;
using Blasphemous.ModdingAPI;
using Framework.Managers;
using Gameplay.UI.Others.MenuLogic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.CustomBackgrounds;

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
        // if there's a mod background currently active, disable it
        if (Main.CustomBackgrounds.IsDisplayingModBackground)
        {
            int index = Main.CustomBackgrounds.ModBackgroundIndex;
            BackgroundRegister.AtIndex(index).GameObj.SetActive(false);
        }

        // if the index after moving will be pointing to a vanilla background, return to executing original method instead
        Main.CustomBackgrounds.BackgroundIndex += movingRight ? 1 : -1;
        if (Main.CustomBackgrounds.IsDisplayingVanillaBackground)
        {
            // special fix for border cases
            if (Main.CustomBackgrounds.BackgroundIndex == 3 && __instance.bgIndex == 3)
                __instance.bgIndex = 4;
            if (Main.CustomBackgrounds.BackgroundIndex == 0 && __instance.bgIndex == 0)
                __instance.bgIndex = -1;
            SetVanillaBackgroundActive(true);
            return true;
        }

        // play GUI audio
        if (___soundOnMove != string.Empty)
        {
            Core.Audio.PlayOneShot(___soundOnMove, default(Vector3));
        }

        // enable mod background and disable vanilla background
        SetVanillaBackgroundActive(false);
        Main.CustomBackgrounds.UnlockedBackgrounds[Main.CustomBackgrounds.ModBackgroundIndex].GameObj.SetActive(true);
        Main.CustomBackgrounds.UnlockedBackgrounds[Main.CustomBackgrounds.ModBackgroundIndex].SetGameObjectLayer();

        // update background selection's displayed label text
        ___backgroundLabel.text = Main.CustomBackgrounds.UnlockedBackgrounds[Main.CustomBackgrounds.ModBackgroundIndex].LocalizedName;

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
        // if background index in config isn't valid, read from vanilla.
        bool shouldReadVanilla = !Main.CustomBackgrounds.backgroundSaveData.currentIsModBackground;
        if (shouldReadVanilla)
        {
            Main.CustomBackgrounds.BackgroundIndex = __instance.bgIndex;
        }
        else
        {
            Main.CustomBackgrounds.BackgroundIndex = 4 + Main.CustomBackgrounds.UnlockedBackgrounds.IndexOf(BackgroundRegister.AtName(Main.CustomBackgrounds.backgroundSaveData.currentModBackground));
#if DEBUG
            ModLog.Warn($"Applying mod background on startup!");
            ModLog.Warn($"Main.CustomBackgrounds.BackgroundIndex: {Main.CustomBackgrounds.BackgroundIndex}");
#endif

            // enable mod background and disable vanilla background
            SetVanillaBackgroundActive(false);
            Main.CustomBackgrounds.UnlockedBackgrounds[Main.CustomBackgrounds.ModBackgroundIndex].GameObj.SetActive(true);
            Main.CustomBackgrounds.UnlockedBackgrounds[Main.CustomBackgrounds.ModBackgroundIndex].SetGameObjectLayer();
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
        if (Main.CustomBackgrounds.IsDisplayingVanillaBackground)
            return true;

        ___backgroundLabel.text = Main.CustomBackgrounds.UnlockedBackgrounds[Main.CustomBackgrounds.ModBackgroundIndex].LocalizedName;
        return false;
    }
}