namespace SdVTouching.Gltf
{
    public class MpegSceneInteractivityController
    {
        public MpegSceneInteractivity CurrentInteractivity { get; set; }

        public void Update()
        {
            UpdateTriggers();
            UpdateBehaviors();
        }

        private void UpdateTriggers()
        {
            foreach (var trigger in CurrentInteractivity.Triggers)
            {
                trigger.Update();
            }
        }

        private void UpdateBehaviors()
        {
            foreach (var behavior in CurrentInteractivity.Behaviors)
            {
                behavior.TryTrigger(CurrentInteractivity);
            }
        }
    }
}