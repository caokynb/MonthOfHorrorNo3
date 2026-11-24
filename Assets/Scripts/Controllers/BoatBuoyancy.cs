using UnityEngine;

namespace EchoesOfTheDeep.Controllers
{
    public class BoatBuoyancy : MonoBehaviour
    {
        [Header("Wave Motion Matching")]
        [SerializeField] private float _waveFrequency = 0.08f;
        [SerializeField] private float _waveSpeed = 1.2f;
        [SerializeField] private float _waveHeight = 0.25f;

        [Header("Rocking Tilt")]
        [SerializeField] private float _rockingAngle = 3.5f;

        private float _baseY;

        private void Start()
        {
            _baseY = transform.position.y;
        }

        private void Update()
        {
            Vector3 pos = transform.position;
            float timeVal = Time.time;

            float w1 = Mathf.Sin(pos.x * _waveFrequency + timeVal * _waveSpeed);
            float w2 = Mathf.Cos(pos.z * (_waveFrequency * 1.3f) + timeVal * (_waveSpeed * 0.85f));
            float wave = (w1 + w2) * 0.5f;

            float targetY = _baseY + wave * _waveHeight;
            transform.position = new Vector3(pos.x, targetY, pos.z);

            float pitch = Mathf.Sin(timeVal * _waveSpeed * 0.9f) * _rockingAngle;
            float roll = Mathf.Cos(timeVal * _waveSpeed * 1.1f) * (_rockingAngle * 0.7f);

            Quaternion waveTilt = Quaternion.Euler(pitch, transform.eulerAngles.y, roll);
            transform.rotation = Quaternion.Slerp(transform.rotation, waveTilt, Time.deltaTime * 3f);
        }
    }
}
