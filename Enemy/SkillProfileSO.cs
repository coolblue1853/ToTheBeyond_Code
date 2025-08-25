using UnityEngine;
using Sirenix.OdinInspector;
public abstract class SkillProfileSO : ScriptableObject
{
    [Title("스킬 기본 정보", bold: true)]
    [BoxGroup("스킬 설정")]
    [LabelText("쿨타임 (초)"), SuffixLabel("s", true)]
    [MinValue(0)]
    [GUIColor(0.85f, 0.95f, 1f)]
    public float coolTime;

    [BoxGroup("스킬 설정")]
    [LabelText("공격력")]
    [MinValue(0)]
    [GUIColor(1f, 0.85f, 0.85f)]
    public int attackDamage;

    [BoxGroup("스킬 설정")]
    [LabelText("지속 시간"), SuffixLabel("초", true)]
    [MinValue(0.1f)]
    [GUIColor(0.9f, 1f, 0.85f)]
    public float duration = 1f;

    [BoxGroup("스킬 설정")]
    [LabelText("이펙트 왼쪽 방향?")]
    [GUIColor(0.95f, 0.85f, 1f)]
    public bool isEffectLeft = false;

    [ReadOnly, HideInInspector]
    public int playerLayerMask;
    protected virtual void OnEnable()
    {
        playerLayerMask = LayerMask.GetMask("Player");
    }
    public abstract void ExecuteAttack(GameObject obj, GameObject target); // 스킬 발동시 호출
    public abstract void OnHit(GameObject obj, GameObject target); // 공격이 명중했을 때 호출
}
