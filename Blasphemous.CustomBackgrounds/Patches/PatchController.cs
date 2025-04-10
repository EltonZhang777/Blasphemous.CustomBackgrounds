using Blasphemous.CustomBackgrounds.Components.Backgrounds;
using Framework.Managers;
using Gameplay.UI;
using HarmonyLib;
using I2.Loc;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blasphemous.CustomBackgrounds.Patches;

/// <summary>
/// Contains static variables and functions that assists Harmony patches
/// </summary>
internal static class PatchController
{
    internal static string unlockPopupBackgroundName = "";
    internal static readonly string VANILLA_POPUP_ID = "PENITENT_ALMS";

    internal static bool IsShowingModPopup => !string.IsNullOrEmpty(unlockPopupBackgroundName);

    /// <summary>
    /// Activate/Deactivate the vanilla counterpart of the given type of background
    /// </summary>
    internal static void SetVanillaCounterpartActive<T>(bool active) where T : BaseBackground
    {
        if (typeof(T) == typeof(DeathBackground))
        {
            List<GameObject> gameObjects =
                [
                GameObject.Find($"Game UI/Content/UI_DEAD_SCREEN/Main Interface/DeathMessage"),
                GameObject.Find($"Game UI/Content/UI_DEAD_SCREEN/Main Interface/Background")
                ];
            gameObjects.ForEach(x => x.SetActive(active));
        }
        else if (typeof(T) == typeof(ArcadeDeathBackground))
        {
            List<GameObject> gameObjects =
                [
                GameObject.Find($"Game UI/Content/UI_DEAD_SCREEN/DemakeEdition/DeathMessage"),
                GameObject.Find($"Game UI/Content/UI_DEAD_SCREEN/DemakeEdition/Background")
                ];
            gameObjects.ForEach(x => x.SetActive(active));
        }
        else if (typeof(T) == typeof(MainMenuBackground))
        {
            List<GameObject> gameObjects =
                [
                GameObject.Find($"Game UI/Content/UI_MAINMENU/Menu/StaticBackground"),
                GameObject.Find($"Game UI/Content/UI_MAINMENU/Menu/AnimatedBackgroundRoot")
                ];
            gameObjects.ForEach(x => x.SetActive(active));
        }
        else if (typeof(T) == typeof(LoadingBackground))
        {
            List<GameObject> gameObjects =
                [
                Traverse.Create(UIController.instance).Field("loadWidget").GetValue<GameObject>()
                ];
            gameObjects.ForEach(x => x.SetActive(active));
        }
        else if (typeof(T) == typeof(ArcadeLoadingBackground))
        {
            List<GameObject> gameObjects =
                [
                Traverse.Create(UIController.instance).Field("loadWidgetDemake").GetValue<GameObject>()
                ];
            gameObjects.ForEach(x => x.SetActive(active));
        }
        else if (typeof(T) == typeof(VictoryBackground))
        {
            UIController.instance.StartCoroutine(VictoryBackgroundCoroutine(active ? 6 : 2));
        }
        else if (typeof(T) == typeof(ArcadeIntroBackground))
        {
            List<GameObject> gameObjects =
                [
                GameObject.Find("Game UI/Content/UI_INTRODEMAKE/alcazar-bg")
                ];
            gameObjects.ForEach(x => x.SetActive(active));
        }
        else
        {
            throw new NotImplementedException();
        }

        IEnumerator VictoryBackgroundCoroutine(int numRepeats = 1)
        {
            List<GameObject> gameObjects =
                [
                GameObject.Find($"Game UI/Content/UI_FULLMESSAGES/Main Interface/Background/Area"),
                GameObject.Find($"Game UI/Content/UI_FULLMESSAGES/Main Interface/Background/Boss"),
                GameObject.Find($"Game UI/Content/UI_FULLMESSAGES/Main Interface/Background/EndBoss")
                ];
            for (int i = 0; i < numRepeats; i++)
            {
                yield return new WaitForEndOfFrame();
                gameObjects.ForEach(x => x.SetActive(active));
            }
        }
    }

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

        [Obsolete("Now localizes unlock pop-up by directly managing GameObject's text.")]
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

        [Obsolete("Now localizes unlock pop-up by directly managing GameObject's text.")]
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
