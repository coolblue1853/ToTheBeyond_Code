using BehaviorDesigner.Runtime;
using UnityEngine;
using Sirenix.OdinInspector;
public enum EnemyType
{
    Normal,
    Boss,
    FinalBoss,
}
[CreateAssetMenu(menuName = "Enemy/Enemy")]
public class EnemySO : ScriptableObject
{
    [BoxGroup("기본 정보")]
    [GUIColor(1f, 0.8f, 0.8f)] // 연붉은색
    [LabelText("Enemy Type")]
    public EnemyType enemyType = EnemyType.Normal;

    [BoxGroup("기본 정보")]
    [GUIColor(1f, 0.8f, 0.8f)]
    [LabelText("Max Health")]
    public float maxHealth = 100f;

    // 감지 관련 (파란색 계열)
    [BoxGroup("플레이어 감지 관련")]
    [GUIColor(0.75f, 0.85f, 1f)]
    [LabelText("정지 거리")]
    public float stopDistance = 1f;

    [BoxGroup("플레이어 감지 관련")]
    [GUIColor(0.75f, 0.85f, 1f)]
    [LabelText("감지 거리")]
    public float detectDistance = 5f;

    [BoxGroup("플레이어 감지 관련")]
    [GUIColor(0.75f, 0.85f, 1f)]
    [LabelText("이동 속도")]
    public float speed = 2f;

    // 순회 관련 (노랑 계열)
    [BoxGroup("적 순회 관련")]
    [GUIColor(1f, 0.95f, 0.75f)]
    [LabelText("최소 순회 거리")]
    public float minPatrolDistance = 4f;

    [BoxGroup("적 순회 관련")]
    [GUIColor(1f, 0.95f, 0.75f)]
    [LabelText("최대 순회 거리")]
    public float maxPatrolDistance = 5f;

    // 대기 시간 관련 (초록 계열)
    [BoxGroup("적 순회 멈추는 시간 관련")]
    [GUIColor(0.8f, 1f, 0.8f)]
    [LabelText("최소 정지 시간")]
    public float minStopTime = 0f;

    [BoxGroup("적 순회 멈추는 시간 관련")]
    [GUIColor(0.8f, 1f, 0.8f)]
    [LabelText("최대 정지 시간")]
    public float maxStopTime = 3f;

    // 스킬 관련 (보라 계열)
    [BoxGroup("적 스킬 관련")]
    [GUIColor(0.9f, 0.8f, 1f)]
    [LabelText("Enemy Skill Set"), InlineEditor(Expanded = true)]
    public EnemySkillSetSO enemySkillSet;
}
