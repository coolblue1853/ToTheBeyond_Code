using System.Collections;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public enum ShootDirection
    {
        Right,
        Left,
        Up,
        Down
    }

    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _shootInterval = 2f;
    [SerializeField] private ShootDirection _direction = ShootDirection.Right;
    [SerializeField] private float _bulletSpeed = 5f;
    [SerializeField] private float _startDelay = 0f;

    private float _xOffset = 0f;
    private float _yOffset = -1f;

    private void Start()
    {
        StartCoroutine(ShootRoutine());
    }

    private IEnumerator ShootRoutine()
    {
        if (_startDelay > 0f)
            yield return new WaitForSeconds(_startDelay);

        WaitForSeconds wait = new WaitForSeconds(_shootInterval);

        while (true)
        {
            SpawnBullet();
            yield return wait;
        }
    }

    private void SpawnBullet()
    {
        Vector3 spawnPos = transform.position;

        // 방향에 따른 오프셋 적용
        switch (_direction)
        {
            case ShootDirection.Right:
                spawnPos += new Vector3(0.2f, -0.5f, 0f); 
                break;
            case ShootDirection.Left:
                spawnPos += new Vector3(-0.2f, 0.5f, 0f); 
                break;
            case ShootDirection.Up:
                spawnPos += new Vector3(0.5f, -0.2f, 0f);
                break;
            case ShootDirection.Down:
                spawnPos += new Vector3(0f, -0.2f, 0f); 
                break;
        }

        GameObject bullet = Instantiate(_bulletPrefab, spawnPos, Quaternion.identity);
        bullet.transform.parent = transform;
        
        // 회전 설정
        switch (_direction)
        {
            case ShootDirection.Right:
                bullet.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case ShootDirection.Left:
                bullet.transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case ShootDirection.Up:
                bullet.transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case ShootDirection.Down:
                bullet.transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
        }

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = GetDirectionVector() * _bulletSpeed;
        }
    }


    private Vector2 GetDirectionVector()
    {
        return _direction switch
        {
            ShootDirection.Right => Vector2.right,
            ShootDirection.Left => Vector2.left,
            ShootDirection.Up => Vector2.up,
            ShootDirection.Down => Vector2.down,
            _ => Vector2.right
        };
    }
}
