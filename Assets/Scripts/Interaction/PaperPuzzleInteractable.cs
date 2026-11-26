using UnityEngine;
using EchoesOfTheDeep.Core;
using EchoesOfTheDeep.Interfaces;
using EchoesOfTheDeep.Managers;
using EchoesOfTheDeep.UI;

namespace EchoesOfTheDeep.Interaction
{
    public class PaperPuzzleInteractable : MonoBehaviour, IInteractable, IInteractableFocus
    {
        [Header("Paper Puzzle")]
        [Tooltip("Prefab tờ giấy ghép của đảo này (ví dụ PaperSheet_Island1).")]
        [SerializeField] private GameObject _sheetPrefab;
        [SerializeField] private bool _oneTimeOnly = false;

        [Header("Custom Interact Popup (TextMeshProUGUI)")]
        [Tooltip("Kéo trực tiếp GameObject chứa TextMeshProUGUI (hoặc Panel popup) bạn tự viết chữ vào đây. Code chỉ bật/tắt object này.")]
        [SerializeField] private GameObject _interactPopupObject;

        [Header("Fallback Prompt (chỉ hiện nếu để trống Custom Popup)")]
        [SerializeField] private string _fallbackPrompt = "";

        private bool _hasCompleted;

        public string InteractionPrompt => _interactPopupObject != null ? string.Empty : _fallbackPrompt;

        public bool CanInteract
        {
            get
            {
                if (_oneTimeOnly && _hasCompleted) return false;

                if (GameModeManager.Instance != null && GameModeManager.Instance.CurrentMode != GameMode.OnFoot)
                {
                    return false;
                }

                if (PaperAssemblyController.Instance != null && PaperAssemblyController.Instance.IsOpen)
                {
                    return false;
                }

                return true;
            }
        }

        private void Awake()
        {
            SetPopupActive(false);
        }

        private void OnDisable()
        {
            SetPopupActive(false);
        }

        public void OnFocusGained()
        {
            SetPopupActive(true);
        }

        public void OnFocusLost()
        {
            SetPopupActive(false);
        }

        public void Interact()
        {
            SetPopupActive(false);

            if (PaperAssemblyController.Instance == null)
            {
                Debug.LogWarning("[PaperPuzzleInteractable] Không tìm thấy PaperAssemblyController trong Scene!");
                return;
            }

            if (_oneTimeOnly)
            {
                _hasCompleted = true;
            }

            PaperAssemblyController.Instance.OpenPuzzle(_sheetPrefab);
        }

        private void SetPopupActive(bool isActive)
        {
            if (_interactPopupObject != null && _interactPopupObject.activeSelf != isActive)
            {
                _interactPopupObject.SetActive(isActive);
            }
        }
    }
}
