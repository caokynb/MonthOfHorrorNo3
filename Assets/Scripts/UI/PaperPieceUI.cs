using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace EchoesOfTheDeep.UI
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class PaperPieceUI : MonoBehaviour,
        IPointerDownHandler,
        IDragHandler,
        IPointerUpHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        IScrollHandler
    {
        [Header("Hit Testing")]
        [Range(0f, 1f)]
        [SerializeField] private float _alphaHitThreshold = 0.1f;

        [Header("Rotation Settings")]
        [SerializeField] private float _keyRotateSpeed = 120f;
        [SerializeField] private float _scrollRotateStep = 15f;

        private RectTransform _rectTransform;
        private UnityEngine.UI.Image _image;
        private Canvas _parentCanvas;
        private RectTransform _boundsRect;
        private PaperAssemblyController _controller;

        private bool _isDragging;

        public RectTransform RectTransform => _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _image = GetComponent<UnityEngine.UI.Image>();
            _parentCanvas = GetComponentInParent<Canvas>();
            ConfigureAlphaHitTest();
        }

        private void OnDisable()
        {
            if (_isDragging)
            {
                _isDragging = false;
                if (_controller != null && _controller.DraggingPiece == this)
                {
                    _controller.SetDraggingPiece(null);
                }
            }

            _controller?.SetHoveredPiece(this, false);
        }

        public void Initialize(PaperAssemblyController controller, RectTransform boundsRect)
        {
            _controller = controller;
            _boundsRect = boundsRect;
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
            if (_image == null) _image = GetComponent<UnityEngine.UI.Image>();
            if (_parentCanvas == null) _parentCanvas = GetComponentInParent<Canvas>();
            ConfigureAlphaHitTest();
        }

        private void ConfigureAlphaHitTest()
        {
            if (_image == null || _image.sprite == null || _image.sprite.texture == null) return;

            if (_image.sprite.texture.isReadable)
            {
                _image.alphaHitTestMinimumThreshold = _alphaHitThreshold;
            }
            else
            {
                Debug.LogWarning($"[PaperPieceUI] Texture '{_image.sprite.texture.name}' chưa bật Read/Write Enabled trong Import Settings nên vùng trong suốt vẫn nhận click.");
            }
        }

        private void Update()
        {
            if (_controller != null)
            {
                if (_controller.GetActiveRotationTarget() != this) return;
            }
            else if (!_isDragging)
            {
                return;
            }

            if (Keyboard.current == null) return;

            float rotateDir = 0f;
            if (Keyboard.current.qKey.isPressed) rotateDir += 1f;
            if (Keyboard.current.eKey.isPressed) rotateDir -= 1f;

            if (Mathf.Abs(rotateDir) > 0.01f)
            {
                RotateBy(rotateDir * _keyRotateSpeed * Time.unscaledDeltaTime);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                // Push overlapping top piece behind so the piece underneath can be grabbed immediately
                transform.SetAsFirstSibling();
                return;
            }

            if (eventData.button != PointerEventData.InputButton.Left) return;

            _isDragging = true;
            transform.SetAsLastSibling();
            _controller?.SetSelectedPiece(this);
            _controller?.SetDraggingPiece(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (!_isDragging || _rectTransform == null) return;

            float scaleFactor = _parentCanvas != null && _parentCanvas.scaleFactor > 0f
                ? _parentCanvas.scaleFactor
                : 1f;

            Vector2 nextPos = _rectTransform.anchoredPosition + (eventData.delta / scaleFactor);
            _rectTransform.anchoredPosition = ClampToBounds(nextPos);
            _controller?.NotifyPieceMoved();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            _isDragging = false;
            if (_controller != null && _controller.DraggingPiece == this)
            {
                _controller.SetDraggingPiece(null);
            }
            _controller?.NotifyPieceMoved();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _controller?.SetHoveredPiece(this, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _controller?.SetHoveredPiece(this, false);
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (Mathf.Abs(eventData.scrollDelta.y) < 0.01f) return;

            float direction = Mathf.Sign(eventData.scrollDelta.y);
            PaperPieceUI target = _controller != null ? _controller.GetActiveRotationTarget() : this;
            if (target != null)
            {
                target.RotateBy(direction * _scrollRotateStep);
            }
        }

        public void RotateBy(float deltaDegrees)
        {
            if (_rectTransform == null) return;

            Vector3 euler = _rectTransform.localEulerAngles;
            euler.z += deltaDegrees;
            _rectTransform.localEulerAngles = euler;
            _controller?.NotifyPieceMoved();
        }

        private Vector2 ClampToBounds(Vector2 anchoredPos)
        {
            if (_boundsRect == null) return anchoredPos;

            Rect bounds = _boundsRect.rect;
            float margin = 40f;
            float minX = bounds.xMin + margin;
            float maxX = bounds.xMax - margin;
            float minY = bounds.yMin + margin;
            float maxY = bounds.yMax - margin;

            if (minX > maxX || minY > maxY) return anchoredPos;

            return new Vector2(
                Mathf.Clamp(anchoredPos.x, minX, maxX),
                Mathf.Clamp(anchoredPos.y, minY, maxY)
            );
        }
    }
}
