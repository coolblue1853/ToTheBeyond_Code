using UnityEngine;

public class Ghost : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    private float fadeTime = 0.5f;
    private float fadeTimer;
    private Color startColor;

    public void Init(Sprite sprite, Vector3 position, Vector3 scale, Color color, float lifetime)
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = sprite;
        transform.position = position;
        transform.localScale = scale;

        startColor = color;
        fadeTime = lifetime;
        fadeTimer = 0f;
        spriteRenderer.color = startColor;

        gameObject.SetActive(true);
    }

    private void Update()
    {
        fadeTimer += Time.deltaTime;
        float t = fadeTimer / fadeTime;

        if (t >= 1f)
        {
            Destroy(gameObject); // ← 완전 파괴
        }
        else
        {
            Color c = startColor;
            c.a = Mathf.Lerp(startColor.a, 0, t);
            spriteRenderer.color = c;
        }
    }
}
