using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SdVTouching.Gltf
{
    public class CollisionTriggerAdapterLoader
    {
        public async UniTask LoadAsync(SceneDescription sd, MpegSceneInteractivity interactivity)
        {
            interactivity.CollisionTriggerAdapters = new Dictionary<int, CollisionTriggerAdapter[]>();
            for (int j = 0; j < interactivity.Triggers.Length; j++)
            {
                if (!(interactivity.Triggers[j] is CollisionTrigger collisionTrigger))
                {
                    continue;
                }
                int minCount = Mathf.Min(collisionTrigger.Nodes.Length, collisionTrigger.Primitives.Length);
                CollisionTriggerAdapter[] adapters = new CollisionTriggerAdapter[minCount];
                for (int k = 0; k < minCount; k++)
                {
                    int nodeIndex = collisionTrigger.Nodes[k];
                    var go = sd.NodeMapping[nodeIndex];

                    var rb = go.AddComponent<Rigidbody>();
                    rb.isKinematic = true;
                    rb.useGravity = false;

                    var primitive = collisionTrigger.Primitives[k];
                    switch (primitive)
                    {
                        case CollisionTrigger.BvSpheroid spheroid:
                            var sphereCollider = go.AddComponent<SphereCollider>();
                            sphereCollider.isTrigger = true;
                            sphereCollider.radius = spheroid.Radius;
                            sphereCollider.center = spheroid.Centroid;
                            break;
                        case CollisionTrigger.BvCuboid cuboid:
                            var boxCollider = go.AddComponent<BoxCollider>();
                            boxCollider.isTrigger = true;
                            boxCollider.size = new Vector3(cuboid.Width, cuboid.Height, cuboid.Depth);
                            boxCollider.center = cuboid.Centroid;
                            break;
                    }

                    var adapter = go.AddComponent<CollisionTriggerAdapter>();
                    collisionTrigger.OnUpdate += adapter.IsTriggered;
                    adapters[k] = adapter;
                }
                interactivity.CollisionTriggerAdapters[j] = adapters;
            }

            await UniTask.Yield();
        }
    }
}