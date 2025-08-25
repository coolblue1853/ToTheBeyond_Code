using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyFlying : MonoBehaviour
{
    [Header("기본 설정")]
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private Vector2 _movementMin = new Vector2(-5, -3);
    [SerializeField] private Vector2 _movementMax = new Vector2(5, 3);

    [Header("무한대 모양 패턴 설정")]
    [SerializeField] private float _infinityScale = 2f;
    [SerializeField] private float _infinityAspectY = 0.5f;
    [SerializeField] private float _infinitySpeed = 1f;

    [Header("다이아몬드 모양 패턴 설정")]
    [SerializeField] private float _rectangleWidth = 5f;
    [SerializeField] private float _rectangleHeight = 1.5f;

    [Header("랜덤 모양 패턴 설정")]
    [SerializeField] private float _randomRange = 2f;
    [SerializeField] private float _minRandomDistance = 3f;
    private bool _isStopped = false;
    public void StopMovement() => _isStopped = true;
    public void ResumeMovement() => _isStopped = false;
    private enum FlyPattern { Infinity, Diamond, Random }
    private FlyPattern _currentPattern;

    private float _time;
    private Vector2[] _diamondPoints;
    private int _diamondIndex;
    private Vector2 _currentTarget;
    private Vector2 _center;
    private bool _isPatternComplete;
    private bool _isMovingToCenter = false;
    
    void Awake()
    {
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
        }
    }
    void Start()
    {
        _center = transform.position;

        _movementMin += (Vector2)transform.position;
        _movementMax += (Vector2)transform.position;

        SetNewPattern();
        InitDiamond();
    }

    void Update()
    {
        if (_isStopped) return;

        if (_isMovingToCenter)
        {
            MoveToCenter();
            return;
        }

        switch (_currentPattern)
        {
            case FlyPattern.Infinity:
                MoveInfinity(); break;
            case FlyPattern.Diamond:
                MoveDiamond(); break;
            case FlyPattern.Random:
                MoveRandom(); break;
        }

        if (_isPatternComplete)
        {
            SetNewPattern();
        }
    }

    void SetNewPattern()
    {
        _isPatternComplete = false;

        FlyPattern previousPattern = _currentPattern;
        if (previousPattern == FlyPattern.Random)
        {
            _currentPattern = (Random.value > 0.5f) ? FlyPattern.Infinity : FlyPattern.Diamond;
        }
        else
        {
            _currentPattern = FlyPattern.Random;
        }

        // 패턴별 마진
        float marginX = 0f, marginY = 0f;
        switch (_currentPattern)
        {
            case FlyPattern.Infinity:
                marginX = _infinityScale;
                marginY = _infinityScale * _infinityAspectY;
                break;
            case FlyPattern.Diamond:
                marginX = _rectangleWidth;
                marginY = _rectangleHeight;
                break;
            case FlyPattern.Random:
                marginX = _randomRange;
                marginY = _randomRange;
                break;
        }

        // 중심 위치는 랜덤 설정, 현재 위치는 그대로
        float centerX = Random.Range(_movementMin.x + marginX, _movementMax.x - marginX);
        float centerY = Random.Range(_movementMin.y + marginY, _movementMax.y - marginY);
        _center = new Vector2(centerX, centerY);

        // 먼저 center까지 이동 후 패턴 시작
        _currentTarget = _center;
        _isMovingToCenter = true;
    }
    void MoveToCenter()
    {
        transform.position = ClampPosition(Vector2.MoveTowards(transform.position, _center, _moveSpeed * Time.deltaTime));

        if (Vector2.Distance(transform.position, _center) < 0.1f)
        {
            _isMovingToCenter = false;

            // 중심 도달 이후 초기화
            switch (_currentPattern)
            {
                case FlyPattern.Infinity:
                    transform.position = _center;
                    _time = 0f;
                    break;

                case FlyPattern.Diamond:
                    InitDiamond();
                    break;

                case FlyPattern.Random:
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 candidate = _center + Random.insideUnitCircle * _randomRange;
                        if (Vector2.Distance(_center, candidate) >= _minRandomDistance)
                        {
                            _currentTarget = ClampPosition(candidate);
                            break;
                        }
                    }
                    break;
            }
        }
    }
    void InitDiamond()
    {
        _diamondPoints = new Vector2[4];
        _diamondPoints[0] = _center + new Vector2(0, _rectangleHeight);
        _diamondPoints[1] = _center + new Vector2(_rectangleWidth, 0);
        _diamondPoints[2] = _center + new Vector2(0, -_rectangleHeight);
        _diamondPoints[3] = _center + new Vector2(-_rectangleWidth, 0);
        _currentTarget = _diamondPoints[0];
        _diamondIndex = 0;
    }

    void MoveInfinity()
    {
        if (_time == 0f)
        {
            Vector2 localPos = (Vector2)transform.position - _center;
            if (localPos.sqrMagnitude > 0.001f)
            {
                _time = Mathf.Atan2(
                    localPos.y / (_infinityScale * _infinityAspectY),
                    localPos.x / _infinityScale
                );
            }
        }

        _time += Time.deltaTime * _infinitySpeed;

        float x = Mathf.Sin(_time) * _infinityScale;
        float y = Mathf.Sin(_time * 2f) * _infinityScale * _infinityAspectY;

        transform.position = ClampPosition(_center + new Vector2(x, y));

        if (_time >= Mathf.PI * 2f)
        {
            _isPatternComplete = true;
        }
    }

    void MoveDiamond()
    {
        if (Vector2.Distance(transform.position, _currentTarget) < 0.1f)
        {
            _diamondIndex++;
            if (_diamondIndex < _diamondPoints.Length)
            {
                _currentTarget = _diamondPoints[_diamondIndex];
            }
            else
            {
                _isPatternComplete = true;
                return;
            }
        }

        transform.position = ClampPosition(Vector2.MoveTowards(transform.position, _currentTarget, _moveSpeed * Time.deltaTime));
    }

    void MoveRandom()
    {
        transform.position = ClampPosition(Vector2.MoveTowards(transform.position, _currentTarget, _moveSpeed * Time.deltaTime));

        if (Vector2.Distance(transform.position, _currentTarget) < 0.1f)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 newTarget = _center + Random.insideUnitCircle * _randomRange;
                if (Vector2.Distance(transform.position, newTarget) >= _minRandomDistance)
                {
                    _currentTarget = ClampPosition(newTarget);
                    return;
                }
            }

            _isPatternComplete = true;
        }
    }
    Vector2 ClampPosition(Vector2 pos)
    {
        return new Vector2(
            Mathf.Clamp(pos.x, _movementMin.x, _movementMax.x),
            Mathf.Clamp(pos.y, _movementMin.y, _movementMax.y)
        );
    }
}

