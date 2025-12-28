using UnityEngine;


public interface IInputAdapter
{
    Vector2 GetDirection(); 
    bool IsActionPressed(); 
}