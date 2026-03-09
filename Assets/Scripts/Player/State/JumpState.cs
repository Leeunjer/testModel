using UnityEngine;
using UnityEngine.InputSystem;

public class JumpState : PlayerState , IPlayerState
{
    public JumpState(PlayerController playerController, Animator animator, PlayerInput playerInput) 
        : base(playerController, animator, playerInput)
    {
    }

    public void Enter()
    {
        _animator.SetTrigger(PlayerController.PlayerAniParamJump);
    }
    public void Update()
    {
        //Ground distance 업데이트
        var playerPosition = _controller.transform.position;
        var distance = CharacterUtility.GetDistanceToGround(playerPosition, Constants.GroundLayerMask, 10f);
        _animator.SetFloat(PlayerController.PlayerAniParamGroundDistance, distance);
        Debug.DrawRay(playerPosition, Vector3.down * distance, Color.red);
    }
    public void Exit()
    {
        
    }

    
}
