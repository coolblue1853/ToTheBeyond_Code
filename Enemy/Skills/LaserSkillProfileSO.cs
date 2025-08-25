using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Skill/Laser")]
public class LaserSkillProfileSO : BaseLaserSkillProfileSO
{
    public override void ExecuteAttack(GameObject obj, GameObject target)
    {
        var animator = obj.GetComponent<Animator>();

        if (obj.name.Contains("FairyQueen") || obj.name.Contains("GuardianGolem") )
        {
            animator.CrossFade("Laser", 0.1f);
        }
        else
        {
            animator.CrossFade("Attack", 0.1f);
        }
        obj.GetComponent<EnemyController>()?.SetCurrentSkill(this, target);
        var flying = obj.GetComponent<EnemyFlying>();
        if (flying != null)
            flying.StopMovement();
    }
    public override void OnHit(GameObject obj, GameObject target)
    {
        if (laserPrefab == null || target == null) return;
        obj.GetComponent<MonoBehaviour>()?.StartCoroutine(FireFollowLaser(obj, target));
    }
    
    // 실제 레이저를 일정 시간 동안 생성하여 데미지를 주는 코루틴
    private IEnumerator FireFollowLaser(GameObject obj, GameObject target)
    {
        var pivot = obj.GetComponent<EnemyAnchors>()?.GetPivot("Laser");

        if(LaserType.AimPlayer == laserType)
            yield return FireFollowLaser( target,pivot,obj);
        else
        {
            yield return FireLaserStraight(target,obj.transform,pivot, attackDamage);
        }
        var flying = obj.GetComponent<EnemyFlying>();
        if (flying != null)
            flying.ResumeMovement();
    }
}
