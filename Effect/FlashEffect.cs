using UnityEngine;
using System.Collections;

public class FlashEffect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float flashDuration = 0.1f;

    private Material originalMaterial;
    private Coroutine _flashRoutine;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            originalMaterial = spriteRenderer.material;
    }

    /// <summary>
    /// 외부에서 안전하게 호출할 수 있는 Flash 트리거
    /// </summary>
    public void TryTriggerFlash()
    {
        // 객체가 파괴되었거나 비활성 상태면 실행하지 않음
        if (this == null || !gameObject.activeInHierarchy || spriteRenderer == null)
            return;

        TriggerFlash();
    }

    private void TriggerFlash()
    {
        if (_flashRoutine != null)
            StopCoroutine(_flashRoutine);

        _flashRoutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        if (spriteRenderer == null || flashMaterial == null || originalMaterial == null)
            yield break;

        spriteRenderer.material = flashMaterial;

        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            // 실행 도중 객체가 파괴되었을 경우 종료
            if (this == null || spriteRenderer == null)
                yield break;

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (spriteRenderer != null)
            spriteRenderer.material = originalMaterial;
    }
}