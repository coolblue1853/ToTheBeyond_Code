using MoreMountains.Feedbacks;
using UnityEngine;

public class JumpEffect : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private Rigidbody2D rb;

    [Header("Effects")]
    [SerializeField] private ParticleSystem jumpParticleSystem;
    [SerializeField] private ParticleSystem landingParticleSystem;

    [Header("Jump Dust Effect")]
    [SerializeField] private GameObject jumpDustPrefab;  // 새로 추가
    [SerializeField] private Transform groundPivot;       // 발 위치 기준

    [Header("Ground Detection")]
    [SerializeField] private float groundCheckDistance = 1.1f;
    [SerializeField] private LayerMask groundLayer = ~0;
    [SerializeField]
    private MMFeedbacks jumpFeedbacks;

    [SerializeField]
    private MMFeedbacks landingFeedbacks;
    private bool isGrounded = false;
    private bool wasGrounded = false;

    void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            TryJump();
        }

        CheckLanding();
    }

    void TryJump()
    {
        if (isGrounded && rb != null)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            Debug.Log("점프됨");

            jumpFeedbacks?.PlayFeedbacks();

            // 점프 이펙트 프리팹 생성
            if (jumpDustPrefab != null && groundPivot != null)
            {
                Instantiate(jumpDustPrefab, groundPivot.position, Quaternion.identity);
            }
        }
    }

    void CheckLanding()
    {
        wasGrounded = isGrounded;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;

        if (!wasGrounded && isGrounded)
        {
            landingFeedbacks?.PlayFeedbacks();
        }
    }
}