using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private Transform _target;
    private Vector2 _lookVector;

    private float _azimuthangle;
    private float _polarAngle;

    [SerializeField] private LayerMask obstacleLayerMask;

    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float distance = 3f;

    private void Awake()
    {
        _azimuthangle = 0f;
        _polarAngle = 0f;
    }

    private void LateUpdate()
    {
        if(_target)
        {
            //마수스 x,y 값을 이용해 카메라 이동
            _azimuthangle += _lookVector.x * rotationSpeed * Time.deltaTime;
            _polarAngle -= _lookVector.y * rotationSpeed * Time.deltaTime;
            _polarAngle = Mathf.Clamp(_polarAngle, -20f,60f);

            //벽이 있을 경우 distance 수정
            var adjustCameraDistance = AdjustCameraDistance();

            //카메라 위치 설정
            var cartesianPosition = GetCameraPostion(adjustCameraDistance, _polarAngle, _azimuthangle);
            

            transform.position = _target.position - cartesianPosition;
            transform.LookAt(_target);
        }
    }
    private Vector3 GetCameraPostion(float r, float polaAngle , float azimuthAngle)
    {
        float b = r * Mathf.Cos(polaAngle *  Mathf.Deg2Rad);
        float x = b * Mathf.Sin(azimuthAngle * Mathf.Deg2Rad);
        float y = b * Mathf.Sin(polaAngle * Mathf .Deg2Rad) * -1;
        float z = b * Mathf.Cos(azimuthAngle * Mathf.Deg2Rad);

        return new Vector3(x, y, z);
    }

    public void SetTarget(Transform target , PlayerInput playerInput)
    {
        _target = target;

        //카메라 위치 설정
        var cartesianPosition = GetCameraPostion(distance, _polarAngle, _azimuthangle);
        transform.position = _target.position - cartesianPosition;
        transform.LookAt(_target);

        playerInput.actions["Look"].performed += OnActionLook;
        playerInput.actions["Look"].canceled += OnActionLook;
    }
    private void OnActionLook(InputAction.CallbackContext context)
    {
        _lookVector = context.ReadValue<Vector2>();
        
    }

    private float AdjustCameraDistance()
    {
        var currentDistance = distance;

        Vector3 disrection = GetCameraPostion(1,_polarAngle, _azimuthangle).normalized;
        RaycastHit hit;

        if(Physics.Raycast(_target.position, -disrection, out hit , distance , obstacleLayerMask))
        {
            float offset = 0.3f;
            currentDistance = hit.distance - offset;
            currentDistance = Mathf.Max(currentDistance, 0.5f);
        }
        return currentDistance;
    }
}
