using UnityEngine;

public class InputHandler
{
    private bool IsPaused() => Time.timeScale == 0;

    public Vector2 GetMoveInput()
    {
        if (IsPaused()) 
            return Vector2.zero;

        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public Vector2 GetLookInput()
    {
        if (IsPaused()) 
            return Vector2.zero;

        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    public bool IsShooting()
    {
        if (IsPaused()) 
            return false;

        return Input.GetMouseButtonDown(0);
    }

    public bool ToggleCharge()
    {
        if (IsPaused()) 
            return false;

        return Input.GetMouseButtonDown(1);
    }

    public bool IsJumping()
    {
        if (IsPaused()) 
            return false;

        return Input.GetKeyDown(KeyCode.Space);
    }

    public bool IsInteracting()
    {
        if (IsPaused()) 
            return false;

        return Input.GetKeyDown(KeyCode.E);
    }
}
