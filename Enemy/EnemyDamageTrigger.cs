using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageTrigger : MonoBehaviour
{ 
    [SerializeField] private int damage = 20;

    public void Initialize(int damage, GameObject obj)
    {
        this.damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerHealth>(out var player))
        {
            player.TakeDamage(damage);
        }
    }
}
