using System;
using System.Collections;
using UnityEngine;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Unity.Cinemachine;

public abstract class BaseDashSkillProfileSO : SkillProfileSO
{
    [Header("대쉬 공통 설정")]
    public float dashSpeed = 10f; // 대시 속도
    public GameObject hitEffectPrefab; // 히트 이펙트 프리팹
    public bool isBoss = false;
    public Vector2 offset = Vector2.zero;
    // 대시 이동 및 충돌 검사 코루틴
    protected IEnumerator MoveWithCollisionCheck(GameObject obj, Vector2 dir, Func<bool> shouldContinue)
    {
        Transform impulseParent = obj.transform.Find("ImpulseSource");
        if (impulseParent != null)
        {
            Transform dashImpulse = impulseParent.Find("Dash");
            if (dashImpulse != null)
            {
                var impulseSource = dashImpulse.GetComponent<CinemachineImpulseSource>();
                if (impulseSource != null)
                {
                    impulseSource.GenerateImpulse(1f); // 강도 조절
                }
            }
        }

        
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        EnemyKnockback knockback = obj.GetComponent<EnemyKnockback>();
        knockback?.EnableSuperArmor();
        //이펙트 위치 계산 
        var pivot = obj.GetComponent<EnemyAnchors>()?.GetPivot("Dash");
        Vector2 spawnPos = pivot.transform.position;

        EffectUtility.SpawnHitEffect(hitEffectPrefab, spawnPos, obj, attackDamage,isEffectLeft);

        while (shouldContinue()) 
        {
            Vector2 origin = obj.transform.position;

            // 전방 레이 Wall 체크
            RaycastHit2D frontHit = Physics2D.Raycast(origin+ offset, dir, 4f, LayerMask.GetMask( "Wall"));
            if (frontHit.collider != null && frontHit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                break;
            }

            // 아래 방향 레이낭떠러지 체크
            Vector2 groundCheckOrigin = (origin + offset) + dir * 0.5f; // 살짝 앞쪽
            RaycastHit2D downHit = Physics2D.Raycast(groundCheckOrigin, Vector2.down, 4f, LayerMask.GetMask("Ground"));
            if (downHit.collider == null)
            {
                break;
            }

            rb.velocity = dir * dashSpeed;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        knockback?.DisableSuperArmor();
        yield return new WaitForSeconds(0.05f);
    }
}
