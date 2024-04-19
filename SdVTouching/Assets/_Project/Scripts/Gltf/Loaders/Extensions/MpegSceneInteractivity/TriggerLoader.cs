using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace SdVTouching.Gltf
{
    public class TriggerLoader
    {
        public async UniTask LoadAsync(MpegSceneInteractivity interactivity, JToken triggersJson)
        {
            int triggerCount = triggersJson.Count();
            interactivity.Triggers = new ITrigger[triggerCount];
            for (int i = 0; i < triggerCount; i++)
            {
                var triggerJson = triggersJson[i];
                ITrigger trigger = TriggerFactory(interactivity, triggerJson);
                interactivity.Triggers[i] = trigger;
            }
            await UniTask.Yield();
        }

        private static ITrigger TriggerFactory(MpegSceneInteractivity interactivity, JToken triggerJson)
        {
            string type = triggerJson["type"].Value<string>();
            ITrigger trigger = null;
            switch (type)
            {
                case "TRIGGER_COLLISION":
                    trigger = CollisionTriggerFactory(interactivity, triggerJson);
                    break;
            }
            return trigger;
        }

        private static CollisionTrigger CollisionTriggerFactory(MpegSceneInteractivity interactivity, JToken triggerJson)
        {
            CollisionTrigger trigger = new CollisionTrigger();
            JToken nodes = triggerJson["nodes"];
            int nodeCount = nodes.Count();
            trigger.Nodes = new int[nodeCount];
            for (int i = 0; i < nodeCount; i++)
            {
                trigger.Nodes[i] = nodes[i].Value<int>();
            }

            JToken primitives = triggerJson["primitives"];
            int primitiveCount = primitives.Count();
            trigger.Primitives = new CollisionTrigger.ICollisionPrimitive[primitiveCount];
            for (int i = 0; i < primitiveCount; i++)
            {
                JToken primitiveJson = primitives[i];
                string primitiveType = primitiveJson["type"].Value<string>();
                switch (primitiveType)
                {
                    case "BV_SPHEROID":
                        CollisionTrigger.BvSpheroid spheroid = new CollisionTrigger.BvSpheroid
                        {
                            Radius = primitiveJson["radius"].Value<float>(),
                            Centroid = new Vector3
                            {
                                x = primitiveJson["centroid"][0].Value<float>(),
                                y = primitiveJson["centroid"][1].Value<float>(),
                                z = primitiveJson["centroid"][2].Value<float>()
                            }
                        };
                        trigger.Primitives[i] = spheroid;
                        break;
                    case "BV_CUBOID":
                        CollisionTrigger.BvCuboid cuboid = new CollisionTrigger.BvCuboid
                        {
                            Width = primitiveJson["width"].Value<float>(),
                            Height = primitiveJson["height"].Value<float>(),
                            Depth = primitiveJson["depth"].Value<float>(),
                            Centroid = new Vector3
                            {
                                x = primitiveJson["centroid"][0].Value<float>(),
                                y = primitiveJson["centroid"][1].Value<float>(),
                                z = primitiveJson["centroid"][2].Value<float>()
                            }
                        };
                        trigger.Primitives[i] = cuboid;
                        break;
                }
            }

            // TODO: Add Adapters for Collider
            return trigger;
        }

    }
}
