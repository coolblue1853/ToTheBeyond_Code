using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKnockback : MonoBehaviour
{
    [SerializeField] private float _knockbackPowerX = 5f;
    [SerializeField] private float _knockbackPowerY = 2f;
    [SerializeField] private float _knockbackDuration = 0.2f;
    [SerializeField] private int _superArmorThreshold = 3;
    [SerializeField] private float _stunDuration = 3;
    [SerializeField] private bool _isSuperArmorMonster = false;
    [SerializeField] private bool _ableSuperArmorMonster = false; // 어떤 조건에서도 슈퍼아머가 되지 않는 몬스터
    public SharedBool isKnockedBack;
    [Header("끝 체크")]
    [SerializeField] private Vector2 _edgeRayOffset = new Vector2(0.3f, 0f);
    [SerializeField] private float _groundCheckDistance = 4f;
    [SerializeField] private LayerMask _groundLayer;
    private Rigidbody2D _rb;
    private int _hitCount;
    private BehaviorTree _bt;
    private EnemyHealth _health;
    private bool _isLowerHealth = false;
    private EnemyController _controller;
    private EnemyHandler _handler;
    public bool IsSuperArmor { get; private set; }
    private void Awake()
    {
        _health = GetComponent<EnemyHealth>();
        _rb = GetComponent<Rigidbody2D>();
        _bt = GetComponent<BehaviorTree>();
        _controller = GetComponent<EnemyController>();
        _handler = GetComponent<EnemyHandler>();
        _groundLayer = LayerMask.GetMask("Ground");
        if (_bt != null)
            isKnockedBack = (SharedBool)_bt.GetVariable("isKnockedBack");
    }

    public void ReceiveHit(Vector3 attackerPosition)
    {
        _hitCount++;

        if (_isSuperArmorMonster || ( _ableSuperArmorMonster && (_hitCount >= _superArmorThreshold || _health.CurrentHealth < _controller.enemy.maxHealth / 3 || IsSuperArmor)))
        {
            _isLowerHealth = true;
            ResetHitCount();
            return;
        }
        
         _handler.ApplySuperArmorStun(_stunDuration);
        Vector2 dir = (transform.position - attackerPosition).normalized;
        _rb.velocity = Vector2.zero;
        
        
        if (!IsGroundInKnockbackDirection(dir.x))
        {
            return;
        }
        
        // X축 넉백
        Vector2 knockbackX = new Vector2(dir.x * _knockbackPowerX, 0f);
        _rb.AddForce(knockbackX, ForceMode2D.Impulse);

        // 딜레이 후 Y축 넉백
        StartCoroutine(ApplyYKnockback());
        
        if (_bt != null)
        {
            isKnockedBack.Value = true;
            StartCoroutine(ResetKnockbackAfterDelay(_knockbackDuration));
        }
    }

    private IEnumerator ResetKnockbackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isKnockedBack.Value = false;
    }
    private IEnumerator ApplyYKnockback()
    {
        yield return new WaitForSeconds(0.05f); // 살짝 딜레이
        
        Vector2 knockbackY = new Vector2(0f, _knockbackPowerY);
        _rb.AddForce(knockbackY, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        _rb.velocity = Vector2.zero;
    }
    public void ResetHitCount() => _hitCount = 0;
    
    public void EnableSuperArmor()
    {
        IsSuperArmor = true;
    }

    public void DisableSuperArmor()
    {
        IsSuperArmor = false;
    }
    
    private bool IsGroundInKnockbackDirection(float dirX)
    {
        Vector3 origin = transform.position + new Vector3(_edgeRayOffset.x * Mathf.Sign(dirX), _edgeRayOffset.y, 0f);
        return Physics2D.Raycast(origin, Vector2.down, _groundCheckDistance, _groundLayer);
    }
}
