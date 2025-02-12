using UnityEngine;

namespace Blasphemous.CustomBackgrounds.Components.Animations;

public class AnimationInfo
{
    public Sprite[] Sprites { get; }
    public float SecondsPerFrame { get; }

    public AnimationInfo(Sprite[] sprites, float secondsPerFrame)
    {
        Sprites = Main.Validate(sprites, x => x != null && x.Length > 0);
        SecondsPerFrame = Main.Validate(secondsPerFrame, x => x > 0);
    }
}
