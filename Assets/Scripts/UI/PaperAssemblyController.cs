using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EchoesOfTheDeep.Core;
using EchoesOfTheDeep.Controllers;
using EchoesOfTheDeep.Interaction;

namespace EchoesOfTheDeep.UI
{
    public class PaperAssemblyController : MonoBehaviour
    {
        public static PaperAssemblyController Instance { get; private set; }

        [Header("Puzzle Setup")]
        [Tooltip("Root panel của giao diện ghép giấy (bật/tắt khi mở hoặc đóng).")]
        [SerializeField] private GameObject _puzzleRoot;
        [Tooltip("Vùng bàn ghép giấy dùng để giới hạn vùng xáo trộn và kéo thả.")]
        [SerializeField] private RectTransform _boardArea;
        [Tooltip("Tùy chọn: Container riêng để chứa mảnh giấy (nằm dưới nút Xác nhận và chữ).")]
        [SerializeField] private RectTransform _sheetContainer;
        [Tooltip("Prefab tờ giấy mặc định (dùng khi gọi OpenPuzzle() hoặc bật Open On Start để test).")]
        [SerializeField] private GameObject _defaultSheetPrefab;
        [Tooltip("Chỉ dùng nếu bạn đặt sẵn các mảnh trực tiếp trong Scene thay vì dùng Prefab.")]
        [SerializeField] private List<PaperPieceUI> _scenePieces = new List<PaperPieceUI>();

        [Header("UI Controls & Feedback")]
        [SerializeField] private UnityEngine.UI.Button _confirmButton;
        [SerializeField] private TextMeshProUGUI _feedbackText;
        [SerializeField] private bool _showLiveFeedback = true;
        [SerializeField] private bool _broadcastPhraseAfterConfirm = false;
        [SerializeField] private float _postConfirmPhraseDuration = 3.5f;
        [SerializeField] private bool _openOnStart = false;

        [Header("Feedback Messages")]
        [SerializeField] private string _tierVeryLowMessage = "Don't care";
        [SerializeField] private string _tierLowMessage = "Okay.";
        [SerializeField] private string _tierMediumMessage = "Please don't.";
        [SerializeField] private string _tierHighMessage = "Stay..";

        [Header("Scatter Settings")]
        [SerializeField] private float _scatterPadding = 140f;
        [SerializeField] private bool _randomizeRotationOnScatter = true;

        [Header("Accuracy Tuning")]
        [SerializeField] private float _positionDeadzone = 12f;
        [SerializeField] private float _maxPositionError = 180f;
        [SerializeField] private float _angleDeadzone = 6f;
        [SerializeField] private float _maxAngleError = 65f;
        [Range(0f, 1f)]
        [SerializeField] private float _positionWeight = 0.7f;

        [Header("Player Control Lock")]
        [SerializeField] private PlayerCharacterController _playerController;

        private readonly List<PaperPieceUI> _activePieces = new List<PaperPieceUI>();
        private GameObject _activeSheetInstance;
        private Vector2[] _expectedLocalOffsets;
        private float[] _expectedRelativeAngles;
        private bool _hasCapturedSolution;
        private bool _wasPlayerControlledBeforeOpen;
        private InteractionDetector _playerInteractionDetector;
        private bool _wasDetectorEnabledBeforeOpen;

        public PaperPieceUI SelectedPiece { get; private set; }
        public PaperPieceUI DraggingPiece { get; private set; }
        public PaperPieceUI HoveredPiece { get; private set; }
        public float LastAccuracy { get; private set; }
        public string LastFeedbackPhrase { get; private set; }
        public bool IsOpen => _puzzleRoot != null && _puzzleRoot.activeSelf;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (_puzzleRoot == null) _puzzleRoot = gameObject;
            if (_boardArea == null) _boardArea = _puzzleRoot.GetComponent<RectTransform>();
            if (_sheetContainer == null) _sheetContainer = _boardArea;

            if (_confirmButton != null)
            {
                _confirmButton.onClick.AddListener(ConfirmAssembly);
            }
        }

        private void Start()
        {
            if (_openOnStart)
            {
                OpenPuzzle();
            }
            else if (_puzzleRoot != null && _puzzleRoot != gameObject)
            {
                _puzzleRoot.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (_confirmButton != null)
            {
                _confirmButton.onClick.RemoveListener(ConfirmAssembly);
            }

            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void OpenPuzzle()
        {
            OpenPuzzle(_defaultSheetPrefab);
        }

        public void OpenPuzzle(GameObject sheetPrefab)
        {
            LockPlayerControl();

            if (_puzzleRoot != null)
            {
                _puzzleRoot.SetActive(true);
            }

            ClearActiveSheetInstance();

            if (sheetPrefab != null)
            {
                LoadSheetFromPrefab(sheetPrefab);
                Canvas.ForceUpdateCanvases();
                CaptureSolutionFromLayout();
            }
            else
            {
                LoadPiecesFromSceneFallback();
                Canvas.ForceUpdateCanvases();
                if (!_hasCapturedSolution)
                {
                    CaptureSolutionFromLayout();
                }
            }

            ScatterPieces();
            UpdateFeedbackDisplay();
        }

        public void ClosePuzzle()
        {
            ClearActiveSheetInstance();

            if (_feedbackText != null)
            {
                _feedbackText.text = string.Empty;
                _feedbackText.gameObject.SetActive(false);
            }

            if (_puzzleRoot != null && _puzzleRoot != gameObject)
            {
                _puzzleRoot.SetActive(false);
            }

            RestorePlayerControl();
        }

        public void ConfirmAssembly()
        {
            float accuracy = CalculateAccuracy();
            string phrase = GetAccuracyPhrase(accuracy);

            LastAccuracy = accuracy;
            LastFeedbackPhrase = phrase;

            ClosePuzzle();

            if (_broadcastPhraseAfterConfirm && !string.IsNullOrEmpty(phrase))
            {
                GameEvents.TriggerNarrativeDisplayed(phrase, _postConfirmPhraseDuration);
            }

            GameEvents.TriggerPaperAssembled(accuracy);
        }

        public string GetAccuracyPhrase(float accuracy)
        {
            if (accuracy < 0.25f) return _tierVeryLowMessage;
            if (accuracy < 0.50f) return _tierLowMessage;
            if (accuracy <= 0.75f) return _tierMediumMessage;
            return _tierHighMessage;
        }

        public void SetSelectedPiece(PaperPieceUI piece)
        {
            SelectedPiece = piece;
        }

        public void SetDraggingPiece(PaperPieceUI piece)
        {
            DraggingPiece = piece;
        }

        public void SetHoveredPiece(PaperPieceUI piece, bool isHovered)
        {
            if (isHovered)
            {
                HoveredPiece = piece;
            }
            else if (HoveredPiece == piece)
            {
                HoveredPiece = null;
            }
        }

        public PaperPieceUI GetActiveRotationTarget()
        {
            if (DraggingPiece != null) return DraggingPiece;
            if (HoveredPiece != null) return HoveredPiece;
            return SelectedPiece;
        }

        public void NotifyPieceMoved()
        {
            UpdateFeedbackDisplay();
        }

        public float CalculateAccuracy()
        {
            if (!_hasCapturedSolution || _activePieces.Count < 2) return 1f;

            PaperPieceUI anchor = _activePieces[0];
            if (anchor == null) return 0f;

            Vector2 anchorPos = anchor.RectTransform.anchoredPosition;
            float anchorRot = anchor.RectTransform.localEulerAngles.z;

            float totalScore = 0f;
            int counted = 0;

            for (int i = 1; i < _activePieces.Count; i++)
            {
                PaperPieceUI piece = _activePieces[i];
                if (piece == null) continue;

                Vector2 expectedOffset = RotateVector2(_expectedLocalOffsets[i], anchorRot);
                Vector2 actualOffset = piece.RectTransform.anchoredPosition - anchorPos;
                float posError = Vector2.Distance(actualOffset, expectedOffset);

                float actualRelAngle = Mathf.DeltaAngle(anchorRot, piece.RectTransform.localEulerAngles.z);
                float angleError = Mathf.Abs(Mathf.DeltaAngle(actualRelAngle, _expectedRelativeAngles[i]));

                float posScore = 1f - Mathf.Clamp01(Mathf.Max(0f, posError - _positionDeadzone) / Mathf.Max(1f, _maxPositionError));
                float angleScore = 1f - Mathf.Clamp01(Mathf.Max(0f, angleError - _angleDeadzone) / Mathf.Max(1f, _maxAngleError));

                float pieceScore = Mathf.Lerp(angleScore, posScore, _positionWeight);
                totalScore += pieceScore;
                counted++;
            }

            return counted > 0 ? Mathf.Clamp01(totalScore / counted) : 1f;
        }

        private void LoadSheetFromPrefab(GameObject sheetPrefab)
        {
            Transform parent = _sheetContainer != null ? _sheetContainer : _boardArea;
            _activeSheetInstance = Instantiate(sheetPrefab, parent, false);

            // Place behind UI controls (Confirm Button & Feedback Text) when sharing _boardArea
            _activeSheetInstance.transform.SetAsFirstSibling();

            if (_activeSheetInstance.TryGetComponent<RectTransform>(out var sheetRect))
            {
                sheetRect.anchoredPosition = Vector2.zero;
                sheetRect.localScale = Vector3.one;
                sheetRect.localRotation = Quaternion.identity;
            }

            _activePieces.Clear();
            _activePieces.AddRange(_activeSheetInstance.GetComponentsInChildren<PaperPieceUI>(true));

            for (int i = 0; i < _activePieces.Count; i++)
            {
                if (_activePieces[i] != null)
                {
                    _activePieces[i].Initialize(this, _boardArea);
                }
            }
        }

        private void LoadPiecesFromSceneFallback()
        {
            _activePieces.Clear();

            if (_scenePieces.Count > 0)
            {
                _activePieces.AddRange(_scenePieces);
            }
            else if (_sheetContainer != null)
            {
                _activePieces.AddRange(_sheetContainer.GetComponentsInChildren<PaperPieceUI>(true));
            }

            for (int i = 0; i < _activePieces.Count; i++)
            {
                if (_activePieces[i] != null)
                {
                    _activePieces[i].Initialize(this, _boardArea);
                }
            }
        }

        private void ClearActiveSheetInstance()
        {
            if (_activeSheetInstance != null)
            {
                Destroy(_activeSheetInstance);
                _activeSheetInstance = null;
                _activePieces.Clear();
                _hasCapturedSolution = false;
            }

            SelectedPiece = null;
            DraggingPiece = null;
            HoveredPiece = null;
        }

        private void CaptureSolutionFromLayout()
        {
            if (_activePieces.Count == 0 || _activePieces[0] == null)
            {
                _hasCapturedSolution = false;
                return;
            }

            int count = _activePieces.Count;
            _expectedLocalOffsets = new Vector2[count];
            _expectedRelativeAngles = new float[count];

            RectTransform anchorRect = _activePieces[0].GetComponent<RectTransform>();
            Vector2 anchorPos = anchorRect.anchoredPosition;
            float anchorRot = anchorRect.localEulerAngles.z;

            for (int i = 0; i < count; i++)
            {
                if (_activePieces[i] == null) continue;

                RectTransform rect = _activePieces[i].GetComponent<RectTransform>();
                Vector2 delta = rect.anchoredPosition - anchorPos;
                _expectedLocalOffsets[i] = RotateVector2(delta, -anchorRot);
                _expectedRelativeAngles[i] = Mathf.DeltaAngle(anchorRot, rect.localEulerAngles.z);
            }

            _hasCapturedSolution = true;
        }

        private void ScatterPieces()
        {
            if (_boardArea == null || _activePieces.Count == 0) return;

            Rect rect = _boardArea.rect;
            float minX = rect.xMin + _scatterPadding;
            float maxX = rect.xMax - _scatterPadding;
            float minY = rect.yMin + _scatterPadding;
            float maxY = rect.yMax - _scatterPadding;

            if (minX > maxX) { minX = -150f; maxX = 150f; }
            if (minY > maxY) { minY = -100f; maxY = 100f; }

            int count = _activePieces.Count;
            int cols = Mathf.Max(1, Mathf.CeilToInt(Mathf.Sqrt(count)));
            int rows = Mathf.Max(1, Mathf.CeilToInt((float)count / cols));

            float cellWidth = (maxX - minX) / cols;
            float cellHeight = (maxY - minY) / rows;

            // Shuffle grid zone indices so pieces start spread across the board without piling on each other
            List<int> cellIndices = new List<int>(cols * rows);
            for (int c = 0; c < cols * rows; c++) cellIndices.Add(c);
            for (int c = cellIndices.Count - 1; c > 0; c--)
            {
                int swapIdx = Random.Range(0, c + 1);
                (cellIndices[c], cellIndices[swapIdx]) = (cellIndices[swapIdx], cellIndices[c]);
            }

            for (int i = 0; i < count; i++)
            {
                PaperPieceUI piece = _activePieces[i];
                if (piece == null) continue;

                int cell = cellIndices[i % cellIndices.Count];
                int col = cell % cols;
                int row = cell / cols;

                float centerX = minX + (col + 0.5f) * cellWidth;
                float centerY = minY + (row + 0.5f) * cellHeight;
                float jitterX = Random.Range(-cellWidth * 0.22f, cellWidth * 0.22f);
                float jitterY = Random.Range(-cellHeight * 0.22f, cellHeight * 0.22f);

                piece.RectTransform.anchoredPosition = new Vector2(centerX + jitterX, centerY + jitterY);

                if (_randomizeRotationOnScatter)
                {
                    float randomAngle = Random.Range(-180f, 180f);
                    piece.RectTransform.localEulerAngles = new Vector3(0f, 0f, randomAngle);
                }
            }

            SelectedPiece = null;
            DraggingPiece = null;
            HoveredPiece = null;
        }

        private void UpdateFeedbackDisplay()
        {
            if (_feedbackText == null) return;

            if (_showLiveFeedback)
            {
                if (!_feedbackText.gameObject.activeSelf)
                {
                    _feedbackText.gameObject.SetActive(true);
                }
                float accuracy = CalculateAccuracy();
                _feedbackText.text = GetAccuracyPhrase(accuracy);
            }
            else
            {
                _feedbackText.text = string.Empty;
                _feedbackText.gameObject.SetActive(false);
            }
        }

        private void LockPlayerControl()
        {
            if (_playerController == null)
            {
                _playerController = FindAnyObjectByType<PlayerCharacterController>();
            }

            if (_playerController != null)
            {
                _wasPlayerControlledBeforeOpen = _playerController.IsControlled;
                _playerController.SetControl(false);

                if (_playerInteractionDetector == null)
                {
                    _playerInteractionDetector = _playerController.GetComponent<InteractionDetector>();
                }
                if (_playerInteractionDetector != null)
                {
                    _wasDetectorEnabledBeforeOpen = _playerInteractionDetector.enabled;
                    _playerInteractionDetector.enabled = false;
                }
            }
        }

        private void RestorePlayerControl()
        {
            if (_playerController != null)
            {
                if (_wasPlayerControlledBeforeOpen)
                {
                    _playerController.SetControl(true);
                }

                if (_playerInteractionDetector != null && _wasDetectorEnabledBeforeOpen)
                {
                    _playerInteractionDetector.enabled = true;
                }
            }
        }

        private static Vector2 RotateVector2(Vector2 v, float degrees)
        {
            float rad = degrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
        }
    }
}
