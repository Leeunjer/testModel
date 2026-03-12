using System;
using UnityEngine;

public class EllenController : PlayerController, IWeaponObserver<GameObject>
{
    [SerializeField] private Transform weaponAttachTransform;

    private MeleeWeaponController _weaponController;

    private void Start()
    {
        var staffObject = Resources.Load<GameObject>("Staff");
        _weaponController = Instantiate(staffObject, weaponAttachTransform).GetComponent<MeleeWeaponController>();
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
        if (enemyController != null) enemyController?.SetHit(10, -transform.forward); //√•±Ú««

    }

    public void OnCompleted()
    {

    }

    public void OnError(Exception error)
    {

    }
}
