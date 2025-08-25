using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class EnemyMeteorProjectile : MonoBehaviour
{
    private Vector3 _targetPosition;
    private float _duration;
    private float _timer;

    private bool _launched = false;

    public GameObject hitEffectPrefab;
    public int damage = 0;
    private bool _impacted = false;
    private CinemachineImpulseSource _impulse; 
    private void Start()
    {
        _impulse  = GetComponentInChildren<CinemachineImpulseSource>();
    }

    public void Launch(Vector3 targetPosition, float duration)
    {
        _targetPosition = targetPosition;
        _duration = duration;
        _timer = 0f;
        _launched = true;
    }

    private void Update()
    {
        if (!_launched || _impacted) return;

        _timer += Time.deltaTime;
        float t = Mathf.Clamp01(_timer / _duration);
        transform.position = Vector3.Lerp(transform.position, _targetPosition, t);
       
        RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.up * 0.3f, Vector2.down, 1f, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            GameObject hitObj = hit.collider.gameObject;

            // Ground에 충돌한 경우
            if (hitObj.layer == LayerMask.NameToLayer("Ground"))
            {
                float distanceToTarget = Vector2.Distance(hit.point, _targetPosition);
                if (distanceToTarget < 0.5f)
                {
                    OnImpact(hit.point);
                }
            }
            // Player에 충돌한 경우
            else if (hitObj.CompareTag("Player"))
            {
                if (hitObj.TryGetComponent<PlayerHealth>(out var playerHealth))
                {
                    if (!playerHealth.IsInvincible)
                    {
                        OnImpact(hit.point);
                        playerHealth.TakeDamage(20);
                    }
                }
            }
        }
    }

    private void OnImpact(Vector3 effectPos)
    {
        if (_impulse != null)
        {
            _impulse.GenerateImpulse(1f); // 필요하면 강도 조절
        }
        _impacted = true;
        Destroy(gameObject);
        GameObject effect = GameObject.Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        Animator anim = effect.GetComponent<Animator>();
        float duration = anim != null ? anim.GetCurrentAnimatorStateInfo(0).length : 1f;
        GameObject.Destroy(effect, duration);
    }
}
