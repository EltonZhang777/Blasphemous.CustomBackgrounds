using Blasphemous.CustomBackgrounds.Components.Backgrounds;
using Framework.Managers;
using Gameplay.UI.Others.MenuLogic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.CustomBackgrounds;

[HarmonyPatch(typeof(NewMainMenu), "ProcessMoveInput")]
class NewMainMenu_ProcessMoveInput_IndexExtension_Patch
{
    public static bool Prefix(
        bool movingRight,
        NewMainMenu __instance,
        string ___soundOnMove,
        Text ___backgroundLabel)
    {
        // disable every mod background
        foreach (Background background in BackgroundRegister.Backgrounds)
        {
            background.GameObj.SetActive(false);
        }

        // if the index after moving will be pointing to a vanilla background, return to executing original method instead
        int numTotalBackgrounds = BackgroundRegister.Total + 4;
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
        BackgroundRegister.AtIndex(Main.CustomBackgrounds.ModBackgroundIndex).GameObj.SetActive(true);
        BackgroundRegister.AtIndex(Main.CustomBackgrounds.ModBackgroundIndex).SetGameObjectLayer();

        // update background selection's displayed label text
        ___backgroundLabel.text = BackgroundRegister.AtIndex(Main.CustomBackgrounds.ModBackgroundIndex).LocalizedName;

        return false;

        void SetVanillaBackgroundActive(bool active)
        {
            __instance.transform.Find("Menu/StaticBackground").gameObject.SetActive(active);
            __instance.transform.Find("Menu/AnimatedBackgroundRoot").gameObject.SetActive(active);
        }
    }
}

[HarmonyPatch(typeof(NewMainMenu), "Awake")]
class NewMainMenu_Awake_UpdateBackgroundIndexToModIndex_Patch
{
    public static bool hasExecuted = false;

    public static void Postfix(NewMainMenu __instance)
    {
        if (hasExecuted)
            return;

        hasExecuted = true;
        Main.CustomBackgrounds.BackgroundIndex = __instance.bgIndex;
    }
}

[HarmonyPatch(typeof(NewMainMenu), "UpdateBackgroundLabelText")]
class NewMainMenu_UpdateBackgroundLabelText_UpdateModText_Patch
{
    public static bool Prefix(Text ___backgroundLabel)
    {
        if (Main.CustomBackgrounds.IsDisplayingVanillaBackground)
            return true;

        ___backgroundLabel.text = BackgroundRegister.AtIndex(Main.CustomBackgrounds.ModBackgroundIndex).LocalizedName;
        return false;
    }
}