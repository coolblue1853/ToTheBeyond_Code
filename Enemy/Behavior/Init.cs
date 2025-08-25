using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
// 적의 초기 설정을 위한 Action노드
public class Init : Action
{
    public SharedGameObject player; // 플레이어 
    public SharedFloat detectDistance; // 탐지 거리
    public SharedFloat stopDistance; // 정지 거리
    public SharedFloat minPatrolDistance; // 최소 배회 거리
    public SharedFloat maxPatrolDistance; // 최대 배회 거리
    public SharedFloat minStopTime; // 최소 정지 시간
    public SharedFloat maxStopTime; // 최대 정지 시간
    public SharedFloat speed; // 이동 속도
    public override void OnStart()
    {
        GameObject foundPlayer = GameObject.FindWithTag("Player");
        if (foundPlayer != null)
        {
            player.Value = foundPlayer;
        }
        
        // EnemyController에서 적 고유 설정값을 불러와 공유 변수에 설정
        EnemyController controller = gameObject.GetComponent<EnemyController>();
        if (controller != null && controller.enemy != null)
        {
            detectDistance.Value = controller.enemy.detectDistance;
            stopDistance.Value = controller.enemy.stopDistance;
            minPatrolDistance.Value = controller.enemy.minPatrolDistance;
            maxPatrolDistance.Value = controller.enemy.maxPatrolDistance;
            minStopTime.Value = controller.enemy.minStopTime;
            maxStopTime.Value = controller.enemy.maxStopTime;
            speed.Value = controller.enemy.speed;
        }
    }
}
