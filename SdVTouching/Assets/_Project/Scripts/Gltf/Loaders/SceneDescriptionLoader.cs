using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using GLTFast;
using GLTFast.Addons;
using GltfImport = GLTFast.Newtonsoft.GltfImport;

namespace SdVTouching.Gltf
{
    public class SceneDescriptionLoader
    {
        public string UrlRoot { get; set; }
        public string SceneDescriptionUrl { get; set; }
        public string ExtensionsUrl { get; set; }
        public string FullSceneDescriptionUrl => Path.Combine(UrlRoot, SceneDescriptionUrl);
        public string FullExtensionsUrl => Path.Combine(UrlRoot, ExtensionsUrl);
        public Transform Parent;
        private Dictionary<int, GameObject> _nodeMapping = new Dictionary<int, GameObject>();

        public async Task<SceneDescription> LoadAsync()
        {
            if (!string.IsNullOrEmpty(SceneDescriptionUrl))
            {
                ImportAddonRegistry.RegisterImportAddon(new MpegMaterialHapticInfoAddon());
                var gltfImport = new GltfImport();
                await gltfImport.Load(FullSceneDescriptionUrl);

                var instantiator = new GameObjectInstantiator(gltfImport, Parent);
                instantiator.NodeCreated += AddNodeMapping;
                instantiator.EndSceneCompleted += () =>
                {
                    instantiator.NodeCreated -= AddNodeMapping;
                };
                await gltfImport.InstantiateMainSceneAsync(instantiator);

                var sd = new SceneDescription()
                {
                    NodeMapping = _nodeMapping,
                    GltfRoot = gltfImport.GetSourceRoot()
                };

                var extensionsLoader = new ExtensionsLoader
                {
                    UrlRoot = UrlRoot,
                    Url = ExtensionsUrl
                };
                var extensions = await extensionsLoader.LoadAsync(sd);

                sd.Extensions = extensions;
                return sd;
            }
            return null;
        }

        private void AddNodeMapping(uint nodeIndex, GameObject gameObject)
        {
            _nodeMapping[(int)nodeIndex] = gameObject;
        }

    }
}
