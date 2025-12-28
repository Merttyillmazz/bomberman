using UnityEngine;


public class KeyboardInputAdapter : IInputAdapter
{
    private KeyCode _up, _down, _left, _right, _action;

    public KeyboardInputAdapter(KeyCode up, KeyCode down, KeyCode left, KeyCode right, KeyCode action)
    {
        _up = up;
        _down = down;
        _left = left;
        _right = right;
        _action = action;
    }

    public Vector2 GetDirection()
    {
        if (Input.GetKey(_up)) return Vector2.up;
        if (Input.GetKey(_down)) return Vector2.down;
        if (Input.GetKey(_left)) return Vector2.left;
        if (Input.GetKey(_right)) return Vector2.right;
        
        return Vector2.zero;
    }

    public bool IsActionPressed()
    {
        return Input.GetKeyDown(_action);
    }
}