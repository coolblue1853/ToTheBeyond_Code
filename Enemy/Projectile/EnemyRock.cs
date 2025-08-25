using UnityEngine;

public class EnemyRock : MonoBehaviour
{
    public int damage;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        int playerLayer = LayerMask.NameToLayer("Player");

        if (other.gameObject.layer == playerLayer && other.TryGetComponent(out PlayerHealth player))
        {
            player.TakeDamage(damage); // 피해 입음
        }
        Destroy(gameObject,3);       // 바위 제거
    }
}
