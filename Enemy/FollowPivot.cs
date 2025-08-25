using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPivot : MonoBehaviour
{
    public string pivotName = "Pivot";  // 따라갈 대상의 이름

    private Transform pivotTransform;
    private Vector3 offset;

    private void Start()
    {
        // 부모 안에서 pivot Transform 찾기
        Transform parent = transform.parent;
        if (parent != null)
        {
            pivotTransform = parent.Find(pivotName);

            if (pivotTransform != null)
            {
                offset = transform.position - pivotTransform.position;
            }
        }
    }

    private void LateUpdate()
    {
        if (pivotTransform != null)
        {
            transform.position = pivotTransform.position + offset;
        }
    }
}
