using UnityEngine;

[CreateAssetMenu(fileName = "BasicBombFactory", menuName = "Bomb/Basic Factory")]
public class BasicBombFactory : BombFactory
{
    public GameObject bombPrefab;

    public override GameObject CreateBomb(Vector2 position)
    {
        if (bombPrefab == null) return null;
        return Instantiate(bombPrefab, position, Quaternion.identity);
    }
}