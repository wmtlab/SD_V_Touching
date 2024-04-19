using UnityEngine;

namespace SdVTouching
{
    public class MockController : IPlayerController, IHapticPlayer
    {
        private Transform _transform;

        private bool _enabled = false;

        public MockController(Transform transform)
        {
            _transform = transform;
        }

        public void Start()
        {
            if (_enabled)
            {
                return;
            }
            _enabled = true;
        }

        public void FixedUpdate()
        {
            if (!_enabled)
            {
                return;
            }
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector3 move = new Vector3(h, v, 0);
            _transform.position += 0.3f * Time.fixedDeltaTime * move;
        }

        public void OnDestroy()
        {
            if (!_enabled)
            {
                return;
            }
            _enabled = false;
        }

        public void PlayValue(float intensity) { }

        public void Stop() { }

    }
}