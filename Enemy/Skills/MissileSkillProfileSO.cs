using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/Skill/Missile")]
public class MissileSkillProfileSO : SkillProfileSO
{
    public GameObject missilePrefab; // 미사일 프리팹
    public int missileCount = 3; // 미사일 갯수
    public float spawnOffsetY = 0.5f; // 적 기준 Y오프셋
    public float spawnOffsetX = -1.5f; // 적 기준 X오프셋
    public float delayBetweenShots = 0.2f; // 미사일 딜레이
    public override void ExecuteAttack(GameObject obj, GameObject target)
    {
        obj.GetComponent<Animator>()?.CrossFade("Missile", 0.1f);
        obj.GetComponent<EnemyController>()?.SetCurrentSkill(this, target);
        var flying = obj.GetComponent<EnemyFlying>();
        if (flying != null)
            flying.StopMovement();
    }

    public override void OnHit(GameObject obj, GameObject target)
    {
        if (missilePrefab == null || target == null) return;

        obj.GetComponent<MonoBehaviour>().StartCoroutine(FireMissiles(obj, target));
    }

    private IEnumerator FireMissiles(GameObject obj, GameObject target)
    {
        Vector3 basePos = obj.transform.position + new Vector3(spawnOffsetX, spawnOffsetY, 0f);
        float facingDir = Mathf.Sign(obj.transform.localScale.x);
        var flying = obj.GetComponent<EnemyFlying>();
        for (int i = 0; i < missileCount; i++)
        {
            GameObject missile = ObjectPooler.SpawnFromPool(missilePrefab, basePos, Quaternion.identity, missileCount);
            if (missile.TryGetComponent<EnemyMissile>(out var missileScript))
            {
                missileScript.Initialize(target.transform, facingDir, missilePrefab);
            }

            yield return new WaitForSeconds(delayBetweenShots);
        }
        // 움직임 재개
        if (flying != null)
            flying.ResumeMovement();
    }
}
