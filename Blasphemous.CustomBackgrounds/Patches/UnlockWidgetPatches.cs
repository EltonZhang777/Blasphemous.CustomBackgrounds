using Blasphemous.CustomBackgrounds.Components.Backgrounds;
using HarmonyLib;
using TMPro;
using UnityEngine.UI;

namespace Blasphemous.CustomBackgrounds.Patches;

/// <summary>
/// Adds background image to popup.
/// </summary>
[HarmonyPatch(typeof(UnlockWidget), "Configurate")]
class UnlockWidget_Configurate_ConfigurateBackgroundUnlockWidget_Patch
{
    public static bool Prefix(
        UnlockWidget __instance,
        Image ___skinPreviewImage)
    {
        if (PatchController.IsShowingModPopup)
        {
            BaseBackground background = BackgroundRegister.AtName(PatchController.unlockPopupBackgroundName);
            ___skinPreviewImage.sprite = background.GameObj.GetComponent<Image>().sprite;
            return false;
        }

        return true;
    }
}

/// <summary>
/// Localize popup text when unlocking mod background.
/// </summary>
[HarmonyPatch(typeof(UnlockWidget), "UpdateTextPro")]
class UnlockWidget_UpdateTextPro_UpdateModBackgroundLocalization_Patch
{
    /// <summary>
    /// Change vanilla localization to modded localization
    /// </summary>
    public static bool Prefix(
        string ___unlockId,
        UnlockWidget __instance)
    {
        if (PatchController.IsShowingModPopup)
        {
            BaseBackground background = BackgroundRegister.AtName(PatchController.unlockPopupBackgroundName);
            Text titleText = __instance.transform.Find("RootObject/Frame/Title").gameObject.GetComponent<Text>();
            TextMeshProUGUI bodyText = __instance.transform.Find("RootObject/Frame/Text").gameObject.GetComponent<TextMeshProUGUI>();
            titleText.text = PatchController.Localizer.LocalizedPopupTitle;
            bodyText.text = PatchController.Localizer.LocalizedPopupText + " " + background.ColoredLocalizedName;

            return false;
        }

        return true;
    }
}