using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
public class PlayIdle : Action
{
    public SharedGameObject targetGameObject;
    private Animator animator;

    public string idleStateName = "Idle";

    public override void OnStart()
    {
        GameObject go = GetDefaultGameObject(targetGameObject.Value);
        animator = go.GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        if (animator == null)
        {
            return TaskStatus.Failure;
        }

        // 상태 이름이 올바르면 Idle 재생
        animator.Play(idleStateName);
        return TaskStatus.Success;
    }

    public override void OnReset()
    {
        targetGameObject = null;
        idleStateName = "Idle";
    }
}
