using System.IO;
using UnityEngine;

namespace SdVTouching
{
    public class App : MonoBehaviour
    {
        [SerializeField]
        private string _urlRoot;
        [SerializeField]
        private string _sceneDescriptionUrl;
        [SerializeField]
        private string _extensionsUrl;
        private SceneDescription _sd;

        async void Start()
        {
            var sdLoader = new SceneDescriptionLoader
            {
                UrlRoot = Path.Combine(Application.streamingAssetsPath, _urlRoot),
                SceneDescriptionUrl = _sceneDescriptionUrl,
                ExtensionsUrl = _extensionsUrl,
                Parent = transform
            };
            _sd = await sdLoader.LoadAsync();
        }

        void Update()
        {
            if (_sd == null)
            {
                return;
            }
            // Do something with the loaded GLTF

        }
    }
}