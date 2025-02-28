using Framework.Managers;
using I2.Loc;
using System.Collections.Generic;

namespace Blasphemous.CustomBackgrounds.Patches;

/// <summary>
/// Contains static variables and functions that assists Harmony patches
/// </summary>
internal static class PatchController
{
    internal static string unlockPopupBackgroundName = "";
    internal static readonly string VANILLA_POPUP_ID = "PENITENT_ALMS";

    internal static bool IsShowingModPopup => !string.IsNullOrEmpty(unlockPopupBackgroundName);

    internal static class Localizer
    {
        internal static readonly string KEY_VANILLA_UNLOCK_POPUP_BODY = "UI_Inventory/TEXT_POPUP_SKINUNLOCKED_BODY";
        internal static readonly string KEY_VANILLA_UNLOCK_POPUP_TITLE = "UI_Inventory/TEXT_POPUP_SKINUNLOCKED_TITLE";
        internal static readonly string KEY_VANILLA_UNLOCK_POPUP_TEXT = "UI_Extras/UNLOCK_PENITENT_ALMS";
        internal static readonly string KEY_MODDED_UNLOCK_POPUP_BODY = "popup.body";
        internal static readonly string KEY_MODDED_UNLOCK_POPUP_TITLE = "popup.title";
        internal static readonly string KEY_MODDED_UNLOCK_POPUP_TEXT = "popup.text";
        internal static Dictionary<string, string> vanillaLocalizationStorage = new()
        {
            { KEY_VANILLA_UNLOCK_POPUP_BODY, "" },
            { KEY_VANILLA_UNLOCK_POPUP_TITLE, "" },
            { KEY_VANILLA_UNLOCK_POPUP_TEXT, "" },
        };
        internal static readonly Dictionary<string, string> vanillaKeyToModdedKey = new()
        {
            { KEY_VANILLA_UNLOCK_POPUP_BODY, KEY_MODDED_UNLOCK_POPUP_BODY },
            { KEY_VANILLA_UNLOCK_POPUP_TITLE, KEY_MODDED_UNLOCK_POPUP_TITLE },
            { KEY_VANILLA_UNLOCK_POPUP_TEXT, KEY_MODDED_UNLOCK_POPUP_TEXT },
        };

        internal static string LocalizedPopupBody => Main.CustomBackgrounds.LocalizationHandler.Localize(KEY_MODDED_UNLOCK_POPUP_BODY);
        internal static string LocalizedPopupTitle => Main.CustomBackgrounds.LocalizationHandler.Localize(KEY_MODDED_UNLOCK_POPUP_TITLE);
        internal static string LocalizedPopupText => Main.CustomBackgrounds.LocalizationHandler.Localize(KEY_MODDED_UNLOCK_POPUP_TEXT);

        internal static bool TryWriteTermToGame(string termKey, string termContent)
        {
            bool result = false;
            int langIndex = Core.Localization.GetCurrentLanguageIndex();

            foreach (LanguageSource source in I2.Loc.LocalizationManager.Sources)
            {
                List<string> allAvailableTerms = source.GetTermsList();
                if (allAvailableTerms.Contains(termKey))
                {
                    result = true;
                    string termText = termContent;
                    source.GetTermData(termKey).Languages[langIndex] = termText;
                }
            }

            return result;
        }

        internal static void StoreVanillaTranslation(string termKey)
        {
            string vanillaText = "";
            int langIndex = Core.Localization.GetCurrentLanguageIndex();
            foreach (LanguageSource source in I2.Loc.LocalizationManager.Sources)
            {
                List<string> allAvailableTerms = source.GetTermsList();
                if (allAvailableTerms.Contains(termKey))
                {
                    vanillaText = source.GetTermData(termKey).Languages[langIndex];
                }
            }
            if (vanillaLocalizationStorage.ContainsKey(termKey))
            {
                vanillaLocalizationStorage[termKey] = vanillaText;
            }
            else
            {
                vanillaLocalizationStorage.Add(termKey, vanillaText);
            }
        }
    }
}
