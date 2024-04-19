using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SdVTouching.Gltf
{
    public class MpegSceneInteractivityLoader
    {
        public async UniTask LoadAsync(SceneDescription sd, Extensions extensions, JToken mpegSceneInteractivityJson)
        {
            int mpegSceneInteractivityCount = mpegSceneInteractivityJson.Count();
            extensions.MpegSceneInteractivities = new MpegSceneInteractivity[mpegSceneInteractivityCount];
            for (int i = 0; i < mpegSceneInteractivityCount; i++)
            {
                var interactivityJson = mpegSceneInteractivityJson[i];
                MpegSceneInteractivity interactivity = new MpegSceneInteractivity();

                var triggersJson = interactivityJson["triggers"];
                TriggerLoader triggerLoader = new TriggerLoader();
                await triggerLoader.LoadAsync(interactivity, triggersJson);

                var actionsJson = interactivityJson["actions"];
                ActionLoader actionLoader = new ActionLoader();
                await actionLoader.LoadAsync(interactivity, actionsJson);

                var behaviorsJson = interactivityJson["behaviors"];
                BehaviorsLoader behaviorsLoader = new BehaviorsLoader();
                await behaviorsLoader.LoadAsync(interactivity, behaviorsJson);

                extensions.MpegSceneInteractivities[i] = interactivity;
            }
            await UniTask.Yield();
        }
    }
}