using System.IO;
using SdVTouching.Gltf;
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
        [SerializeField]
        private int _waveformUpdateDump = 1;
        private SceneDescription _sd;
        private IPlayerController _playerController;
        private Transform _playerTransform;
        private WaveformController _waveformController;
        private bool _initialized = false;

        private async void Start()
        {
            var sdLoader = new SceneDescriptionLoader
            {
                UrlRoot = Path.Combine(Application.streamingAssetsPath, _urlRoot),
                SceneDescriptionUrl = _sceneDescriptionUrl,
                ExtensionsUrl = _extensionsUrl,
                Parent = transform
            };
            _sd = await sdLoader.LoadAsync();

            _playerTransform = GameObject.Find("player").transform;
            var device = new MockController(_playerTransform);
            _playerController = device;
            device.Start();

            _waveformController = new WaveformController(FindObjectOfType<Canvas>().transform, 0f, 1f, _waveformUpdateDump);

            _initialized = true;
        }

        // TODO: interactivity controller
        // TODO: haptic controller & haptic driver
        void FixedUpdate()
        {
            if (!_initialized)
            {
                return;
            }
            // Do something with the loaded GLTF

            _playerController?.FixedUpdate();
            // var curSignal = _hapticDriver?.CurSignal ?? 0f;
            // _waveformController?.Update(curSignal);
        }

        void OnDrawGizmos()
        {
            if (_playerTransform)
            {
                //if (_playerCollisionTriggerAdapter.IsTriggered())
                //{
                //    Gizmos.color = Color.green;
                //}
                //else
                //{
                Gizmos.color = Color.red;
                //}
                Gizmos.DrawSphere(_playerTransform.position, _playerTransform.lossyScale.x / 2f);
            }
        }
    }
}