using System.Collections;
using UnityEngine;

public class ZoneSpawner : MonoBehaviour
{
    [SerializeField] private EnemySpawnManager _spawnManager;
    [SerializeField] private Transform[] _zones; 
    [SerializeField] private float _zoneDelay = 1f;
    
    private int _currentZoneIndex = -1;
    private EnemyAliveChecker _aliveChecker = new EnemyAliveChecker();
    private bool _waitingNextZone = false;
    private bool _triggered = false;
    private MapComponent  _mapComponent;
    
    private void Awake()
    {
        _mapComponent = transform.parent.GetComponent<MapComponent>();
    }

    public void StartZones()
    {
        if (_zones == null || _zones.Length == 0)
        {
            return;
        }

        _triggered = true;
        this.enabled = true; 

        // 첫 Zone 활성화
        StartNextZone();
    }

    private void Update()
    {
        if (!_triggered || _waitingNextZone) return;
        
        bool allDead = _aliveChecker.AreAllDead();

        if (allDead)
        {
            if (_currentZoneIndex >= _zones.Length - 1)
            {
                OnAllZonesCleared();
            }
            else
            {
                _waitingNextZone = true;
                StartCoroutine(StartNextZoneWithDelay());
            }
        }
    }

    private IEnumerator StartNextZoneWithDelay()
    {
        yield return new WaitForSeconds(_zoneDelay);

        StartNextZone();
        _waitingNextZone = false;
    }

    private void StartNextZone()
    {
        _currentZoneIndex++;

        if (_currentZoneIndex >= _zones.Length)
            return;

        var zone = _zones[_currentZoneIndex];
        zone.gameObject.SetActive(true);
        
        var spawned = _spawnManager.SpawnFromParent(zone, _mapComponent.transform);
        _aliveChecker.SetEnemies(spawned);
    }
    public void ClearEnemies()
    {
        _aliveChecker.Clear();
    }
    private void OnAllZonesCleared()
    {
        this.enabled = false; 
        
        var portal = _mapComponent.portal;
                
        if (portal != null)
        {  portal.gameObject.SetActive(true);
            portal.EnablePortal();
            RewardManager.Instance.GenerateRewards(_mapComponent.rewardSpawnPoint);
        }
    }
}
