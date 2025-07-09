using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerFSM
{
    private InputHandler input;
    private PlayerController controller;
    private Action shootAction;
    private Action toggleChargeAction;

    private List<BasePlayerState> playerStates;
    private BasePlayerState currentState;

    public PlayerController GetController() => controller;
    public InputHandler GetInput() => input;

    public PlayerFSM(PlayerController controller, InputHandler input, Action shootAction, Action toggleChargeAction)
    {
        this.controller = controller;
        this.input = input;
        this.shootAction = shootAction;
        this.toggleChargeAction = toggleChargeAction;

        playerStates = new List<BasePlayerState>
        {
            new IdlePlayerState(this),
            new RunPlayerState(this),
            new JumpPlayerState(this),
            new ShootPlayerState(this, shootAction),
            new HurtPlayerState(this),
            new DeadPlayerState(this)
        };

        ChangeState(PlayerState.Idle);
    }

    public void OnUpdate()
    {
        if (input.ToggleCharge())
            toggleChargeAction?.Invoke();

        if (input.IsInteracting())
            controller.TryPossess();

        if (input.IsShooting())
        {
            bool isGrounded = controller.IsGrounded();
            bool isMoving = IsMoving();
            bool isIdle = currentState.playerState == PlayerState.Idle;

            if (isGrounded && !isMoving && isIdle)
            {
                ChangeState(PlayerState.Shoot);
                return;
            }
        }

        currentState?.OnUpdate();
    }

    public void ChangeState(PlayerState newState)
    {
        currentState?.OnExit();
        currentState = playerStates.Find(s => s.playerState == newState);

        if (currentState == null)
            return;

        currentState.OnEnter();
    }

    public bool IsMoving()
    {
        if (input == null)
            return false;

        Vector2 moveInput = input.GetMoveInput();

        if (moveInput.sqrMagnitude > 0.01f)
            return true;

        return false;
    }
}


