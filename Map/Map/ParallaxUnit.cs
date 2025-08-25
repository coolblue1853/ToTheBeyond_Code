using UnityEngine;

public class ParallaxUnit : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _spriteWidth;

    private void Start()
    {
        if (_cameraTransform == null)
            _cameraTransform = Camera.main.transform;

        if (_spriteWidth == 0f)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                _spriteWidth = sr.bounds.size.x;
        }
    }

    private void Update()
    {
        float distance = _cameraTransform.position.x - transform.position.x;

        if (Mathf.Abs(distance) >= _spriteWidth)
        {
            float offset = _spriteWidth * 2f * Mathf.Sign(distance);
            transform.position += new Vector3(offset, 0f, 0f);
        }
    }
}