using System.Collections;
using UnityEngine;

public class EnemyMissile : MonoBehaviour
{
    [SerializeField] private float _speed = 7f; // 미사일 비행 속도
    [SerializeField] private float _riseDuration = 0.5f; // 상승하는 시간
    [SerializeField] private float _pauseDuration = 0.5f; // 멈춰 있는 시간
    [SerializeField] private float _lifeTime = 5f; // 전체 시간

    private Rigidbody2D _rb;
    private Transform _target;

    private int _phase = 0; // 현재 단계 (0: 상승, 1: 정지, 2: 발사)
    private float _phaseStartTime; // 현재 단계 시작 시간 기록

    private float _initialDirection = 1f; // 초기 방향
    private Vector2 _finalDirection; // 실제 발사 방향 
    private bool _initialized = false; // Initialize 호출 여부 체크
    private GameObject _originalPrefab; // 오브젝트 풀 반환 시 참조용 원본
    public void Initialize(Transform target, float direction, GameObject returnRef = null)
    {
        _target = target;
        _initialDirection = Mathf.Sign(direction);
        _originalPrefab = returnRef;
        _initialized = true;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        if (_initialized)
        {
            // 초기 상태 설정
            _phase = 0;
            _phaseStartTime = Time.time;
            // 자동 반환 타이머 시작
            StartCoroutine(AutoReturnCoroutine(_lifeTime));
        }
    }

    private void FixedUpdate()
    {
        switch (_phase)
        {
            case 0: HandleRising(); break; // 상승 단계 
            case 1: HandlePause(); break; // 정지 단계
            case 2: HandleFire(); break; // 타겟 방향으로 발사
        }
    }
    // 미사일 상승
    private void HandleRising()
    {
        Vector2 riseDirection = new Vector2(1f * _initialDirection, 1f).normalized;
        _rb.velocity = riseDirection * _speed * 0.5f;

        if (Time.time - _phaseStartTime >= _riseDuration)
        {
            _rb.velocity = Vector2.zero;
            _phaseStartTime = Time.time;
            _phase = 1;
        }
    }
    // 미사일 정지 상태 유지
    private void HandlePause()
    {
        if (Time.time - _phaseStartTime >= _pauseDuration)
        {
            if (_target != null)
            {
                _finalDirection = ((Vector2)_target.position - _rb.position).normalized;
                float angle = Mathf.Atan2(_finalDirection.y, _finalDirection.x) * Mathf.Rad2Deg - 90f;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            _phase = 2;
        }
    }
    // 미사일 위쪽 방향으로 발사
    private void HandleFire()
    {
        _rb.velocity = transform.up * _speed;
    }
    // 플레이어 충돌 시 데미지를 주고 오브젝트 제거
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                playerHealth.TakeDamage(20);
            }
            Destroy(gameObject);
        }
    }
    // 일정 시간이 지나면 오브젝트 풀로 반환
    private IEnumerator AutoReturnCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        ObjectPooler.ReturnToPool(_originalPrefab, gameObject);
    }
}