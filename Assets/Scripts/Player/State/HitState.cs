using UnityEngine;
using UnityEngine.InputSystem;

public class HitState : PlayerState, ICharacterState
{
    public HitState(PlayerController playerController, Animator animator,
        PlayerInput playerInput) : base(playerController, animator, playerInput)
    {
    }

    public void Enter()
    {
        _animator.SetTrigger(PlayerController.PlayerAniParamHit);
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        
    }
}
