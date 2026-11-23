using UnityEngine;
using TMPro;
using EchoesOfTheDeep.Core;

namespace EchoesOfTheDeep.UI
{
    public class SimplePromptUI : MonoBehaviour
    {
        [Header("Narrative UI (Lời thoại đảo ở giữa màn hình)")]
        [SerializeField] private TextMeshProUGUI _narrativeText;
        [SerializeField] private GameObject _narrativeContainer;

        [Header("Interaction Prompt UI (Nút bấm [E] ở góc màn hình)")]
        [SerializeField] private TextMeshProUGUI _interactionPromptText;
        [SerializeField] private GameObject _interactionPromptContainer;

        [Header("GUI Fallback (Tự vẽ khi chưa gắn Canvas/TextMeshPro)")]
        [SerializeField] private bool _enableGuiFallback = true;

        private string _currentNarrative = string.Empty;
        private string _currentInteractionPrompt = string.Empty;
        private float _narrativeTimer = 0f;

        private void OnEnable()
        {
            GameEvents.OnNarrativeDisplayed += HandleNarrativeDisplayed;
            GameEvents.OnNarrativeCleared += HandleNarrativeCleared;
            GameEvents.OnInteractionPromptChanged += HandleInteractionPromptChanged;
            GameEvents.OnInteractionPromptCleared += HandleInteractionPromptCleared;

            // Backward compatibility
            GameEvents.OnPromptDisplayed += HandleLegacyPrompt;
            GameEvents.OnPromptCleared += HandleLegacyPromptCleared;
        }

        private void OnDisable()
        {
            GameEvents.OnNarrativeDisplayed -= HandleNarrativeDisplayed;
            GameEvents.OnNarrativeCleared -= HandleNarrativeCleared;
            GameEvents.OnInteractionPromptChanged -= HandleInteractionPromptChanged;
            GameEvents.OnInteractionPromptCleared -= HandleInteractionPromptCleared;

            GameEvents.OnPromptDisplayed -= HandleLegacyPrompt;
            GameEvents.OnPromptCleared -= HandleLegacyPromptCleared;
        }

        private void Update()
        {
            if (_narrativeTimer > 0f)
            {
                _narrativeTimer -= Time.deltaTime;
                if (_narrativeTimer <= 0f)
                {
                    HandleNarrativeCleared();
                }
            }
        }

        private void HandleNarrativeDisplayed(string message, float duration)
        {
            _currentNarrative = message;
            _narrativeTimer = duration > 0f ? duration : 7f;

            if (_narrativeText != null) _narrativeText.text = message;
            if (_narrativeContainer != null) _narrativeContainer.SetActive(true);
        }

        private void HandleNarrativeCleared()
        {
            _currentNarrative = string.Empty;
            _narrativeTimer = 0f;

            if (_narrativeText != null) _narrativeText.text = string.Empty;
            if (_narrativeContainer != null) _narrativeContainer.SetActive(false);
        }

        private void HandleInteractionPromptChanged(string prompt)
        {
            _currentInteractionPrompt = prompt;

            if (_interactionPromptText != null) _interactionPromptText.text = prompt;
            if (_interactionPromptContainer != null) _interactionPromptContainer.SetActive(true);
        }

        private void HandleInteractionPromptCleared()
        {
            _currentInteractionPrompt = string.Empty;

            if (_interactionPromptText != null) _interactionPromptText.text = string.Empty;
            if (_interactionPromptContainer != null) _interactionPromptContainer.SetActive(false);
        }

        private void HandleLegacyPrompt(string message)
        {
            if (string.IsNullOrEmpty(_currentNarrative))
            {
                _currentNarrative = message;
                _narrativeTimer = 7f;
            }
        }

        private void HandleLegacyPromptCleared()
        {
            // Không xóa narrative khi sự kiện legacy trigger
        }

        private void OnGUI()
        {
            if (!_enableGuiFallback) return;

            // 1. Vẽ thông điệp cốt truyện / lời thoại kinh dị ở giữa màn hình (màu đỏ)
            if (!string.IsNullOrEmpty(_currentNarrative) && _narrativeText == null)
            {
                GUIStyle narrativeStyle = new GUIStyle(GUI.skin.box)
                {
                    fontSize = 24,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
                narrativeStyle.normal.textColor = new Color(1f, 0.25f, 0.25f);

                float width = Mathf.Min(650f, Screen.width - 40f);
                float height = 65f;
                float x = (Screen.width - width) * 0.5f;
                float y = Screen.height * 0.35f;

                GUI.Box(new Rect(x, y, width, height), _currentNarrative, narrativeStyle);
            }
        }
    }
}
