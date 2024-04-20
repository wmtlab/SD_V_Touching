using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SdVTouching.Gltf
{
    public class HapticController
    {
        private SceneDescription _sd;
        private ChannelWriter<byte[]> _hapticWriter;
        private bool _useCodec;
        private float[] _signals;
        private int _cacheIndex = 0;

        public HapticController(SceneDescription sd, ChannelWriter<byte[]> hapticWriter, bool useCodec, int cacheSize = 1)
        {
            _sd = sd;
            _hapticWriter = hapticWriter;
            _useCodec = useCodec;
            cacheSize = Math.Max(1, cacheSize);
            _signals = new float[cacheSize];
            foreach (var interactivity in _sd.Extensions.MpegSceneInteractivities)
            {
                foreach (var action in interactivity.Actions)
                {
                    if (action is SetHapticAction setHapticAction)
                    {
                        setHapticAction.ActionHandler += OnSetHaptic;
                    }
                }
            }
        }

        public void OnSetHaptic(SetHapticAction actionData)
        {
            foreach (var actionNode in actionData.HapticActionNodes)
            {
                if (!actionNode.UseCollider)
                {
                    continue;
                }
                if (_sd.NodeMapping.TryGetValue(actionNode.Node, out var go))
                {
                    var adapter = go.GetComponent<CollisionTriggerAdapter>();
                    if (adapter && adapter.IsTriggered())
                    {
                        var collidedWith = adapter.CollidedWith;
                        if (!collidedWith)
                        {
                            continue;
                        }
                        var materialHapticAdapter = collidedWith.GetComponent<MpegMaterialHapticInfo>();
                        if (!materialHapticAdapter)
                        {
                            continue;
                        }
                        if (!materialHapticAdapter.Initialized)
                        {
                            materialHapticAdapter.Initialize();
                        }
                        if (materialHapticAdapter.mpegMaterialHaptic >= _sd.Extensions.MpegMaterialHaptics.Length)
                        {
                            continue;
                        }
                        var materialHaptic = _sd.Extensions.MpegMaterialHaptics[materialHapticAdapter.mpegMaterialHaptic];
                        var textureIndex = materialHaptic.Vibrations[0].Texture.index;
                        if (_sd.Extensions.HapticTextures.TryGetValue(textureIndex, out var texture))
                        {
                            var position = go.transform.position;
                            var normal = go.transform.up;
                            if (materialHapticAdapter.TryRaycast(position, normal, out var uv))
                            {
                                float signal = texture.GetPixelBilinear(uv.x, uv.y).r;
                                _signals[_cacheIndex] = signal * Mathf.Sign(_cacheIndex);
                                _cacheIndex++;
                                if (_cacheIndex >= _signals.Length)
                                {
                                    _cacheIndex = 0;
                                    if (_useCodec)
                                    {
                                        HmpgHapticCodec.EncodeAsync(_signals, hmpg =>
                                        {
                                            _hapticWriter.TryWrite(hmpg);
                                        }).Forget();
                                    }
                                    else
                                    {
                                        _hapticWriter.TryWrite(_signals.SelectMany(BitConverter.GetBytes).ToArray());
                                    }
                                }
                            }

                        }
                    }
                }
            }

        }


    }
}