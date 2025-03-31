using Blasphemous.CustomBackgrounds.Extensions;
using Blasphemous.ModdingAPI.Files;
using Gameplay.UI;
using UnityEngine;

namespace Blasphemous.CustomBackgrounds.Components.Backgrounds;

/// <summary>
/// Custom victory screen background object
/// </summary>
public class VictoryBackground : BaseBackground
{
    /// <inheritdoc/>
    protected override bool ShouldShowPopup => false;

    internal UIController.FullMensages victoryType;

    /// <inheritdoc/>
    public VictoryBackground(
        FileHandler fileHandler,
        VictoryBackgroundInfo backgroundInfo)
        : base(fileHandler, backgroundInfo)
    {
        this.victoryType = backgroundInfo.victoryType;
    }

    /// <inheritdoc/>
    public VictoryBackground(
        FileHandler fileHandler,
        string backgroundInfoJsonFileLocation)
        : this(fileHandler, fileHandler.LoadDataAsJson<VictoryBackgroundInfo>(backgroundInfoJsonFileLocation))
    { }

    /// <inheritdoc/>
    protected internal override void SetGameObjectLayer()
    {
        Transform targetTransform = GameObject.Find($"Game UI/Content/UI_FULLMESSAGES/Main Interface/Background").transform;
        gameObj.transform.SetParent(targetTransform, false);
        gameObj.transform.SetSiblingIndex(info.blocksVanillaCounterpart
            ? 3
            : 0);
    }
}
