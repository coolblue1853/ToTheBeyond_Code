using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BreakableType { Normal, Box }
public class BreakablePart : MonoBehaviour, IDamageablePart
{
    public BreakableType type = BreakableType.Normal;
    private ObjectExplodeEffecct _explode;
    
    [Header("Box 드랍 설정")]
    public GameObject[] dropPrefabs; 
    [Range(0f,1f)] public float dropChance = 1f;
    
    private bool _broken;
    private int _randomSeed = 0;
    private void Awake()
    {
        _explode = GetComponent<ObjectExplodeEffecct>();
    }

    public void TakePartDamage(Vector3 attackerPosition, float baseDamage = 0f, bool isCrit = false)
    {
        if (_broken) return;
        _broken = true;

        _explode?.Break();

        if (type == BreakableType.Box && Random.value <= dropChance)
            SpawnSingleDrop(); 
    }
    
    private void SpawnSingleDrop()
    {
        if (dropPrefabs == null || dropPrefabs.Length == 0) return;
        
        if (_randomSeed != 0)
            Random.InitState(_randomSeed);

        var prefab = dropPrefabs[Random.Range(0, dropPrefabs.Length)];
        if (prefab == null) return;
        
        var item = Instantiate(prefab,  transform.position, Quaternion.identity);
    }
}
