using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState
{
    protected Animator _animator;
    protected PlayerController _controller;
    protected PlayerInput _playerInput;

    public PlayerState(PlayerController playerController, Animator animator , PlayerInput playerInput)
    {
        _playerInput = playerInput;
        _animator = animator;
        _controller = playerController;
    }

    //캐릭터 회전
    protected void Rotate(float x, float z)
    {
        if (_playerInput.camera != null)
        {
            var cameraTransForm = _playerInput.camera.transform;
            var cameraForward = cameraTransForm.forward;
            var cameraRight = cameraTransForm.right;

            cameraForward.y = 0;
            cameraRight.y = 0;

            var moveDirection = cameraForward * z + cameraRight * x;

            if (moveDirection != Vector3.zero)
            {
                moveDirection.Normalize();
                _controller.transform.rotation = Quaternion.LookRotation(moveDirection);
            }
        }
    }

    protected void Jump(InputAction.CallbackContext context)
    {
        _controller.Jump();
        _controller.SetState(PlayerController.EPlayerState.Jump);
    }
}
