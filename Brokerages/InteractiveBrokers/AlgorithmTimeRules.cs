namespace QuantConnect.Brokerages.InteractiveBrokers
{
    using System;

    public class AlgorithmTimeRules
    {
        public TimeSpan OpenHandlerTime
        {
            get; set;
        }

        public TimeSpan TradeStartTime
        {
            get; set;
        }

        public TimeSpan Interval1Time
        {
            get; set;
        }

        public TimeSpan Interval2Time
        {
            get; set;
        }

        public TimeSpan Interval3Time
        {
            get; set;
        }

        public TimeSpan StartScaleByGainTime
        {
            get; set;
        }

        public TimeSpan P5StopLogicTime
        {
            get; set;
        }

        public TimeSpan InitialStopLogic
        {
            get; set;
        }

        public TimeSpan SendAdjustedStopTime
        {
            get; set;
        }

        public TimeSpan CancelInitialOrdersTime
        {
            get; set;
        }

        public TimeSpan TradeFinishTime
        {
            get; set;
        }

        public TimeSpan ScaleByTime1StartTime
        {
            get; set;
        }

        public TimeSpan ScaleByTime2Time
        {
            get; set;
        }

        public TimeSpan ScaleByTime3Time1
        {
            get; set;
        }

        public TimeSpan SendMOCTime
        {
            get; set;
        }

        public TimeSpan ScaleByTime1FinishTime
        {
            get; set;
        }

        public TimeSpan EmaFirstTime
        {
            get; set;
        }

        public TimeSpan EmaLastTime
        {
            get; set;
        }

        public TimeSpan RequestPreviousDayELV
        {
            get; set;
        }

        public TimeSpan PerformanceStopBegin
        {
            get; set;
        }

        public TimeSpan PerformanceStopEnd
        {
            get; set;
        }

        public TimeRulesApi TimeRulesApi
        {
            get; set;
        }
    }
}
