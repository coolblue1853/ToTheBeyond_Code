using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    public EnemyHealth _health;
    private EnemyKnockback _knockback;
    private EnemyStunned _stunned;
    private void Awake()
    {
        _health = GetComponent<EnemyHealth>();
        _knockback = GetComponent<EnemyKnockback>();
        _stunned = GetComponent<EnemyStunned>();
    }

    public void TakeDamage(Vector3 attackerPosition, float damage, bool isCrit = false)
    {
        _health.TakeDamage(damage, isCrit);
        _knockback?.ReceiveHit(attackerPosition);
    }

    public void ApplyDebuff(DebuffEffectSO debuff)
    {
        _health.ApplyDebuff(debuff);
    }

    public void ApplyStunned(float duration)
    {
        _stunned.ApplyStun(duration);
    }
    
    public void ApplySuperArmorStun(float duration)
    {
        _stunned.ApplySuperArmorStun(duration);
    }
    
    public void ResetHitCount()
    {
        _knockback?.ResetHitCount();
    }
}
