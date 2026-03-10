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
        _waitTime = 0f;
        _animator.SetBool(EnemyController.EnemyAniParamIdle, true);
    }

    public void Exit()
    {
        _animator.SetBool(EnemyController.EnemyAniParamIdle, false);

    }

    public void Update()
    {
        if(_waitTime> _enemyController.PatrolWaitTime)
        {
            var randomValue = Random.Range(0, 100);
            if(randomValue < 30)
            {
                //Á¤Âû ½ÃÀÛ
            }

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
