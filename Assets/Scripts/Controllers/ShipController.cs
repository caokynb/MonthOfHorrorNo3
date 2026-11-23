using UnityEngine;
using UnityEngine.InputSystem;

namespace EchoesOfTheDeep.Controllers
{
    [RequireComponent(typeof(Rigidbody))]
    public class ShipController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _forwardAcceleration = 18f;
        [SerializeField] private float _reverseAcceleration = 8f;
        [SerializeField] private float _turnTorque = 90f;
        [SerializeField] private float _linearDrag = 1.8f;
        [SerializeField] private float _angularDrag = 3.5f;

        [Header("Sluggishness (Dread Escalation)")]
        [Range(0.2f, 1f)]
        [SerializeField] private float _handlingMultiplier = 1f;

        [Header("Input Action (Optional)")]
        [SerializeField] private InputActionReference _moveAction;

        private Rigidbody _rigidbody;
        private Vector2 _inputVector;
        private bool _isControlled = true;

        public bool IsControlled => _isControlled;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
            _rigidbody.linearDamping = _linearDrag;
            _rigidbody.angularDamping = _angularDrag;
            _rigidbody.constraints = RigidbodyConstraints.FreezePositionY |
                                     RigidbodyConstraints.FreezeRotationX |
                                     RigidbodyConstraints.FreezeRotationZ;
        }

        private void OnEnable()
        {
            if (_moveAction != null) _moveAction.action.Enable();
        }

        private void OnDisable()
        {
            if (_moveAction != null) _moveAction.action.Disable();
        }

        private void Update()
        {
            if (!_isControlled)
            {
                _inputVector = Vector2.zero;
                return;
            }

            ReadInput();
        }

        private void FixedUpdate()
        {
            if (!_isControlled) return;

            ApplyPhysics();
        }

        private void ReadInput()
        {
            if (_moveAction != null && _moveAction.action.enabled)
            {
                _inputVector = _moveAction.action.ReadValue<Vector2>();
            }
            else if (Keyboard.current != null)
            {
                float x = 0f;
                float y = 0f;
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) y += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) y -= 1f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x += 1f;
                _inputVector = new Vector2(x, y);
            }
        }

        private void ApplyPhysics()
        {
            float throttle = _inputVector.y;
            float steering = _inputVector.x;

            if (throttle > 0f)
            {
                Vector3 force = transform.forward * (throttle * _forwardAcceleration * _handlingMultiplier);
                _rigidbody.AddForce(force, ForceMode.Acceleration);
            }
            else if (throttle < 0f)
            {
                Vector3 force = transform.forward * (throttle * _reverseAcceleration * _handlingMultiplier);
                _rigidbody.AddForce(force, ForceMode.Acceleration);
            }

            if (Mathf.Abs(steering) > 0.01f)
            {
                float turnAmount = steering * _turnTorque * _handlingMultiplier * Time.fixedDeltaTime;
                Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
                _rigidbody.MoveRotation(_rigidbody.rotation * turnRotation);
            }
        }

        public void SetControl(bool enable)
        {
            _isControlled = enable;
            if (!enable)
            {
                _inputVector = Vector2.zero;
            }
        }

        public void SetHandlingMultiplier(float multiplier)
        {
            _handlingMultiplier = Mathf.Clamp(multiplier, 0.2f, 1f);
        }
    }
}
