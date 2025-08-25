using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactVisual : MonoBehaviour
{
    private TrailRenderer _trail;

    private void Awake()
    {
        _trail = GetComponent<TrailRenderer>();
    }

    private void OnEnable()
    {
        StartCoroutine(ResetTrail());
    }

    private IEnumerator ResetTrail()
    {
        yield return null; // 다음 프레임까지 기다림
        _trail?.Clear();
    }
}
