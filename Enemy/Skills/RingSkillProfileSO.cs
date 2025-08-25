using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/Skill/Ring")]
public class RingSkillProfileSO : SkillProfileSO
{
    public GameObject projectilePrefab; // 발사할 탄환 프리팹
    public int bulletCount = 36; // 총 탄환 수
    public float radius = 4f; // 플레이어 기준 탄환 생성 반지름
    public float speed = 5f; // 탄환 속도
    public float safeAngle = 30f; // 안전 지대 각도

    public override void ExecuteAttack(GameObject obj, GameObject target)
    {
        obj.GetComponent<Animator>()?.Play("Attack", 0, 0f);
        obj.GetComponent<EnemyController>()?.SetCurrentSkill(this, target);
    }

    public override void OnHit(GameObject obj, GameObject target)
    {
        // 체력 조건 추가
        /*if (!target.TryGetComponent<PlayerHealth>(out var stats) || health.CurrentHealth > 30)
            return;*/
        if (projectilePrefab == null) return;

        Vector3 center = target.transform.position; // 플레이어 위치 중심
        float angleStep = 360f / bulletCount; // 각 탄환 사이 간격
        float safeStartAngle = Random.Range(0f, 360f); // 안전 구역 시작 각도

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * angleStep;
            // 안전 구역 범위 내에 있는 각도 스킵
            if (Mathf.Abs(Mathf.DeltaAngle(angle, safeStartAngle)) < safeAngle / 2f) // 안전한 위치
                continue; 
            // 각도를 방향 벡터로 변환
            Vector3 dir = Quaternion.Euler(0, 0, angle) * Vector3.right;
            Vector3 spawnPos = center + dir * radius;
            Quaternion rot = Quaternion.Euler(0, 0, angle + 180f); // 탄환이 중심을 바라보도록
            // 탄환 생성(오브젝트 풀 이용)
            GameObject projectile = ObjectPooler.SpawnFromPool(projectilePrefab, spawnPos, rot, bulletCount);
            if (projectile.TryGetComponent<EnemyProjectile>(out var ep))
            {
                ep.Initialize((center - spawnPos).normalized, attackDamage, speed, projectilePrefab, center);
                ep.StartAutoReturn(3.5f);
            }
        }
    }
}
