using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] _waveParents;
    [SerializeField] private EnemySpawnManager _spawnManager;
    [SerializeField] private float _waveDelay = 1f;
    
    private int _currentWaveIndex = 0;
    private EnemyAliveChecker _aliveChecker = new EnemyAliveChecker();
    private bool _triggered = false;
    private bool _waitingNextWave = false;
    private MapComponent  _mapComponent;


    private void Awake()
    {
        _mapComponent = transform.parent.GetComponent<MapComponent>();
    }

    public void StartWave()
    {
        _triggered = true;
        this.enabled = true;
        SpawnWave(_currentWaveIndex);
    }

    private void Update()
    {
        if (!_triggered || _waitingNextWave) return;

        if (_aliveChecker.AreAllDead())
        {
            if (_currentWaveIndex >= _waveParents.Length - 1)
            {
                // 마지막 웨이브 클리어
                OnAllWavesCleared();
            }
            else
            {
                _waitingNextWave = true;
                StartCoroutine(StartNextWaveAfterDelay());
            }
        }
    }
    public void ClearEnemies()
    {
        _aliveChecker.Clear();
    }

    private IEnumerator StartNextWaveAfterDelay()
    {
        yield return new WaitForSeconds(_waveDelay);

        _currentWaveIndex++;
        SpawnWave(_currentWaveIndex);
        _waitingNextWave = false;
    }

    private void SpawnWave(int index)
    {
        if (index >= _waveParents.Length) return;

        var spawnedEnemies = _spawnManager.SpawnFromParent(_waveParents[index], _mapComponent.transform);

        _aliveChecker.SetEnemies(spawnedEnemies);
    }
    
    private void OnAllWavesCleared()
    {
        this.enabled = false;

        var portal = _mapComponent.portal;
        if (portal != null)
        {
            portal.gameObject.SetActive(true);
            portal.EnablePortal();
            RewardManager.Instance.GenerateRewards(_mapComponent.rewardSpawnPoint);
        }
    }
}
