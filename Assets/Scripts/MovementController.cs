using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementController : MonoBehaviour, IGameSubject
{
    public Rigidbody2D rb { get; private set; }
    private Vector2 direction = Vector2.down;
    public float speed = 5f;

    [Header("Input Settings")]
    public KeyCode inputUp = KeyCode.W;
    public KeyCode inputDown = KeyCode.S;
    public KeyCode inputLeft = KeyCode.A;
    public KeyCode inputRight = KeyCode.D;
    public KeyCode inputBomb = KeyCode.Space; 

    private IInputAdapter _inputAdapter; 
    [Header("Sprites")]
    public AnimatedSpriteRenderer spriteRendererUp;
    public AnimatedSpriteRenderer spriteRendererDown;
    public AnimatedSpriteRenderer spriteRendererLeft;
    public AnimatedSpriteRenderer spriteRendererRight;
    public AnimatedSpriteRenderer spriteRendererDeath;

    public AnimatedSpriteRenderer activeSpriteRenderer;

    private List<IGameObserver> _observers = new List<IGameObserver>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        activeSpriteRenderer = spriteRendererDown;

       
        _inputAdapter = new KeyboardInputAdapter(inputUp, inputDown, inputLeft, inputRight, inputBomb);
    }

    private void Update()
    {
        if (this.enabled) 
        {
            HandleInput();
        }
    }

    public void HandleInput()
    {
        
        Vector2 inputDir = _inputAdapter.GetDirection();

        if (inputDir == Vector2.up) {
            SetDirection(Vector2.up, spriteRendererUp);
        } else if (inputDir == Vector2.down) {
            SetDirection(Vector2.down, spriteRendererDown);
        } else if (inputDir == Vector2.left) {
            SetDirection(Vector2.left, spriteRendererLeft);
        } else if (inputDir == Vector2.right) {
            SetDirection(Vector2.right, spriteRendererRight);
        } else {
            SetDirection(Vector2.zero, activeSpriteRenderer);
        }
        
       
    }

    private void FixedUpdate()
    {
        Vector2 position = rb.position;
        Vector2 translation = direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(position + translation);
    }

    public void SetDirection(Vector2 newDirection, AnimatedSpriteRenderer spriteRenderer)
    {
        direction = newDirection;

        if (activeSpriteRenderer != spriteRenderer)
        {
            activeSpriteRenderer.idle = true;
            activeSpriteRenderer.enabled = false;
            activeSpriteRenderer = spriteRenderer;
            activeSpriteRenderer.enabled = true;
        }

        activeSpriteRenderer.idle = direction == Vector2.zero;
    }

    public void DeathSequence()
    {
        if (!this.enabled) return;

        this.enabled = false;
        GetComponent<BombController>().enabled = false;

        spriteRendererUp.enabled = false;
        spriteRendererDown.enabled = false;
        spriteRendererLeft.enabled = false;
        spriteRendererRight.enabled = false;

        if (spriteRendererDeath != null)
        {
            activeSpriteRenderer = spriteRendererDeath;
            activeSpriteRenderer.enabled = true;
        }

        Invoke(nameof(OnDeathComplete), 1.25f);
    }

    private void OnDeathComplete()
    {
        gameObject.SetActive(false);
        NotifyObservers("PlayerDied");
    }

    public void AddObserver(IGameObserver observer)
    {
        if (!_observers.Contains(observer)) _observers.Add(observer);
    }

    public void RemoveObserver(IGameObserver observer)
    {
        if (_observers.Contains(observer)) _observers.Remove(observer);
    }

    public void NotifyObservers(string eventName)
    {
        foreach (var observer in _observers.ToArray())
        {
            observer.OnNotify(eventName, this);
        }
    }
}