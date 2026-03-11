using UnityEngine;

public class EllenController : PlayerController
{
    [SerializeField] private Transform weaponAttachTransform;

    private MeleeWeaponController _weaponController;

    private void Start()
    {
        var staffObject = Resources.Load<GameObject>("Staff");
        _weaponController = Instantiate(staffObject,weaponAttachTransform).GetComponent<MeleeWeaponController>();
    }

    public void MeleeAttackStart()
    {

    }
    public void MeleeAttackEnd()
    {

    }
}
