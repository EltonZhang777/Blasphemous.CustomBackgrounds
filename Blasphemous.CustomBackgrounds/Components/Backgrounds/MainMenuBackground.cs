using Blasphemous.CustomBackgrounds.Extensions;
using Blasphemous.ModdingAPI.Files;
using UnityEngine;

namespace Blasphemous.CustomBackgrounds.Components.Backgrounds;

/// <summary>
/// Custom main menu background object
/// </summary>
public class MainMenuBackground : BaseBackground
{
    protected override bool ShouldShowPopup => true;
    /// <summary>
    /// Constructor for custom background object
    /// </summary>
    /// <param name="fileHandler">The registering mod's FileHandler</param>
    /// <param name="backgroundInfo">Deserialized info object</param>
    public MainMenuBackground(
        FileHandler fileHandler,
        BackgroundInfo backgroundInfo)
        : base(fileHandler, backgroundInfo)
    { }

    /// <summary>
    /// Recommended constructor for custom background object. Handles JSON deserialization on its own.
    /// </summary>
    /// <param name="fileHandler">The registering mod's FileHandler</param>
    /// <param name="backgroundInfoJsonFileLocation">JSON file location of `backgroundInfo`</param>
    public MainMenuBackground(
        FileHandler fileHandler,
        string backgroundInfoJsonFileLocation)
        : this(fileHandler, fileHandler.LoadDataAsJson<BackgroundInfo>(backgroundInfoJsonFileLocation))
    { }

    protected internal override void SetGameObjectLayer()
    {
        Transform targetTransform = GameObject.Find($"Game UI/Content/UI_MAINMENU/Menu").transform;
        gameObj.transform.SetParent(targetTransform, false);
        int index = GameObject.Find($"Game UI/Content/UI_MAINMENU/Menu/StaticBackground").transform.GetSiblingIndex();
        gameObj.transform.SetSiblingIndex(index + 1);
    }
}
