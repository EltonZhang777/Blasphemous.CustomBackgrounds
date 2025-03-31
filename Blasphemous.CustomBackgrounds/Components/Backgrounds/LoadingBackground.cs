using Blasphemous.CustomBackgrounds.Extensions;
using Blasphemous.ModdingAPI.Files;
using UnityEngine;

namespace Blasphemous.CustomBackgrounds.Components.Backgrounds;

/// <summary>
/// Custom loading screen background object
/// </summary>
public class LoadingBackground : BaseBackground
{
    /// <inheritdoc/>
    protected override bool ShouldShowPopup => false;

    /// <inheritdoc/>
    public LoadingBackground(
        FileHandler fileHandler,
        BaseBackgroundInfo backgroundInfo)
        : base(fileHandler, backgroundInfo)
    {
    }

    /// <inheritdoc/>
    public LoadingBackground(
        FileHandler fileHandler,
        string backgroundInfoJsonFileLocation)
        : this(fileHandler, fileHandler.LoadDataAsJson<BaseBackgroundInfo>(backgroundInfoJsonFileLocation))
    { }

    /// <inheritdoc/>
    protected internal override void SetGameObjectLayer()
    {
        Transform targetTransform = GameObject.Find($"Game UI/Content/UI_LOADING").transform;
        gameObj.transform.SetParent(targetTransform, false);
        int index = GameObject.Find($"Game UI/Content/UI_LOADING/Icon").transform.GetSiblingIndex();
        gameObj.transform.SetSiblingIndex(info.blocksVanillaCounterpart
            ? index + 1
            : index - 1);
    }
}
