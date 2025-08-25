using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/Skill/TeleportBehindPlayer")]
public class TeleportBehindPlayerSkillSO : SkillProfileSO
{
    public float teleportDistance = 2f; // 플레이어로부터 얼마나 뒤로 이동할지
    public float liftHeight = 1.5f; // 위로 얼마나 뜰지
    public float dropDuration = 0.3f; // 떨어지는 시간
    public override void ExecuteAttack(GameObject obj, GameObject target)
    {
        //obj.GetComponent<Animator>()?.Play("Teleport", 0, 0f);
        obj.GetComponent<EnemyController>()?.SetCurrentSkill(this, target);
    }
    // 순간이동 및 낙하 시작
    public override void OnHit(GameObject obj, GameObject target)
    {
        if (target == null) return;

        float dir = Mathf.Sign(target.transform.localScale.x);
        Vector3 offset = new Vector3(-dir * teleportDistance, 0f, 0f); // 반대방향으로 이동
        Vector3 targetPos = target.transform.position + offset;

        // 위로 
        Vector3 liftPos = targetPos + Vector3.up * liftHeight;
        obj.transform.position = liftPos;

        // 아래로
        obj.GetComponent<MonoBehaviour>().StartCoroutine(DropAndFacePlayer(obj, targetPos, dir));
    }
    // 공중에서 떨어지며 플레이어 방향으로 스프라이트 방향 전환
    private IEnumerator DropAndFacePlayer(GameObject obj, Vector3 finalPos, float lookDir)
    {
        Vector3 startPos = obj.transform.position;
        float elapsed = 0f;

        while (elapsed < dropDuration)
        {
            float t = elapsed / dropDuration;
            obj.transform.position = Vector3.Lerp(startPos, finalPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = finalPos;

        // 플레이어 방향 바라보게
        Vector3 scale = obj.transform.localScale;
        scale.x = lookDir * Mathf.Abs(scale.x);
        obj.transform.localScale = scale;
    }
}
