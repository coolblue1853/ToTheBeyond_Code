using UnityEngine;
using Sirenix.OdinInspector;
[CreateAssetMenu(menuName = "Enemy/Skill/RadicalRange")]
public class RadicalRangedSkillProfileSO : SkillProfileSO
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
    // 타격 타이밍에 실제로 투사체들을 원형으로 발사
    public override void OnHit(GameObject obj, GameObject target)
    {
        if (projectilePrefab == null) return;

        for (int i = 0; i < 12; i++)
        {
            float angle = (360f / 12) * i;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f).normalized;
            GameObject projectile = ObjectPooler.SpawnFromPool(projectilePrefab, obj.transform.position + dir * 0.5f, Quaternion.identity, 12);
            if (projectile.TryGetComponent<EnemyProjectile>(out var ep))
            {
                ep.Initialize(dir, attackDamage, speed, projectilePrefab);
                ep.StartAutoReturn(3f);
            }
        }
    }
}
