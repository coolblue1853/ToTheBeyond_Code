using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Enemy/Skill/Meteor")]
public class MeteorSkillProfileSO : SkillProfileSO
{
    [Header("이펙트 프리팹")]
    public GameObject laserEffectPrefab; // 미리 경고용으로 표시할 레이저 이펙트
    public GameObject fallingSpherePrefab; // 낙하하는 구체 프리팹
    public GameObject hitEffectPrefab; // 충돌 시 발생할 이펙트

    [Header("세부 설정")]
    public float laserDuration = 1f; // 레이저가 보여지는 시간
    public float sphereFallDelay = 0.1f; // 구체 간 낙하 간격
    public float projectileDuration = 0.6f; // 구체 낙하 지속 시간
    
    [Header("처음 위치")]
    public float startY = 10f;
    
    [Header("레이저 설정")]
    public float laserLength = 2f;
    public float laserWidth = 2f;
    public float headWidth = 2f;
    private Vector3 _meteorPivot; // 운선이 떨어지는 기준 위치
    private List<Transform> _platformPoints; // 운석이 떨어질 플랫폼 위치들
    
    [Header("미리보기")]
    [SerializeField] private GameObject redIndicatorPrefab;
    [SerializeField] private float indicatorDuration = 0.1f; 


    [Header("메테오 갯수")]
    public int totalWaves = 5;
    public int meteorsPerWave = 5;

    public void CollectPlatformPointsFromBoss()
    {
        _platformPoints = new List<Transform>();
    
        GameObject boss = GameObject.Find("Forest_FinalBoss(Clone)");
        if (boss == null)
        {
            return;
        }

        Transform graphics = boss.transform.Find("Graphics");

        foreach (Transform child in graphics)
        {
            string name = child.name;
            if (name.Contains("Platform") && name != "Platform_Tile")
            {
                _platformPoints.Add(child);
            }
            else if (name.Contains("Ground"))
            {
                _platformPoints.Add(child);
            }
        }

        _platformPoints = _platformPoints.OrderBy(x => UnityEngine.Random.value).ToList();
    }

    public override void ExecuteAttack(GameObject obj, GameObject target)
    {
        CollectPlatformPointsFromBoss();
        if (_platformPoints == null || _platformPoints.Count == 0)
        {
            return;
        }
        
        obj.GetComponent<Animator>().CrossFade("Meteor", 0.1f);
        obj.GetComponent<EnemyController>()?.SetCurrentSkill(this, target);
        _meteorPivot = obj.transform.position + new Vector3(0, startY, 0);
        
    }
    private GameObject SpawnWarningLaser(GameObject owner)
    {
        if (laserEffectPrefab == null) return null;
        var pivot = owner.GetComponent<EnemyAnchors>()?.GetPivot("Meteor");
        Vector3 spawnPos = pivot.position + Vector3.up * 1f;
        Quaternion rotation = Quaternion.AngleAxis(90f, Vector3.forward);

        GameObject laser = GameObject.Instantiate(laserEffectPrefab, spawnPos, rotation, owner.transform);

        // Tail 크기 조절
        Transform tail = laser.transform.Find("Tail");
        if (tail != null)
        {
            tail.localScale = new Vector3(laserWidth, laserLength, 1f);
        }

        // Head 크기 조절
        Transform head = laser.transform.Find("Head");
        if (head != null)
        {
            head.localScale = new Vector3(headWidth, headWidth, 1f); // 예: 따로 설정된 값
        }

        return laser;
    }

    private IEnumerator LaserDropRoutine(GameObject owner)
    {
        GameObject laser = SpawnWarningLaser(owner); 
        yield return new WaitForSeconds(laserDuration);


        float delayBetweenBatches = 1f;

        for (int i = 0; i < totalWaves; i++)
        {
            List<Vector3> targetPositions = new List<Vector3>();
            
            for (int j = 0; j < meteorsPerWave; j++)
            {
                Transform platform = _platformPoints[Random.Range(0, _platformPoints.Count)];
                var (targetPos, width) = GetRandomPointAndWidth(platform);

                if (redIndicatorPrefab != null)
                {
                    GameObject indicator = Instantiate(redIndicatorPrefab, targetPos, Quaternion.identity);
                    width = Mathf.Clamp(width, 1f, 10f);
                    indicator.transform.localScale = new Vector3(width, indicator.transform.localScale.y, 1f);
                    indicator.transform.position = new Vector3(targetPos.x, targetPos.y, 0f);
                    Destroy(indicator, indicatorDuration); // 조금 여유 있게
                }

                targetPositions.Add(targetPos);
            }
            
            yield return new WaitForSeconds(indicatorDuration);
            
            foreach (var targetPos in targetPositions)
            {
                Vector3 startPos = _meteorPivot;
                GameObject meteor = Instantiate(fallingSpherePrefab, startPos, Quaternion.identity);
                var proj = meteor.GetComponent<EnemyMeteorProjectile>();
                if (proj != null)
                {
                    proj.hitEffectPrefab = hitEffectPrefab;
                    proj.damage = 5;
                    proj.Launch(targetPos, projectileDuration);
                }
            }
            yield return new WaitForSeconds(delayBetweenBatches);
        }

        Destroy(laser);
    }

    public override void OnHit(GameObject obj, GameObject target)
    {
        obj.GetComponent<MonoBehaviour>().StartCoroutine(LaserDropRoutine(obj));
    }
    
    private (Vector3 point, float width) GetRandomPointAndWidth(Transform platform)
    {
        float platformWidth = 2f;
        Vector3 center = platform.position;

        if (platform.TryGetComponent<BoxCollider2D>(out var col))
        {
            platformWidth = col.size.x * platform.lossyScale.x;
            center = col.bounds.center;
        }
        else if (platform.TryGetComponent<SpriteRenderer>(out var renderer))
        {
            platformWidth = renderer.bounds.size.x;
            center = renderer.bounds.center;
        }
        else if (platform.TryGetComponent<TilemapCollider2D>(out var tilemapCol))
        {
            platformWidth = tilemapCol.bounds.size.x * platform.lossyScale.x;
            center = tilemapCol.bounds.center;
        }

        float half = platformWidth / 2f;
        float randomX = Random.Range(-half + 2f, half - 2f);

        Vector3 startPos = center + new Vector3(randomX, 5f, 0f);
        RaycastHit2D hit = Physics2D.Raycast(startPos, Vector2.down, 20f, LayerMask.GetMask("Ground"));

        Vector3 finalPos = hit.collider != null ? (Vector3)(hit.point + Vector2.up * 0.1f) : center + new Vector3(randomX, 0.5f, 0f);

        return (finalPos, platformWidth);
    }
}
