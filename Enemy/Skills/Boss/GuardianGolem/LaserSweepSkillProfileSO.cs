using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Enemy/Skill/LaserSweep")]
public class LaserSweepSkillProfileSO : BaseLaserSkillProfileSO
{
    [Header("스윕 범위 설정")]
    public float xMin = -10f; // 스킬 실행 시 도달할 왼쪽 끝 위치
    public float xMax = 10f; // 스킬 실행 시 도달할 오른쪽 끝 위치
    public int tileCount = 10; // 번개 갯수

    [Header("돌기둥 위치 관련")]
    public float yMin = -3f; // 번개 시작 아래 위치
    public float yMax = 3f; // 번개 시작 위에 위치
    [Header("돌기둥 이펙트 관련")]
    public GameObject stoneExplodedPrefab;
    public float stoneOffset = 3f;
    
    [Header("대쉬 관련")]
    public float dashSpeed = 15f;
    private bool _isAttacking = false;
    public GameObject hitEffectPrefab;
    [SerializeField] private float _aimUpDegrees = 3f; 
    private CinemachineImpulseSource _impulseSource; 
    private void OnEnable()
    {
        _isAttacking = false; // 강제 초기화
    }
    public override void ExecuteAttack(GameObject obj, GameObject target)
    {
        if (_isAttacking) return; 
        var animator = obj.GetComponent<Animator>();
        animator.CrossFade("Laser", 0.1f);
        obj.GetComponent<EnemyController>()?.SetCurrentSkill(this, target);
    }

    private IEnumerator LaserSweepSequence(GameObject obj, GameObject target)
    {
        _isAttacking = true;
        EnemyController controller = obj.GetComponent<EnemyController>();

        // 1. 랜덤 방향 결정
        bool fromLeft = Random.value > 0.5f;
        float targetDashX = fromLeft ? xMin : xMax;
        Vector2 dashDir = fromLeft ? Vector2.left : Vector2.right;

        // 2. 현재 위치 기준으로 대쉬 필요 여부 결정
        bool needsDash = fromLeft ? obj.transform.position.x > xMin + 1f : obj.transform.position.x < xMax - 1f;

        if (needsDash)
        {
            yield return Dash(obj, targetDashX, dashDir);
            yield return new WaitForSeconds(1f);
        }

        // 3. 대쉬 후 시선 → 플레이어
        if (controller != null && target != null)
        {
            float directionX = target.transform.position.x - obj.transform.position.x;
            controller.FlipToDirection(directionX);
        }
        // 4. 레이저
        yield return FireSweepLaser(obj, fromLeft);
        yield return new WaitForSeconds(1f);
        
        // 5. 돌기둥 폭격
        float segmentWidth = (xMax - xMin) / tileCount;

        if (!fromLeft)
        {
            for (int i = tileCount - 1; i >= 0; i--)
            {
                float x = xMin + segmentWidth * i + segmentWidth / 2f;
                if (x < obj.transform.position.x)
                    x -= stoneOffset;

                yield return SpawnStonePillarSmash(x,obj,fromLeft);
            }
        }
        else
        {
            for (int i = 0; i < tileCount; i++)
            {
                float x = xMin + segmentWidth * i + segmentWidth / 2f;
                if (x > obj.transform.position.x)
                    x += stoneOffset;

                yield return SpawnStonePillarSmash(x,obj,fromLeft);
            }
        }
        _isAttacking = false;
    }

    // 수직 번개 생성 함수
    private IEnumerator SpawnStonePillarSmash(float x, GameObject obj, bool fromLeft)
    {
        Vector3 spawnPos = new Vector3(x, yMax, 0f);
        if (stoneExplodedPrefab != null)
        {
            GameObject effect = GameObject.Instantiate(stoneExplodedPrefab, spawnPos, Quaternion.identity);
       
            if (effect.gameObject.TryGetComponent<EnemyDamageTrigger>(out var trigger))
            {
                trigger.Initialize(attackDamage, obj);
            }
            _impulseSource = effect.GetComponent<CinemachineImpulseSource>();
            if (_impulseSource != null)
            {
                _impulseSource.GenerateImpulse(); 
            }

            Vector3 effectScale = effect.transform.localScale;
            effectScale.x = (fromLeft ? 1f : -1f) * Mathf.Abs(effectScale.x);
            effect.transform.localScale = effectScale;

            Animator anim = effect.GetComponent<Animator>();
            if (anim != null)
            {
                float duration = anim.GetCurrentAnimatorStateInfo(0).length;
                GameObject.Destroy(effect, duration);
            }
            else
            {
                GameObject.Destroy(effect, 0.3f);
            }
        }

        yield return new WaitForSeconds(0.3f);
    }
    // 지정 위치로 이동
    private IEnumerator Dash(GameObject obj, float targetX, Vector2 dir)
    {
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        EnemyKnockback knockback = obj.GetComponent<EnemyKnockback>();
        var pivot = obj.GetComponent<EnemyAnchors>()?.GetPivot("Dash");
        EnemyController controller = obj.GetComponent<EnemyController>();
        controller?.FlipToDirection(dir.x);
        if (pivot != null)
        {
            Vector2 effectPos = pivot.transform.position;
            EffectUtility.SpawnHitEffect(hitEffectPrefab, effectPos, obj,attackDamage, true);
        }

        knockback?.EnableSuperArmor();

        while ((dir.x > 0 && obj.transform.position.x < targetX) || (dir.x < 0 && obj.transform.position.x > targetX))
        {
            rb.velocity = dir * dashSpeed;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        
        knockback?.DisableSuperArmor();
        yield return new WaitForSeconds(0.2f);
    }

    private IEnumerator FireSweepLaser(GameObject obj, bool fromLeft)
    {
        var controller = obj.GetComponent<EnemyController>();
        controller?.FlipToDirection(fromLeft ? 1f : -1f);
        var pivot = obj.GetComponent<EnemyAnchors>()?.GetPivot("Laser");
        if (pivot == null || laserPrefab == null) yield break;

        Vector3 headPos = pivot.position;

        float startX = fromLeft ? xMin : xMax;
        float endX = fromLeft ? xMax + 10f : xMin - 10f;
        float offset = fromLeft ? _aimUpDegrees : -_aimUpDegrees;
        float sweepTime = 1.0f;
        float elapsed = 0f;

        // === 프리뷰 생성 ===
        GameObject preview = Instantiate(previewLaserPrefab, headPos, Quaternion.identity, obj.transform);
        Transform pHead = preview.transform.Find("Head");
        Transform pTail = preview.transform.Find("Tail");

        SetAlpha(pHead.gameObject, 0.1f);
        SetAlpha(pTail.gameObject, 0.1f);

        if (pHead)
        {
            pHead.position = headPos + Vector3.down;
            pHead.localScale *= 1.2f;
        }

        float baseLenPreview = 1f;
        var srP = pTail ? pTail.GetComponent<SpriteRenderer>() : null;
        if (srP && srP.sprite)
            baseLenPreview = srP.sprite.rect.width / srP.sprite.pixelsPerUnit;


        float dirSign = fromLeft ? -1f : +1f;
        float parentSign = Mathf.Sign(obj.transform.lossyScale.x);


        if (pTail)
        {
            float dist = Mathf.Abs((fromLeft ? (xMin - headPos.x) : (xMax - headPos.x)));
            Vector3 s = pTail.localScale;
            s.x = (dist / baseLenPreview) * (dirSign * parentSign);
            pTail.localScale = s;
            
            bool isCenterPivot = srP && srP.sprite && srP.sprite.pivot == srP.sprite.rect.size * 0.5f;
            pTail.position = headPos + (isCenterPivot ? Vector3.right * dirSign * (dist * 0.5f) : Vector3.zero);
        }
        
        yield return new WaitForSeconds(sweepTime);
        Destroy(preview);

        // === 실제 레이저 생성 ===
        GameObject laser = Instantiate(laserPrefab, headPos, Quaternion.identity, obj.transform);
        Transform tail = laser.transform.Find("Tail");
        Transform head = laser.transform.Find("Head");

        if (head)
        {
            head.position = headPos + Vector3.down;
            head.localScale *= 1.2f;
        }

        // === 실제 레이저: sweepTime 동안 회전+길이 조절 ===
        elapsed = 0f;
        while (elapsed < sweepTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / sweepTime);

            float targetX = Mathf.Lerp(startX, endX, t);
            Vector3 targetPos = new Vector3(targetX, yMin, 0f);
            Vector3 dir = (targetPos - headPos).normalized;
            float dist = Vector3.Distance(headPos, targetPos);

            if (tail)
            {
                float baseLen = 1f;
                var sr = tail.GetComponent<SpriteRenderer>();
                if (sr && sr.sprite)
                    baseLen = sr.sprite.rect.width / sr.sprite.pixelsPerUnit;

                tail.position = headPos;
                tail.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + offset);

                Vector3 scale = tail.localScale;
                scale.x = dist / baseLen;
                tail.localScale = scale;
            }

            yield return null;
        }

        yield return new WaitForSeconds(laserDuration);
        Destroy(laser);
    }


    public override void OnHit(GameObject obj, GameObject target)
    {
        obj.GetComponent<MonoBehaviour>().StartCoroutine(LaserSweepSequence(obj, target));
    }
}
