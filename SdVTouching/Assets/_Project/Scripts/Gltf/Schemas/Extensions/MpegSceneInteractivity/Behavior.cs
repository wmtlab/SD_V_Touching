using System;
using System.Collections.Generic;

namespace SdVTouching.Gltf
{
    public class Behavior
    {
        public int[] Triggers { get; set; }
        public int[] Actions { get; set; }

        #region TriggersCombinationControl
        private string _triggersCombinationControl = string.Empty;
        private IExpressionNode _expressionRoot = null;
        private bool _isTriggerMethodsDirty = true;
        public string TriggersCombinationControl
        {
            get => _triggersCombinationControl;
            set
            {
                _triggersCombinationControl = value;
            }
        }
        #endregion

        public TriggersActivationControlType TriggersActivationControl { get; set; } = TriggersActivationControlType.FirstEnter;
        public ActionsControlType ActionsControl { get; set; } = ActionsControlType.Sequential;
        public int Priority { get; set; } = 0;

        private TriggerState _triggerState = TriggerState.FirstOff;
        public void TryTrigger(MpegSceneInteractivity interactivity)
        {
            _triggerState = UpdateTriggerState(interactivity, _triggerState);
            if (IsActivationMatched(TriggersActivationControl, _triggerState))
            {
                foreach (var actionIndex in Actions)
                {
                    var action = interactivity.Actions[actionIndex];
                    action?.Invoke();
                }
            }
        }

        private bool IsActivationMatched(TriggersActivationControlType required, TriggerState state)
        {
            return required switch
            {
                TriggersActivationControlType.None => false,
                TriggersActivationControlType.FirstEnter => state == TriggerState.FirstEnter,
                TriggersActivationControlType.EachEnter => state == TriggerState.Enter || state == TriggerState.FirstEnter,
                TriggersActivationControlType.On => state == TriggerState.On || state == TriggerState.FirstOn,
                TriggersActivationControlType.FirstExit => state == TriggerState.FirstExit,
                TriggersActivationControlType.EachExit => state == TriggerState.Exit || state == TriggerState.FirstExit,
                TriggersActivationControlType.Off => state == TriggerState.Off || state == TriggerState.FirstOff,
                _ => false
            };
        }

        private TriggerState UpdateTriggerState(MpegSceneInteractivity interactivity, TriggerState oldState)
        {
            bool isTriggered = CheckTrigger(interactivity);
            var newState = oldState switch
            {
                TriggerState.FirstOff => isTriggered ? TriggerState.FirstEnter : TriggerState.FirstOff,
                TriggerState.FirstEnter => isTriggered ? TriggerState.FirstOn : TriggerState.FirstExit,
                TriggerState.FirstOn => isTriggered ? TriggerState.FirstOn : TriggerState.FirstExit,
                TriggerState.FirstExit => isTriggered ? TriggerState.Enter : TriggerState.Off,
                TriggerState.Off => isTriggered ? TriggerState.Enter : TriggerState.Off,
                TriggerState.Enter => isTriggered ? TriggerState.On : TriggerState.Exit,
                TriggerState.On => isTriggered ? TriggerState.On : TriggerState.Exit,
                TriggerState.Exit => isTriggered ? TriggerState.Enter : TriggerState.Off,
                _ => TriggerState.FirstOff
            };
            return newState;
        }

        private bool CheckTrigger(MpegSceneInteractivity interactivity)
        {
            if (_isTriggerMethodsDirty)
            {
                var triggerMethods = new Dictionary<int, Func<bool>>();
                foreach (var triggerIndex in Triggers)
                {
                    var trigger = interactivity.Triggers[triggerIndex];
                    triggerMethods[triggerIndex] = trigger.IsTriggered;
                }
                _expressionRoot = IExpressionNode.Factory.Create(TriggersCombinationControl, triggerMethods);
                _isTriggerMethodsDirty = false;
            }

            return _expressionRoot.Evaluate();
        }

        public enum TriggerState
        {
            FirstOff,
            FirstEnter,
            FirstOn,
            FirstExit,
            Off,
            Enter,
            On,
            Exit
        }

        public enum TriggersActivationControlType
        {
            None,
            FirstEnter,
            EachEnter,
            On,
            FirstExit,
            EachExit,
            Off
        }
        public enum ActionsControlType
        {
            Sequential,
            Parallel
        }
    }

}