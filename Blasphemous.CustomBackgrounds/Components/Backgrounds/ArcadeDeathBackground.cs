using Blasphemous.CustomBackgrounds.Extensions;
using Blasphemous.ModdingAPI.Files;
using UnityEngine;

namespace Blasphemous.CustomBackgrounds.Components.Backgrounds;

/// <summary>
/// Custom death screen background object for arcade mini-game
/// </summary>
public class ArcadeDeathBackground : BaseBackground
{
    /// <inheritdoc/>
    protected override bool ShouldShowPopup => false;

    /// <inheritdoc/>
    public ArcadeDeathBackground(
        FileHandler fileHandler,
        BaseBackgroundInfo backgroundInfo)
        : base(fileHandler, backgroundInfo)
    { }

    /// <inheritdoc/>
    public ArcadeDeathBackground(
        FileHandler fileHandler,
        string backgroundInfoJsonFileLocation)
        : this(fileHandler, fileHandler.LoadDataAsJson<BaseBackgroundInfo>(backgroundInfoJsonFileLocation))
    { }

    /// <inheritdoc/>
    protected internal override void SetGameObjectLayer()
    {
        Transform targetTransform = GameObject.Find($"Game UI/Content/UI_DEAD_SCREEN/DemakeEdition").transform;
        int index = GameObject.Find($"Game UI/Content/UI_DEAD_SCREEN/DemakeEdition/DeathMessage").transform.GetSiblingIndex();

        gameObj.transform.SetParent(targetTransform, false);
        gameObj.transform.SetSiblingIndex(info.blocksVanillaCounterpart
            ? index + 1
            : index - 1);
    }
}
