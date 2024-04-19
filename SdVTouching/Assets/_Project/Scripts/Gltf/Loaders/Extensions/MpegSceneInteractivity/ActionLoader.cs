using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SdVTouching.Gltf
{
    public class ActionLoader
    {
        public async UniTask LoadAsync(MpegSceneInteractivity interactivity, JToken actionsJson)
        {
            int actionCount = actionsJson.Count();
            interactivity.Actions = new IAction[actionCount];
            for (int i = 0; i < actionCount; i++)
            {
                var actionJson = actionsJson[i];
                IAction action = ActionFactory(actionJson);
                interactivity.Actions[i] = action;
            }
            await UniTask.Yield();
        }

        private static IAction ActionFactory(JToken actionJson)
        {
            string type = actionJson["type"].Value<string>();
            IAction action = null;
            switch (type)
            {
                case "ACTION_SET_HAPTIC":
                    action = SetHapticActionFactory(actionJson);
                    break;
            }
            return action;
        }

        private static SetHapticAction SetHapticActionFactory(JToken actionJson)
        {
            SetHapticAction action = new SetHapticAction();
            JToken hapticActionNodes = actionJson["hapticActionNodes"];
            int hapticActionNodesCount = hapticActionNodes.Count();
            action.HapticActionNodes = new SetHapticAction.HapticActionNode[hapticActionNodesCount];
            for (int i = 0; i < hapticActionNodesCount; i++)
            {
                JToken hapticActionNode = hapticActionNodes[i];
                SetHapticAction.HapticActionNode node = new SetHapticAction.HapticActionNode
                {
                    Node = hapticActionNode["node"].Value<int>(),
                    UseCollider = hapticActionNode["useCollider"].Value<bool>(),
                };
                JToken hapticActionMedias = hapticActionNode["hapticActionMedias"];
                int mediaCount = hapticActionMedias.Count();
                node.HapticActionMedias = new SetHapticAction.HapticActionNode.HapticActionMedia[mediaCount];
                for (int j = 0; j < mediaCount; j++)
                {
                    JToken hapticActionMedia = hapticActionMedias[j];
                    SetHapticAction.HapticActionNode.HapticActionMedia media = new SetHapticAction.HapticActionNode.HapticActionMedia
                    {
                        MediaIndex = hapticActionMedia["mediaIndex"].Value<int>(),
                    };
                    JToken perceptionIndices = hapticActionMedia["perceptionIndices"];
                    int perceptionIndicesCount = perceptionIndices.Count();
                    media.PerceptionIndices = new int[perceptionIndicesCount];
                    for (int k = 0; k < perceptionIndicesCount; k++)
                    {
                        JToken perceptionIndex = perceptionIndices[k];

                        media.PerceptionIndices[k] = perceptionIndex.Value<int>();
                    }
                    node.HapticActionMedias[j] = media;
                }
                action.HapticActionNodes[i] = node;
            }
            return action;
        }
    }
}