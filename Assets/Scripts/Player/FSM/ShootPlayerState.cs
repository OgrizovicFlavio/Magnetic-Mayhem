using UnityEngine;
using System;

public class ShootPlayerState : BasePlayerState
{
    private float shootTimer = 0f;
    private float fireRate = 0.5f;

    private Action shootAction;

    public ShootPlayerState(PlayerFSM fsm, Action shootAction) : base(fsm)
    {
        this.shootAction = shootAction;
        playerState = PlayerState.Shoot;
    }

    public override void OnEnter()
    {
        shootAction?.Invoke();
        fsm.GetController().GetAnimator().SetInteger("State", (int)PlayerState.Shoot);

        fsm.GetController().SetMoveInput(Vector2.zero);

        Rigidbody rb = fsm.GetController().GetRigidbody();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }

        shootTimer = 0f;
    }

    public override void OnUpdate()
    {
        shootTimer += Time.deltaTime;

        if (shootTimer >= fireRate)
        {
            fsm.ChangeState(PlayerState.Idle);
        }
    }

    public override void OnExit() { }
}

