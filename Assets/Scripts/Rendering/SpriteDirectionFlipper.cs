using UnityEngine;

namespace EchoesOfTheDeep.Rendering
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteDirectionFlipper : MonoBehaviour
    {
        [SerializeField] private float _velocityThreshold = 0.05f;
        [SerializeField] private bool _invertFacing = false;

        private SpriteRenderer _spriteRenderer;
        private Vector3 _lastPosition;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _lastPosition = transform.position;
        }

        private void LateUpdate()
        {
            float horizontalDelta = transform.position.x - _lastPosition.x;

            if (Mathf.Abs(horizontalDelta) > _velocityThreshold * Time.deltaTime)
            {
                bool movingLeft = horizontalDelta < 0f;
                _spriteRenderer.flipX = _invertFacing ? !movingLeft : movingLeft;
            }

            _lastPosition = transform.position;
        }
    }
}
