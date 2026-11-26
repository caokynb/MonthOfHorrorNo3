using UnityEngine;

namespace EchoesOfTheDeep.Controllers
{
    public class BoatWakeTrail : MonoBehaviour
    {
        [Header("Wake Trail")]
        [SerializeField] private TrailRenderer _wakeTrail;
        [SerializeField] private float _trailLifetime = 0.65f;
        [SerializeField] private float _wakeWidth = 0.85f;
        [SerializeField] private Vector3 _sternLocalOffset = new Vector3(0f, -0.44f, -0.22f);

        [Header("Speed Sensitivity")]
        [SerializeField] private float _minSpeedToEmit = 0.35f;
        [SerializeField] private float _maxSpeedForFullWidth = 7f;

        [Header("Material")]
        [SerializeField] private Material _wakeMaterial;

        private Rigidbody _rigidbody;

        private void Reset()
        {
#if UNITY_EDITOR
            if (_wakeMaterial == null)
            {
                _wakeMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/BoatWakeTrail.mat");
            }
#endif
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

#if UNITY_EDITOR
            if (_wakeMaterial == null)
            {
                _wakeMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/BoatWakeTrail.mat");
            }
#endif

            CleanupOldEffects();
            EnsureWakeTrailExists();
        }

        private void Update()
        {
            if (_wakeTrail == null) return;

            float speed = _rigidbody != null ? _rigidbody.linearVelocity.magnitude : 0f;
            bool isMoving = speed > _minSpeedToEmit;

            _wakeTrail.emitting = isMoving;

            if (isMoving)
            {
                float speedFactor = Mathf.Clamp01((speed - _minSpeedToEmit) / Mathf.Max(0.01f, _maxSpeedForFullWidth - _minSpeedToEmit));
                _wakeTrail.widthMultiplier = Mathf.Lerp(0.65f, 1f, speedFactor) * _wakeWidth;
            }
        }

        private void LateUpdate()
        {
            if (_wakeTrail != null)
            {
                // Keep trail normal pointing straight up in world space while matching boat Y heading
                _wakeTrail.transform.rotation = Quaternion.Euler(-90f, transform.eulerAngles.y, 0f);
            }
        }

        private void CleanupOldEffects()
        {
            string[] legacyNames = { "Wake_Left", "Wake_Right", "DonStarveWakeRipples" };
            foreach (string childName in legacyNames)
            {
                Transform child = transform.Find(childName);
                if (child != null) Destroy(child.gameObject);
            }
        }

        private void EnsureWakeTrailExists()
        {
            if (_wakeTrail != null) return;

            Transform existing = transform.Find("Wake_Center");
            if (existing != null)
            {
                _wakeTrail = existing.GetComponent<TrailRenderer>();
            }

            if (_wakeTrail == null)
            {
                var trailGo = new GameObject("Wake_Center");
                trailGo.transform.SetParent(transform);
                trailGo.transform.localPosition = _sternLocalOffset;
                // Rotate -90 on X so local +Z points straight UP, lying the trail flat on the water
                trailGo.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

                _wakeTrail = trailGo.AddComponent<TrailRenderer>();
            }

            _wakeTrail.time = _trailLifetime;
            _wakeTrail.minVertexDistance = 0.1f;
            _wakeTrail.alignment = LineAlignment.TransformZ;
            _wakeTrail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _wakeTrail.receiveShadows = false;
            _wakeTrail.widthMultiplier = _wakeWidth;

            // Uniform width along the trail so noise fills the wake band cleanly
            AnimationCurve widthCurve = new AnimationCurve();
            widthCurve.AddKey(new Keyframe(0f, 1f));
            widthCurve.AddKey(new Keyframe(1f, 1f));
            _wakeTrail.widthCurve = widthCurve;

            // Drives noise thinning toward the tail without any alpha gradient blur
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(Color.white, 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(0f, 0f),
                    new GradientAlphaKey(1f, 0.12f),
                    new GradientAlphaKey(0.8f, 0.5f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            _wakeTrail.colorGradient = gradient;

            if (_wakeMaterial != null)
            {
                _wakeTrail.material = _wakeMaterial;
            }

            _wakeTrail.emitting = false;
        }
    }
}
