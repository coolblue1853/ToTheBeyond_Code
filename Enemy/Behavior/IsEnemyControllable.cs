using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
public class IsEnemyControllable : Conditional
{
    public SharedBool isControllable;

    public override TaskStatus OnUpdate()
    {
        return isControllable.Value ? TaskStatus.Success : TaskStatus.Failure;
    }
}
