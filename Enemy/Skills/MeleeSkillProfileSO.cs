using UnityEngine;
using Sirenix.OdinInspector;
using Unity.Cinemachine;

public enum MeleeType
{
    Normal,
    Dash
}

public enum MeleeSize
{
    Small,
    Large,
}
[CreateAssetMenu(menuName = "Enemy/Skill/Melee")]
public class MeleeSkillProfileSO : SkillProfileSO
{
    [BoxGroup("근접 스킬 설정")]
    [LabelText("히트 이펙트 프리팹"), PreviewField(50), GUIColor(0.95f, 0.85f, 1f)]
    public GameObject hitEffectPrefab;

    [BoxGroup("근접 스킬 설정")]
    [LabelText("공격 타입"), GUIColor(1f, 0.95f, 0.8f)]
    public MeleeType meleeType = MeleeType.Normal;
    public MeleeSize meleeSize = MeleeSize.Small;
    [BoxGroup("근접 스킬 설정")]
    [LabelText("대시 최소 거리"), SuffixLabel("unit", true), ShowIf("meleeType", MeleeType.Dash)]
    [MinValue(0)]
    [GUIColor(0.8f, 1f, 1f)]
    public float dashMeleeMinRange = 3f;

    // 스킬 실행 시 애니메이션 재생 및 이팩트 생설
    public override void ExecuteAttack(GameObject obj, GameObject target)
    {
        // 현재 스킬 정보를 Enemy에 저장해두기
        if(meleeSize == MeleeSize.Small)
            obj.GetComponent<Animator>()?.CrossFade("Attack", 0.1f);
        else
            obj.GetComponent<Animator>()?.CrossFade("LargeAttack", 0.1f);
        obj.GetComponent<EnemyController>()?.SetCurrentSkill(this, target);

        if (meleeType == MeleeType.Dash && target != null)
        {
            if (target != null)
            {
                float distance = Vector2.Distance(obj.transform.position, target.transform.position);
                if (distance > dashMeleeMinRange) return; 
            }
        }
    }

    public override void OnHit(GameObject obj, GameObject target)
    {
        var pivot = obj.GetComponent<EnemyAnchors>()?.GetPivot("Melee");
        Vector2 spawnPos = pivot.transform.position;
        // 이펙트 생성
        if (hitEffectPrefab != null)
        {
            EffectUtility.SpawnHitEffect(hitEffectPrefab, spawnPos,obj,attackDamage);
            CinemachineImpulseSource impulse = hitEffectPrefab.GetComponentInChildren<CinemachineImpulseSource>();
            if (impulse != null)
            {
                impulse.GenerateImpulse();
            }

        }
    }
}

