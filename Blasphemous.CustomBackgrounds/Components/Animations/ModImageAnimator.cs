using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.CustomBackgrounds.Components.Animations;

[RequireComponent(typeof(SpriteRenderer))]
public class ModImageAnimator : MonoBehaviour
{
    private AnimationInfo _animation;
    private float _nextUpdateTime;
    private int _currentIdx;
    private Image image;

    public AnimationInfo Animation
    {
        get => _animation;
        set
        {
            _animation = value;
            if (value != null)
            {
                image.sprite = _animation.Sprites[0];
                _nextUpdateTime = Time.time + _animation.SecondsPerFrame;
                _currentIdx = 0;
            }
        }
    }

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (_animation == null || Time.time < _nextUpdateTime)
            return;

        if (++_currentIdx >= _animation.Sprites.Length - 1)
            _currentIdx = 0;

        image.sprite = _animation.Sprites[_currentIdx];
        _nextUpdateTime = Time.time + _animation.SecondsPerFrame;
    }
}
