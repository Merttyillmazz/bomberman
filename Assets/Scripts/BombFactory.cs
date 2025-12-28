using UnityEngine;

public abstract class BombFactory : ScriptableObject
{
   
    public abstract GameObject CreateBomb(Vector2 position);
}