namespace Blasphemous.CustomBackgrounds.Components.Backgrounds;

/// <summary>
/// Serializable class containing information of <see cref="LoadingBackground"/> import settings
/// </summary>
public class LoadingBackgroundInfo : BaseBackgroundInfo
{
    /// <summary>
    /// Background will be activated when this flag is true
    /// </summary>
    public string activeFlag;

    /// <summary>
    /// Whether to show the vanilla spinning icon when loading
    /// </summary>
    public bool disableVanillaLoadingIcon = false;
}
