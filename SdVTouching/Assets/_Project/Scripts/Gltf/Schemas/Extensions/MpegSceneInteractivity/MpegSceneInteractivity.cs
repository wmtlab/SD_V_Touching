using System.Collections.Generic;

namespace SdVTouching.Gltf
{
    public class MpegSceneInteractivity
    {
        public ITrigger[] Triggers { get; set; }
        public IAction[] Actions { get; set; }
        public Behavior[] Behaviors { get; set; }
        public Dictionary<int, CollisionTriggerAdapter[]> CollisionTriggerAdapters { get; set; }
    }
}