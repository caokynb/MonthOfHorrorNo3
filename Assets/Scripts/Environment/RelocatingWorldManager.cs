using UnityEngine;
using EchoesOfTheDeep.Core;
using EchoesOfTheDeep.Controllers;
using EchoesOfTheDeep.Managers;

namespace EchoesOfTheDeep.Environment
{
    public class RelocatingWorldManager : MonoBehaviour
    {
        [Header("Target & Map")]
        [SerializeField] private Transform _ship;
        [SerializeField] private Transform _mainMapContainer;

        [Header("Distance Thresholds")]
        [SerializeField] private float _relocateDistanceThreshold = 260f;
        [SerializeField] private float _reappearDistanceAhead = 200f;

        private void Start()
        {
            if (_ship == null)
            {
                var shipController = FindAnyObjectByType<ShipController>();
                if (shipController != null) _ship = shipController.transform;
            }
        }

        private void Update()
        {
            if (_ship == null || _mainMapContainer == null) return;

            if (GameModeManager.Instance != null && GameModeManager.Instance.CurrentMode != GameMode.Sailing)
            {
                return;
            }

            CheckAndRelocateWorld();
        }

        private void CheckAndRelocateWorld()
        {
            float distanceSqr = (_ship.position - _mainMapContainer.position).sqrMagnitude;
            float thresholdSqr = _relocateDistanceThreshold * _relocateDistanceThreshold;

            if (distanceSqr > thresholdSqr)
            {
                Vector3 shipHeading = _ship.forward;
                shipHeading.y = 0f;

                if (shipHeading.sqrMagnitude < 0.001f)
                {
                    shipHeading = Vector3.forward;
                }
                else
                {
                    shipHeading.Normalize();
                }

                Vector3 targetPosition = _ship.position + shipHeading * _reappearDistanceAhead;
                targetPosition.y = _mainMapContainer.position.y;

                _mainMapContainer.position = targetPosition;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_mainMapContainer != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(_mainMapContainer.position, _relocateDistanceThreshold);
            }
        }
    }
}
