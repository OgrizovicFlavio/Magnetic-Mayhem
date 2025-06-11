using UnityEngine;
public interface IPlayerInput
{
    Vector2 GetMoveInput();
    Vector2 GetLookInput();
    bool IsShooting();
    bool ToggleCharge();
    bool IsJumping();
    bool IsInteracting();
}
