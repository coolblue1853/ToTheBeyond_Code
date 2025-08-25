using UnityEngine;

public class MuzzleFlashController : MonoBehaviour
{
    [Header("Muzzle Flash Object")]
    public GameObject muzzleFlash; // Flash 오브젝트 연결

    [Header("Flash Settings")]
    public float flashDuration = 0.05f; // 머즐플래시 지속 시간
    public bool randomizeRotation = true; // 회전 랜덤화 여부
    public Vector2 randomScaleRange = new Vector2(0.9f, 1.2f); // 크기 랜덤 범위

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            TriggerMuzzleFlash();
            // 여기에 총알 발사 로직 등도 추가 가능
        }
    }

    void TriggerMuzzleFlash()
    {
        if (muzzleFlash == null) return;

        // 회전 랜덤화
        if (randomizeRotation)
        {
            muzzleFlash.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        }

        // 스케일 랜덤화
        float scale = Random.Range(randomScaleRange.x, randomScaleRange.y);
        muzzleFlash.transform.localScale = new Vector3(scale, scale, 1f);

        // 플래시 활성화
        muzzleFlash.SetActive(true);

        // 일정 시간 후 비활성화
        CancelInvoke(nameof(DisableFlash)); // 중첩 방지
        Invoke(nameof(DisableFlash), flashDuration);
    }

    void DisableFlash()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(false);
        }
    }
}