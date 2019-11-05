using System;

namespace TheTimer.Timer
{
    public enum TimerState
    {
        Stopped = 0,
        Paused,
        Running,
        Complete,
    }

    public static class Aux_TimerState
    {
        public static string Description(this TimerState state)
        {
            switch (state)
            {
                case TimerState.Stopped: return "Stop the timer, prepare for new start";
                case TimerState.Paused: return "Pause the timer until manual resume by \"Play\" button press";
                case TimerState.Running: return "Start or resume timer";
                case TimerState.Complete: return string.Empty;
                default:
                    throw new NotImplementedException($"{typeof(TimerState).FullName}.{state}");
            }
        }
    }
}
