using UnityEngine;

//Define los estados del plpayer y qué hacer en cada estado
public class PlayerFSM : MonoBehaviour
{
    [SerializeField] private PlayerMotor motor;

    private IPlayerInput input;
    private PlayerState currentState;
    private PlayerState lastState;

    private void Start()
    {
        //Instancio el input por mouse y teclado y se lo paso al motor
        input = new KeyboardMouseInput();
        motor.SetInput(input);
        currentState = PlayerState.Idle;
    }

    private void Update()
    {
        if (input != null)
        {
            motor.RotateCamera(input.GetLookInput());
        }

        switch (currentState)
        {
            case PlayerState.Idle:
                HandleIdle();
                break;
            case PlayerState.Move:
                HandleMove();
                break;
            case PlayerState.Jump:
                HandleJump();
                break;
            case PlayerState.Shoot:
                HandleShoot();
                break;
            case PlayerState.Hurt:
                break;
            case PlayerState.Dead:
                break;
        }
    }

    private void HandleIdle()
    {
        Vector2 move = input.GetMoveInput();
        motor.SetMoveInput(Vector2.zero);

        if (move.magnitude > 0.1f)
        {
            motor.SetMoveInput(move);
            TransitionTo(PlayerState.Move);
        }
        else if (input.IsJumping())
        {
            TransitionTo(PlayerState.Jump);
        }
        else if (input.IsShooting())
        {
            TransitionTo(PlayerState.Shoot);
        }
    }

    private void HandleMove()
    {
        Vector2 move = input.GetMoveInput();
        motor.SetMoveInput(move);

        if (move.magnitude < 0.1f)
        {
            TransitionTo(PlayerState.Idle);
        }
        else if (input.IsJumping())
        {
            TransitionTo(PlayerState.Jump);
        }
        else if (input.IsShooting())
        {
            TransitionTo(PlayerState.Shoot);
        }
    }

    private void HandleJump()
    {
        motor.Jump();
        TransitionTo(PlayerState.Idle);
    }

    private void HandleShoot()
    {
        motor.Shoot();
        TransitionTo(PlayerState.Idle);
    }

    private void TransitionTo(PlayerState newState)
    {
        lastState = currentState;
        currentState = newState;
    }
}


