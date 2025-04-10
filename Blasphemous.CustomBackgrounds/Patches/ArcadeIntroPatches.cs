using Blasphemous.CustomBackgrounds.Components.Backgrounds;
using Blasphemous.ModdingAPI;
using Framework.Managers;
using Gameplay.UI;
using Gameplay.UI.Others.MenuLogic;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.CustomBackgrounds.Patches;

[HarmonyPatch(typeof(IntroDemakeWidget), "Open", new System.Type[] { })]
class IntroDemakeWidget_Open_ShowModArcadeIntroBackground_Patch
{
    public static void Prefix()
    {
        // disable all modded backgrounds first
        BackgroundRegister.ArcadeIntroBackgrounds.SelectUnlocked(true).ToList().ForEach(x => x.SetActive(false));

        List<ArcadeIntroBackground> activeBackgrounds = BackgroundRegister.ArcadeIntroBackgrounds.SelectUnlocked(true).ToList().Where(x => Core.Events.GetFlag(x.info.activeFlag)).ToList();

        if (activeBackgrounds.Count == 0)
            return;

        if (activeBackgrounds.Count > 1)
        {
            ModLog.Warn($"More than one ArcadeIntroBackgrounds are active simultaneously!");
        }

        // enable mod backgrounds whose corresponding activeFlag is true
        if (activeBackgrounds.Any(x => x.info.disablesVanillaCounterpart))
        {
            PatchController.SetVanillaCounterpartActive<ArcadeIntroBackground>(false);
        }
        activeBackgrounds.ForEach(x => x.SetActive(true));
    }

    public static void Postfix()
    {
        GameObject blackBackground = GameObject.Find("Game UI/Content/UI_INTRODEMAKE/black-bg");
        blackBackground.SetActive(true);
        blackBackground.GetComponent<Image>().enabled = true;
        List<ArcadeIntroBackground> activeBackgrounds = BackgroundRegister.ArcadeIntroBackgrounds.SelectUnlocked(true).ToList().Where(x => Core.Events.GetFlag(x.info.activeFlag)).ToList();

        if (activeBackgrounds.Count == 0)
            return;

        foreach (ArcadeIntroBackground background in activeBackgrounds)
        {
            Color tempColor = background.GameObj.GetComponent<Image>().color;
            background.GameObj.GetComponent<Image>().color = new Color(tempColor.r, tempColor.g, tempColor.b, 0f);
            UIController.instance.StartCoroutine(FadeArcadeBackgroundCoroutine(background, 1f, 2f));
        }
    }

    private static IEnumerator FadeArcadeBackgroundCoroutine(
        ArcadeIntroBackground background,
        float endValue,
        float duration)
    {
        Image image = background.GameObj.GetComponent<Image>();
        float startValue = image.color.a;
        float alphaIncrement = (endValue - startValue) / (duration / Time.deltaTime);

        while (!Mathf.Approximately(image.color.a, endValue))
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + alphaIncrement);
            yield return null;
        }
        GameObject blackBackground = GameObject.Find("Game UI/Content/UI_INTRODEMAKE/black-bg");
        blackBackground.SetActive(false);
        blackBackground.GetComponent<Image>().enabled = false;
        yield break;
    }
}

[HarmonyPatch(typeof(IntroDemakeWidget), "Close")]
class IntroDemakeWidget_Close_HideArcadeIntroBackground_Patch
{
    public static void Postfix()
    {
        PatchController.SetVanillaCounterpartActive<ArcadeIntroBackground>(true);
    }
}