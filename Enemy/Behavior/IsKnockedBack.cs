using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class IsKnockedBack : Conditional
{
    public SharedBool isKnockedBack;

    public override TaskStatus OnUpdate()
    {
        return isKnockedBack.Value ? TaskStatus.Success : TaskStatus.Failure;
    }
}
