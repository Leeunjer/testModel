using UnityEngine;
using UnityEngine.AI;

public class IdleEnemyState : EnemyState , ICharacterState
{
    private float _waitTime;
    public IdleEnemyState(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent)
        : base(enemyController, animator, navMeshAgent)
    {
    }

    public void Enter()
    {
        Debug.Log("Enter Idle");
        _waitTime = 0f;
        _animator.SetBool(EnemyController.EnemyAniParamIdle, true);
    }

    public void Exit()
    {
        _animator.SetBool(EnemyController.EnemyAniParamIdle, false);

    }

    public void Update()
    {
        //Enemy 주변에서 Player를 찾는 함수 호출
        var detectionTargetTransform = _enemyController.DetectionTargetInCircle();

        
        if (detectionTargetTransform)
        {
            //주변에서 Player를 찾으면 추격으로 상태 전환
            _navMeshAgent.SetDestination(detectionTargetTransform.position);
            _enemyController.SetState(EnemyController.EEnemyState.Chase);
        }

        //설정된 PatrolWaitTime을 초과하면 정찰 시도
        if(_waitTime > _enemyController.PatrolWaitTime)
        {
            //설정된 PatrolWaitTIme 값보다 작은 랜덤 값이 나오면 정찰 시작
            var randomValue = Random.Range(0, 100);
            if(randomValue < _enemyController.PatrolChance)
            {
                //정찰 위치 찾기
                var patrolPosition = FindRandomPatrolPosition();

                //정찰 위치가 현 위치에서 2unit 이상 벗어 났을 경우 정찰 시작
                var realDistance = Vector3.Magnitude(patrolPosition - _enemyController.transform.position);
                var minimumDistance = _navMeshAgent.stoppingDistance + 2;
                if (realDistance > minimumDistance)
                {
                    _navMeshAgent.SetDestination(patrolPosition);
                    _enemyController.SetState(EnemyController.EEnemyState.Patrol);
                }
            }
            _waitTime = 0f;
        }
        _waitTime += Time.deltaTime;
    }

    private Vector3 FindRandomPatrolPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * _enemyController.PatrolDetectionDistandce;
        randomDirection += _enemyController.transform.position;

        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomDirection,out hit , _enemyController.PatrolDetectionDistandce , NavMesh.AllAreas)) 
        {
            return hit.position;
        }
        else
        {
            return _enemyController.transform.position;
        }


       
    }
}
