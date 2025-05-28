using UnityEngine;

public class KeyboardMouseInput : IPlayerInput
{
    public Vector2 GetMoveInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public Vector2 GetLookInput()
    {
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    public bool IsShooting()
    {
        return Input.GetMouseButtonDown(0);
    }

    public bool TogglePolarity()
    {
        return Input.GetMouseButtonDown(1);
    }

    public bool IsJumping()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public bool IsInteracting()
    {
        return Input.GetKeyDown(KeyCode.E);
    }
}
