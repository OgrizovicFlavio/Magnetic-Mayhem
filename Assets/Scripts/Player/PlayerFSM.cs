using UnityEngine;
using System.Collections.Generic;

public class PlayerFSM : MonoBehaviour
{
    [SerializeField] private Controller controller;
    [SerializeField] private PlayerShoot shooter;

    private IPlayerInput input;
    private List<BaseState> states;
    private BaseState currentState;

    public Controller GetController() => controller;
    public PlayerShoot GetShooter() => shooter;
    public IPlayerInput GetInput() => input;

    private void Start()
    {
        input = new KeyboardMouseInput();
        controller.SetInput(input);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        states = new List<BaseState>
        {
            new IdleState(this),
            new RunState(this),
            new JumpState(this),
            new ShootState(this),
            new HurtState(this),
            new DeadState(this)
        };

        ChangeState(PlayerState.Idle);
    }

    private void Update()
    {
        controller.RotateCamera(input.GetLookInput());

        if (input.ToggleCharge())
            controller.ToggleCharge();

        controller.TryPossess();
        currentState?.OnUpdate();
    }

    public void ChangeState(PlayerState newState)
    {
        currentState?.OnExit();
        currentState = states.Find(s => s.playerState == newState);

        if (currentState == null)
        {
            Debug.LogError("Estado no encontrado: " + newState);
            return;
        }

        //Debug.Log("Entrando al estado: " + currentState.playerState);
        currentState.OnEnter();
    }
}


