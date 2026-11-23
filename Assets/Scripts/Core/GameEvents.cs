using System;
using UnityEngine;

namespace EchoesOfTheDeep.Core
{
    public static class GameEvents
    {
        public static event Action<GameMode> OnGameModeChanged;
        public static event Action<int> OnIslandReached;

        // Sự kiện hiển thị thông điệp cốt truyện / lời thoại kinh dị (ở giữa màn hình)
        public static event Action<string, float> OnNarrativeDisplayed;
        public static event Action OnNarrativeCleared;

        // Sự kiện hiển thị phím bấm tương tác [E]
        public static event Action<string> OnInteractionPromptChanged;
        public static event Action OnInteractionPromptCleared;

        // Backward compatibility
        public static event Action<string> OnPromptDisplayed;
        public static event Action OnPromptCleared;

        public static void TriggerGameModeChanged(GameMode mode) => OnGameModeChanged?.Invoke(mode);
        public static void TriggerIslandReached(int islandIndex) => OnIslandReached?.Invoke(islandIndex);

        public static void TriggerNarrativeDisplayed(string message, float duration = 7f)
        {
            OnNarrativeDisplayed?.Invoke(message, duration);
            OnPromptDisplayed?.Invoke(message);
        }

        public static void TriggerNarrativeCleared()
        {
            OnNarrativeCleared?.Invoke();
            OnPromptCleared?.Invoke();
        }

        public static void TriggerInteractionPromptChanged(string prompt)
        {
            OnInteractionPromptChanged?.Invoke(prompt);
        }

        public static void TriggerInteractionPromptCleared()
        {
            OnInteractionPromptCleared?.Invoke();
        }

        public static void TriggerPromptDisplayed(string prompt) => OnPromptDisplayed?.Invoke(prompt);
        public static void TriggerPromptCleared() => OnPromptCleared?.Invoke();
    }
}
