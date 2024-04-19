using UnityEngine;

namespace SdVTouching.Gltf
{
    public class CollisionTriggerAdapter : MonoBehaviour
    {
        private bool _isTriggered = false;
        public bool IsTriggered() => _isTriggered;
        public Collider CollidedWith { get; private set; }

        private void OnTriggerEnter(Collider other)
        {
            _isTriggered = true;
            CollidedWith = other;
        }

        private void OnTriggerExit(Collider other)
        {
            _isTriggered = false;
            CollidedWith = null;
        }
    }
}