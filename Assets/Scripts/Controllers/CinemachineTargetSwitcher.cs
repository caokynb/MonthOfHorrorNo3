using UnityEngine;
using Unity.Cinemachine;
using EchoesOfTheDeep.Core;

namespace EchoesOfTheDeep.Controllers
{
    public class CinemachineTargetSwitcher : MonoBehaviour
    {
        [Header("Cinemachine Camera")]
        [SerializeField] private CinemachineCamera _cinemachineCamera;

        [Header("Targets")]
        [SerializeField] private Transform _shipTarget;
        [SerializeField] private Transform _playerTarget;

        private void Awake()
        {
            if (_cinemachineCamera == null)
            {
                _cinemachineCamera = GetComponent<CinemachineCamera>();
            }
        }

        private void OnEnable()
        {
            GameEvents.OnGameModeChanged += HandleGameModeChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnGameModeChanged -= HandleGameModeChanged;
        }

        private void HandleGameModeChanged(GameMode mode)
        {
            if (_cinemachineCamera == null) return;

            Transform newTarget = mode == GameMode.Sailing ? _shipTarget : _playerTarget;
            SetTarget(newTarget);
        }

        public void SetTarget(Transform target)
        {
            if (_cinemachineCamera == null || target == null) return;
            _cinemachineCamera.Target.TrackingTarget = target;
        }
    }
}
