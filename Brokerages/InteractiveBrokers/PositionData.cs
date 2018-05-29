namespace QuantConnect.Brokerages.InteractiveBrokers
{
    public class PositionData
    {
        public string Contract { get; set; }

        public int Position { get; set; }

        public double MarketPrice { get; set; }

        public double MarketValue { get; set; }

        public double AverageCost { get; set; }

        public double UnrealisedPnL { get; set; }

        public double UnrealizedPnL => UnrealisedPnL;

        public double RealisedPnL { get; set; }

        public double RealizedPnL => RealisedPnL;

        public string AccountName { get; set; }
    }
}
