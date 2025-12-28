using UnityEngine;

public class HardWall : MonoBehaviour
{
    [Header("Hasar Ayarları")]
    public int maxHealth = 3;
    private int currentHealth;

    
    private SpriteRenderer spriteRenderer;

    [Header("Kırılma")]
    public float destructionTime = 0.5f; 
    [Range(0f, 1f)]
    public float itemSpawnChance = 0.3f;
    public GameObject[] spawnableItems;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        
        
        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;
    }

    
    public void TakeDamage()
    {
        currentHealth--;
        
        Debug.Log($"HardWall hasar aldı! Kalan can: {currentHealth}");

    
        UpdateColor();

        if (currentHealth <= 0)
        {
            DestroyWall();
        }
    }

    private void UpdateColor()
    {
        if (spriteRenderer == null) return;

    

        if (currentHealth == 2)
        {
    
            spriteRenderer.color = new Color(0.8f, 0.8f, 0.8f); 
        }
        else if (currentHealth == 1)
        {
    
            spriteRenderer.color = new Color(0.6f, 0.4f, 0.4f); 
        }
    }

    private void DestroyWall()
    {
        Debug.Log("HardWall kırıldı!");

        if (spawnableItems.Length > 0 && Random.value < itemSpawnChance)
        {
            int randomIndex = Random.Range(0, spawnableItems.Length);
            Instantiate(spawnableItems[randomIndex], transform.position, Quaternion.identity);
        }

    
        Destroy(gameObject);
    }
    
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Explosion")) TakeDamage();
    }
}