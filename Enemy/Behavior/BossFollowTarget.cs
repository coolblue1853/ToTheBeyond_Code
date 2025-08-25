using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
public class BossFollowTarget : BaseCheckGround
{
    public SharedGameObject player;
    public SharedFloat stopDistance;
    public SharedFloat followDuration = 0.5f; // 추적 지속 시간 (초)
    private EnemyController _enemyController;
    private float _followTimer = 0f;

    public override void OnStart()
    {
        _enemyController = GetComponent<EnemyController>();
        _followTimer = followDuration.Value;
    }

    public override TaskStatus OnUpdate()
    {
        if (!IsPlayerValid())
            return TaskStatus.Failure;

        if (_followTimer <= 0f)
            return TaskStatus.Success;

        _followTimer -= Time.deltaTime;

        Vector3 dir = GetDirectionToTarget();
        _direction = Mathf.RoundToInt(Mathf.Sign(dir.x));
        
        float baseSpeed = 1f;
        float adjustedSpeed = baseSpeed * (_enemyController?.SpeedMultiplier ?? 1f);
        Move(new Vector3(dir.x, 0, 0) * adjustedSpeed);

        if (_enemyController != null)
        {
            float dirToPlayer = player.Value.transform.position.x - transform.position.x;
            _enemyController.FlipToDirection(dirToPlayer);
        }

        return TaskStatus.Running;
    }

    private bool IsPlayerValid() => player.Value != null;

    private Vector3 GetDirectionToTarget() =>
        (player.Value.transform.position - transform.position).normalized;

    private bool IsStopDistance()
    {
        float dist = Vector3.Distance(transform.position, player.Value.transform.position);
        return dist <= stopDistance.Value;
    }
}
