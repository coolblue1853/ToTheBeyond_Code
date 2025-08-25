using BehaviorDesigner.Runtime;
using System.Collections;
using UnityEngine;

public class EnemyStunned : MonoBehaviour
{
    public SharedBool isStunned;

    private Rigidbody2D _rb;
    private BehaviorTree _bt;
    private Coroutine _stunCoroutine;
    private float _remainingStunTime = 0f;

    public bool externalControlDisabled = false; // 외부에서 강제로 AI 제어 차단할 때

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _bt = GetComponent<BehaviorTree>();
        if (_bt != null)
            isStunned = (SharedBool)_bt.GetVariable("isStunned");
    }

    public void ApplyStun(float duration)
    {
        if (_bt == null || externalControlDisabled) return;

        isStunned.Value = true;
        _remainingStunTime = duration;
        Debug.Log("Stunned for " + duration + " seconds");

        _bt.DisableBehavior();
        if (_rb != null)
        {
            _rb.velocity = Vector2.zero;
            _rb.isKinematic = true;
        }

        if (_stunCoroutine != null) StopCoroutine(_stunCoroutine);
        _stunCoroutine = StartCoroutine(StunRoutine());
    }

    public void ApplySuperArmorStun(float duration)
    {
        if (_bt == null || externalControlDisabled) return;

        _remainingStunTime = duration;

        _bt.DisableBehavior();

        if (_stunCoroutine != null) StopCoroutine(_stunCoroutine);
        _stunCoroutine = StartCoroutine(StunRoutine());
    }

    private IEnumerator StunRoutine()
    {
        while (_remainingStunTime > 0f)
        {
            _remainingStunTime -= Time.deltaTime;
            yield return null;
        }

        if (externalControlDisabled) yield break;

        isStunned.Value = false;
        _bt.EnableBehavior();
        if (_rb != null)
        {
            _rb.isKinematic = false;
        }
    }
}