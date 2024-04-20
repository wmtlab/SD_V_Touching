using GLTFast;
using GLTFast.Addons;
using UnityEngine;
using GltfImport = GLTFast.Newtonsoft.GltfImport;

namespace SdVTouching.Gltf
{
    public class MpegMaterialHapticInfoAddon : ImportAddon<MpegMaterialHapticInfoAddonInstance> { }
    public class MpegMaterialHapticInfoAddonInstance : ImportAddonInstance
    {
        GltfImport m_GltfImport;

        public override void Dispose() { }

        public override void Inject(GltfImportBase gltfImport)
        {
            var newtonsoftGltfImport = gltfImport as GltfImport;
            if (newtonsoftGltfImport == null)
                return;

            m_GltfImport = newtonsoftGltfImport;
            newtonsoftGltfImport.AddImportAddonInstance(this);
        }

        public override void Inject(IInstantiator instantiator)
        {
            var goInstantiator = instantiator as GameObjectInstantiator;
            if (goInstantiator == null)
                return;
            var _ = new MpegMaterialHapticInfoInstantiatorAddon(m_GltfImport, goInstantiator);
        }

        public override bool SupportsGltfExtension(string extensionName)
        {
            return false;
        }

        class MpegMaterialHapticInfoInstantiatorAddon
        {
            GltfImport m_GltfImport;
            GameObjectInstantiator m_Instantiator;

            public MpegMaterialHapticInfoInstantiatorAddon(GltfImport gltfImport, GameObjectInstantiator instantiator)
            {
                m_GltfImport = gltfImport;
                m_Instantiator = instantiator;
                m_Instantiator.NodeCreated += OnNodeCreated;
                m_Instantiator.EndSceneCompleted += () =>
                {
                    m_Instantiator.NodeCreated -= OnNodeCreated;
                };
            }

            void OnNodeCreated(uint nodeIndex, GameObject gameObject)
            {
                // De-serialize glTF JSON
                var gltf = m_GltfImport.GetSourceRoot();

                var node = gltf.Nodes[(int)nodeIndex] as GLTFast.Newtonsoft.Schema.Node;
                if (node.mesh < 0)
                {
                    return;
                }
                var mesh = gltf.Meshes[node.mesh] as GLTFast.Newtonsoft.Schema.Mesh;
                var extensions = mesh.extensions;
                if (extensions == null)
                    return;

                // Access values in the extras property
                if (extensions.TryGetValue("MPEG_material_haptic", out int extraValue))
                {
                    var component = gameObject.AddComponent<MpegMaterialHapticInfo>();
                    component.mpegMaterialHaptic = extraValue;
                }
            }
        }
    }
}