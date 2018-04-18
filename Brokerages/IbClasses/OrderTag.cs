namespace QuantConnect.Brokerages.IbClasses
{
    public enum OrderTag
    {
        MOOOrder = 1,
        MOCOrder = 2,
        GapRule = 3,
        ScaleByTime = 4,
        ScaleByGain = 5,
        InitialStop = 6,
        P3InitialStop = 7,
        AdjustedStop = 8,
        AdjustedHalfStop = 9,
        MultiAdjustedStop = 10,
        StopBasedOnGain = 11,
        MOOOrderSecondPart = 12,
        UserExitPosition = 13
    }
}
