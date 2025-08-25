using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/Skill/Dash")]
public class DashSkillProfileSO : BaseDashSkillProfileSO
{
    public float dashDistance = 5f;

    public override void ExecuteAttack(GameObject obj, GameObject target)
    {
        if (target == null) return;
        obj.GetComponent<Animator>()?.CrossFade("Dash", 0.1f);
        obj.GetComponent<EnemyController>()?.SetCurrentSkill(this, target);
    }

    private IEnumerator DashAndHit(GameObject obj)
    {
        Vector2 startPos = obj.transform.position;
        // 현재 바라보는 방향을 기준으로 이동 방향 결정
        EnemyController controller = obj.GetComponent<EnemyController>();
        Vector2 dir = controller.IsFacingLeft() ? Vector2.left : Vector2.right;
        
        yield return MoveWithCollisionCheck(obj, -dir, () =>
        {
            float moved = Vector2.Distance(startPos, obj.transform.position);
            return moved < dashDistance;
        });
    }

    public override void OnHit(GameObject obj, GameObject target)
    {
        obj.GetComponent<MonoBehaviour>().StartCoroutine(DashAndHit(obj));
    }
}
