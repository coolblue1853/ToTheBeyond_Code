using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    private RuntimeStat _stat;
    private PlayerController _playerController;

    [SerializeField] private float _invincibleDuration = 1.0f;
    [SerializeField] private float _currentHealth;
    private bool _isInvincible = false;
    private bool _isDead = false;

    [Header("Damage Outline Effect")]
    [SerializeField] private Image _damageOutlineImage;
    [SerializeField] private float _flashFadeDuration = 0.5f;
    private Tween _damageFlashTween;

    // MaxHealth 변화 추적용 캐시
    private float _lastKnownMaxHealth;

    // (옵션) MaxHealth 변경 시 풀피로 회복할지 여부
    private bool _restoreFullOnMaxHealthChange = false;

    public float CurrentHealth
    {
        get => _currentHealth;
        set => _currentHealth = Mathf.Clamp(value, 0, MaxHealth);
    }

    public float MaxHealth => _stat != null ? _stat.MaxHealth : 1f;
    public bool IsInvincible => _isInvincible;

    public event Action OnMaxHealthChanged;
    public event Action OnDamaged;
    public event Action OnHealed;
    public event Action OnDeath;

    public void Init(PlayerController playerController)
    {
        _playerController = playerController;
        _stat = _playerController.runtimeStat;

        _stat.OnStatChanged += HandleStatChanged;
        OnDeath += () => GameManager.Instance.ShowDeathUI();

        // 초기 체력/캐시 셋업
        _currentHealth = MaxHealth;
        _lastKnownMaxHealth = MaxHealth;
    }

    private void OnDestroy()
    {
        if (_stat != null)
            _stat.OnStatChanged -= HandleStatChanged;

        _damageFlashTween?.Kill();
    }

    private void HandleStatChanged(StatType type)
    {
        if (type == StatType.MaxHealth)
        {
            float newMax = MaxHealth;
            float delta = newMax - _lastKnownMaxHealth;

            if (_restoreFullOnMaxHealthChange)
            {
                _currentHealth = newMax;
            }
            else
            {
                // 변화량만큼 현재체력 이동
                _currentHealth = _currentHealth + delta;

                // 장착 해제(최대체력 감소) 시 최소 1 HP 보장
                if (delta < 0 && _currentHealth < 1f)
                {
                    _currentHealth = 1f;
                }

                // MaxHealth 범위로 클램프
                _currentHealth = Mathf.Clamp(_currentHealth, 0f, newMax);
            }

            _lastKnownMaxHealth = newMax;
            OnMaxHealthChanged?.Invoke();
        }
    }

    public void ApplyPermanentUpgradeBonuses()
    {
        foreach (var upgrade in PermanentUpgradeManager.Instance.upgrades)
        {
            float bonus = PermanentUpgradeManager.Instance.GetUpgradeBonus(upgrade.statType);

            _stat.RemoveAllModifiersOfTypeWithTag(upgrade.statType, "PermanentUpgrade");

            if (bonus != 0)
            {
                _stat.AddModifier(new StatModifier(upgrade.statType, bonus, StatModifier.ModifierMode.Additive, "PermanentUpgrade"));
            }
            else
            {
                _stat.NotifyStatChanged(upgrade.statType);
            }
        }
    }

    public void SetRestoreFullOnMaxHealthChange(bool value)
    {
        _restoreFullOnMaxHealthChange = value;
    }

    public void TakeDamage(float amount)
    {
        if (_isInvincible || amount <= 0f || _isDead) return;

        float damageMultiplier = 1f;
        if (_stat != null)
            damageMultiplier += _stat.DamageTakenIncrease;

        float finalDamage = amount * damageMultiplier;

        _playerController.effectController.PlayBlinkEffect(_invincibleDuration);

        _currentHealth -= finalDamage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth);
        OnDamaged?.Invoke();

        PlayDamageFlash();

        if (_currentHealth <= 0)
        {
            _isDead = true;
            _playerController.isControllable = false;
            _playerController.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            OnDeath?.Invoke();
        }
        else
        {
            StartCoroutine(StartInvincibility());
        }
    }

    public void Heal(float amount)
    {
        if (amount <= 0f) return;

        _currentHealth += amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth);
        OnHealed?.Invoke();
    }

    public bool UseHealth(float amount)
    {
        if (_currentHealth >= amount)
        {
            _currentHealth -= amount;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth);
            OnDamaged?.Invoke();
            return true;
        }
        return false;
    }

    private IEnumerator StartInvincibility()
    {
        _isInvincible = true;
        yield return new WaitForSeconds(_invincibleDuration);
        _isInvincible = false;
    }

    public void StartInvincibility(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(InvincibleCoroutine(duration));
    }

    private IEnumerator InvincibleCoroutine(float duration)
    {
        _isInvincible = true;
        yield return new WaitForSeconds(duration);
        _isInvincible = false;
    }

    public void SetInvincible(bool value)
    {
        _isInvincible = value;
    }

    public void ResetHealth()
    {
        _currentHealth = MaxHealth;
        _isDead = false;
        OnHealed?.Invoke();
    }

    private void PlayDamageFlash()
    {
        if (_damageOutlineImage == null) return;

        _damageFlashTween?.Kill();

        Color baseColor = _damageOutlineImage.color;
        baseColor.a = 0.5f;
        _damageOutlineImage.color = baseColor;

        _damageFlashTween = _damageOutlineImage
            .DOFade(0f, _flashFadeDuration)
            .SetEase(Ease.OutQuad);
    }
}
