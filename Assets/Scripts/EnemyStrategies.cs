using UnityEngine;

public interface IEnemyStrategy
{
    Vector2 GetDirection(); 
    void OnHitWall();
}

public class FoolStrategy : IEnemyStrategy
{
    private Vector2 _direction;
    private Vector2[] _allDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    public FoolStrategy()
    {
        
        _direction = _allDirections[Random.Range(0, _allDirections.Length)];
    }

    public Vector2 GetDirection()
    {
        
        if (_direction == Vector2.zero)
        {
            _direction = Vector2.right;
        }
        
        return _direction;
    }

    public void OnHitWall()
    {
        PickRandomDir();
    }

    private void PickRandomDir()
    {
        Vector2 oldDirection = _direction;
        Vector2 oppositeDirection = -_direction; 
        
        int maxAttempts = 10;
        int attempts = 0;
        
        do
        {
            _direction = _allDirections[Random.Range(0, _allDirections.Length)];
            attempts++;
            
            if (_direction != oldDirection && _direction != oppositeDirection)
            {
                break;
            }
        }
        while (attempts < maxAttempts);
        
        if (_direction == oppositeDirection)
        {
            _direction = Random.value > 0.5f ? Vector2.right : Vector2.up;
        }
        
    }
}

public class NormalStrategy : IEnemyStrategy
{
    private Vector2 _direction;
    private Vector2[] _allDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    private float _changeDirectionTimer = 0f;
    private float _changeDirectionInterval = 2f; 
    public NormalStrategy()
    {
        _direction = _allDirections[Random.Range(0, _allDirections.Length)];
    }

    public Vector2 GetDirection()
    {
        _changeDirectionTimer += Time.fixedDeltaTime;
        
        if (_changeDirectionTimer >= _changeDirectionInterval)
        {
            _changeDirectionTimer = 0f;
            _direction = _allDirections[Random.Range(0, _allDirections.Length)];
        }
        
        return _direction;
    }

    public void OnHitWall()
    {
        
        _direction = -_direction;
    }
}


public class CleverStrategy : IEnemyStrategy
{
    private Vector2 _direction;
    private Transform _enemyTransform;
    private Transform _playerTransform;

    public CleverStrategy(Transform enemy)
    {
        _enemyTransform = enemy;
        _direction = Vector2.right;
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }
    }

    public Vector2 GetDirection()
    {
        if (_playerTransform == null || _enemyTransform == null)
        {
            return _direction;
        }
        
        Vector2 toPlayer = (_playerTransform.position - _enemyTransform.position).normalized;
        
        
        if (Mathf.Abs(toPlayer.x) > Mathf.Abs(toPlayer.y))
        {
            _direction = toPlayer.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            _direction = toPlayer.y > 0 ? Vector2.up : Vector2.down;
        }
        
        return _direction;
    }

    public void OnHitWall()
    {
        if (Mathf.Abs(_direction.x) > 0)
        {
            _direction = Random.value > 0.5f ? Vector2.up : Vector2.down;
        }
        else
        {
            _direction = Random.value > 0.5f ? Vector2.right : Vector2.left;
        }
    }
}