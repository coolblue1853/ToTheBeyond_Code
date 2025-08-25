using System.Collections;
using UnityEngine;

public class Thorn : MonoBehaviour
{
    [SerializeField] private float _bounceForceX = 7f;
    [SerializeField] private float _bounceForceY = 5f;
    [SerializeField] private float _damage = 5f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerHealth>(out var playerHealth))
        {
            if (playerHealth.IsInvincible) return;
            playerHealth.TakeDamage(_damage);
            if (collision.TryGetComponent<Rigidbody2D>(out Rigidbody2D playerRb))
            {
                Vector2 direction = (collision.transform.position - transform.position).normalized;

                if (direction.magnitude < 0.01f)
                    direction = Vector2.up + Vector2.right;

                playerRb.velocity = Vector2.zero; // 속도 초기화

                // X축 넉백
                Vector2 knockbackX = new Vector2(direction.x * _bounceForceX, 0f);
                playerRb.AddForce(knockbackX, ForceMode2D.Impulse);

                // Y축 넉백은 약간의 딜레이 후 적용
                collision.GetComponent<MonoBehaviour>()?.StartCoroutine(ApplyYBounce(playerRb));
            }
        }
        if (collision.TryGetComponent<DamageablePart>(out var enemy))
        {
            enemy.TakePartDamage(transform.position, _damage * 10, false);
            if (collision.TryGetComponent<Rigidbody2D>(out Rigidbody2D playerRb))
            {
                Vector2 direction = (collision.transform.position - transform.position).normalized;

                if (direction.magnitude < 0.01f)
                    direction = Vector2.up + Vector2.right;

                playerRb.velocity = Vector2.zero; // 속도 초기화
                
                // Y축 넉백은 약간의 딜레이 후 적용
                collision.GetComponent<MonoBehaviour>()?.StartCoroutine(ApplyYBounce(playerRb));
            }
        }
    }
    
    private IEnumerator ApplyYBounce(Rigidbody2D rb)
    {
        if (rb == null) yield break;
        
        yield return new WaitForSeconds(0.05f); // 짧은 딜레이
        rb.AddForce(new Vector2(0f, _bounceForceY), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f);
        rb.velocity = Vector2.zero; // 일정 시간 후 멈춤
    }
}
