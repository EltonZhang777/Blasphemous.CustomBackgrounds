using Blasphemous.CustomBackgrounds.Extensions;
using Blasphemous.ModdingAPI.Files;
using UnityEngine;

namespace Blasphemous.CustomBackgrounds.Components.Backgrounds;

/// <summary>
/// Custom death screen background object
/// </summary>
public class DeathBackground : BaseBackground
{
    /// <inheritdoc/>
    protected override bool ShouldShowPopup => false;

    /// <inheritdoc/>
    public DeathBackground(
        FileHandler fileHandler,
        BaseBackgroundInfo backgroundInfo)
        : base(fileHandler, backgroundInfo)
    {
    }

    /// <inheritdoc/>
    public DeathBackground(
        FileHandler fileHandler,
        string backgroundInfoJsonFileLocation)
        : this(fileHandler, fileHandler.LoadDataAsJson<BaseBackgroundInfo>(backgroundInfoJsonFileLocation))
    { }

    /// <inheritdoc/>
    protected internal override void SetGameObjectLayer()
    {
        Transform targetTransform = GameObject.Find($"Game UI/Content/UI_DEAD_SCREEN/Main Interface").transform;
        gameObj.transform.SetParent(targetTransform, false);
        int index = GameObject.Find($"Game UI/Content/UI_DEAD_SCREEN/Main Interface/DeathMessage").transform.GetSiblingIndex();
        gameObj.transform.SetSiblingIndex(info.blocksVanillaCounterpart
            ? index + 1
            : index - 1);
    }
}
