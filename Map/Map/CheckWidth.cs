using UnityEngine;

public class CheckWidthFromPrefab : MonoBehaviour
{
    [SerializeField] private GameObject _targetPrefab;

    void Start()
    {
        if (_targetPrefab == null)
        {
            return;
        }

        SpriteRenderer sr = _targetPrefab.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float width = sr.bounds.size.x;
            Debug.Log($"'{_targetPrefab.name}'의 가로 길이: {width}");
        }
        else
        {
            Debug.LogWarning("SpriteRenderer가 프리팹에 없습니다.");
        }
    }
}