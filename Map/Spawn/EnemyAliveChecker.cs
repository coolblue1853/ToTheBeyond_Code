using System.Collections.Generic;
using UnityEngine;

public class EnemyAliveChecker : MonoBehaviour
{
    private List<GameObject> _enemies = new();

    public void SetEnemies(List<GameObject> enemyList)
    {
        _enemies = enemyList;
    }

    public bool AreAllDead()
    {
        for (int i = _enemies.Count - 1; i >= 0; i--)
        {
            if (_enemies[i] == null)
            {
                _enemies.RemoveAt(i);
            }
        }

        return _enemies.Count == 0;
    }
    
    public void Clear()
    {
        foreach (var enemy in _enemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        _enemies.Clear();
    }

}
