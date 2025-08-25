using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public abstract class BaseCheckGround : Action
{
    [Header("이동 관련")]
    public SharedFloat speed = 2f; // 이동 속도

    [Header("Ray 관련")]
    public float _groundCheckDistance = 1f; // 아래 방향으로 땅 체크할 Ray 길이
    public float _wallCheckDistance = 1f;
    [Header("Ray 위치")]
    private Vector2 _wallRayOffset = new Vector2(0.8f, 0f); // 벽 체크용 Ray 발사 위치
    private Vector2 _edgeRayOffset = new Vector2(0.8f, -0.5f); // 낭떠러지인지 체크용 Ray 발사 위치
    protected int _direction = 1; 
    private LayerMask _groundLayer;
    private LayerMask _wallLayer;
    
    private static int _groundLayerMask = -1;
    private static int _wallLayerMask = -1;

    public override void OnStart()
    {
        if (_groundLayerMask == -1)
            _groundLayerMask = LayerMask.GetMask("Ground");

        if (_wallLayerMask == -1)
            _wallLayerMask = LayerMask.GetMask("Wall");

        _groundLayer = _groundLayerMask;
        _wallLayer = _wallLayerMask;
    }
    // 앞쪽에 벽이 있는지 확인
    protected bool IsWall()
    {
        Vector3 origin = transform.position + new Vector3(_wallRayOffset.x * _direction, _wallRayOffset.y, 0f);
#if UNITY_EDITOR
        // Scene 뷰에서 확인용
        Debug.DrawRay(origin, Vector2.right * _direction * _wallCheckDistance, Color.red);
#endif
        return Physics2D.Raycast(origin, Vector2.right * _direction, _wallCheckDistance, _wallLayer);
    }
    
    // 앞쪽에 낭떠러지인지 확인
    protected bool IsEdge()
    {
        Vector3 origin = transform.position + new Vector3(_edgeRayOffset.x * _direction, _edgeRayOffset.y, 0f);
#if UNITY_EDITOR
        Debug.DrawRay(origin, Vector2.down * _groundCheckDistance, Color.blue);
#endif
        return !Physics2D.Raycast(origin, Vector2.down, _groundCheckDistance, _groundLayer);
    }
    
    // 이동
    protected void Move(Vector3 moveDir)
    {
        transform.position += moveDir * speed.Value * Time.deltaTime;
    }
}
