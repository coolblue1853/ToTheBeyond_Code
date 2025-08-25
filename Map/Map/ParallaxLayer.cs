using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField, Range(0f, 1f)] private float _parallaxFactorX = 0.5f;
    [SerializeField, Range(0f, 1f)] private float _parallaxFactorY = 0f;

    private Vector3 _previousCamPos;

    void Start()
    {
        if (_cameraTransform == null)
            _cameraTransform = Camera.main.transform;

        _previousCamPos = _cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 delta = _cameraTransform.position - _previousCamPos;
        
        float moveX = delta.x * _parallaxFactorX;
        float moveY = delta.y * _parallaxFactorY;
        
        transform.position += new Vector3(moveX, moveY, 0);
        _previousCamPos = _cameraTransform.position;
    }
    
}

