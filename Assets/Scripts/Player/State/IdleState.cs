using UnityEngine;
using UnityEngine.InputSystem;

public class IdleState : PlayerState,IPlayerState
{
    public IdleState(PlayerController playerController, Animator animator, PlayerInput playerInput)
     : base(playerController, animator, playerInput) { }

    public void Enter()
    {
        //Idle 에니메이션 실행
        _animator.SetBool(PlayerController.PlayerAniParamIdle, true);

        //액션 할당
        _playerInput.actions["Jump"].performed += Jump;
    }

    public void Update()
    {
        if (_playerInput.actions["Move"].IsPressed())
        {
            _controller.SetState(PlayerController.EPlayerState.Move);
        }
    }

    public void Exit()
    {
        //idle 애니메이션 중단
        _animator.SetBool(PlayerController.PlayerAniParamIdle, false);

        //할당된 액션 해제
        _playerInput.actions["Jump"].performed -= Jump;
    }

}
