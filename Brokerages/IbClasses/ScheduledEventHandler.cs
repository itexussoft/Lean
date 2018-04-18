namespace QuantConnect.Brokerages.IbClasses
{ 
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;
    using Utils.Common;
    using Timer = System.Timers.Timer;

    public class ScheduledEventHandler
    {
        private Timer scheduleTimer;
        private Thread timerThread;
        private readonly Dictionary<TimeSpan, List<ScheduledAction>> scheduledEvents = new Dictionary<TimeSpan, List<ScheduledAction>>();

        public void Initialize()
        {
            timerThread = new Thread(this.Run) {IsBackground = true, Name = "Timer thread"};
            timerThread.Start();
        }

        public void AddScheduledEventAsync(TimeSpan time, Action scheduledAction)
        {
            this.AddScheduledEvent(time, new ScheduledAction(scheduledAction, true));
        }

        public void AddScheduledEvent(TimeSpan time, Action scheduledAction)
        {
            this.AddScheduledEvent(time, new ScheduledAction(scheduledAction, false));
        }

        private void AddScheduledEvent(TimeSpan time, ScheduledAction scheduledAction)
        {
            if (scheduledEvents.ContainsKey(time))
            {
                scheduledEvents[time].Add(scheduledAction);
            }
            else
            {
                scheduledEvents.Add(time, new List<ScheduledAction> { scheduledAction });
            }
        }

        private void TimerEvent(object source, ElapsedEventArgs e)
        {
            scheduleTimer.Interval = 1000 - DateTime.Now.Millisecond;
            scheduleTimer.Start();

            var estNow = DateTimeHelper.EstNow().TimeOfDay;

            foreach (var scheduledEvent in scheduledEvents)
            {
                if (estNow.Hours == scheduledEvent.Key.Hours &&
                    estNow.Minutes == scheduledEvent.Key.Minutes &&
                    estNow.Seconds == scheduledEvent.Key.Seconds)
                {
                    foreach (var scheduledAction in scheduledEvent.Value)
                    {
                        if (scheduledAction.IsTask)
                        {
                            Task.Run(() => { scheduledAction.Action(); });
                        }
                        else
                        {
                            scheduledAction.Action();
                        }
                    }
                }
            }
        }

        private void Run()
        {
            var now = DateTime.Now;

            scheduleTimer = new Timer();
            scheduleTimer.Elapsed += this.TimerEvent;
            scheduleTimer.Interval = 1000;

            var sleep = 1000 - now.Millisecond;

            Thread.Sleep(sleep);

            scheduleTimer.Enabled = true;
        }
    }

    public class ScheduledAction
    {
        public ScheduledAction(Action action): this(action, false)
        {
        }

        public ScheduledAction(Action action, bool isTask)
        {
            this.Action = action;
            this.IsTask = isTask;
        }

        public Action Action { get; set; }
        public bool IsTask { get; set; }
    }
}
