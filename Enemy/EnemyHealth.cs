using UnityEngine;
using DamageNumbersPro;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using System.Linq;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    private float _currentHealth;
    public SharedBool isControllable;
    public float CurrentHealth => _currentHealth;
    public bool IsDead => _currentHealth <= 0f;
    private EnemyDebuffHandler _debuffHandler;
    private EnemyController _enemyController;
    private PlayerController _playerController;
    private Animator _animator;
    private BehaviorTree _bt;

    [SerializeField] private DamageNumber _damageNumber;
    [SerializeField] private DamageNumber _critDamageNumber;
    [SerializeField] private BoxCollider2D _damageArea;

    [Header("보스 페이지 관리")]
    private EnemyType _enemyType;
    [SerializeField] private float _phaseDelay = 1.0f;
    [SerializeField] private Animator _phaseAnimator;
    private bool _phase2Started = false;
    private bool _phase3Started = false;
    private bool _isPhaseTransitioning = false;

    [Header("피격 이펙트")]
    private HitEffect _hitEffect;
    private FlashEffect _flashEffect;
    private ShakeEffect _shakeEffect;

    [Header("체력 관련")]
    private Image _healthImage;
    private Image _healthImage2;
    private Image _activeHealthImage;
    private GameObject _healthPanel;
    private Coroutine _hideHealthBarCoroutine;
    private Coroutine _updateHealthBarCoroutine;

    [SerializeField] private GameObject healItemPrefab;
    [SerializeField] private GameObject coinItemPrefab;
    [SerializeField] private GameObject ingameCoinPrefab;
    [SerializeField] private bool forceCoinDrop = false;
    [SerializeField] private float coinSpread = 0.35f; // 코인들 사이 간격
    [SerializeField] private float coinJitter = 0.1f;  // 살짝 랜덤 흔들림

    private void Awake()
    {
        _debuffHandler = GetComponent<EnemyDebuffHandler>();
        _enemyController = GetComponent<EnemyController>();
        _animator = GetComponent<Animator>();

        _hitEffect = GetComponent<HitEffect>();
        _flashEffect = GetComponent<FlashEffect>();
        _shakeEffect = GetComponent<ShakeEffect>();

        _phaseAnimator = GetComponentsInChildren<Animator>().FirstOrDefault(anim => anim.gameObject != this.gameObject);
        _playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        if (_enemyController != null)
        {
            _currentHealth = _enemyController.enemy.maxHealth;
            _enemyType = _enemyController.enemy.enemyType;
        }

        _bt = GetComponent<BehaviorTree>();
        if (_bt != null)
            isControllable = (SharedBool)_bt.GetVariable("isControllable");

        Transform panelTransform = transform.Find("Canvas/Panel");
        if (panelTransform != null)
        {
            _healthPanel = panelTransform.gameObject;
            var h1 = panelTransform.Find("health");
            var h2 = panelTransform.Find("health2");
            if (h1 != null) _healthImage = h1.GetComponent<Image>();
            if (h2 != null) _healthImage2 = h2.GetComponent<Image>();
            _activeHealthImage = _healthImage;
        }
    }

   public void TakeDamage(float damage, bool isCrit)
{
    if (_isPhaseTransitioning) return;

    float damageMultiplier = _debuffHandler != null ? _debuffHandler.GetDamageTakenModifier() : 1f;
    float finalDamage = damage * damageMultiplier;
    float maxHealth = _enemyController.enemy.maxHealth;

    bool isPhaseCut = false;

    // 페이즈 컷 처리
    if (_enemyType == EnemyType.Boss && !_phase2Started && _currentHealth - finalDamage <= maxHealth * 0.5f)
    {
        _currentHealth = maxHealth * 0.5f;
        _phase2Started = true;
        isPhaseCut = true;
        StartCoroutine(EnterPhaseWithDelay(2));
    }
    else if (_enemyType == EnemyType.FinalBoss)
    {
        if (!_phase2Started && _currentHealth - finalDamage <= maxHealth * 0.7f)
        {
            _currentHealth = maxHealth * 0.7f;
            _phase2Started = true;
            isPhaseCut = true;
            StartCoroutine(EnterPhaseWithDelay(2));
        }
        else if (!_phase3Started && _currentHealth - finalDamage <= maxHealth * 0.3f)
        {
            _currentHealth = maxHealth * 0.3f;
            _phase3Started = true;
            isPhaseCut = true;
            StartCoroutine(EnterPhaseWithDelay(3));
        }
    }

    // 페이즈 컷이 아닐 경우에만 실제 체력 차감
    if (!isPhaseCut)
        _currentHealth -= finalDamage;

    // 체력 UI 갱신
    if (_healthPanel != null && _enemyController != null)
    {
        _healthPanel.gameObject.SetActive(true);
        float fill = CalculateHealthFill();
        _activeHealthImage.fillAmount = fill;

        if (_updateHealthBarCoroutine != null)
            StopCoroutine(_updateHealthBarCoroutine);
        _updateHealthBarCoroutine = StartCoroutine(AnimateHealthBar(fill));
    }

    // 피격 이펙트
    _hitEffect?.PlayHitEffect();
    if (_flashEffect != null)
        _flashEffect.TryTriggerFlash(); // 안전하게 호출
    _shakeEffect?.Shake();

    Vector2 spawnPos = GetRandomPointInBox(_damageArea);
    if (isCrit) _critDamageNumber.Spawn(spawnPos, (int)finalDamage + "!");
    else _damageNumber.Spawn(spawnPos, (int)finalDamage);

    // 사망 조건
    if (_currentHealth <= 0)
    {
        if (!_isPhaseTransitioning)
            Die();
    }
}


    private IEnumerator HideHealthBarAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_healthPanel != null)
            _healthPanel.gameObject.SetActive(false);
    }

    private IEnumerator AnimateHealthBar(float targetFill)
    {
        if (_activeHealthImage == null) yield break;

        float currentFill = _healthImage.fillAmount;
        float elapsed = 0f;
        float duration = 0.3f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            _healthImage.fillAmount = Mathf.Lerp(currentFill, targetFill, t);
            yield return null;
        }

        _activeHealthImage.fillAmount = targetFill;
    }

    private Vector2 GetRandomPointInBox(BoxCollider2D box)
    {
        if (box == null && this.gameObject != null) return transform.position; // fallback 위치

        try
        {
            Vector2 center = box.bounds.center;
            Vector2 size = box.bounds.size;
            float x = Random.Range(center.x - size.x / 2f, center.x + size.x / 2f);
            float y = Random.Range(center.y - size.y / 2f, center.y + size.y / 2f);
            return new Vector2(x, y);
        }
        catch
        {
            return transform.position;
        }
    }


    public void ApplyDebuff(DebuffEffectSO debuff)
    {
        if (_debuffHandler == null) return;
        var debuffInfo = DebuffInfo.FromSO(debuff);
        _debuffHandler.ApplyDebuff(debuffInfo);
    }
    public void ApplyDebuff(DebuffEffectSO debuff, DamagePayload payload)
    {
        if (_debuffHandler == null) return;

        DebuffInfo info = DebuffInfo.FromSO(debuff);
        _debuffHandler.ApplyDebuff(info, payload);
    }

    public void ApplyDebuff(DebuffInfo info)
    {
        if (_debuffHandler == null) return;
        _debuffHandler.ApplyDebuff(info);
    }

    private void Die()
    {
        _animator.Play("Die", 0, 0f);

        if (forceCoinDrop)
        {
            var coin = Instantiate(coinItemPrefab, transform.position, Quaternion.identity);
            coin.transform.SetParent(transform.parent);
        }
        else
        {
            float chance = Random.value;
            if (chance < 0.05f)
            {
                var heal = Instantiate(healItemPrefab, transform.position, Quaternion.identity);
                heal.transform.SetParent(transform.parent);
            }
            else if (chance < 0.15f)
            {
                var coin = Instantiate(coinItemPrefab, transform.position, Quaternion.identity);
                coin.transform.SetParent(transform.parent);
            }
            else if (chance < 0.35f)
            {
                // 1개 70%, 2개 20%, 3개 10%
                float r = Random.value;
                int count = (r < 0.70f) ? 1 : (r < 0.90f ? 2 : 3);
                SpawnIngameCoins(count);
            }

        }

        GetComponent<EnemyDeathEffect>().PlayDeathEffect();
        Destroy(this.gameObject);
    }
    private void SpawnIngameCoins(int count)
    {
        if (ingameCoinPrefab == null || count <= 0) return;

        Transform parent = transform.parent;
        Vector3 basePos = transform.position;

        // 가운데 정렬되도록 시작 오프셋 계산
        float start = -(count - 1) * coinSpread * 0.5f;

        for (int i = 0; i < count; i++)
        {
            Vector3 offset = new Vector3(
                start + i * coinSpread + Random.Range(-coinJitter, coinJitter),
                Random.Range(0f, coinJitter), // 살짝 위/아래
                0f
            );

            var coin = Instantiate(ingameCoinPrefab, basePos + offset, Quaternion.identity);
            if (parent != null) coin.transform.SetParent(parent);
        }
    }


    private IEnumerator EnterPhaseWithDelay(int phase)
    {
        _isPhaseTransitioning = true;

        _phaseAnimator.gameObject.SetActive(true);
        isControllable.Value = false;
        _playerController.isControllable = false;
        _playerController.ResetVelocity();
        _enemyController.SetPhaseTransition(true);

        yield return new WaitForSeconds(_phaseDelay);

        float animLength = 0f;
        if (_phaseAnimator != null)
        {
            _phaseAnimator.Play($"Phase{phase}", 0, 0f);
            yield return null;
            AnimatorStateInfo info = _phaseAnimator.GetCurrentAnimatorStateInfo(0);
            animLength = info.length;
        }

        if (phase == 2 && _healthImage2 != null)
        {
            _activeHealthImage.gameObject.SetActive(false);
            _activeHealthImage = _healthImage2;
            _activeHealthImage.gameObject.SetActive(true);
        }

        _enemyController?.EnterPhase(phase);

        yield return new WaitForSeconds(animLength + 1f);

        _phaseAnimator.gameObject.SetActive(false);
        isControllable.Value = true;
        _playerController.isControllable = true;
        _enemyController.SetPhaseTransition(false);

        _isPhaseTransitioning = false;
    }

    private float CalculateHealthFill()
    {
        float maxHealth = _enemyController.enemy.maxHealth;
        if (_enemyType == EnemyType.Boss)
            return CalculateBossHealthFill(_currentHealth, maxHealth);
        return Mathf.Clamp01(_currentHealth / maxHealth);
    }

    private float CalculateBossHealthFill(float currentHealth, float maxHealth)
    {
        float half = maxHealth * 0.5f;
        if (_activeHealthImage == _healthImage)
            return Mathf.Clamp01((currentHealth - half) / half); // 100~50
        else
            return Mathf.Clamp01(currentHealth / half); // 50~0
    }
}
