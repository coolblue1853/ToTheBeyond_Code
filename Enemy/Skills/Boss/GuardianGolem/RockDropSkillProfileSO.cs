using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/Skill/RockDrop")]
public class RockDropSkillProfileSO : SkillProfileSO
{
    [Header("떨어지는 돌 설정")]
    public GameObject rockPrefab;
    public int numberOfRocks = 10; // 총 낙하할 바위 수
    public float dropInterval = 0.2f; // 바위 간 낙하 간격

    [Header("떨어지는 범위")]
    public float dropXMin = -10f; // 낙하 범위 X좌표 최소 값
    public float dropXMax = 10f; // 낙하 범위 X좌표 최대 값
    public float dropY = 10f; // 바위 생성 Y 좌표
    
    
    [Header("인디케이터 설정")]
    public GameObject warningPrefab;
    [SerializeField] private float indicatorY = -3f;
    [SerializeField] private float indicatorDuration = 0.3f;
    [SerializeField] private Vector3 indicatorScale = new Vector3(2.5f, 2f, 1f);
    public override void ExecuteAttack(GameObject obj, GameObject target)
    {
        obj.GetComponent<Animator>()?.Play("Slam", 0, 0f);
        obj.GetComponent<EnemyController>()?.SetCurrentSkill(this, target);
    }

    private IEnumerator SpawnRocks(GameObject obj)
    {
        for (int i = 0; i < numberOfRocks; i++)
        {
            float x = Random.Range(dropXMin, dropXMax);
            Vector3 warningPos = new Vector3(x, indicatorY, 0f);

            // [1] 인디케이터 생성
            if (warningPrefab != null)
            {
                GameObject indicator = Instantiate(warningPrefab, warningPos, Quaternion.identity);
                indicator.transform.localScale = indicatorScale;
                Destroy(indicator, indicatorDuration + 0.1f); // 약간 여유
            }
            Vector3 spawnPos = new Vector3(x, dropY, 0f);
            GameObject rock = Instantiate(rockPrefab, spawnPos, Quaternion.identity);
            if (rock.TryGetComponent<EnemySlam>(out var slam))
            {
                slam.SetGroundY(-5f); 
            }
            Vector3 rockScale = rock.transform.localScale;
            float directionX = Mathf.Sign(obj.transform.localScale.x);
            rockScale.x = Mathf.Abs(rockScale.x) * directionX;
            rock.transform.localScale = rockScale;
            
            EnemyRock enemyRock = rock.GetComponent<EnemyRock>();
            if (enemyRock != null)
                enemyRock.damage = attackDamage;

            yield return new WaitForSeconds(dropInterval);
        }
    }

    public override void OnHit(GameObject obj, GameObject target)
    {
        obj.GetComponent<MonoBehaviour>().StartCoroutine(SpawnRocks(obj));
    }
}
