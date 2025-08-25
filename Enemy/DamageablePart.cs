  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageablePart : MonoBehaviour, IDamageablePart
{
    [SerializeField] private float _damageMultiplier = 1.0f;
    public EnemyHandler enemyHandler;
    public bool isDebuffalbe = true;
    private void Awake()
    {
        enemyHandler = GetComponentInParent<EnemyHandler>();
    }

    public void TakePartDamage(Vector3 attackerPosition, float baseDamage, bool isCrit)
    {
        float finalDamage = baseDamage * _damageMultiplier;
        enemyHandler.TakeDamage(attackerPosition,finalDamage,  isCrit);
    }
}
