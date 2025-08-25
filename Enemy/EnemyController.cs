using UnityEngine;
using BehaviorDesigner.Runtime;
using System.Collections.Generic;
using System;
public class EnemyController : MonoBehaviour
{
    public EnemySkillSetSO skillSet;
    public EnemySO enemy;
    public GameObject sprite;
    public List<SkillProfileSO> GetSkills() => skillSet.skills;
    private SkillProfileSO _currentSkill;
    private GameObject _currentTarget;
    private EnemyHealth _health;
    public Action OnSkillSetChanged;
    public bool IsInPhaseTransition { get; private set; } = false;
    public float SpeedMultiplier { get; private set; } = 1f;
    public EnemyHealth  Health => _health;
    [SerializeField] private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _health = GetComponent<EnemyHealth>();
        SetSpeedMultiplier(1f); 
    }

    public void SetSpeedMultiplier(float value)
    {
        SpeedMultiplier = Mathf.Clamp(value, 0.1f, 1f);
        if (_animator != null)
            _animator.speed = SpeedMultiplier;
    }
    public void SetCurrentSkill(SkillProfileSO skill, GameObject target)
    {
        _currentSkill = skill;
        _currentTarget = target;
    }
    public void SetPhaseTransition(bool active)
    {
        IsInPhaseTransition = active;
    }
    public void TriggerHit()
    {
        if (_currentSkill != null)
            _currentSkill.OnHit(gameObject, _currentTarget);
    }

    public void EnterPhase(int phase)
    {
        switch (phase)
        {
            case 2:
                if (enemy is BossSO boss && boss.phase2SkillSet != null)
                {
                    skillSet = boss.phase2SkillSet;
                }
                break;
            /*
            case 3:
                if (enemy.phase3SkillSet != null)
                {
                    skillSet = enemy.phase3SkillSet;
                    Debug.Log("3페이즈 스킬셋 적용");
                }
                break;
            */
        } 
    }
    
    public void FlipToDirection(float directionX)
    {
        if (sprite == null) return;

        Vector3 scale = sprite.transform.localScale;
        scale.x = directionX < 0 ? 1f : -1f;
        sprite.transform.localScale = scale;
    }
    
    public bool IsFacingLeft()
    {
        if (sprite == null) return false;
        return sprite.transform.localScale.x < 0;
    }
}
