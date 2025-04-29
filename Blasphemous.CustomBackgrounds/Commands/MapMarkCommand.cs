using Blasphemous.CheatConsole;
using Blasphemous.CustomBackgrounds.Components.Backgrounds;
using Blasphemous.CustomBackgrounds.Components.Map;
using Blasphemous.ModdingAPI;
using Gameplay.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Blasphemous.CustomBackgrounds.Commands;

internal class MapMarkCommand : ModCommand
{
    protected override string CommandName => "mapmark";

    protected override bool AllowUppercase => true;

    protected override Dictionary<string, Action<string[]>> AddSubCommands()
    {
        Dictionary<string, Action<string[]>> result = new()
        {
            { "help", SubCommand_Help },
            { "list", SubCommand_List },
            { "clear", SubCommand_Clear },
        };
#if DEBUG
        result.Add("exportjson", SubCommand_ExportToJson);
        result.Add("current_pointing_cell", SubCommand_ShowCurrentPointingCellKey);
        result.Add("debug_coroutine", SubCommand_DebugCoroutine);
#endif

        return result;
    }

    private void SubCommand_Help(string[] parameters)
    {
        if (!ValidateParameterList(parameters, 0))
            return;

        Write($"Available {CommandName} commands:");
        Write($"{CommandName} list registered: list all registered map marks");
        Write($"{CommandName} list current: list all modded map marks currently marked on this save");
        Write($"{CommandName} clear: clear all modded map marks currently marked on this save");
#if DEBUG
        Write($"{CommandName} exportjson [markName] : (debug use) export the info of specified map mark to JSON file");
        Write($"{CommandName} current_pointing_cell : (debug use) show the current cell coordinates at which the cursor is pointing at");
        Write($"{CommandName} debug_coroutine : (debug use) show debug coroutine of marking three calvary bosses with test map marks");
#endif
    }

    private void SubCommand_List(string[] parameters)
    {
        if (!ValidateParameterList(parameters, 1))
            return;

        string actionType = parameters[0];
        bool hasAny = false;
        if (actionType.Equals("registered"))
        {
            Write($"All {actionType} map marks: ");
            foreach (ModMapMark mapMark in ModMapManager.registeredMarks.Values)
            {
                hasAny = true;
                Write($"  {mapMark.info.id}");
            }
            if (!hasAny)
            {
                Write($"No map mark is found!");
            }
        }
        else if (actionType.Equals("current"))
        {
            Write($"All {actionType} map marks: ");
            foreach (ModMapMark mapMark in ModMapManager.modMapMarkInstances)
            {
                hasAny = true;
                Write($"  `{mapMark.info.id}` marked at {mapMark.cellKey}");
            }
            if (!hasAny)
            {
                Write($"No map mark is found!");
            }
        }
        else
        {
            Write($"Unknown query type `{actionType}`!");
        }
    }

    private void SubCommand_Clear(string[] parameters)
    {
        if (!ValidateParameterList(parameters, 0))
            return;

        ModMapManager.ClearAllMapMarks();
    }

    private void SubCommand_ExportToJson(string[] parameters)
    {
        // WIP
        throw new NotImplementedException();

        if (!ValidateParameterList(parameters, 1))
            return;

        string backgroundName = parameters[0];
        if (!ModMapMarkExists(backgroundName))
            return;

        string exportPath = Path.Combine(Main.CustomBackgrounds.FileHandler.ContentFolder, $"exported--{backgroundName}.json");
        File.WriteAllText(
            exportPath,
            JsonConvert.SerializeObject(BackgroundRegister.AtName(backgroundName).info, Formatting.Indented));
        Write($"Successfully exported `{backgroundName}` info to `{exportPath}`!");
    }

    private void SubCommand_DebugCoroutine(string[] parameters)
    {
        if (!ValidateParameterList(parameters, 0))
            return;

#if DEBUG
        ModLog.Warn($"attempting to start coroutine!");
#endif
        UIController.instance.StartCoroutine(Patches.DialogStart_DialogEnded_ShowThreeCalvaryBossesMapMarks_Patch.ShowThreeCalvaryBossesMapMarksCoroutine());
    }

    private void SubCommand_ShowCurrentPointingCellKey(string[] parameters)
    {
        if (!ValidateParameterList(parameters, 0))
            return;

        if (!ModMapManager.VanillaPauseWidget.IsActive())
        {
            Write($"Map is not open!");
            return;
        }

        Write($"current cursor pointing at: {ModMapManager.VanillaMapRenderer.GetCenterCell()}");
    }

    private bool ValidateParameterList(string[] parameters, List<int> validParameterLengths)
    {
        if (!validParameterLengths.Contains(parameters.Length))
        {
            StringBuilder sb = new();
            sb.Append($"This command takes ");
            for (int i = 0; i < validParameterLengths.Count; i++)
            {
                sb.Append($"{i} ");
                if (i != validParameterLengths.Count - 1)
                    sb.Append("or ");
            }
            sb.Append($"parameters.  You passed {parameters.Length}");
            Write(sb.ToString());

            return false;
        }

        return true;
    }

    private bool ModMapMarkExists(string id)
    {
        if (!ModMapManager.MapMarkTypeExists(id))
        {
            Write($"ModMapMark of type `{id}` not found!");
            return false;
        }
        return true;
    }
}
