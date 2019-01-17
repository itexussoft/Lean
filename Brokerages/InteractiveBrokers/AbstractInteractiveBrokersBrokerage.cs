namespace QuantConnect.Brokerages.InteractiveBrokers
{
    using QuantConnect.Data;
    using QuantConnect.Packets;
    using System;
    using System.Collections.Generic;
    using IB = QuantConnect.Brokerages.InteractiveBrokers.Client;

    public abstract class AbstractInteractiveBrokersBrokerage : Brokerage
    {
        public event EventHandler<string> OnConnectionLost;

        private readonly IB.IBrokerageClient _client;

        public IB.IBrokerageClient Client
        {
            get { return _client; }
        }

        public AbstractInteractiveBrokersBrokerage(string name)
            : base(name)
        {}

        public abstract void SetDateTimeFunctions(Func<DateTime> estNow, Func<TimeSpan, bool> isGatewayClosableTime);
        public abstract List<Holding> GetAllAccountHoldings();
        public abstract Dictionary<string, string> GetAccountSummary();
        public abstract Dictionary<string, string> GetAccountSummary(string tag);
        public abstract string GetPrevDayELV();
        public abstract List<IB.ExecutionDetailsEventArgs> GetExecutions(string symbol, string type, string exchange, DateTime? timeSince, string side);
        public abstract void ProcessExecutionDetailsQueue();
        public abstract IEnumerable<BaseData> GetNextTicks();
        public abstract Symbol GetSubscribedSymbol(int tickerId);
        public abstract void Subscribe(LiveNodePacket job, IEnumerable<Symbol> symbols);
        public abstract void Unsubscribe(LiveNodePacket job, IEnumerable<Symbol> symbols);
        public abstract IEnumerable<Symbol> LookupSymbols(string lookupName, SecurityType securityType, string securityCurrency = null, string securityExchange = null);
        public abstract void ResetGatewayConnection();
    }
}
