using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    public static StatusEffectManager Instance;

    [SerializeField] private Transform effectPanel;
    [SerializeField] private StatusEffectIcon effectIconPrefab;

    private Dictionary<string, StatusEffectIcon> activeEffects = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        StatusEffectEvents.OnEffectApplied += ApplyEffectFromSkill;
        StatusEffectEvents.OnEffectRemoved += RemoveEffect;
    }

    private void OnDisable()
    {
        StatusEffectEvents.OnEffectApplied -= ApplyEffectFromSkill;
        StatusEffectEvents.OnEffectRemoved -= RemoveEffect;
    }

    public void ApplyEffectFromSkill(SkillSO skill)
    {
        if (effectIconPrefab == null || skill == null) return;

        // 고유한 ID로 설정 (skillName은 중복 가능성 있음)
        string id = $"SKILL_{skill.slot}_{skill.name}";
        float duration = skill.duration;
        Sprite icon = skill.iconSprite;
        Sprite overlay = skill.iconBackSprite;

        if (activeEffects.TryGetValue(id, out var iconUI))
        {
            iconUI.StartCooldown(duration);
        }
        else
        {
            var newIconUI = Instantiate(effectIconPrefab, effectPanel);
            newIconUI.Initialize(icon, overlay, duration);
            newIconUI.StartCooldown(duration);
            newIconUI.OnEffectEnded += () => RemoveEffect(id);
            activeEffects[id] = newIconUI;
        }
    }
    public void ApplyEffectFromSkill(SkillSO skill, float duration)
    {
        if (effectIconPrefab == null || skill == null) return;

        string id = $"SKILL_{skill.slot}_{skill.name}";
        Sprite icon = skill.iconSprite;
        Sprite overlay = skill.iconBackSprite;

        if (activeEffects.TryGetValue(id, out var iconUI))
        {
            iconUI.StartCooldown(duration);
        }
        else
        {
            var newIconUI = Instantiate(effectIconPrefab, effectPanel);
            newIconUI.Initialize(icon, overlay, duration);
            newIconUI.StartCooldown(duration);
            newIconUI.OnEffectEnded += () => RemoveEffect(id);
            activeEffects[id] = newIconUI;
        }
    }

    private void RemoveEffect(string effectId)
    {
        if (activeEffects.TryGetValue(effectId, out var icon))
        {
            Destroy(icon.gameObject);
            activeEffects.Remove(effectId);
        }
    }
}
