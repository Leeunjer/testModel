using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static PlayerController;


[Serializable]
public struct EnemyStatus
{
    public int maxHp;
    public int hp;
}

[RequireComponent (typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]



public class EnemyController : MonoBehaviour
{

    //Ai 관련
    [SerializeField] private float patrolDetectionDistandce = 10f; // 정찰 범위
    [SerializeField] private float patrolWaitTime = 1f; // 정찰 대기 시간
    [SerializeField] private float patrolChance = 30f; // 정찰 확률
    [SerializeField] private LayerMask detectionTargetLayerMask; // 추격 대상의 Layer Mask
    [SerializeField] private float detectionSightAnlge = 30f;
    [SerializeField] private float minimumRunDistans = 1f;

    [Header("Status")]
    [SerializeField] private EnemyStatus enemyStatus;




    public float PatrolDetectionDistandce => patrolDetectionDistandce;
    public float PatrolWaitTime => patrolWaitTime;
    public float PatrolChance => patrolChance;
    public float DetectionSightAnlge =>detectionSightAnlge;
    public float MinimumRunDistance => minimumRunDistans;

    private Animator _animator;
    private NavMeshAgent _navMeshAgent;

    //HP 표시
    private HPBarController _hpBarController;

    //상태
    public enum EEnemyState
    {
        None, Idle , Patrol, Chase, Attack , Hit , Dead
    }
    public EEnemyState State { get; private set; }
    private Dictionary<EEnemyState, ICharacterState> _states;

    //애니메이터 파라미터
    public static readonly int EnemyAniParamIdle = Animator.StringToHash("idle");
    public static readonly int EnemyAniParamPatrol = Animator.StringToHash("patrol");
    public static readonly int EnemyAniParamChase = Animator.StringToHash("chase");
    public static readonly int EnemyAniParamAttack = Animator.StringToHash("attack");
    public static readonly int EnemyAniParamHit = Animator.StringToHash("hit");
    public static readonly int EnemyAniParamDead = Animator.StringToHash("dead");
    public static readonly int EnemyAniParamMoveSpeed = Animator.StringToHash("move_speed");


    // 추격 대상 찾기
    private Transform _targetTransform;
    private Collider[] _detectionResults = new Collider[1];


    //dead 연출
    private Rigidbody _rigidBody;
    private Collider _collider;

    private void Awake()
    {
        //필수 요소 초기화
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        //Navmesh Agent 설정
        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = true;

        var idleEnemyState = new IdleEnemyState(this,_animator , _navMeshAgent);
        var patrolEnemyState = new PatrolEnemyState(this,_animator ,_navMeshAgent);
        var chaseEnemyState = new ChaseEnemyState(this, _animator, _navMeshAgent);
        var attackEnemyState = new AttackEnemyState(this,_animator , _navMeshAgent);
        var hitEnemyState = new HitEnemyState(this,_animator , _navMeshAgent);
        var deadEnemyState = new DeadEnemyState(this,_animator , _navMeshAgent);

        _states = new Dictionary<EEnemyState, ICharacterState>
        {
            {EEnemyState.Idle, idleEnemyState},
            {EEnemyState.Patrol, patrolEnemyState},
            {EEnemyState.Chase, chaseEnemyState},
            {EEnemyState.Attack, attackEnemyState},
            {EEnemyState.Hit,hitEnemyState },
            {EEnemyState.Dead, deadEnemyState},
        };
        SetState(EEnemyState.Idle);

        //추격 정보 초기화
        _targetTransform = null;

        _hpBarController = GetComponent<HPBarController>();
    }

    public void SetState(EEnemyState state)
    {
        if (State == state) return;
        if (State != EEnemyState.None) _states[State].Exit();
        State = state;
        if (State != EEnemyState.None) _states[State].Enter();
        

    }
    private void Update()
    {
        Debug.Log("Chomper update");
        if (State != EEnemyState.Dead && State != EEnemyState.None)
        {
            _states[State].Update();
        }
    }

    private void OnAnimatorMove()
    {
        var position = _animator.rootPosition;
        _navMeshAgent.nextPosition = position;
        transform.position = position;
    }

    //일정 거리 안에 Player가 있는지 확인 후 Player의 TransForm 정보를 반환, 
    //없으면 null 반환

    public Transform DetectionTargetInCircle()
    {
        if (!_targetTransform)
        { 
            //_targetTransform이 없으면 , 새롭게 찾기
            Physics.OverlapSphereNonAlloc(transform.position, PatrolDetectionDistandce,
                _detectionResults, detectionTargetLayerMask); //?

            //_detectionResults =
            //    Physics.OverlapSphere(transform.position, patrolDetectionDistandce, detectionTargetLayerMask);

            //detectionResults 배열의 0번 인덱스에 값이 있다면 그걸 _targetTransform에 할당해라
            _targetTransform = _detectionResults[0]?.transform;
        }
        else
        {
            //targetTrasnform이 있으면, 그 대상과의 거리를 계산해서 정해지 거리를 벗어나면 _targetTarnsform 정보 초기화
            float playerDistance = Vector3.Distance(transform.position,_targetTransform.position);
            if(playerDistance> patrolDetectionDistandce)
            {
                _targetTransform = null;
                _detectionResults[0] = null;
            }
        }
        return _targetTransform;

    }

    private void OnDrawGizmos()
    {
        //감지 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,patrolDetectionDistandce);

        //시야각
        Gizmos.color = Color.red;
        Vector3 rightDirection = Quaternion.Euler(0,detectionSightAnlge,0)*transform.forward;
        Vector3 leftDirection = Quaternion.Euler(0, -detectionSightAnlge, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, rightDirection * patrolDetectionDistandce);
        Gizmos.DrawRay(transform.position, leftDirection * patrolDetectionDistandce);
        Gizmos.DrawRay(transform.position, transform.forward * patrolDetectionDistandce);




        //Agent 목적지
        if (_navMeshAgent && _navMeshAgent.hasPath)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_navMeshAgent.destination,0.5f);
            Gizmos.DrawLine(transform.position,_navMeshAgent.destination);
        }
    }

    public void SetHit(int damage, Vector3 attackDirection)
    {
        float hpResult = (float)enemyStatus.hp / enemyStatus.maxHp;

        if(enemyStatus.hp <= 0)
        {
            SetState(EEnemyState.Dead);

            _rigidBody.isKinematic = false;
            _rigidBody.useGravity = true;

            var direction = attackDirection;
            direction.y = 1f;
            direction = direction.normalized;
            var force = direction * 5f;

            _rigidBody.AddForce(force,ForceMode.Impulse);

            _collider.isTrigger = false;
        }
        else
        {
            // 피격 당했을 때 감지, 데미지를 적용
            Debug.Log("Damaged" + damage);
            enemyStatus.hp -= damage;
            _hpBarController.SetHp(hpResult);
            SetState(EEnemyState.Hit);
            StartCoroutine(Knockback(attackDirection));
        }

        
        
    }
    private IEnumerator Knockback(Vector3 direction)
    {
        Vector3 knockbackDirection = direction;
        float knockbackDistance = 1f;
        float knockbackDuration = 0.2f;
        float elapsed = 0f;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + knockbackDirection * knockbackDistance;
        targetPosition.y = transform.position.y;

        while (elapsed < knockbackDuration)
        {
            Vector3 lerpPosition = Vector3.Lerp(startPosition, targetPosition, elapsed / knockbackDuration);
            lerpPosition.y = startPosition.y;
            transform.position = lerpPosition;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }
}
