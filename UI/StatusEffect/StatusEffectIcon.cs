using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class StatusEffectIcon : MonoBehaviour
{
     // 스킬 쿨타임 아이콘 
    public Image Icon;
    public Image CooldownOverlay;

    private float _cooldownDuration;
    private float _cooldownStartTime;
    private float _cooldownEndTime;
    private bool _isActiveCooldown = false;
    
    public event Action OnEffectEnded;

    public void Initialize(Sprite iconSprite, Sprite backSprite, float cooldown)
    {
        Icon.sprite = iconSprite;
        CooldownOverlay.sprite = backSprite;
        _cooldownDuration = cooldown;

        _isActiveCooldown = false;
        CooldownOverlay.fillAmount = 1f;
    }

    public void StartCooldown(float duration)
    {
        _cooldownDuration = duration;
        _cooldownStartTime = Time.time;
        _cooldownEndTime = Time.time + duration;
        _isActiveCooldown = true;
        CooldownOverlay.fillAmount = 0f;
    }

    private void Update()
    {
        if (!_isActiveCooldown)
        {
            return;
        }

        float elapsed = Time.time - _cooldownStartTime;
        float ratio = Mathf.Clamp01(elapsed / _cooldownDuration);
        CooldownOverlay.fillAmount = ratio;

        if (Time.time >= _cooldownEndTime)
        {
            CooldownOverlay.fillAmount = 1f;
            _isActiveCooldown = false;
            
            OnEffectEnded?.Invoke();
        }
    }
}
