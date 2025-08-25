using UnityEngine;

public class FallingDust : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 3f;
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private LayerMask groundLayer;

    private float timer;

    void Update()
    {
        // 아래로 이동
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        timer += Time.deltaTime;

        // Ground에 닿으면 파괴
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer);
        if (hit.collider != null)
        {
            Destroy(gameObject);
        }

        // 시간 초과로도 제거
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}