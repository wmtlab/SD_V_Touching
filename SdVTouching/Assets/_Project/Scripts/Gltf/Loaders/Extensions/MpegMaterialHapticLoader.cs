using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using GLTFast.Newtonsoft.Schema;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace SdVTouching
{
    public class MpegMaterialHapticLoader
    {
        public string UrlRoot { get; set; }
        public async UniTask LoadAsync(SceneDescription sd, Extensions extensions, JToken mpegMaterialHapticJson)
        {
            int mpegMaterialHapticCount = mpegMaterialHapticJson.Count();
            extensions.mpegMaterialHaptic = new MpegMaterialHaptic[mpegMaterialHapticCount];
            extensions.hapticTextures = new Dictionary<uint, Texture2D>();
            HashSet<int> textureIndices = new HashSet<int>();
            for (int i = 0; i < mpegMaterialHapticCount; i++)
            {
                var materialJson = mpegMaterialHapticJson[i];
                MpegMaterialHaptic mpegMaterialHaptic = new MpegMaterialHaptic
                {
                    name = materialJson["name"].Value<string>(),
                };
                var vibrationJson = materialJson["vibration"];
                int vibrationCount = vibrationJson.Count();
                mpegMaterialHaptic.vibration = new MpegMaterialHaptic.Vibration[vibrationCount];
                for (int j = 0; j < vibrationCount; j++)
                {
                    var vibJson = vibrationJson[j];
                    MpegMaterialHaptic.Vibration vibration = new MpegMaterialHaptic.Vibration
                    {
                        type = vibJson["type"].Value<string>(),
                    };
                    var textureJson = vibJson["texture"];
                    TextureInfo textureInfo = new TextureInfo
                    {
                        index = textureJson["index"].Value<int>(),
                    };
                    textureIndices.Add(textureInfo.index);
                    vibration.texture = textureInfo;
                    mpegMaterialHaptic.vibration[j] = vibration;
                }
                extensions.mpegMaterialHaptic[i] = mpegMaterialHaptic;
            }

            List<UniTask> tasks = new List<UniTask>();
            foreach (int index in textureIndices)
            {
                var uri = sd.GltfRoot.images[sd.GltfRoot.textures[index].source].uri;
                var task = LoadHapticMaterialTextureAsync(extensions, index, uri);
                tasks.Add(task);
            }

            await UniTask.WhenAll(tasks);
        }

        private async UniTask LoadHapticMaterialTextureAsync(Extensions extensions, int index, string uri)
        {
            var textureReq = UnityWebRequestTexture.GetTexture(Path.Combine(UrlRoot, uri));
            await textureReq.SendWebRequest();
            Texture2D texture = DownloadHandlerTexture.GetContent(textureReq);
            extensions.hapticTextures.Add((uint)index, texture);
        }
    }
}