using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/Skill/SummonRangedMob")]
public class SummonRangedMobSkillProfileSO : SkillProfileSO
{
    [Header("소환 관련")]
    public GameObject rangedMinionPrefab; // 소환할 원거리 미니언 프리팹
    public int summonCount = 2; // 몇 마리 소환할지
    public float summonRadius = 3f; // 소환 위치 X범위
    public float delayBetweenSummons = 0.3f; // 소환 간격
    public float spawnY = -1f; // 소환되는 Y 위치
    public override void ExecuteAttack(GameObject obj, GameObject target)
    {
        if (rangedMinionPrefab == null) return;

        obj.GetComponent<Animator>()?.CrossFade("Summon", 0.1f);
        obj.GetComponent<EnemyController>()?.SetCurrentSkill(this, target);
    }

    private IEnumerator SummonMinions(GameObject obj, GameObject target)
    {
        // 소환 위치 계산은 그대로 유지
        for (int i = 0; i < summonCount; i++)
        {
            float offsetX = Random.Range(-summonRadius, summonRadius);
            Vector3 spawnPosition = new Vector3(obj.transform.position.x + offsetX, spawnY, 0f);

            // 현재 맵을 부모로 설정
            Transform mapParent = obj.GetComponentInParent<MapComponent>()?.transform;
            GameObject minion = Object.Instantiate(rangedMinionPrefab, spawnPosition, Quaternion.identity, mapParent);

            yield return new WaitForSeconds(delayBetweenSummons);
        }
    }


    public override void OnHit(GameObject obj, GameObject target)
    {
        obj.GetComponent<MonoBehaviour>().StartCoroutine(SummonMinions(obj, target));
    }
}
