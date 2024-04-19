using System.Threading.Tasks;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;

namespace SdVTouching
{
    public class ExtensionsLoader
    {
        public string UrlRoot { get; set; }
        public string Url { get; set; }
        public string FullUrl => Path.Combine(UrlRoot, Url);

        public async Task<Extensions> LoadAsync(SceneDescription sd)
        {
            Extensions extensions = new Extensions();
            var extensionsReq = UnityWebRequest.Get(FullUrl);
            await extensionsReq.SendWebRequest();
            string text = extensionsReq.downloadHandler.text;
            JObject jObject = JObject.Parse(text);
            var extensionsJson = jObject["extensions"];

            var mpegMaterialHapticJson = extensionsJson["MPEG_material_haptic"];
            var mpegMaterialHapticLoader = new MpegMaterialHapticLoader
            {
                UrlRoot = UrlRoot
            };
            await mpegMaterialHapticLoader.LoadAsync(sd, extensions, mpegMaterialHapticJson);

            var mpegSceneInteractivityJson = extensionsJson["MPEG_scene_interactivity"];
            var mpegSceneInteractivityLoader = new MpegSceneInteractivityLoader();
            await mpegSceneInteractivityLoader.LoadAsync(sd, extensions, mpegSceneInteractivityJson);

            return extensions;
        }


    }
}