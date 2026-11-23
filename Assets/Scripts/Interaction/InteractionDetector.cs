using UnityEngine;
using UnityEngine.InputSystem;
using EchoesOfTheDeep.Interfaces;
using EchoesOfTheDeep.Core;

namespace EchoesOfTheDeep.Interaction
{
    public class InteractionDetector : MonoBehaviour
    {
        [Header("Detection Settings")]
        [SerializeField] private float _detectionRadius = 7f;
        [SerializeField] private LayerMask _interactionLayer = ~0;

        [Header("Input (Optional)")]
        [SerializeField] private InputActionReference _interactAction;

        [Header("GUI Prompt Display")]
        [SerializeField] private bool _showOnGuiPrompt = true;

        private IInteractable _currentInteractable;
        private readonly Collider[] _overlapResults = new Collider[16];

        public IInteractable CurrentInteractable => _currentInteractable;

        private void OnEnable()
        {
            if (_interactAction != null) _interactAction.action.Enable();
        }

        private void OnDisable()
        {
            if (_interactAction != null) _interactAction.action.Disable();
        }

        private void Update()
        {
            DetectInteractable();
            CheckInput();
        }

        private void DetectInteractable()
        {
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _detectionRadius, _overlapResults, _interactionLayer);
            IInteractable closestInteractable = null;
            float closestDistanceSqr = float.MaxValue;

            for (int i = 0; i < hitCount; i++)
            {
                Collider col = _overlapResults[i];
                if (col.transform == transform || col.transform.IsChildOf(transform)) continue;

                var interactable = col.GetComponentInParent<IInteractable>();
                if (interactable == null)
                {
                    interactable = col.GetComponentInChildren<IInteractable>();
                }

                if (interactable != null && interactable.CanInteract)
                {
                    Vector3 targetPos = col.ClosestPoint(transform.position);
                    float distSqr = (targetPos - transform.position).sqrMagnitude;

                    if (distSqr < closestDistanceSqr)
                    {
                        closestDistanceSqr = distSqr;
                        closestInteractable = interactable;
                    }
                }
            }

            if (closestInteractable != _currentInteractable)
            {
                _currentInteractable = closestInteractable;
                if (_currentInteractable != null)
                {
                    GameEvents.TriggerInteractionPromptChanged(_currentInteractable.InteractionPrompt);
                }
                else
                {
                    GameEvents.TriggerInteractionPromptCleared();
                }
            }
        }

        private void CheckInput()
        {
            if (_currentInteractable == null || !_currentInteractable.CanInteract) return;

            bool triggered = false;
            if (_interactAction != null && _interactAction.action.enabled)
            {
                triggered = _interactAction.action.WasPressedThisFrame();
            }
            else if (Keyboard.current != null)
            {
                triggered = Keyboard.current.eKey.wasPressedThisFrame;
            }

            if (triggered)
            {
                _currentInteractable.Interact();
            }
        }

        private void OnGUI()
        {
            if (!_showOnGuiPrompt || _currentInteractable == null) return;

            GUIStyle style = new GUIStyle(GUI.skin.box)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            style.normal.textColor = Color.yellow;

            float width = 340f;
            float height = 48f;
            float x = (Screen.width - width) * 0.5f;
            float y = Screen.height - 110f;

            GUI.Box(new Rect(x, y, width, height), _currentInteractable.InteractionPrompt, style);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);
        }
    }
}
