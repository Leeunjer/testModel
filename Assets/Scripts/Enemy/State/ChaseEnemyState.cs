using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class ChaseEnemyState : EnemyState, ICharacterState
{
    public ChaseEnemyState(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent)
        : base(enemyController, animator, navMeshAgent)
    {
    }

    public void Enter()
    {
        _animator.SetBool(EnemyController.EnemyAniParamChase, true);
    }
public void Update()
    {
        var detectionTargetTransform = _enemyController.DetectionTargetInCircle();
        if (detectionTargetTransform)
        {
            //공격 여부 판단
            if (!_navMeshAgent.pathPending &&
                _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance &&
                DetectionTargetInsight(detectionTargetTransform.position))
            {
                _enemyController.SetState(EnemyController.EEnemyState.Attack);
            }


            //달리기 여부 판단
            if(DetectionTargetInsight(detectionTargetTransform.position) &&
                _navMeshAgent.remainingDistance > _enemyController.MinimumRunDistance)
            {
                //시야각 안에 들어왔고, 거리도 일정 거리 이상으로 떨어졌을 때,달리기

                _animator.SetFloat(EnemyController.EnemyAniParamMoveSpeed, 1);
            }
            else
            {
                _animator.SetFloat(EnemyController.EnemyAniParamMoveSpeed, 0);

            }
            _navMeshAgent.SetDestination(detectionTargetTransform.position);

        }
        else
        {
            _enemyController.SetState(EnemyController.EEnemyState.Idle);
        }
    }

    public void Exit()
    {
        _animator.SetBool(EnemyController.EnemyAniParamChase, false);

    }

    private bool DetectionTargetInsight(Vector3 position)
    {
        var cosTheta = Vector3.Dot(_enemyController.transform.forward, (position - _enemyController.transform.position).normalized);

        var angle = Mathf.Acos(cosTheta)*Mathf.Rad2Deg;

        if(angle < _enemyController.DetectionSightAnlge)
        {
            return true;
        }
        else
        {
            return false;
        }

        
    }
}
