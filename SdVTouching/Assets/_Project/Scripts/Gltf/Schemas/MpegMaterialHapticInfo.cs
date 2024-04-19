using UnityEngine;

namespace SdVTouching.Gltf
{
    public class MpegMaterialHapticInfo : MonoBehaviour
    {
        public int mpegMaterialHaptic;
        public bool Initialized { get; private set; }
        public static readonly Vector3 MeshColliderOffset = new Vector3(0f, 1f, 0f);
        private static int _meshLayerMask;
        public void Initialize()
        {
            if (Initialized)
            {
                return;
            }
            var meshColliderGo = new GameObject("MeshCollider")
            {
                layer = LayerMask.NameToLayer("TactileObjectMesh")
            };
            _meshLayerMask = 1 << meshColliderGo.layer;
            var meshColliderTransform = meshColliderGo.transform;
            meshColliderTransform.SetParent(transform);
            meshColliderTransform.position = transform.position + MeshColliderOffset;
            meshColliderTransform.localRotation = Quaternion.identity;
            meshColliderTransform.localScale = Vector3.one;
            var meshCollider = meshColliderTransform.gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = transform.GetComponent<MeshFilter>().sharedMesh;
            Initialized = true;
        }

        public bool TryRaycast(Vector3 position, Vector3 normal, out Vector2 uv)
        {
            uv = Vector2.zero;
            float rayLength = 1f;
            Vector3 origin = position - normal * rayLength + MeshColliderOffset;
            // Debug.DrawRay(origin, normal * rayLength, Color.red);
            // Debug.DrawRay(origin - MeshColliderOffset, normal * rayLength, Color.blue);
            if (Physics.Raycast(origin, normal, out var hit, rayLength, _meshLayerMask))
            {
                uv = hit.textureCoord;
                return true;
            }
            return false;
        }
    }
}

