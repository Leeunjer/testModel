
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform headTransform;

    [Header("이동")]
    [SerializeField]
    [Range(0f, 5f)] private float breakForce = 1f;

    [SerializeField] private float jumpHeight = 2f;
    public float BreakForce => breakForce;

    private Animator _animator;
    private PlayerInput _playerInput;
    private CharacterController _characterController;

    //애니메이션 키
    public static readonly int PlayerAniParamIdle = Animator.StringToHash("idle");
    public static readonly int PlayerMove = Animator.StringToHash("Move");
    public static readonly int PlayerAniParamJump = Animator.StringToHash("jump");
    public static readonly int PlayerMoveSpeed = Animator.StringToHash("move_speed");
    public static readonly int PlayerAniParamGroundDistance = Animator.StringToHash("ground_distance");
    public static readonly int PlayerAniParamAttack = Animator.StringToHash("attack");


    public enum EPlayerState
    {
        None , Idle , Move ,Jump , Attack
    }

    //물리
    private float _velocityY;

    //상태에 대한 정보
    public EPlayerState PlayerState { get; private set; }

    //상태와 상태 객채를 담고 있는 Dictionary
    private Dictionary<EPlayerState, ICharacterState> _playerStates;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _characterController = GetComponent<CharacterController>();


        //상태 객체 초기화
        var idlePlayerState = new IdleState(this, _animator, _playerInput);
        var movePlayerState = new MoveState(this, _animator, _playerInput);
        var jumpPlayerState = new JumpState(this, _animator, _playerInput);
        var attackPlayerState = new AttackState(this, _animator, _playerInput);

        _playerStates = new Dictionary<EPlayerState, ICharacterState>
        {
            {EPlayerState.Idle , idlePlayerState },
            {EPlayerState.Move, movePlayerState },
            {EPlayerState.Jump, jumpPlayerState },
            {EPlayerState.Attack, attackPlayerState },
        };

        //카메라 할당
        var playerCamera = Camera.main;
        if(playerCamera != null)
        {
            _playerInput.camera = playerCamera;
            playerCamera.GetComponent<CameraController>().SetTarget(headTransform.transform , _playerInput);
        }

        //커서 숨기기
        _playerInput.actions["Cursor"].performed += _ => GameManager.Instance.SetCursorLock();

    }



    private void Start()
    {
        PlayerState = EPlayerState.None;
    }

    private void Update()
    {
        if(PlayerState != EPlayerState.None) 
            _playerStates[PlayerState].Update();
    }

    //새로운 상태를 할당하는 함수
    public void SetState(EPlayerState state)
    {
        if(PlayerState == state)return;
        if (PlayerState != EPlayerState.None)
            _playerStates[PlayerState].Exit();
        PlayerState = state;
        if (PlayerState != EPlayerState.None) 
            _playerStates[PlayerState].Enter();
    }

    public void Jump()
    {
        if(!_characterController.isGrounded) return;
        _velocityY = Mathf.Sqrt(jumpHeight * -2f * Constants.Gravity);// 점프 공식
    }

    private void OnAnimatorMove() // 추가 공부 필요
    {
        Vector3 movePosition;
        if (_characterController.isGrounded)
        {
            movePosition = _animator.deltaPosition;
        }
        else
        {
            movePosition = _characterController.velocity * Time.deltaTime;
        }

        _velocityY += Constants.Gravity * Time.deltaTime;
        movePosition.y = _velocityY * Time.deltaTime;
        _characterController.Move(movePosition);
    }
    

}
