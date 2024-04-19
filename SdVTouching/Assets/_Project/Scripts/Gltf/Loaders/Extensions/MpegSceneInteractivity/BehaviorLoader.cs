using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SdVTouching.Gltf
{
    public class BehaviorsLoader
    {
        public async UniTask LoadAsync(MpegSceneInteractivity interactivity, JToken behaviorsJson)
        {
            int behaviorCount = behaviorsJson.Count();
            interactivity.Behaviors = new Behavior[behaviorCount];
            for (int i = 0; i < behaviorCount; i++)
            {
                var behaviorJson = behaviorsJson[i];
                Behavior behavior = new Behavior
                {
                    TriggersCombinationControl = behaviorJson["triggersCombinationControl"].Value<string>(),
                    TriggersActivationControl = behaviorJson["triggersActivationControl"].Value<string>() switch
                    {
                        "TRIGGER_ACTIVATE_FIRST_ENTER" => Behavior.TriggersActivationControlType.FirstEnter,
                        "TRIGGER_ACTIVATE_EACH_ENTER" => Behavior.TriggersActivationControlType.EachEnter,
                        "TRIGGER_ACTIVATE_ON" => Behavior.TriggersActivationControlType.On,
                        "TRIGGER_ACTIVATE_FIRST_EXIT" => Behavior.TriggersActivationControlType.FirstExit,
                        "TRIGGER_ACTIVATE_EACH_EXIT" => Behavior.TriggersActivationControlType.EachExit,
                        "TRIGGER_ACTIVATE_OFF" => Behavior.TriggersActivationControlType.Off,
                        _ => Behavior.TriggersActivationControlType.None
                    },
                    ActionsControl = behaviorJson["actionsControl"].Value<string>() switch
                    {
                        "ACTION_ACTIVATE_SEQUENTIAL" => Behavior.ActionsControlType.Sequential,
                        "ACTION_ACTIVATE_PARALLEL" => Behavior.ActionsControlType.Parallel,
                        _ => Behavior.ActionsControlType.Sequential
                    },
                    Priority = behaviorJson["priority"].Value<int>()
                };
                int triggersCount = behaviorJson["triggers"].Count();
                behavior.Triggers = new int[triggersCount];
                for (int j = 0; j < triggersCount; j++)
                {
                    behavior.Triggers[j] = behaviorJson["triggers"][j].Value<int>();
                }
                int actionsCount = behaviorJson["actions"].Count();
                behavior.Actions = new int[actionsCount];
                for (int j = 0; j < actionsCount; j++)
                {
                    behavior.Actions[j] = behaviorJson["actions"][j].Value<int>();
                }
                interactivity.Behaviors[i] = behavior;
            }
            await UniTask.Yield();
        }
    }
}
