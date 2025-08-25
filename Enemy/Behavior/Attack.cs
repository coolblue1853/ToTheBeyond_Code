using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Action
{
    public SharedGameObject player;
    private EnemyController _enemyController;
    private float[] _lastUsed;
    private int _currentSkillIndex = -1;
    private bool _isAttacking = false;
    [SerializeField] private Material _originalMaterial;
    private SpriteRenderer _spriteRenderer;
    private float _attackEndTime = 0f;
    private int _prevSkillIndex = -1; // 직전 스킬
    private float[] _lastEndTime;    
    private bool _subscribed;
    private readonly List<int> _avail = new();
    private readonly List<float> _weights = new();
    public override void OnStart()
    {
        _enemyController = gameObject.GetComponent<EnemyController>();
        if (_enemyController != null && !_subscribed) {
            _enemyController.OnSkillSetChanged += ResetAttackState; // 여기서 1회만 할당 발생
            _subscribed = true;
        }
        
        _spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
    }

     public override TaskStatus OnUpdate()
    {
        _avail.Clear();
        if (_enemyController.IsInPhaseTransition)
        {
            ResetAttackState();
            return TaskStatus.Failure;
        }

        var skills = _enemyController.GetSkills();

        if (_lastEndTime == null || _lastEndTime.Length != skills.Count)
        {
            InitLastEndTime();
            ResetAttackState();
        }

        // 공격 중이면 종료 시점 체크
        if (_isAttacking)
        {
            if (Time.time >= _attackEndTime)
            {
                // 공격 끝남
                _isAttacking = false;
                _prevSkillIndex = _currentSkillIndex;
                _currentSkillIndex = -1;
                return TaskStatus.Success; // 이제서야 성공 반환
            }

            return TaskStatus.Running; // 진행 중엔 Running
        }
        
        for (int i = 0; i < skills.Count; i++)
        {
            if (Time.time >= _lastEndTime[i] + skills[i].coolTime)
                _avail.Add(i);
        }

        if (_avail.Count == 0)
            return TaskStatus.Failure;
        
        if (_avail.Count > 1 && _prevSkillIndex >= 0)
            _avail.Remove(_prevSkillIndex);
        

        int chosenIndex = _avail[Random.Range(0, _avail.Count)];
        chosenIndex = ChooseWithLruBias(_avail, skills);

        var skill = skills[chosenIndex];

        // 방향 회전
        if (_enemyController != null && player.Value != null)
        {
            float directionX = player.Value.transform.position.x - transform.position.x;
            _enemyController.FlipToDirection(directionX);
        }

        // 스킬 실행
        skill.ExecuteAttack(gameObject, player.Value);

        float speedMul = _enemyController?.SpeedMultiplier ?? 1f;
        float actualDuration = skill.duration / Mathf.Max(0.0001f, speedMul);

        // 공격 종료 시간(실제 끝나는 시점)
        _attackEndTime = Time.time + actualDuration;

        // 재사용 가능 시점 관리: “끝난 시각” 기록
        _lastEndTime[chosenIndex] = _attackEndTime;

        _currentSkillIndex = chosenIndex;
        _isAttacking = true;

        return TaskStatus.Running; // 시작 직후도 Running
    }

    private void InitLastEndTime()
    {
        var count = _enemyController.GetSkills().Count;
        _lastEndTime = new float[count];
        for (int i = 0; i < count; i++)
            _lastEndTime[i] = float.NegativeInfinity; // 처음엔 바로 사용 가능
    }

    private void ResetAttackState()
    {
        _isAttacking = false;
        _currentSkillIndex = -1;
        if (_spriteRenderer != null && _originalMaterial != null)
            _spriteRenderer.material = _originalMaterial;
    }

    private int ChooseWithLruBias(List<int> candidates, List<SkillProfileSO> skills)
    {
        _weights.Clear(); 
        float total = 0f;
        foreach (int i in candidates)
        {
            float denom = Mathf.Max(0.0001f, skills[i].coolTime+ skills[i].duration);
            float score = Mathf.Clamp01((Time.time - _lastEndTime[i]) / denom);

            float w = 0.3f + 0.7f * score;
            _weights.Add(w);
            total += w;
        }

        float r = Random.Range(0f, total);
        float c = 0f;
        for (int k = 0; k < _weights.Count; k++)
        {
            c += _weights[k];
            if (r <= c) return candidates[k];
        }
        return candidates[candidates.Count - 1];
    }
    
    public override void OnEnd() {
        if (_enemyController != null && _subscribed) {
            _enemyController.OnSkillSetChanged -= ResetAttackState;
            _subscribed = false;
        }
    }
}

