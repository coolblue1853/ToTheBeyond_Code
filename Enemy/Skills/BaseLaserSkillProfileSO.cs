using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

public enum LaserType
{
    AimPlayer, // 플레이어를 조준하여 발사
    StraightForward // 바라보는 방향으로 발사
}
public abstract class BaseLaserSkillProfileSO : SkillProfileSO
{
    [Header("레이저 설정")]
    public GameObject laserPrefab;
    public GameObject previewLaserPrefab;
    public float laserLength = 2f;
    public float laserWidth = 2f;
    public float laserDuration = 2f;
    public float preivewLaserDuration = 1f;
    public LaserType laserType = LaserType.AimPlayer;
    public float laserHeadWidth = 5f;

    private EnemyController _ememyController;
    
    public IEnumerator FireFollowLaser( GameObject target, Transform parent, GameObject obj, LaserType laserType = LaserType.AimPlayer, float moveSpeed = 5f)
    {
        if (laserPrefab == null || target == null || previewLaserPrefab == null) yield break;
        
        Vector2 firePoint = parent.transform.position;
        // 최초 방향 결정
        Vector2 direction;
        if (laserType == LaserType.AimPlayer && target != null)
        {
            Vector2 targetPos = target.transform.position;
            direction = (targetPos - firePoint).normalized;
        }
        else
        {
            direction = parent.right;
        }

        // 프리뷰 생성
        GameObject preview = GameObject.Instantiate(previewLaserPrefab, firePoint, Quaternion.identity,obj.transform);
        preview.transform.right = direction;
        
        Transform previewtail = preview.transform.Find("Tail");
        Transform previewhead = preview.transform.Find("Head");
        
        SetAlpha(previewtail.gameObject, 0.3f);
        SetAlpha(previewhead.gameObject, 0.3f);
        if (previewtail != null)
        {
            previewtail.transform.position = firePoint;
            previewtail.transform.right = direction;
            previewtail.transform.localScale = new Vector3(laserLength, laserWidth, 1f); 
        }

        if (previewhead != null)
        {
            previewhead.transform.right = direction;
            Vector3 headScale = previewhead.localScale;
            headScale.y = laserHeadWidth;
            headScale.x = laserHeadWidth;
            previewhead.localScale = headScale;
        }

        yield return new WaitForSeconds(preivewLaserDuration);
        
        Quaternion rotation = preview.transform.rotation;
        Object.Destroy(preview);

        GameObject laser = GameObject.Instantiate(laserPrefab, firePoint, rotation,obj.transform);
        Transform tail = laser.transform.Find("Tail");
        Transform head = laser.transform.Find("Head");
        if (tail.TryGetComponent<EnemyDamageTrigger>(out var trigger))
        {
            trigger.Initialize(attackDamage, parent.gameObject);
        }
        if (head.TryGetComponent<EnemyDamageTrigger>(out var trigger2))
        {
            trigger.Initialize(attackDamage, parent.gameObject);
        }
        Vector3 headLaserScale = head.localScale;
        headLaserScale.y = laserHeadWidth;
        headLaserScale.x = laserHeadWidth;
        head.localScale = headLaserScale;
        
        float timer = 0f;

        while (timer < laserDuration)
        {
            timer += Time.deltaTime;

            if (laserType == LaserType.AimPlayer && target != null)
            {
                Vector2 currentTargetPos = target.transform.position;
                Vector2 currentDir = (currentTargetPos - (Vector2)laser.transform.position).normalized;
                laser.transform.right = Vector2.Lerp(laser.transform.right, currentDir, Time.deltaTime * 2f);
            }
            if (tail)
            {
                var tailScale = tail.localScale;
                tailScale.x = laserLength;
                tailScale.y = laserWidth;
                tail.localScale = tailScale;
            }
            yield return null;
        }

        Object.Destroy(laser);
    }
    
    public IEnumerator FireLaserStraight(GameObject target, Transform parent, Transform pivot, int damage)
    {
        if (_ememyController == null && parent != null || previewLaserPrefab == null)
        {
            _ememyController = parent.GetComponentInParent<EnemyController>();
        }

        float directionX = -1f;
        Vector2 firePoint = pivot.position;
        if (_ememyController != null && _ememyController.IsFacingLeft())
        {
            directionX = 1f;
        }

        GameObject preview = GameObject.Instantiate(previewLaserPrefab, firePoint, Quaternion.identity, parent.transform);
    
        Vector3 previewScale = preview.transform.localScale;
        previewScale.x = directionX;
        preview.transform.localScale = previewScale;

        Transform previewTail = preview.transform.Find("Tail");
        Transform previewHead = preview.transform.Find("Head");

        SetAlpha(previewTail.gameObject, 0.1f);
        SetAlpha(previewHead.gameObject, 0.1f);

        if (previewTail != null)
        {
            previewTail.position = firePoint;
            previewTail.localScale = new Vector3(laserWidth, laserLength, 1f);
        }

        if (previewHead != null)
        {
          previewHead.position = firePoint;
        }

        yield return new WaitForSeconds(preivewLaserDuration);

        GameObject.Destroy(preview);
        
        GameObject laser = GameObject.Instantiate(laserPrefab, firePoint, Quaternion.identity, parent.transform);
        
        Vector3 laserScale = laser.transform.localScale;
        laserScale.x = directionX;
        laser.transform.localScale = laserScale;

        Transform tail = laser.transform.Find("Tail");
        Transform head = laser.transform.Find("Head");
        if (tail != null)
        {
            tail.position = firePoint;
            tail.localScale = new Vector3(laserWidth, laserLength, 1f);
        }

        if (tail.TryGetComponent<EnemyDamageTrigger>(out var trigger))
        {
            trigger.Initialize(attackDamage, parent.gameObject);
        }
        if (head.TryGetComponent<EnemyDamageTrigger>(out var trigger2))
        {
            trigger.Initialize(attackDamage, parent.gameObject);
        }


        yield return new WaitForSeconds(laserDuration);
        GameObject.Destroy(laser);
        yield return new WaitForSeconds(0.5f);
    }
    
    public void SetAlpha(GameObject obj, float alpha)
    {
        foreach (var renderer in obj.GetComponentsInChildren<SpriteRenderer>())
        {
            Color c = renderer.color;
            c.a = alpha;
            renderer.color = c;
        }
    }
}
