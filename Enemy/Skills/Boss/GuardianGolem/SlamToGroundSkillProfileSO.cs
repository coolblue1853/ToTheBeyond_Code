using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/Skill/SlamToGround")]
public class SlamToGroundSkillProfileSO : SkillProfileSO
{
    [Header("슬램 설정")]
    public GameObject warningPrefab; // 경고 이펙트 프리팹
    public GameObject slamFistPrefab; // 떨어지는 주먹 프리팹
    public float warningDuration = 1f; // 경고 표시 유지 시간
    public float spawnHeight = 5f; // 주먹이 떨어질 시작 높이
	[Header("인디케이터 설정")]
    [SerializeField] private float indicatorDuration = 0.5f;
    [SerializeField] private float indicatorWidthMultiplier = 2.5f;
    [SerializeField] private float indicatorHeightMultiplier = 2f;
    public override void ExecuteAttack(GameObject obj, GameObject target)
    {
        if (target == null || slamFistPrefab == null) return;

        obj.GetComponent<Animator>()?.Play("Slam", 0, 0f);
        obj.GetComponent<EnemyController>()?.SetCurrentSkill(this, target);
    }

    private IEnumerator SlamCoroutine(Vector3 targetPosition)
    {
        Vector3 groundPos = new Vector3(targetPosition.x, targetPosition.y, 0f);

       if (warningPrefab != null)
        {
            GameObject indicator = Instantiate(warningPrefab, targetPosition, Quaternion.identity);

            if (indicator.TryGetComponent<SpriteRenderer>(out var renderer))
            {
                float width = renderer.bounds.size.x;
                indicator.transform.localScale = new Vector3(
                    width * indicatorWidthMultiplier,
                    indicator.transform.localScale.y * indicatorHeightMultiplier,
                    1f
                );
            }

            indicator.transform.position = new Vector3(targetPosition.x, targetPosition.y, 0f);
            Destroy(indicator, indicatorDuration);
        }
        // 대기
        yield return new WaitForSeconds(warningDuration);

        // 주먹 생성
        Vector3 slamSpawnPos = new Vector3(groundPos.x, groundPos.y + spawnHeight, 0f);
        GameObject fist = GameObject.Instantiate(slamFistPrefab, slamSpawnPos, Quaternion.identity);

        // 바닥 Y값 설정 
        if (fist.TryGetComponent<EnemySlam>(out var slam))
        {
            slam.SetGroundY(groundPos.y);
        }
    }

    public override void OnHit(GameObject obj, GameObject target)
    {
        
        obj.GetComponent<MonoBehaviour>().StartCoroutine(SlamCoroutine(target.transform.position));
    }
}
