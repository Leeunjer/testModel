using System;
using UnityEngine;

public class ChomperEnemyController : EnemyController, IWeaponObserver<GameObject>
{
    private MeleeWeaponController _meleeWeaponController;

    public void PlayStep()
    {
        _meleeWeaponController = GetComponent<MeleeWeaponController>();
        _meleeWeaponController.Subscribe(this);
    }
    public void Grunt()
    {

    }

    public void AttackBegin()
    {
        _meleeWeaponController.StartTrigger();
    } 
    public void AttackEnd() 
    {
        _meleeWeaponController.EndTrigger();
    }

    public void OnNext(GameObject value)
    {
        
        var playerController = value.GetComponent<PlayerController>();


        //대상에게 데미지를 전달
        if(playerController) playerController?.SetHit(10 , -transform.forward);
    }

    public void OnCompleted()
    {
        // 구독을 취소
        _meleeWeaponController.Unsubscribe(this);
    }

    public void OnError(Exception error)
    {
        //오류 처리
    }
}
