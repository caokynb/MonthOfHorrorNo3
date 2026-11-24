using UnityEngine;

namespace EchoesOfTheDeep.Rendering
{
    [ExecuteAlways]
    public class BillboardSprite : MonoBehaviour
    {
        [Header("Billboard Alignment")]
        [Tooltip("If true, the sprite matches camera tilt directly so it never snaps upright.")]
        [SerializeField] private bool _matchCameraTilt = true;

        [Tooltip("Lock to vertical Y-axis only (90 degrees to ground, like classic Doom/Mario Kart).")]
        [SerializeField] private bool _lockYAxisOnly = false;

        [Header("Custom Tilt Adjustment")]
        [Tooltip("Additional manual tilt offset in degrees.")]
        [SerializeField] private float _tiltOffsetAngle = 0f;

        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera == null) return;
            }

            if (_matchCameraTilt)
            {
                // Match camera view direction completely so the sprite never snaps or distorts
                transform.rotation = _mainCamera.transform.rotation;
                if (Mathf.Abs(_tiltOffsetAngle) > 0.01f)
                {
                    transform.Rotate(Vector3.right, _tiltOffsetAngle, Space.Self);
                }
            }
            else
            {
                Vector3 directionToCamera = _mainCamera.transform.position - transform.position;

                if (_lockYAxisOnly)
                {
                    directionToCamera.y = 0f;
                }

                if (directionToCamera.sqrMagnitude > 0.001f)
                {
                    transform.rotation = Quaternion.LookRotation(-directionToCamera);
                    if (Mathf.Abs(_tiltOffsetAngle) > 0.01f)
                    {
                        transform.Rotate(Vector3.right, _tiltOffsetAngle, Space.Self);
                    }
                }
            }
        }
    }
}
