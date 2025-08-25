using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _spawnEffectPrefab;

    public List<GameObject> SpawnFromParent(Transform parent, Transform container = null, Action onCompleted = null)
    {
        List<GameObject> spawned = new List<GameObject>();

        if (parent == null)
            return spawned;

        EnemySpawnPoint[] points = parent.GetComponentsInChildren<EnemySpawnPoint>();

        foreach (var point in points)
        {
            if (point.enemyPrefabs == null || point.enemyPrefabs.Count == 0) continue;

            foreach (var prefab in point.enemyPrefabs)
            {
                if (prefab == null) continue;

                Vector3 spawnPos = point.transform.position;

                PlaySpawnEffect(spawnPos);

                GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity, container);
                spawned.Add(enemy);
            }
        }

        onCompleted?.Invoke();
        return spawned;
    }

    private void PlaySpawnEffect(Vector3 position)
    {
        if (_spawnEffectPrefab == null) return;

        GameObject fx = Instantiate(_spawnEffectPrefab, position, Quaternion.identity);
        Destroy(fx,0.5f);
    }
}