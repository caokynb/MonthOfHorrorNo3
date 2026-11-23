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
        private Vector2 _inputVector;
        private Vector3 _movementVelocity;
        private bool _isControlled = false;

        public bool IsControlled => _isControlled;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
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

        public void TeleportTo(Vector3 worldPosition)
        {
            _characterController.enabled = false;
            transform.position = worldPosition;
            _characterController.enabled = true;
        }
    }
}
