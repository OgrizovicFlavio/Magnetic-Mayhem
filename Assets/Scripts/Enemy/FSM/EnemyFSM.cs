using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM
{
    private EnemyController controller;
    private List<BaseEnemyState> states;
    private BaseEnemyState currentState;
    private MagnetizedEnemyState magnetizedState;

    public EnemyController GetController() => controller;

    public EnemyState CurrentStateType => currentState.GetStateType();

    public EnemyFSM(EnemyController controller)
    {
        this.controller = controller;

        magnetizedState = new MagnetizedEnemyState(this);

        states = new List<BaseEnemyState>
        {
            new PatrolEnemyState(this),
            new ChaseEnemyState(this),
            new AttackEnemyState(this),
            magnetizedState,
        };

        ChangeState(EnemyState.Patrol);
    }

    public void OnUpdate()
    {
        currentState?.OnUpdate();
    }

    public void ChangeState(EnemyState newState)
    {
        currentState?.OnExit();
        currentState = states.Find(s => s.GetStateType() == newState);

        if (currentState == null)
        {
            Debug.LogError("Enemy state not found: " + newState);
            return;
        }

        currentState.OnEnter();
    }

    public void EnterMagnetizedState()
    {
        if (currentState.GetStateType() == EnemyState.Magnetized)
            return; // Ya está magnetizado, no sobreescribas el estado anterior

        magnetizedState.Set(currentState.GetStateType());
        ChangeState(EnemyState.Magnetized);
    }

    public void ExitMagnetizedState()
    {
        if (currentState.GetStateType() == EnemyState.Magnetized)
        {
            Debug.Log("FSM: Salgo del estado Magnetized hacia: " + magnetizedState.PreviousState);
            ChangeState(magnetizedState.PreviousState);
        }
    }
}
