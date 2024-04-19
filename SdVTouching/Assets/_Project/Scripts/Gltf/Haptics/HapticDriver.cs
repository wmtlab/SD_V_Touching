using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SdVTouching.Gltf
{
    public class HapticDriver
    {
        private IHapticPlayer _hapticPlayer;
        private ChannelReader<byte[]> _hapticReader;
        private float _vibrationMagnification;
        private bool _useCodec;
        private float[] _signals;
        private int _curSignalIndex = -1;

        public float CurSignal { get; private set; }

        public HapticDriver(IHapticPlayer hapticPlayer, ChannelReader<byte[]> hapticReader,
            float vibrationMagnification, bool useCodec)
        {
            _hapticPlayer = hapticPlayer;
            _hapticReader = hapticReader;
            _vibrationMagnification = vibrationMagnification;
            _useCodec = useCodec;
        }

        public void Update()
        {
            if (_hapticReader.TryRead(out var haptic))
            {
                if (_useCodec)
                {
                    HmpgHapticCodec.DecodeAsync(haptic, signals =>
                    {
                        _signals = signals;
                        _curSignalIndex = 0;
                    }).Forget();
                }
                else
                {
                    _signals = new float[haptic.Length / sizeof(float)];
                    Buffer.BlockCopy(haptic, 0, _signals, 0, haptic.Length);
                    _curSignalIndex = 0;
                }
            }

            if (_curSignalIndex >= 0)
            {
                if (_curSignalIndex < _signals.Length)
                {
                    CurSignal = Mathf.Clamp01(Mathf.Abs(_signals[_curSignalIndex] * _vibrationMagnification));
                    _hapticPlayer.PlayValue(CurSignal);
                    _curSignalIndex++;
                }
                else
                {
                    _curSignalIndex = -1;
                    CurSignal = 0f;
                }
            }
        }
    }
}