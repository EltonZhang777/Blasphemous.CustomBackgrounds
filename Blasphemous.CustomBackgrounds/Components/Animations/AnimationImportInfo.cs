
namespace Blasphemous.CustomBackgrounds.Components.Animations;

public class AnimationImportInfo
{
    public int Width { get; }
    public int Height { get; }
    public float SecondsPerFrame { get; }

    public AnimationImportInfo(string name, string filePath, int width, int height, float secondsPerFrame)
    {
        Width = Main.Validate(width, x => x > 0);
        Height = Main.Validate(height, x => x > 0);
        SecondsPerFrame = Main.Validate(secondsPerFrame, x => x > 0);
    }
}
