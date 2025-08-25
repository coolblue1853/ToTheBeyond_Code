using UnityEngine;

public class BackgroundSwapper : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _backgroundRenderer;
    [SerializeField] private Sprite[] _backgrounds;

    public void SetBackground(int index)
    {
        if (index >= 0 && index < _backgrounds.Length)
        {
            _backgroundRenderer.sprite = _backgrounds[index];
        }
        else
        {
            Debug.LogWarning("Background index out of range");
        }
    }

    public void SetBackground(Sprite sprite)
    {
        if (sprite != null)
            _backgroundRenderer.sprite = sprite;
    }
}
