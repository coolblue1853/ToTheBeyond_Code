using UnityEngine;
using Sirenix.OdinInspector;
[CreateAssetMenu(menuName = "Enemy/Skill/Ranged")]
public class RangedSkillProfileSO : SkillProfileSO
{
    [Title("투사체 설정", bold: true)]
    [BoxGroup("투사체 설정")]
    [LabelText("투사체 프리팹"), PreviewField(50), GUIColor(0.85f, 1f, 0.95f)]
    [Tooltip("발사할 투사체 프리팹")]
    public GameObject projectilePrefab;

    [BoxGroup("투사체 설정")]
    [LabelText("투사체 속도"), SuffixLabel("unit/sec", true), MinValue(0)]
    [Tooltip("투사체의 이동 속도")]
    [GUIColor(0.75f, 0.9f, 1f)]
    public float speed = 5f;

    public override void ExecuteAttack(GameObject obj, GameObject target)
    {
        obj.GetComponent<Animator>()?.Play("Attack", 0, 0f);
        obj.GetComponent<EnemyController>()?.SetCurrentSkill(this, target);
    }

    public override void OnHit(GameObject obj, GameObject target)
    {
        if (projectilePrefab == null || target == null) return;
        // 방향 및 회전 계산
        Vector3 dir = (target.transform.position - obj.transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0, 0, angle);
        // 약간 앞쪽에 투사체 생성
        GameObject projectile = ObjectPooler.SpawnFromPool(projectilePrefab, obj.transform.position + dir * 0.5f, rot, 5);
        if (projectile.TryGetComponent<EnemyProjectile>(out var ep))
        {
            ep.Initialize(dir, attackDamage, speed, projectilePrefab);
            ep.StartAutoReturn(3f);
        }
    }
}
