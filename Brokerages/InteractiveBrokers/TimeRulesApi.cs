namespace QuantConnect.Brokerages.InteractiveBrokers
{
    using System;

    public class TimeRulesApi
    {
        public TimeSpan BeforePreOpen
        {
            get; set;
        }

        public TimeSpan PreOpen
        {
            get; set;
        }

        public TimeSpan CancelationTime
        {
            get; set;
        }

        public TimeSpan StartTime
        {
            get; set;
        }
        public TimeSpan FinishTime
        {
            get; set;
        }

        public TimeSpan JustAfterFinish
        {
            get; set;
        }

        public TimeSpan HourBeforeStart
        {
            get; set;
        }
    }
}
