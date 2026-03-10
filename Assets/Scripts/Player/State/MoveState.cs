using UnityEngine;
using UnityEngine.InputSystem;

public class MoveState : PlayerState, ICharacterState
{
    private float _moveSpeed;

    public MoveState(PlayerController playerController, Animator animator, PlayerInput playerInput) : base(playerController, animator, playerInput)
    {

    }

    public void Enter()
    {
        // move 에니메이션 실행
        _animator.SetBool(PlayerController.PlayerMove, true);
        _moveSpeed = 0;

        //액션 할당
        _playerInput.actions["Jump"].performed += Jump;
    }

    public void Update()
    {
        var moveVector = _playerInput.actions["Move"].ReadValue<Vector2>();
        if (moveVector != Vector2.zero)
        {
            // TODO: 캐릭터 회전
            Rotate(moveVector.x, moveVector.y);
        }
        else
        {
            _controller.SetState(PlayerController.EPlayerState.Idle);
        }

        //이동 스피드 설정
        var isRun = _playerInput.actions["Run"].IsPressed();
        if (isRun && _moveSpeed < 1f)
        {
            _moveSpeed += Time.deltaTime;
            _moveSpeed = Mathf.Clamp01(_moveSpeed);
        } else if (!isRun && _moveSpeed > 0)
        {
            _moveSpeed -= Time.deltaTime*_controller.BreakForce;
            _moveSpeed = Mathf.Clamp01((_moveSpeed));
        }
        _animator.SetFloat(PlayerController.PlayerMoveSpeed, _moveSpeed);
    }

    public void Exit()
    {
        // move 에니메이션 종료

        _animator.SetBool(PlayerController.PlayerMove, false);

        
        //할당된 액션 해제
        _playerInput.actions["Jump"].performed -= Jump;
    }

}
