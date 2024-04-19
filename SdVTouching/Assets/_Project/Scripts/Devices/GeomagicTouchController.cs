using System.Runtime.InteropServices;
using UnityEngine;

namespace SdVTouching
{
    public class GeomagicTouchController : IHapticPlayer, IPlayerController
    {
        private Transform _transform;
        private string _deviceIdentifier = "Default Device";
        private double[] _vibrationGDir = new double[3] { 0.0, 0.0, 1.0 };
        private int _vibrationGFrequency = 100;
        private float _scaleFactor = 0.001f;

        private bool _enabled = false;

        public GeomagicTouchController(Transform transform)
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
            initDevice(_deviceIdentifier);
            startSchedulers();
        }

        private readonly static Quaternion _rotationLeftCache = Quaternion.Inverse(Quaternion.Euler(0, 180, 0));
        private readonly static Quaternion _rotationRightCache = Quaternion.Euler(-90, 180, 0);
        public void FixedUpdate()
        {
            if (!_enabled)
            {
                return;
            }
            if (TryGetRawMatrix(out var rawMatrix))
            {
                var matrix = rawMatrix;
                Vector3 lossyScale = _transform.lossyScale;
                _transform.FromMatrix(matrix);
                _transform.localScale = lossyScale;
                _transform.rotation = _rotationLeftCache * _transform.rotation * _rotationRightCache;
                var position = _transform.position;
                position.x = -position.x;
                position.z = -position.z;
                _transform.position = position;
            }
        }


        public void OnDestroy()
        {
            if (!_enabled)
            {
                return;
            }
            _enabled = false;
            //disconnectAllDevices();
        }

        public void PlayValue(float intensity)
        {
            if (!_enabled)
            {
                return;
            }
            setVibrationValues(_deviceIdentifier, _vibrationGDir, intensity, _vibrationGFrequency, 0.0);
        }

        public void Stop()
        {
            if (!_enabled)
            {
                return;
            }
            setVibrationValues(_deviceIdentifier, _vibrationGDir, 0.0, 0, 0.0);
        }

        private double[] _matInput = new double[16];
        private bool TryGetRawMatrix(out Matrix4x4 rawMatrix)
        {
            getTransform(_deviceIdentifier, _matInput);

            for (int ii = 0; ii < 16; ii++)
                if (ii % 4 != 3)
                    _matInput[ii] *= _scaleFactor;

            Matrix4x4 mat;
            mat.m00 = (float)_matInput[0];
            mat.m01 = (float)_matInput[1];
            mat.m02 = (float)_matInput[2];
            mat.m03 = (float)_matInput[3];
            mat.m10 = (float)_matInput[4];
            mat.m11 = (float)_matInput[5];
            mat.m12 = (float)_matInput[6];
            mat.m13 = (float)_matInput[7];
            mat.m20 = (float)_matInput[8];
            mat.m21 = (float)_matInput[9];
            mat.m22 = (float)_matInput[10];
            mat.m23 = (float)_matInput[11];
            mat.m30 = (float)_matInput[12];
            mat.m31 = (float)_matInput[13];
            mat.m32 = (float)_matInput[14];
            mat.m33 = (float)_matInput[15];
            rawMatrix = mat.transpose;
            return true;
        }

        [DllImport("HapticsDirect")] public static extern int initDevice(string deviceName);
        [DllImport("HapticsDirect")] public static extern void startSchedulers();
        [DllImport("HapticsDirect")] public static extern void setVibrationValues(string configName, double[] direction3, double magnitude, double frequency, double time);
        [DllImport("HapticsDirect")] public static extern void disconnectAllDevices();

        [DllImport("HapticsDirect")] public static extern void getTransform(string configName, double[] matrix16);

    }
}