using System.Collections.Generic;
using UnityEngine;

public class PlatformCollector : MonoBehaviour
{
    public Transform graphicsRoot;
    public string platformPrefix = "Meteo";

    private List<Transform> _platformPoints = new List<Transform>();
    public List<Transform> PlatformPoints => _platformPoints;
    private void Awake()
    {
        CollectPlatformPoints();
    }
    
    private void CollectPlatformPoints()
    {
        _platformPoints.Clear();

        foreach (Transform child in graphicsRoot)
        {
            if (child.name.StartsWith(platformPrefix))
            {
               _platformPoints.Add(child);
            }
        }
    }
}
