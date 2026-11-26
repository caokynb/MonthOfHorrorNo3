using UnityEngine;
using UnityEngine.InputSystem;

namespace EchoesOfTheDeep.Controllers
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerCharacterController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _moveSpeed = 4.5f;
        [SerializeField] private float _turnSpeed = 12f;
        [SerializeField] private float _gravity = 9.81f;

        [Header("Input Action (Optional)")]
        [SerializeField] private InputActionReference _moveAction;

        private CharacterController _characterController;
        private Collider[] _colliders;
        private Vector2 _inputVector;
        private Vector3 _movementVelocity;
        private bool _isControlled = false;

        private Transform _attachedBoat;
        private Vector3 _boatLocalOffset;
        private Vector3 _defaultScale;

        public bool IsControlled => _isControlled;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _colliders = GetComponents<Collider>();
            _defaultScale = transform.localScale;
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
            if (!_isControlled) return;

            ReadInput();
            MoveCharacter();
        }

        private void LateUpdate()
        {
            if (_attachedBoat != null)
            {
                // Follow boat position without parenting to avoid non-uniform scale shear
                transform.position = _attachedBoat.TransformPoint(_boatLocalOffset);
            }
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
                _inputVector = new Vector2(x, y).normalized;
            }
        }

        private void MoveCharacter()
        {
            Vector3 moveDirection = new Vector3(_inputVector.x, 0f, _inputVector.y);

            if (_characterController != null && _characterController.enabled)
            {
                if (_characterController.isGrounded)
                {
                    _movementVelocity.y = -0.5f;
                }
                else
                {
                    _movementVelocity.y -= _gravity * Time.deltaTime;
                }

                Vector3 finalMove = moveDirection * _moveSpeed;
                finalMove.y = _movementVelocity.y;

                _characterController.Move(finalMove * Time.deltaTime);
            }

            if (moveDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);
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

        public void AttachToBoat(Transform boatTransform, Vector3 localOffset)
        {
            SetControl(false);
            SetCollidersEnabled(false);

            transform.SetParent(null);
            transform.localScale = _defaultScale;
            transform.rotation = Quaternion.identity;

            _attachedBoat = boatTransform;
            _boatLocalOffset = localOffset;
            if (_attachedBoat != null)
            {
                transform.position = _attachedBoat.TransformPoint(_boatLocalOffset);
            }
        }

        public void DetachFromBoat(Vector3 worldLandingPos)
        {
            _attachedBoat = null;
            transform.SetParent(null);
            transform.localScale = _defaultScale;

            SetCollidersEnabled(false);
            transform.position = worldLandingPos;
            SetCollidersEnabled(true);
            SetControl(true);
        }

        private void SetCollidersEnabled(bool enabled)
        {
            if (_colliders == null) _colliders = GetComponents<Collider>();
            for (int i = 0; i < _colliders.Length; i++)
            {
                if (_colliders[i] != null)
                {
                    _colliders[i].enabled = enabled;
                }
            }
        }

        public void TeleportTo(Vector3 worldPosition)
        {
            if (_characterController == null) _characterController = GetComponent<CharacterController>();
            if (_characterController != null)
            {
                _characterController.enabled = false;
                transform.position = worldPosition;
                _characterController.enabled = true;
            }
            else
            {
                transform.position = worldPosition;
            }
        }
    }
}
