using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class MoveToTarget : BaseCheckGround
{
    public SharedGameObject player;
    public SharedFloat stopDistance;
    public bool isFalling = false;
    private EnemyController _enemyController;
    public override void OnStart()
    {
        _enemyController = GetComponent<EnemyController>();
    }
    public override TaskStatus OnUpdate()
    {
        if (!IsPlayerValid())
        {
            return TaskStatus.Failure;
        }

        // 플레이어 방향 계산
        Vector3 dir = GetDirectionToTarget();
        _direction = Mathf.RoundToInt(Mathf.Sign(dir.x)); // 좌우 방향 결정
        // 도달 조건 체크
        if (IsStopDistance())
            return TaskStatus.Success;
        // 벽 또는 낭떠러지 감지 시 이동 중단
        if (!isFalling)
        {
            if (IsEdge() || IsWall())
                return TaskStatus.Failure;
        }

        // 플레이어 방향으로 이동
        float baseSpeed = 1f;
        float adjustedSpeed = baseSpeed * (_enemyController?.SpeedMultiplier ?? 1f);
        Move(new Vector3(dir.x, 0, 0) * adjustedSpeed); // deltaTime 제거


        // 방향에 따라 스프라이트 반전
        if (_enemyController != null)
        {
            float dirToPlayer = player.Value.transform.position.x - transform.position.x;
            _enemyController.FlipToDirection(dirToPlayer);
        }
        return TaskStatus.Running;
    }
    // 플레이어가 유효한지 확인
    private bool IsPlayerValid()
    {
        return player.Value != null;
    }
    // 플레이어 방향 벡터 계산
    private Vector3 GetDirectionToTarget()
    {
        return (player.Value.transform.position - transform.position).normalized;
    }
    // 플레이어와의 거리가 멈춰야 하는 거리 이내인지 체크
    private bool IsStopDistance()
    {
        float dist = Vector3.Distance(transform.position, player.Value.transform.position);
        return dist <= stopDistance.Value;
    }
}
