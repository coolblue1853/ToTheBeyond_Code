using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Cinemachine;

public class EnemySlam : MonoBehaviour
{
   
    [SerializeField]private float _dropSpeed = 5f;         // 하강 속도
    [SerializeField]private int _damage = 30; // 플레이어에게 줄 피해량
    [SerializeField] private float _damageRadius = 1.5f;
    [SerializeField] private float _slamDuration = 0.4f; // 슬램 시간
    [SerializeField] private Ease _easeType = Ease.InQuad; // 내리찍는 느낌
    [SerializeField] private GameObject _hitEffectPrefab;
    private float _currentSpeed;
    private float _groundY = -1000f; // 초기값은 충분히 낮게

    private bool _hasSlammed = false;
    private CinemachineImpulseSource _impulseSource; 
    public void SetGroundY(float y)
    {
        _groundY = y;
    }

    private void Start()
    {
        float targetY = _groundY;
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        transform.DOMoveY(targetY, _slamDuration).SetEase(_easeType).SetAutoKill(true).SetLink(gameObject, LinkBehaviour.KillOnDestroy) 
            .OnComplete(() =>
            {
                if (_hasSlammed) return;
                _hasSlammed = true;
                OnSlam(); 
            });
        
        Destroy(gameObject, 4f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasSlammed) return;

        if (other.CompareTag("Player"))
        {
            // 플레이어에게 데미지
            if (other.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                playerHealth.TakeDamage(_damage);
            }

            _hasSlammed = true;
            OnSlam(); 
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (!other.gameObject.name.Contains("Platform"))
            {
                _hasSlammed = true;
                OnSlam(); 
            }
        }
    }

    private void OnSlam()
    {
        Destroy(gameObject);
        GameObject effect = GameObject.Instantiate(_hitEffectPrefab, transform.position, Quaternion.identity);
        Animator anim = effect.GetComponent<Animator>();
        float duration = anim != null ? anim.GetCurrentAnimatorStateInfo(0).length : 1f;
        GameObject.Destroy(effect, duration);
        if (_impulseSource != null)
        {
            _impulseSource.GenerateImpulse(); 
        }
    }
}
