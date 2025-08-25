using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectUtility
{
    // 공격 이펙트를 생성하고 필요한 초기화를 진행
    public static void SpawnHitEffect(GameObject hitEffectPrefab,Vector2 spawnPos, GameObject sourceObject, int attackDamage, bool isEffectLeft = false)
    {
        if (hitEffectPrefab == null) return;

        // sprite 방향 기준으로 localScale 가져오기
        EnemyController controller = sourceObject.GetComponent<EnemyController>();
        float direction = 1f;

        if (controller != null)
        {
            direction = controller.IsFacingLeft() ? -1f : 1f;
        }

        GameObject effect = GameObject.Instantiate(hitEffectPrefab, spawnPos, Quaternion.identity);
        effect.transform.SetParent(sourceObject.transform);
        
        // 데미지 트리거 초기화
        if (effect.TryGetComponent<EnemyDamageTrigger>(out var trigger))
        {
            trigger.Initialize(attackDamage, sourceObject);
        }

        // 스케일 반영
        Vector3 effectScale = effect.transform.localScale;
        if(isEffectLeft)
            effectScale.x = -direction * Mathf.Abs(effectScale.x);
        else
            effectScale.x = direction * Mathf.Abs(effectScale.x);

        effect.transform.localScale = effectScale;
        
        // 애니메이션 길이만큼 대기 후 삭제
        Animator anim = effect.GetComponent<Animator>();
        float duration = anim != null ? anim.GetCurrentAnimatorStateInfo(0).length : 1f;
        GameObject.Destroy(effect, duration);
    }
}
