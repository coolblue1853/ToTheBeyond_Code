using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
public class IsPlayerInRange : Conditional
{
    public enum DetectionMode { MeleeBox, RangedCircle }
    public DetectionMode mode = DetectionMode.MeleeBox;

    public SharedGameObject player;

    [Header("근거리 사각형 설정")]
    public float detectHeight = 1f;

    [Header("원거리 원형 설정")]
    public SharedFloat detectDistance = 5f;

    public override TaskStatus OnUpdate()
    {
        if (player == null || player.Value == null)
            return TaskStatus.Failure;

        Vector2 origin = (Vector2)transform.position;
        Vector2 target = player.Value.transform.position;

        if (mode == DetectionMode.MeleeBox)
        {
            // 1) 사각형 범위 체크
            Vector2 delta = target - origin;
            if (Mathf.Abs(delta.x) > detectDistance.Value * 0.5f) return TaskStatus.Failure;
            if (Mathf.Abs(delta.y) > detectHeight * 0.5f) return TaskStatus.Failure;
        }
        else if (mode == DetectionMode.RangedCircle)
        {
            // 1) 원형 범위 체크
            float dist = Vector2.Distance(origin, target);
            if (dist > detectDistance.Value) return TaskStatus.Failure;
        }

        return TaskStatus.Success;
    }
}
