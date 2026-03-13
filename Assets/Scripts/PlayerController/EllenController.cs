using System;
using UnityEngine;

public class EllenController : PlayerController, IWeaponObserver<GameObject>
{
    [SerializeField] private Transform weaponAttachTransform;
    [SerializeField] private int strngth;


    private MeleeWeaponController _weaponController;

    private void Start()
    {
        var staffObject = Resources.Load<GameObject>("Staff");
        _weaponController = Instantiate(staffObject, weaponAttachTransform).GetComponent<MeleeWeaponController>();
        _weaponController.Subscribe(this);
    }

    public void MeleeAttackStart()
    {

        _weaponController.StartTrigger();
    }
    public void MeleeAttackEnd()
    {
        _weaponController.EndTrigger();
    }

    public void OnNext(GameObject value)
    {
        var enemyController = value.GetComponent<EnemyController>();
        if (enemyController) enemyController?.SetHit(strngth, transform.forward); 

    }

    public void OnCompleted()
    {

    }

    public void OnError(Exception error)
    {

    }
}
