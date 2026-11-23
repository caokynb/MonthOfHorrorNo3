using UnityEngine;

namespace EchoesOfTheDeep.Rendering
{
    [ExecuteAlways]
    public class BillboardSprite : MonoBehaviour
    {
        [SerializeField] private bool _lockYAxisOnly = true;

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

            Vector3 directionToCamera = _mainCamera.transform.position - transform.position;

            if (_lockYAxisOnly)
            {
                directionToCamera.y = 0f;
            }

            if (directionToCamera.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(-directionToCamera);
            }
        }
    }
}
