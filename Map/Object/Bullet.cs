using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _lifetime = 5f;
    private bool _hasHit = false; // 중복 충돌 방지 플래그

    private int _damage = 3; // 데미지 값

    private void Start()
    {
        Destroy(gameObject, _lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasHit) return; // 중복 처리 방지
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                playerHealth.TakeDamage(_damage);
                Destroy(gameObject);
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }

    }
}
