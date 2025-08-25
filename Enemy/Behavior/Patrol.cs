using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class Patrol : BaseCheckGround
{
    // 순찰 거리 범위
    public SharedFloat minPatrolDistance;
    public SharedFloat maxPatrolDistance;
    private Vector3 _startPos; // 현재 순찰 위치
    private float _targetDistance; // 이동해야 할 순찰 거리
    private float _startTime;
    private EnemyController _enemyController;
    public override void OnStart()
    {
        _enemyController = GetComponent<EnemyController>();
        _startPos = transform.position; // 현재 위치를 순찰 시작점으로 저장
        // 순찰 방향 랜덤 지정
        _direction = Random.value < 0.5f ? -1 : 1;
        // 이동할 거리 랜덤 설정
        ChooseNewTargetDistance();
        // 방향에 맞게 스프라이트 반전
        ReverseDirection();
        
        _startTime = Time.time; 
    }

    public override TaskStatus OnUpdate()
    {
        if (Time.time - _startTime >= 5f)
        {
            return TaskStatus.Success;
        }

        if (IsEdge() || IsWall())
            return TaskStatus.Failure;

        // 지정된 방향으로 이동
        float baseSpeed = 1f; // 원하는 기본 속도
        float adjustedSpeed = baseSpeed * (_enemyController?.SpeedMultiplier ?? 1f);
        Move(Vector3.right * _direction * adjustedSpeed);


        
        // 시작점에서 목표 거리만큼 이동했는지 체크
        if (Mathf.Abs(transform.position.x - _startPos.x) >= _targetDistance)
        {
            ReverseDirection();
            return TaskStatus.Success; // 이동 완료
        }

        return TaskStatus.Running; // 계속 이동 중
    }
    
    // 순찰 거리를 랜덤으로 설정
    private void ChooseNewTargetDistance()
    {
        _targetDistance = Random.Range(minPatrolDistance.Value, maxPatrolDistance.Value);
    }
    
    // 방향을 반전하고 새로운 순찰 거리 설정
    private void ReverseDirection()
    {
        if (_enemyController != null)
        {
            _enemyController.FlipToDirection(_direction);
        }
    }
}
