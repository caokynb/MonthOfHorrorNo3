using UnityEngine;

namespace EchoesOfTheDeep.Controllers
{
    public class TopDownCameraFollow : MonoBehaviour
    {
        [Header("Target & Offset")]
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 15f, -11f);
        [SerializeField] private float _smoothTime = 0.25f;

        [Header("Rotation Settings")]
        [SerializeField] private float _pitchAngle = 55f;

        private Vector3 _currentVelocity;

        private void Start()
        {
            ApplyCameraRotation();
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            Vector3 targetPosition = _target.position + _offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, _smoothTime);
        }

        public void SetTarget(Transform newTarget)
        {
            _target = newTarget;
        }

        [ContextMenu("Apply Rotation")]
        public void ApplyCameraRotation()
        {
            transform.rotation = Quaternion.Euler(_pitchAngle, 0f, 0f);
        }
    }
}
