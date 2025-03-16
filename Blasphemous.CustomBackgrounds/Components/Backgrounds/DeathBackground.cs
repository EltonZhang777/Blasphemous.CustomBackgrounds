using Blasphemous.CustomBackgrounds.Extensions;
using Blasphemous.ModdingAPI.Files;
using UnityEngine;

namespace Blasphemous.CustomBackgrounds.Components.Backgrounds;

/// <summary>
/// Custom death screen background object
/// </summary>
public class DeathBackground : BaseBackground
{
    internal string activeFlag;

    /// <inheritdoc/>
    protected override bool ShouldShowPopup => false;

    /// <inheritdoc/>
    public DeathBackground(
        FileHandler fileHandler,
        DeathBackgroundInfo backgroundInfo)
        : base(fileHandler, backgroundInfo)
    {
        this.activeFlag = backgroundInfo.activeFlag;
    }

    /// <inheritdoc/>
    public DeathBackground(
        FileHandler fileHandler,
        string backgroundInfoJsonFileLocation)
        : this(fileHandler, fileHandler.LoadDataAsJson<DeathBackgroundInfo>(backgroundInfoJsonFileLocation))
    { }

    /// <inheritdoc/>
    protected internal override void SetGameObjectLayer()
    {
        Transform targetTransform = GameObject.Find($"Game UI/Content/UI_DEAD_SCREEN/Main Interface").transform;
        gameObj.transform.SetParent(targetTransform, false);
        int index = GameObject.Find($"Game UI/Content/UI_DEAD_SCREEN/Main Interface/DeathMessage").transform.GetSiblingIndex();
        gameObj.transform.SetSiblingIndex(index + 1);
        gameObj.SetActive(true);
    }
}
