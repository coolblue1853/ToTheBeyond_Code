using System.Collections;
using UnityEngine;
public enum DashType
{
    OneWay,
    RoundTrip
}
[CreateAssetMenu(menuName = "Enemy/Skill/DashToEdge")]
public class DashToEdgeSkillProfileSO : BaseDashSkillProfileSO
{
    public DashType dashType = DashType.OneWay;
    // 돌진 목표 지점
    public float dashTargetXLeft = -10f;
    public float dashTargetXRight = 10f;
    private Vector2? _lastDashDir = null;
    private bool isAlreadyAtTarget;
    public override void ExecuteAttack(GameObject obj, GameObject target)
    {
        if (target == null) return;
        EnemyController controller = obj.GetComponent<EnemyController>();
        Vector2 dir = controller.IsFacingLeft() ? Vector2.left : Vector2.right;
        Vector2 dashDir = -dir;

        float currentX = obj.transform.position.x;
        bool alreadyAtTarget = dashDir.x > 0 ? currentX >= dashTargetXRight - 0.1f : currentX <= dashTargetXLeft + 0.1f;

        if (alreadyAtTarget)
        {
            return;
        }
        obj.GetComponent<Animator>()?.Play("Dash", 0, 0f);
        obj.GetComponent<EnemyController>()?.SetCurrentSkill(this, target);
    }
    
    public override void OnHit(GameObject obj, GameObject target)
    {
        obj.GetComponent<MonoBehaviour>().StartCoroutine(DashToEdge(obj));
    }

    private IEnumerator DashToEdge(GameObject obj)
    {
        EnemyController controller = obj.GetComponent<EnemyController>();
        if (controller == null) yield break;

        // 현재 바라보는 방향 기준
        Vector2 dir = controller.IsFacingLeft() ? Vector2.left : Vector2.right;
        Vector2 dashDir = -dir;
        
        float targetX = dashDir.x > 0 ? dashTargetXRight : dashTargetXLeft;

        yield return MoveWithCollisionCheck(obj, dashDir, () => dashDir.x > 0 ? obj.transform.position.x < targetX - 0.05f : obj.transform.position.x > targetX + 0.05f);
        // 왕복 설정일 경우 반대 방향으로 한 번 더
        if (dashType == DashType.RoundTrip)
        {
            // 반대 방향 설정
            Vector2 returnDir = -dashDir;
            controller.FlipToDirection(returnDir.x);

            // 반대 타겟 설정
            float returnTargetX = returnDir.x > 0 ? dashTargetXRight : dashTargetXLeft;

            // 같은 방식으로 여유를 둔 종료 조건
            yield return MoveWithCollisionCheck(obj, returnDir,
                () => returnDir.x > 0 ? obj.transform.position.x < returnTargetX - 0.05f : obj.transform.position.x > returnTargetX + 0.05f);
        }
    }
}

