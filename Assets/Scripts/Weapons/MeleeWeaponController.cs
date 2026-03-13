
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct WeaponTriggerZone // class 와 struct의 차이점 기입할 것
{
    public Vector3 position;
    public float radius;
}

public class MeleeWeaponController : MonoBehaviour, IWeaPonObservalbe<GameObject>
{

    [SerializeField] private WeaponTriggerZone[] triggerZone;
    [SerializeField] private LayerMask targetLayerMask;

    private HashSet<Collider> _hitColliders;
    private Vector3[] _previousTriggerPositions;

    private List<IWeaponObserver<GameObject>> _observers =
        new List<IWeaponObserver<GameObject>>();

    private bool _isTriggering;
    private void Awake()
    {
        _previousTriggerPositions = new Vector3[triggerZone.Length];
        _hitColliders = new HashSet<Collider>();

        _isTriggering = false;
    }


    //무기의 주인이 무이게에 트리거 작동을 시작하라고 전달 함수
    public void StartTrigger()
    {
        _hitColliders.Clear();
        for (int i = 0; i < triggerZone.Length; i++)
        {
            _previousTriggerPositions[i] = transform.TransformDirection(triggerZone[i].position);
        }
        _isTriggering = true;
    }

    //무기의 주인이 무기에게 트리거 작동을 중단하라고 전달 함수
    public void EndTrigger()
    {
        foreach (var hitCollider in _hitColliders)
        {
            Notify(hitCollider.gameObject);
        }
        _isTriggering = false;
    }

    private void FixedUpdate()
    {
        if (!_isTriggering) return;

        for (int i = 0; i < triggerZone.Length; i++)
        {
            var worldPosition = transform.TransformPoint(triggerZone[i].position);
            var direction = transform.TransformDirection(_previousTriggerPositions[i] - worldPosition);
            var maxDistance = Vector3.Distance(triggerZone[i].position, _previousTriggerPositions[i]);

            Ray ray = new Ray(worldPosition, direction);

            RaycastHit[] hits = new RaycastHit[1];

            var hitCount = Physics.SphereCastNonAlloc(ray, triggerZone[i].radius, hits,
                maxDistance, targetLayerMask); // NonAlloc은 객채를 계속해서 만들어 내는 것이 아니라 하나의 배열을 이용하여 해당 정보를 hits로 보내줌
            //for (int j = 0; j < hitCount; j++)
            //{
            //    var hit = hits[j];
            //    _hitColliders.Add(hit.collider);
            //}

            if (hitCount > 0)
            {
                Notify(hits[0].collider.gameObject);
                _isTriggering = false;
            }
            _previousTriggerPositions[i] = triggerZone[i].position;
        }
    }

    private Vector3 GetTriggerWorldPosition(Vector3 position)
    {
        return transform.position + transform.TransformDirection(position);
    }

    private void OnDrawGizmos()
    {
        if(!Application.isPlaying) return;

        for (int i = 0; i < triggerZone.Length; i++)
        {
            var triggerZonePosition = transform.TransformPoint(triggerZone[i].position);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(triggerZonePosition, triggerZone[i].radius);

            var previousTriggerZonePosition = transform.TransformPoint(_previousTriggerPositions[i]);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(previousTriggerZonePosition, triggerZone[i].radius);
        }
    }

    #region 옵저버 패턴 코드

    public void Subscribe(IWeaponObserver<GameObject> observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    public void Unsubscribe(IWeaponObserver<GameObject> observer)
    {
        _observers.Remove(observer);
    }

    public void Notify(GameObject value)
    {
        foreach (var observer in _observers)
        {
            observer.OnNext(value);
        }
    }
    #endregion
}
