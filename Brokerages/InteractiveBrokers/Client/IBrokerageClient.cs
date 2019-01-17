namespace QuantConnect.Brokerages.InteractiveBrokers.Client
{
    using System;

    public interface IBrokerageClient
    {
        event EventHandler<TickPriceEventArgs> TickPrice;
    }
}