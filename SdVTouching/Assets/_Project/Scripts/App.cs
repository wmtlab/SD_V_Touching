using System.IO;
using Cysharp.Threading.Tasks;
using SdVTouching.Gltf;
using UnityEngine;

namespace SdVTouching
{
    public class App : MonoBehaviour
    {
        [SerializeField]
        private string _urlRoot;
        [SerializeField]
        private string _sceneDescriptionName;
        [SerializeField]
        private string _extensionsName;
        [SerializeField]
        private bool _useMock = false;
        [SerializeField, Range(0f, 20f), Header("[Modify it before play.]")]
        private float _vibrationMagnification = 1f;

        [SerializeField]
        private bool _useCodec = true;
        [SerializeField, Header("[If useCodec is false, this value will be set to 1.]")]
        private int _tactileSignalCacheSize = 16;
        [SerializeField]
        private int _waveformUpdateDump = 1;
        private SceneDescription _sd;

        private MpegSceneInteractivityController _interactivityController;
        private Channel<byte[]> _hapticChannel;
        private HapticController _hapticController;
        private HapticDriver _hapticDriver;

        private IPlayerController _playerController;
        private Transform _playerTransform;
        private CollisionTriggerAdapter _playerCollisionTriggerAdapter;
        private WaveformController _waveformController;
        private bool _initialized = false;

        private async void Start()
        {
            if (!_useCodec)
            {
                _tactileSignalCacheSize = 1;
            }
            var sdLoader = new SceneDescriptionLoader
            {
                UrlRoot = Path.Combine(Application.streamingAssetsPath, _urlRoot),
                SceneDescriptionUrl = _sceneDescriptionName,
                ExtensionsUrl = _extensionsName,
                Parent = transform
            };
            _sd = await sdLoader.LoadAsync();

            _playerTransform = GameObject.Find("player").transform;

            IHapticPlayer hapticPlayer;
            if (_useMock)
            {
                var device = new MockController(_playerTransform);
                _playerController = device;
                hapticPlayer = device;
                device.Start();
            }
            else
            {
                var device = new GeomagicTouchController(_playerTransform);
                _playerController = device;
                hapticPlayer = device;
                device.Start();
            }

            _interactivityController = new MpegSceneInteractivityController
            {
                CurrentInteractivity = _sd.Extensions.MpegSceneInteractivities[_sd.GltfRoot.scene]
            };
            _hapticChannel = Channel.CreateSingleConsumerUnbounded<byte[]>();
            _hapticController = new HapticController(_sd, _hapticChannel.Writer, _useCodec, _tactileSignalCacheSize);
            _hapticDriver = new HapticDriver(hapticPlayer, _hapticChannel.Reader, _vibrationMagnification, _useCodec);

            _playerCollisionTriggerAdapter = _playerTransform.GetComponent<CollisionTriggerAdapter>();
            _waveformController = new WaveformController(FindObjectOfType<Canvas>().transform, 0f, 1f, _waveformUpdateDump);

            _initialized = true;
        }

        void FixedUpdate()
        {
            if (!_initialized)
            {
                return;
            }
            // Do something with the loaded GLTF

            _playerController?.FixedUpdate();
            _interactivityController?.Update();
            _hapticDriver?.Update();
            var curSignal = _hapticDriver?.CurSignal ?? 0f;
            _waveformController?.Update(curSignal);
        }

        void OnDestroy()
        {
            _playerController?.OnDestroy();
            _hapticChannel.Writer.TryComplete();
        }

        void OnDrawGizmos()
        {
            if (_playerTransform)
            {
                if (_playerCollisionTriggerAdapter.IsTriggered())
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.red;
                }
                Gizmos.DrawSphere(_playerTransform.position, _playerTransform.lossyScale.x / 2f);
            }
        }
    }
}