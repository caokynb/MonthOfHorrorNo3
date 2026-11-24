using UnityEngine;
using Unity.Cinemachine;
using EchoesOfTheDeep.Core;
using EchoesOfTheDeep.Controllers;

namespace EchoesOfTheDeep.Managers
{
    public class GameModeManager : MonoBehaviour
    {
        public static GameModeManager Instance { get; private set; }

        [Header("Controllers")]
        [SerializeField] private ShipController _shipController;
        [SerializeField] private PlayerCharacterController _playerController;

        [Header("Boat Passenger")]
        [Tooltip("Local position offset for the player when standing on the boat deck.")]
        [SerializeField] private Vector3 _playerStandingOffsetOnBoat = new Vector3(0f, 0.4f, 0f);
        [Tooltip("Optional custom standing transform point on the boat.")]
        [SerializeField] private Transform _boatStandingPoint;

        [Header("Camera Tracking")]
        [SerializeField] private CinemachineCamera _cinemachineCamera;
        [SerializeField] private TopDownCameraFollow _fallbackCameraFollow;

        [Header("Initial State")]
        [SerializeField] private GameMode _initialMode = GameMode.Sailing;

        private GameMode _currentMode;
        public GameMode CurrentMode => _currentMode;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            SetMode(_initialMode, Vector3.zero);
        }

        public void SwitchToOnFoot(Vector3 disembarkPosition)
        {
            SetMode(GameMode.OnFoot, disembarkPosition);
        }

        public void SwitchToSailing()
        {
            SetMode(GameMode.Sailing, Vector3.zero);
        }

        private void SetMode(GameMode newMode, Vector3 playerSpawnPos)
        {
            _currentMode = newMode;

            if (_currentMode == GameMode.Sailing)
            {
                Vector3 standingOffset = _boatStandingPoint != null 
                    ? _boatStandingPoint.localPosition 
                    : _playerStandingOffsetOnBoat;

                if (_playerController != null)
                {
                    if (_shipController != null)
                    {
                        _playerController.AttachToBoat(_shipController.transform, standingOffset);
                    }
                    else
                    {
                        _playerController.SetControl(false);
                    }
                    _playerController.gameObject.SetActive(true);
                }

                if (_shipController != null)
                {
                    _shipController.SetControl(true);
                    UpdateCameraTarget(_shipController.transform);
                }
            }
            else if (_currentMode == GameMode.OnFoot)
            {
                if (_shipController != null)
                {
                    _shipController.SetControl(false);
                }

                if (_playerController != null)
                {
                    _playerController.DetachFromBoat(playerSpawnPos);
                    _playerController.gameObject.SetActive(true);
                    UpdateCameraTarget(_playerController.transform);
                }
            }

            GameEvents.TriggerGameModeChanged(_currentMode);
        }

        private void UpdateCameraTarget(Transform target)
        {
            if (target == null) return;

            if (_cinemachineCamera != null)
            {
                _cinemachineCamera.Target.TrackingTarget = target;
            }

            if (_fallbackCameraFollow != null)
            {
                _fallbackCameraFollow.SetTarget(target);
            }
        }
    }
}
