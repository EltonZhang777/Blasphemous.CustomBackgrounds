using Blasphemous.CustomBackgrounds.Extensions;
using Blasphemous.ModdingAPI.Files;
using UnityEngine;

namespace Blasphemous.CustomBackgrounds.Components.Backgrounds;

/// <summary>
/// Custom loading screen background object for arcade mini-game
/// </summary>
public class ArcadeLoadingBackground : BaseBackground
{
    /// <inheritdoc/>
    protected override bool ShouldShowPopup => false;

    /// <inheritdoc/>
    public ArcadeLoadingBackground(
        FileHandler fileHandler,
        BaseBackgroundInfo backgroundInfo)
        : base(fileHandler, backgroundInfo)
    {
    }

    /// <inheritdoc/>
    public ArcadeLoadingBackground(
        FileHandler fileHandler,
        string backgroundInfoJsonFileLocation)
        : this(fileHandler, fileHandler.LoadDataAsJson<BaseBackgroundInfo>(backgroundInfoJsonFileLocation))
    { }

    /// <inheritdoc/>
    protected internal override void SetGameObjectLayer()
    {
        Transform targetTransform = GameObject.Find($"Game UI/Content/UI_LOADING_DEMAKE").transform;
        int index = GameObject.Find($"Game UI/Content/UI_LOADING_DEMAKE/Icon").transform.GetSiblingIndex();

        gameObj.transform.SetParent(targetTransform, false);
        gameObj.transform.SetSiblingIndex(info.blocksVanillaCounterpart
            ? index + 1
            : index - 1);
    }
}
