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
            if (_currentInteractable != null)
            {
                if (_currentInteractable is IInteractableFocus focus)
                {
                    focus.OnFocusLost();
                }
                _currentInteractable = null;
                GameEvents.TriggerInteractionPromptCleared();
            }
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
                    // Prefer trigger item colliders when distance ties (e.g. player standing on island ground collider)
                    bool preferTie = col.isTrigger && Mathf.Abs(distSqr - closestDistanceSqr) < 0.05f;

                    if (distSqr < closestDistanceSqr || preferTie)
                    {
                        closestDistanceSqr = distSqr;
                        closestInteractable = interactable;
                    }
                }
            }

            if (closestInteractable != _currentInteractable)
            {
                if (_currentInteractable is IInteractableFocus oldFocus)
                {
                    oldFocus.OnFocusLost();
                }

                _currentInteractable = closestInteractable;

                if (_currentInteractable is IInteractableFocus newFocus)
                {
                    newFocus.OnFocusGained();
                }

                if (_currentInteractable != null && !string.IsNullOrEmpty(_currentInteractable.InteractionPrompt))
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
            if (string.IsNullOrEmpty(_currentInteractable.InteractionPrompt)) return;

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
