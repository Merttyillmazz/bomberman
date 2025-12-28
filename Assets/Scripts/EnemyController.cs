using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class EnemyController : MonoBehaviour
{
    public float speed = 2f;
    public LayerMask wallLayer; 
    
    private IEnemyStrategy _moveStrategy;
    private Rigidbody2D _rb;
    private Vector2 _currentDirection;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f; 
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        _rb.freezeRotation = true;
        _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void Start()
    {
        _moveStrategy = new NormalStrategy();
        _currentDirection = _moveStrategy.GetDirection();
        
        Vector2 pos = transform.position;
        transform.position = new Vector2(
            Mathf.Round(pos.x), 
            Mathf.Round(pos.y)
        );
    }

    private void FixedUpdate()
    {
        if (_moveStrategy != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                _rb.position, 
                _currentDirection, 
                0.6f, 
                wallLayer
            );
            
            if (hit.collider != null)
            {
                HandleWallDetection();
            }
            
            _rb.linearVelocity = _currentDirection * speed;
        }
    }

    private void HandleWallDetection()
    {
        
        Vector2 currentPos = _rb.position;
        float newX = Mathf.Round(currentPos.x);
        float newY = Mathf.Round(currentPos.y);
        _rb.position = new Vector2(newX, newY);
        
        
        _moveStrategy.OnHitWall();
        _currentDirection = _moveStrategy.GetDirection();
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var player = collision.gameObject.GetComponent<MovementController>();
            if (player != null)
            {
                player.DeathSequence();
            }
        }
    }
}