using UnityEngine;

public class PinchBirdManager : MonoBehaviour
{
    [Header("핀치새 프리팹과 소환 위치")]
    [SerializeField] private Transform _pinchBirdTransform; // 씬에 미리 배치된 오브젝트
    [SerializeField] private Transform _spawnPoint;

    [SerializeField] private float _offsetX = -2f;
    [SerializeField] private float _offsetY = 3f;
    
    private float _speed = 7f;
    [SerializeField] private Rigidbody2D _rb;
    private Transform _player;
    
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;

        if (_pinchBirdTransform != null)
        {
            Vector3 startPos = _spawnPoint != null ? _spawnPoint.position : _player.position + Vector3.up * _offsetY;
            _rb.position = startPos;
        }
    }

    private void FixedUpdate()
    {
        if (_player == null || _pinchBirdTransform == null)
            return;

        Vector3 targetPos = _player.position + new Vector3(_offsetX, _offsetY, 0f);
        Vector2 newPosition = Vector2.MoveTowards(_rb.position, targetPos, _speed * Time.fixedDeltaTime);
        
        float direction = targetPos.x - _rb.position.x;
        
        if (Mathf.Abs(direction) > 0.01f) 
        {
            Vector3 scale = _pinchBirdTransform.localScale;
            scale.x = direction > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            _pinchBirdTransform.localScale = scale;
        }
        
        _rb.MovePosition(newPosition);
    }

}