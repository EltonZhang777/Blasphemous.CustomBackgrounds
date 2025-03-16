using Blasphemous.ModdingAPI.Files;
using UnityEngine;

namespace Blasphemous.CustomBackgrounds.Components.Backgrounds;

/// <summary>
/// Custom main menu background object
/// </summary>
public class MainMenuBackground : BaseBackground
{
    /// <inheritdoc/>
    protected override bool ShouldShowPopup => true;

    /// <inheritdoc/>
    public MainMenuBackground(
        FileHandler fileHandler,
        BaseBackgroundInfo backgroundInfo)
        : base(fileHandler, backgroundInfo)
    { }

    /// <inheritdoc/>
    public MainMenuBackground(
        FileHandler fileHandler,
        string backgroundInfoJsonFileLocation)
        : base(fileHandler, backgroundInfoJsonFileLocation)
    { }

    /// <inheritdoc/>
    protected internal override void SetGameObjectLayer()
    {
        Transform targetTransform = GameObject.Find($"Game UI/Content/UI_MAINMENU/Menu").transform;
        gameObj.transform.SetParent(targetTransform, false);
        int index = GameObject.Find($"Game UI/Content/UI_MAINMENU/Menu/StaticBackground").transform.GetSiblingIndex();
        gameObj.transform.SetSiblingIndex(index + 1);
    }
}
