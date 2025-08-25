using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.UI;

public class ResetMaterial : Action
{
    public SharedGameObject target;
    public Material defaultMaterial;

    public override TaskStatus OnUpdate()
    {
        if (target.Value == null || defaultMaterial == null)
            return TaskStatus.Failure;

        var renderer = target.Value.GetComponentInChildren<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.material = defaultMaterial;
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}
