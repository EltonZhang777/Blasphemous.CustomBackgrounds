using Blasphemous.CheatConsole;
using Blasphemous.CustomBackgrounds.Components.Backgrounds;
using Framework.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blasphemous.CustomBackgrounds.Commands;

internal class BackgroundCommand : ModCommand
{
    protected override string CommandName => "background";

    protected override bool AllowUppercase => true;

    protected override Dictionary<string, Action<string[]>> AddSubCommands()
    {
        Dictionary<string, Action<string[]>> result = new()
        {
            { "help", SubCommand_Help },
            { "list", SubCommand_List },
            { "unlock", SubCommand_Unlock },
            { "lock", SubCommand_Lock },
            { "activate", SubCommand_Activate },
            { "deactivate", SubCommand_Deactivate }
        };
#if DEBUG
        result.Add("showpopup", SubCommand_ShowPopUp);
#endif

        return result;
    }

    private void SubCommand_Help(string[] parameters)
    {
        if (!ValidateParameterList(parameters, 0))
            return;

        Write($"Available {CommandName} commands:");
        Write($"{CommandName} list : list all loaded backgrounds");
        Write($"{CommandName} list [unlocked/locked] : list all unlocked/locked backgrounds");
        Write($"{CommandName} unlock [patchName] : unlock the specified background");
        Write($"{CommandName} lock [patchName] : lock the specified background");
        Write($"{CommandName} activate [patchName] : activate and use the specified background.");
        Write($"{CommandName} activate [patchName] : deactivate the specified background. (does not work for main menu backgrounds)");
#if DEBUG
        Write($"{CommandName} showpopup [patchName] : (debug use) show the unlock popup of specified background");
#endif
    }

    private void SubCommand_List(string[] parameters)
    {
        if (!ValidateParameterList(parameters, [0, 1]))
            return;

        bool hasAny = false;
        if (parameters.Length == 0)
        {
            Write($"All loaded backgrounds: ");
            foreach (BaseBackground background in BackgroundRegister.Backgrounds)
            {
                hasAny = true;
                string unlockState = background.isUnlocked ? "yes" : "no";
                Write($"  {background.info.name}\n    [unlocked?: {unlockState}]\n    [type: {background.GetType()}]");
            }
            if (!hasAny)
            {
                Write($"No background is found!");
            }
        }
        else
        {
            if (parameters[0].Equals("unlocked"))
            {
                Write($"All unlocked backgrounds: ");
                foreach (BaseBackground background in BackgroundRegister.Backgrounds.Where(x => x.isUnlocked == true))
                {
                    hasAny = true;
                    Write($"  {background.info.name}");
                }
                if (!hasAny)
                {
                    Write($"No background is found!");
                }
            }
            else if (parameters[0].Equals("locked"))
            {
                Write($"All locked backgrounds: ");
                foreach (BaseBackground background in BackgroundRegister.Backgrounds.Where(x => x.isUnlocked == false))
                {
                    hasAny = true;
                    Write($"  {background.info.name}");
                }
                if (!hasAny)
                {
                    Write($"No background is found!");
                }
            }
            else
            {
                Write($"Unknown parameter #0 `{parameters[0]}`!");
            }
        }
    }

    private void SubCommand_Unlock(string[] parameters)
    {
        if (!ValidateParameterList(parameters, 1))
            return;

        string backgroundName = parameters[0];
        if (!BackgroundExists(backgroundName))
            return;

        BaseBackground targetBackground = BackgroundRegister.Backgrounds.First(x => x.info.name.Equals(backgroundName));
        targetBackground.SetUnlocked(true);
    }

    private void SubCommand_Lock(string[] parameters)
    {
        if (!ValidateParameterList(parameters, 1))
            return;

        string backgroundName = parameters[0];
        if (!BackgroundExists(backgroundName))
            return;

        BaseBackground targetBackground = BackgroundRegister.Backgrounds.First(x => x.info.name.Equals(backgroundName));
        targetBackground.SetUnlocked(false);
    }

    private void SubCommand_ShowPopUp(string[] parameters)
    {
        if (!ValidateParameterList(parameters, 1))
            return;

        string backgroundName = parameters[0];
        if (!BackgroundExists(backgroundName))
            return;

        BaseBackground targetBackground = BackgroundRegister.Backgrounds.First(x => x.info.name.Equals(backgroundName));
        targetBackground.ShowUnlockPopUp();
    }

    private void SubCommand_Activate(string[] parameters)
    {
        if (!ValidateParameterList(parameters, 1))
            return;

        string backgroundName = parameters[0];
        if (!BackgroundExists(backgroundName))
            return;

        BaseBackground targetBackground = BackgroundRegister.Backgrounds.First(x => x.info.name.Equals(backgroundName));
        switch (targetBackground)
        {
            case MainMenuBackground mainMenuBackground:
                Main.CustomBackgrounds.backgroundSaveData.currentModMainMenuBg = mainMenuBackground.info.name;
                break;
            case DeathBackground deathBackground:
                Core.Events.SetFlag(deathBackground.activeFlag, true);
                break;
            case LoadingBackground loadingBackground:
                Core.Events.SetFlag(loadingBackground.activeFlag, true);
                break;
            default:
                Write($"Unsupported background type!");
                break;
        }
    }

    private void SubCommand_Deactivate(string[] parameters)
    {
        if (!ValidateParameterList(parameters, 1))
            return;

        string backgroundName = parameters[0];
        if (!BackgroundExists(backgroundName))
            return;

        BaseBackground targetBackground = BackgroundRegister.Backgrounds.First(x => x.info.name.Equals(backgroundName));
        switch (targetBackground)
        {
            case DeathBackground deathBackground:
                Core.Events.SetFlag(deathBackground.activeFlag, false);
                break;
            case LoadingBackground loadingBackground:
                Core.Events.SetFlag(loadingBackground.activeFlag, false);
                break;
            default:
                Write($"Unsupported background type!");
                break;
        }
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

    private bool BackgroundExists(string name)
    {
        if (!BackgroundRegister.Exists(name))
        {
            Write($"Background `{name}` not found!");
            return false;
        }
        return true;
    }
}
