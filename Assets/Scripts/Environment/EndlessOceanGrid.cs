using UnityEngine;
using EchoesOfTheDeep.Controllers;

namespace EchoesOfTheDeep.Environment
{
    [ExecuteAlways]
    public class EndlessOceanGrid : MonoBehaviour
    {
        [Header("Tracking Target")]
        [SerializeField] private Transform _target;

        [Header("Grid Configuration")]
        [SerializeField] private float _tileSize = 100f;

        [Header("All 9 Planes (Main + 8 Surrounding)")]
        [SerializeField] private Transform _mainIsland;
        [SerializeField] private Transform _planeN;
        [SerializeField] private Transform _planeS;
        [SerializeField] private Transform _planeE;
        [SerializeField] private Transform _planeW;
        [SerializeField] private Transform _planeNE;
        [SerializeField] private Transform _planeNW;
        [SerializeField] private Transform _planeSE;
        [SerializeField] private Transform _planeSW;

        private readonly Transform[] _allPlanes = new Transform[9];
        private int _gridCenterX = 0;
        private int _gridCenterZ = 0;

        private void OnValidate()
        {
            if (_tileSize <= 0.1f)
            {
                AutoMeasureTileSize();
            }
        }

        private void Awake()
        {
            CollectPlanes();
            if (_tileSize <= 0.1f) AutoMeasureTileSize();
        }

        private void Start()
        {
            CollectPlanes();

            if (_target == null && Application.isPlaying)
            {
                var ship = FindAnyObjectByType<ShipController>();
                if (ship != null) _target = ship.transform;
            }

            if (_tileSize <= 0.1f) AutoMeasureTileSize();

            if (_target != null)
            {
                _gridCenterX = Mathf.RoundToInt(_target.position.x / _tileSize);
                _gridCenterZ = Mathf.RoundToInt(_target.position.z / _tileSize);
            }
        }

        private void Update()
        {
            if (_target == null) return;

            if (Application.isPlaying)
            {
                CheckAndShiftGrid();
            }
        }

        private void CheckAndShiftGrid()
        {
            if (_tileSize <= 0.1f) return;

            int boatGridX = Mathf.RoundToInt(_target.position.x / _tileSize);
            int boatGridZ = Mathf.RoundToInt(_target.position.z / _tileSize);

            while (boatGridX > _gridCenterX)
            {
                ShiftColumn(_gridCenterX - 1, +3);
                _gridCenterX++;
            }

            while (boatGridX < _gridCenterX)
            {
                ShiftColumn(_gridCenterX + 1, -3);
                _gridCenterX--;
            }

            while (boatGridZ > _gridCenterZ)
            {
                ShiftRow(_gridCenterZ - 1, +3);
                _gridCenterZ++;
            }

            while (boatGridZ < _gridCenterZ)
            {
                ShiftRow(_gridCenterZ + 1, -3);
                _gridCenterZ--;
            }
        }

        private void ShiftColumn(int columnGridX, int shiftMultiplier)
        {
            float shiftAmount = shiftMultiplier * _tileSize;

            for (int i = 0; i < _allPlanes.Length; i++)
            {
                Transform plane = _allPlanes[i];
                if (plane == null) continue;

                int planeGridX = Mathf.RoundToInt(plane.position.x / _tileSize);
                if (planeGridX == columnGridX)
                {
                    plane.position += new Vector3(shiftAmount, 0f, 0f);
                }
            }
        }

        private void ShiftRow(int rowGridZ, int shiftMultiplier)
        {
            float shiftAmount = shiftMultiplier * _tileSize;

            for (int i = 0; i < _allPlanes.Length; i++)
            {
                Transform plane = _allPlanes[i];
                if (plane == null) continue;

                int planeGridZ = Mathf.RoundToInt(plane.position.z / _tileSize);
                if (planeGridZ == rowGridZ)
                {
                    plane.position += new Vector3(0f, 0f, shiftAmount);
                }
            }
        }

        [ContextMenu("Align 3x3 Grid Initially")]
        public void Align3x3GridInitially()
        {
            CollectPlanes();
            AutoMeasureTileSize();

            Vector3 centerPos = _mainIsland != null ? _mainIsland.position : transform.position;
            float cx = Mathf.Round(centerPos.x / _tileSize) * _tileSize;
            float cz = Mathf.Round(centerPos.z / _tileSize) * _tileSize;
            float y = centerPos.y;

            if (_mainIsland != null) _mainIsland.position = new Vector3(cx, y, cz);
            if (_planeN != null) _planeN.position = new Vector3(cx, y, cz + _tileSize);
            if (_planeS != null) _planeS.position = new Vector3(cx, y, cz - _tileSize);
            if (_planeE != null) _planeE.position = new Vector3(cx + _tileSize, y, cz);
            if (_planeW != null) _planeW.position = new Vector3(cx - _tileSize, y, cz);
            if (_planeNE != null) _planeNE.position = new Vector3(cx + _tileSize, y, cz + _tileSize);
            if (_planeNW != null) _planeNW.position = new Vector3(cx - _tileSize, y, cz + _tileSize);
            if (_planeSE != null) _planeSE.position = new Vector3(cx + _tileSize, y, cz - _tileSize);
            if (_planeSW != null) _planeSW.position = new Vector3(cx - _tileSize, y, cz - _tileSize);

            _gridCenterX = Mathf.RoundToInt(cx / _tileSize);
            _gridCenterZ = Mathf.RoundToInt(cz / _tileSize);
        }

        [ContextMenu("Auto Measure Tile Size")]
        public void AutoMeasureTileSize()
        {
            CollectPlanes();

            Transform sample = _planeN != null ? _planeN : _mainIsland;
            if (sample != null)
            {
                MeshRenderer mr = sample.GetComponentInChildren<MeshRenderer>();
                if (mr != null && mr.bounds.size.x > 0.1f)
                {
                    _tileSize = Mathf.Round(mr.bounds.size.x);
                }
                else
                {
                    _tileSize = Mathf.Round(sample.lossyScale.x * 10f);
                }
            }

            if (_tileSize <= 0.1f) _tileSize = 100f;
        }

        [ContextMenu("Auto Assign References By Name")]
        public void AutoAssignReferencesByName()
        {
            foreach (Transform child in transform)
            {
                string lower = child.name.ToLower();
                if (lower.Contains("main")) _mainIsland = child;
                else if (lower == "planen" || lower == "north") _planeN = child;
                else if (lower == "planes" || lower == "south") _planeS = child;
                else if (lower == "planee" || lower == "east") _planeE = child;
                else if (lower == "planew" || lower == "west") _planeW = child;
                else if (lower == "planene") _planeNE = child;
                else if (lower == "planenw") _planeNW = child;
                else if (lower == "planese") _planeSE = child;
                else if (lower == "planesw") _planeSW = child;
            }

            CollectPlanes();
            AutoMeasureTileSize();
        }

        private void CollectPlanes()
        {
            _allPlanes[0] = _mainIsland;
            _allPlanes[1] = _planeN;
            _allPlanes[2] = _planeS;
            _allPlanes[3] = _planeE;
            _allPlanes[4] = _planeW;
            _allPlanes[5] = _planeNE;
            _allPlanes[6] = _planeNW;
            _allPlanes[7] = _planeSE;
            _allPlanes[8] = _planeSW;
        }
    }
}
