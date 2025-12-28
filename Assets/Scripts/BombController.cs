using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BombController : MonoBehaviour
{
    [Header("Factory")]
    public BombFactory bombFactory; 

    [Header("Settings")]
    public KeyCode inputKey = KeyCode.Space;
    public float bombFuseTime = 3f;
    public int bombAmount = 1;
    private int bombsRemaining;
    
    [Header("Explosion")]
    public Explosions explosionPrefab;
    public LayerMask explosionLayerMask;
    public float explosionDuration = 1f;
    public int explosionRadius = 1;

    [Header("Map")]
    public Tilemap destructibleTiles;
    public Destructible destructiblePrefab;

    private IBombStats _currentBombStats;

    private void Awake()
    {
        _currentBombStats = new BasicBombStats();
    }

    private void OnEnable()
    {
        bombsRemaining = bombAmount;
    }

    private void Update()
    {
        if (bombsRemaining > 0 && Input.GetKeyDown(inputKey)) {
            StartCoroutine(PlaceBomb());
        }
    }

    public void IncreaseBlastRadius()
    {
        _currentBombStats = new RadiusEnhancer(_currentBombStats);
        Debug.Log("PowerUp Alındı! Menzil: " + _currentBombStats.GetRadius());
    }

    private IEnumerator PlaceBomb()
    {
        Vector2 position = transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);

        if (_currentBombStats != null)
        {
            explosionRadius = _currentBombStats.GetRadius();
        }

        GameObject bomb = bombFactory.CreateBomb(position);
        
        bombsRemaining--;

        yield return new WaitForSeconds(bombFuseTime);
        
        if (bomb != null)
        {
            position = bomb.transform.position;
            position.x = Mathf.Round(position.x);
            position.y = Mathf.Round(position.y);
            Destroy(bomb); 
        }

        SpawnExplosion(position);     

        Explode(position, Vector2.up, explosionRadius);
        Explode(position, Vector2.down, explosionRadius);
        Explode(position, Vector2.left, explosionRadius);
        Explode(position, Vector2.right, explosionRadius);
        
        bombsRemaining++;
    }

    private void SpawnExplosion(Vector2 position)
    {
        Explosions explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.SetActiveRenderer(explosion.start);
        explosion.DestroyAfter(explosionDuration);
    }
    
    private void Explode(Vector2 position, Vector2 direction, int length)
    {
        if (length <= 0) return;
        
        position += direction;
        Collider2D hit = Physics2D.OverlapBox(position, Vector2.one / 2f, 0f, explosionLayerMask);

        if (hit)
        {
            ClearDestructible(position);

           
            HardWall wall = hit.GetComponent<HardWall>();
            if (wall != null)
            {
                wall.TakeDamage(); 
            }

            return; 
        }

        if(Physics2D.OverlapBox(position,Vector2.one/2f, 0f, explosionLayerMask))
        {
            ClearDestructible(position);
            return;
        }
        
        Explosions explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.SetActiveRenderer(length > 1 ? explosion.middle : explosion.end);
        explosion.SetDirection(direction);
        explosion.DestroyAfter(explosionDuration);

        Explode(position, direction, length - 1);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bomb")) {
            other.isTrigger = false;
        }
    }

    private void ClearDestructible(Vector2 position)
    {
        Vector3Int cell = destructibleTiles.WorldToCell(position);
        TileBase tile = destructibleTiles.GetTile(cell);

        if (tile != null)
        {
            Instantiate(destructiblePrefab, position, Quaternion.identity);
            destructibleTiles.SetTile(cell, null);
        }
    }
    
    public void AddBomb()
    {
        bombAmount++;
        bombsRemaining++;
    }
}