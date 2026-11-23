using UnityEngine;

namespace EchoesOfTheDeep.Rendering
{
    [ExecuteAlways]
    public class DropShadow : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _groundY = 0.02f;
        [SerializeField] private LayerMask _groundLayer = ~0;
        [SerializeField] private bool _useRaycast = false;

        private void LateUpdate()
        {
            if (_target == null) return;

            Vector3 shadowPosition = _target.position;

            if (_useRaycast && Application.isPlaying)
            {
                if (Physics.Raycast(_target.position + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 10f, _groundLayer))
                {
                    shadowPosition.y = hit.point.y + 0.02f;
                }
                else
                {
                    shadowPosition.y = _groundY;
                }
            }
            else
            {
                shadowPosition.y = _groundY;
            }

            transform.position = shadowPosition;
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }
}
