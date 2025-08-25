using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class IsStunned : Conditional
{
    public SharedBool isStunned;

    public override TaskStatus OnUpdate()
    {
        return isStunned.Value ? TaskStatus.Success : TaskStatus.Failure;
    }
}
