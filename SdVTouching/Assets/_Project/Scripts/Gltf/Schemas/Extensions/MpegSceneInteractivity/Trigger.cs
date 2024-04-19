using System;
using UnityEngine;

namespace SdVTouching.Gltf
{
    public interface ITrigger
    {
        bool IsTriggered();
        event Func<bool> OnUpdate;
        void Update();
    }
    public class CollisionTrigger : ITrigger
    {
        public int[] Nodes { get; set; }
        public ICollisionPrimitive[] Primitives { get; set; }
        private bool _isTriggered = false;
        public bool IsTriggered()
        {
            return _isTriggered;
        }
        public event Func<bool> OnUpdate;
        public void Update()
        {
            if (OnUpdate != null)
            {
                bool whenAny = false;
                foreach (var update in OnUpdate.GetInvocationList())
                {
                    whenAny |= (bool)update.DynamicInvoke();
                }
                _isTriggered = whenAny;
            }
        }
        public interface ICollisionPrimitive { }
        public class BvSpheroid : ICollisionPrimitive
        {
            public float Radius { get; set; } = 1.0f;
            public Vector3 Centroid { get; set; } = Vector3.zero;
        }
        public class BvCuboid : ICollisionPrimitive
        {
            public float Width { get; set; } = 1.0f;
            public float Height { get; set; } = 1.0f;
            public float Depth { get; set; } = 1.0f;
            public Vector3 Centroid { get; set; } = Vector3.zero;
        }
    }
}