using UnityEngine;

public class PlayerAliveState : IPlayerState
{
    private MovementController _player;

    public PlayerAliveState(MovementController player)
    {
        _player = player;
    }

    
    public void HandleInput()
    {
        if (_player != null)
        {
            _player.HandleInput();
        }
    }

    
    public void UpdateState() { }
}