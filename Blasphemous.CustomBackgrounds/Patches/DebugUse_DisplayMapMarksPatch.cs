using Blasphemous.CustomBackgrounds.Components.Map;
using Blasphemous.ModdingAPI;
using DG.Tweening;
using Framework.Managers;
using Framework.Map;
using Gameplay.UI;
using HarmonyLib;
using System.Collections;
using Tools.Playmaker2.Action;
using UnityEngine;

namespace Blasphemous.CustomBackgrounds.Patches;

// Give the boots when finishing a certain dialog
[HarmonyPatch(typeof(DialogStart), "DialogEnded")]
class DialogStart_DialogEnded_ShowThreeCalvaryBossesMapMarks_Patch
{
    public static void Prefix(string id)
    {
        if (id == "DLG_0114")
        {
            UIController.instance.StartCoroutine(ShowThreeCalvaryBossesMapMarksCoroutine());
        }
    }

    internal static IEnumerator ShowThreeCalvaryBossesMapMarksCoroutine()
    {
        Vector2 worldPosition;
        string sceneName;
        float moveMapCenterDuration = 1f;
        float waitDurationBeforeAddMark = 0.33f;
        float waitDurationAfterAddMark = 1.33f;
        float initialDelay = 0.5f;
        CellKey targetCellKey;
        Tween tween;
        string inputBlocker = "ShowThreeCalvaryBossesMapMarksBlocker";

#if DEBUG
        ModLog.Warn($"opening map!");
#endif
        ModMapManager.ShowMap(true);
#if DEBUG
        ModLog.Warn($"opened map!");
#endif
        //Core.Input.SetBlocker(inputBlocker, true);
        yield return new WaitForSecondsRealtime(initialDelay);

#if DEBUG
        ModLog.Warn($"starting coroutine!");
#endif

        // Ten Piedad
        worldPosition = new(-95f, -95f);
        sceneName = "D01Z04S18";
        yield return MarkAtPositionCoroutine();

        // Charred Visage
        worldPosition = new(-530f, 260f);
        sceneName = "D02Z03S20";
        yield return MarkAtPositionCoroutine();

        // Tres Angustias
        worldPosition = new(-520f, -230f);
        sceneName = "D03Z03S15";  // alternative : D03BZ01S01
        yield return MarkAtPositionCoroutine();
        //Core.Input.SetBlocker(inputBlocker, false);
#if DEBUG
        ModLog.Warn($"ending coroutine!");
#endif

        IEnumerator MarkAtPositionCoroutine()
        {
#if DEBUG
            ModLog.Warn($"perparing to mark cell");
#endif
            targetCellKey = Core.NewMapManager.GetCellKeyFromPosition(worldPosition);
            ModMapManager.RemoveMapMark("test_mark", targetCellKey);
#if DEBUG
            ModLog.Warn($"start marking cell at {targetCellKey}");
#endif

            //Core.NewMapManager.RevealCellInPosition(worldPosition);
            ModMapManager.ForceRevealCell(targetCellKey);
            ModMapManager.ForceRenderMap();
            //ModMapManager.VanillaMapRenderer.SetCenterCell(targetCellKey);  // instantly teleport cursor focus
            // gradually move cursor focus
            tween = ModMapManager.TweenMapCenterToWorldPosition(worldPosition, moveMapCenterDuration);
            yield return tween.WaitForCompletion();

            yield return new WaitForSecondsRealtime(waitDurationBeforeAddMark);
            ModMapManager.AddMapMark("test_mark", targetCellKey);
            ModMapManager.ShowAllModMapMarks(true);
            Core.Audio.PlayOneShot("event:/SFX/UI/BossRushRankS");
            yield return new WaitForSecondsRealtime(waitDurationAfterAddMark);
        }
    }
}