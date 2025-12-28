using UnityEngine;

public class PlayerDeadState : IPlayerState
{
    private MovementController _player;

    public PlayerDeadState(MovementController player)
    {
        _player = player;
    }

    public void HandleInput() { }

    public void UpdateState() { }
}