using System;
using System.Collections;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Vector3 _dir; // 이동 방향
    private int _damage; // 데미지 값
    private float _speed; // 이동 속도
    private GameObject _originalPrefab; // 풀 참조용
    private Animator _animator;
    
    private bool _hasHit = false; // 중복 충돌 방지 플래그
    private bool _useCenterCheck = false; // 중심점 도달 여부 체크 여부
    private Vector3 _center; // 중심점 좌표
    private float _minDistance = 0.1f; // 중심점과의 최소 거리 조건
    // 투사체 초기화
    public void Initialize(Vector3 direction, int damage, float speed = 6f, GameObject returnRef = null, Vector3? center = null)
    {
        _dir = direction.normalized;
        _damage = damage;
        _speed = speed;
        _originalPrefab = returnRef;
        _hasHit = false;

        if (center.HasValue)
        {
            _useCenterCheck = true;
            _center = center.Value;
        }
        else
        {
            _useCenterCheck = false;
        }
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.CrossFade("Appeared", 0.1f);
    }

    // 매 프레임 이동 처리 및 중심점 도달 여부 확인
    private void Update()
    {
        transform.position += _dir * _speed * Time.deltaTime;

        if (_useCenterCheck)
        {
            float distanceToCenter = Vector3.Distance(transform.position, _center);
            if (distanceToCenter <= _minDistance)
            {
                StartCoroutine(AutoReturnCoroutine(0f));
            }
        }
    }
    
    // 플레이어 충돌 시 데미지 부여 후 반환
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasHit) return; // 중복 처리 방지
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                if (!playerHealth.IsInvincible)
                {
                    playerHealth.TakeDamage(_damage);
                    _hasHit = true;
                    StartCoroutine(AutoReturnCoroutine(0f));
                }
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            StartCoroutine(AutoReturnCoroutine(0f));
        }
        
    }
    // 외부에서 일정 시간 후 자동 반환을 시작할 수 있음
    public void StartAutoReturn(float delay)
    {
        StartCoroutine(AutoReturnCoroutine(delay));
    }
    // 일정 시간이 지나면 자동으로 오브젝트 풀로 반환
    private IEnumerator AutoReturnCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        _dir = Vector3.zero;
        
        _animator.CrossFade("Disappeared", 0.1f);
        
        yield return null;
        float animLength = _animator.GetCurrentAnimatorStateInfo(0).length;
        
        yield return new WaitForSeconds(animLength);
        ObjectPooler.ReturnToPool(_originalPrefab, gameObject);
    }
}
