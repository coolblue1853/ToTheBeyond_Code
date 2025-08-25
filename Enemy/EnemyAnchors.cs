using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnchors : MonoBehaviour
{
    [System.Serializable]
    public struct PivotEntry
    {
        public string name;    // Pivot 이름 (중복 X)
        public Transform pivot;
    }
    
    [SerializeField] private PivotEntry[] pivots;
    private Dictionary<string, Transform> _pivotDict;
    
    private void Awake()
    {
        _pivotDict = new Dictionary<string, Transform>();
    
        foreach (var entry in pivots)
        {
            if (!string.IsNullOrEmpty(entry.name) && entry.pivot != null)
            {
                if (!_pivotDict.ContainsKey(entry.name))
                    _pivotDict.Add(entry.name, entry.pivot);

            }
        }
    }
    
    public Transform GetPivot(string pivotName)
    {
        if (_pivotDict.TryGetValue(pivotName, out var pivot))
            return pivot;
        
        return transform; // fallback
    }
}
