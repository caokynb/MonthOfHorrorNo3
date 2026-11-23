using UnityEngine;
using EchoesOfTheDeep.Interfaces;
using EchoesOfTheDeep.Managers;
using EchoesOfTheDeep.Core;
using EchoesOfTheDeep.Controllers;

namespace EchoesOfTheDeep.Interaction
{
    public class DockableIsland : MonoBehaviour, IInteractable
    {
        [Header("Landing Adjustment")]
        [SerializeField] private float _landingOffsetFromEdge = 1.8f;
        [SerializeField] private float _landingHeightOffset = 1.2f;

        [Header("Embark Settings")]
        [Tooltip("Khoảng cách tối đa từ người chơi tới thuyền để có thể bấm [E] lên lại thuyền.")]
        [SerializeField] private float _embarkDistance = 4.5f;

        [Header("Island Details")]
        [SerializeField] private int _islandIndex = 0;
        [SerializeField] private string _islandPromptText = "\"Why didn't you smile?\"";
        [Tooltip("Thời gian hiển thị thông điệp cốt truyện (giây).")]
        [SerializeField] private float _narrativeDuration = 7f;

        [Header("Interaction Strings")]
        [SerializeField] private string _dockPrompt = "[E] Cập bến";
        [SerializeField] private string _embarkPrompt = "[E] Lên thuyền";

        private Collider _islandCollider;
        private ShipController _cachedShip;
        private PlayerCharacterController _cachedPlayer;

        private void Awake()
        {
            _islandCollider = GetComponent<Collider>();
        }

        public string InteractionPrompt
        {
            get
            {
                if (GameModeManager.Instance == null) return _dockPrompt;
                return GameModeManager.Instance.CurrentMode == GameMode.Sailing ? _dockPrompt : _embarkPrompt;
            }
        }

        public bool CanInteract
        {
            get
            {
                if (GameModeManager.Instance == null) return false;

                // Khi đang lái thuyền: đảo luôn sẵn sàng nhận tương tác cập bến
                if (GameModeManager.Instance.CurrentMode == GameMode.Sailing)
                {
                    return true;
                }

                // Khi đang đi bộ trên đảo: CHỈ cho phép tương tác (lên thuyền) khi đứng gần thuyền
                var ship = GetShip();
                var player = GetPlayer();

                if (ship != null && player != null)
                {
                    Vector3 shipPos = ship.transform.position;
                    Vector3 playerPos = player.transform.position;

                    // Đo khoảng cách trên mặt phẳng ngang XZ (bỏ qua chênh lệch độ cao Y giữa mặt biển và bờ đảo)
                    shipPos.y = 0f;
                    playerPos.y = 0f;

                    return Vector3.Distance(playerPos, shipPos) <= _embarkDistance;
                }

                return false;
            }
        }

        public void Interact()
        {
            if (GameModeManager.Instance == null) return;

            if (GameModeManager.Instance.CurrentMode == GameMode.Sailing)
            {
                Vector3 shipPos = transform.position;
                var ship = GetShip();
                if (ship != null)
                {
                    shipPos = ship.transform.position;
                }

                Vector3 landingPos = CalculateLandingPosition(shipPos);

                GameModeManager.Instance.SwitchToOnFoot(landingPos);
                GameEvents.TriggerIslandReached(_islandIndex);
                GameEvents.TriggerNarrativeDisplayed(_islandPromptText, _narrativeDuration);
            }
            else
            {
                GameModeManager.Instance.SwitchToSailing();
                GameEvents.TriggerInteractionPromptCleared();
            }
        }

        private ShipController GetShip()
        {
            if (_cachedShip == null) _cachedShip = FindAnyObjectByType<ShipController>();
            return _cachedShip;
        }

        private PlayerCharacterController GetPlayer()
        {
            if (_cachedPlayer == null) _cachedPlayer = FindAnyObjectByType<PlayerCharacterController>();
            return _cachedPlayer;
        }

        private Vector3 CalculateLandingPosition(Vector3 shipPosition)
        {
            if (_islandCollider != null)
            {
                Vector3 closestPointOnRim = _islandCollider.ClosestPoint(shipPosition);

                Vector3 directionToCenter = transform.position - closestPointOnRim;
                directionToCenter.y = 0f;

                if (directionToCenter.sqrMagnitude > 0.001f)
                {
                    directionToCenter.Normalize();
                }

                Vector3 landingPosition = closestPointOnRim + directionToCenter * _landingOffsetFromEdge;
                landingPosition.y = transform.position.y + _landingHeightOffset;
                return landingPosition;
            }

            Vector3 directionFromCenter = shipPosition - transform.position;
            directionFromCenter.y = 0f;
            Vector3 pos = transform.position + directionFromCenter.normalized * 4f;
            pos.y = transform.position.y + _landingHeightOffset;
            return pos;
        }

        private void OnDrawGizmosSelected()
        {
            var ship = GetShip();
            if (ship != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(ship.transform.position, _embarkDistance);
            }
        }
    }
}
