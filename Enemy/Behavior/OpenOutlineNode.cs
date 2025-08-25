using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.UI;
public class OpenOutlineNode : Action
{
    public SharedGameObject target;
    public Material outlineMaterial;
    private bool applied = false;

    public override void OnStart()
    {
        applied = false;
    }

    public override TaskStatus OnUpdate()
    {
        if (target.Value == null || outlineMaterial == null)
            return TaskStatus.Failure;

        if (!applied)
        {
            var renderer = target.Value.GetComponentInChildren<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.material = outlineMaterial;
                applied = true;
            }
        }

        return TaskStatus.Success;
    }
}
