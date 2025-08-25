using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class IsPlayerInAttackRange : Conditional
{
    public enum DetectionMode { MeleeBox, RangedCircle }
    public DetectionMode mode = DetectionMode.MeleeBox;

    public SharedGameObject player; // 공격 대상
    public SharedFloat stopDistance; // 공격 사거리(원형 모드에서 사용)
    public SharedBool hasDetectedPlayer; // 감지 상태 유지
    [Range(1f, 2f)] public float releaseMultiplier = 1.2f;
    [Header("근거리 사각형 옵션")]
    public float detectHeight = 1f;


    public override TaskStatus OnUpdate()
    {
        if (player == null || player.Value == null)
            return TaskStatus.Failure;

        Vector2 origin =  (Vector2)transform.position;
        Vector2 target = player.Value.transform.position;

        bool inRangeNow = false;

        if (mode == DetectionMode.MeleeBox)
        {
            Vector2 d = target - origin;
           
            inRangeNow = Mathf.Abs(d.x) <= stopDistance.Value  && Mathf.Abs(d.y) <= detectHeight ;
        }
        else // RangedCircle
        {
            float dist = Vector2.Distance(origin, target);
            inRangeNow = dist <= stopDistance.Value;
        }

        // --- 상태 전이 로직 ---
        if (inRangeNow)
        {
            hasDetectedPlayer.Value = true;   // 근접하면 켬
        }
        else
        {
            // 충분히 멀어지면 끔 (되돌림)
            if (mode == DetectionMode.MeleeBox)
            {
                Vector2 d = target - origin;
                bool outByBox = Mathf.Abs(d.x) > stopDistance.Value * 0.5f * releaseMultiplier ||
                                Mathf.Abs(d.y) > detectHeight * 0.5f * releaseMultiplier;
                if (outByBox) hasDetectedPlayer.Value = false;
            }
            else
            {
                float dist = Vector2.Distance(origin, target);
                if (dist > stopDistance.Value * releaseMultiplier)
                    hasDetectedPlayer.Value = false;
            }
        }

        // 조건 노드는 "지금 공격해도 됨?"을 반환해야 하므로 현재 inRange 기준으로 돌려줌
        return inRangeNow ? TaskStatus.Success : TaskStatus.Failure;
    }
}
