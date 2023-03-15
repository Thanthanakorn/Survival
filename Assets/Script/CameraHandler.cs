using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public Transform targetTransform;
    public Transform cameraTransform;
    public Transform cameraPivotTransform;
    private Transform _myTransform;
    private Vector3 _cameraTransformPosition;
    private LayerMask _ignoreLayers;
    private Vector3 _cameraFollowVelocity = Vector3.zero;
    
    public static CameraHandler Singleton;

    public float lookSpeed = 0.1f;
    public float followSpeed = 0.1f;
    public float pivotSpeed = 0.03f;

    private float _defaultPosition;
    private float _lookAngle;
    private float _pivotAngle;
    public float minimumPivot = -35;
    public float maximumPivot = 35;

    public float cameraSphereRadius = 0.2f;
    public float cameraCollisionOffSet = 0.2f;
    public float minimumCollisionOffset = 0.2f;

    private void Awake()
    {
        Singleton = this;
        _myTransform = transform;
        _defaultPosition = cameraTransform.localPosition.z;
        _ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
    }

    public void FollowTarget(float delta)
    {
        var targetPosition = Vector3.SmoothDamp
            (_myTransform.position, targetTransform.position, ref _cameraFollowVelocity, delta / followSpeed);
        _myTransform.position = targetPosition;
        HandleCameraCollisions(delta);
    }

    public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
    {
        _lookAngle += (mouseXInput * lookSpeed) / delta;
        _pivotAngle -= (mouseYInput * pivotSpeed) / delta;
        _pivotAngle = Mathf.Clamp(_pivotAngle, minimumPivot, maximumPivot);

        var rotation = Vector3.zero;
        rotation.y = _lookAngle;
        var targetRotation = Quaternion.Euler(rotation);
        _myTransform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = _pivotAngle;

        targetRotation = Quaternion.Euler(rotation);
        cameraPivotTransform.localRotation = targetRotation;

    }

    private void HandleCameraCollisions(float delta)
    {
        var targetPosition = _defaultPosition;
        var position = cameraTransform.position;
        var direction = position - position;
        direction.Normalize();

        if (Physics.SphereCast
            (cameraPivotTransform.position, cameraSphereRadius, direction, out var hit, Mathf.Abs(targetPosition)
                , _ignoreLayers))
        {
            float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetPosition = -(dis - cameraCollisionOffSet);
        }

        if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition = -minimumCollisionOffset;
        }

        _cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
        cameraTransform.localPosition = _cameraTransformPosition;
    }
}
