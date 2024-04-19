using System;

namespace SdVTouching.Gltf
{
    public interface IAction
    {
        void Invoke();
    }
    public interface IAction<T> : IAction where T : IAction<T>
    {
        event Action<T> ActionHandler;
    }
    public class SetHapticAction : IAction<SetHapticAction>
    {
        public HapticActionNode[] HapticActionNodes { get; set; }
        public event Action<SetHapticAction> ActionHandler;
        public void Invoke()
        {
            ActionHandler?.Invoke(this);
        }
        public class HapticActionNode
        {
            public int Node { get; set; } = -1;
            public bool UseCollider { get; set; } = false;
            public HapticActionMedia[] HapticActionMedias { get; set; }
            public class HapticActionMedia
            {
                public int MediaIndex { get; set; } = -1;
                public int[] PerceptionIndices { get; set; }
            }
        }
    }
}