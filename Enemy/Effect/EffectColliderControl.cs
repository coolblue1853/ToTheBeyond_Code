using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectColliderControl : MonoBehaviour
{
    public float startCol = 0.1f;
    public float duration = 0.3f;
    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _collider.enabled = false;
    }

    private void Start()
    {
        EnableColliderTemporarily();
    }

    public void EnableColliderTemporarily()
    {
        StartCoroutine(DisableAfterDelay());
    }

    private IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(startCol);
        _collider.enabled = true;
        yield return new WaitForSeconds(duration);
        _collider.enabled = false;
    }
}
