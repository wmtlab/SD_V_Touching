using System;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;

namespace SdVTouching.Gltf
{
    public static class HmpgHapticCodec
    {
        private static bool _encoding = false;
        public static async UniTaskVoid EncodeAsync(float[] signals, Action<byte[]> onEncoded)
        {
            if (_encoding)
            {
                return;
            }
            await UniTask.SwitchToThreadPool();
            var hmpg = Encode(signals);
            await UniTask.SwitchToMainThread();
            onEncoded?.Invoke(hmpg);
        }
        private static byte[] Encode(float[] signals)
        {
            if (_encoding)
            {
                return default;
            }
            _encoding = true;

            IntPtr output = IntPtr.Zero;
            int outputSize = 0;
            int result = Encode(signals, signals.Length, ref output, ref outputSize);
            byte[] hmpg = new byte[outputSize];
            Marshal.Copy(output, hmpg, 0, outputSize);
            Marshal.FreeHGlobal(output);

            _encoding = false;
            return hmpg;
        }

        private static bool _decoding = false;
        public static async UniTaskVoid DecodeAsync(byte[] hmpg, Action<float[]> onDecoded)
        {
            if (_encoding)
            {
                return;
            }
            await UniTask.SwitchToThreadPool();
            var signals = Decode(hmpg);
            await UniTask.SwitchToMainThread();
            onDecoded?.Invoke(signals);
        }
        private static float[] Decode(byte[] hmpg)
        {
            if (_decoding)
            {
                return default;
            }
            _decoding = true;

            IntPtr output = IntPtr.Zero;
            int outputSize = 0;
            int result = Decode(hmpg, hmpg.Length, ref output, ref outputSize);
            float[] signals = new float[outputSize];
            Marshal.Copy(output, signals, 0, outputSize);
            Marshal.FreeHGlobal(output);

            _decoding = false;
            return signals;
        }

        [DllImport("MpegHapticCodec")]
        private static extern int Encode(float[] input, int inputSize, ref IntPtr output, ref int outputSize);

        [DllImport("MpegHapticCodec")]
        private static extern int Decode(byte[] input, int inputSize, ref IntPtr output, ref int outputSize);

    }
}