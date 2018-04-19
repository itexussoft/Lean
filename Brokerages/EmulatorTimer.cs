using IBApi;
using QuantConnect.Brokerages.IbClasses;
using QuantConnect.Brokerages.InteractiveBrokers;
using QuantConnect.Interfaces;
using QuantConnect.Securities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantConnect.Brokerages
{
    public class EmulatorTimer
    {
        private readonly Emulator emulator;
        private readonly ScheduledEventHandler scheduledEventHandler = new ScheduledEventHandler();
        private readonly IOrderProvider brokerageTransactionHandler;

        private int currentTickType = (int)9;

        private Execution execution = new Execution()
        {
            AvgPrice = 100,
            Price = 100,
            Shares = 50
        };

        private enum TickType
        {
            LastPrice = 4
        }

        private static readonly TimeSpan StartTickerPrice = AlgorithmHelper.GetExecutionTime("12:17:00");
        private static readonly TimeSpan StartExecutionDetails = AlgorithmHelper.GetExecutionTime("10:59:01");
        
        public EmulatorTimer(Emulator emulator, IOrderProvider brokerageTransactionHandler)
        {
            this.emulator = emulator;
            this.emulator.OrderPlaced += ExecutionDetailsInvoke;
            this.brokerageTransactionHandler = brokerageTransactionHandler;
        }

        public void AddTimerEvents()
        {
            this.scheduledEventHandler.AddScheduledEvent(StartTickerPrice, TickerPriceInvoke);
            //this.scheduledEventHandler.AddScheduledEvent(StartExecutionDetails, ExecutionDetailsInvoke);
        }

        public void InitializeTimer()
        {
            this.scheduledEventHandler.Initialize();
        }

        public void TickerPriceInvoke()
        {
            var copy = CloneDictionaryCloningValues(this.emulator.SubscribedSymbols);
            foreach (var symbol in copy)
            {
                this.emulator.Client.tickPrice(symbol.Value, currentTickType, new Random().NextDouble() * 100, new TickAttrib());
            }
        }

        public void ExecutionDetailsInvoke(int orderId)
        {
            Contract contract;
            var order = this.brokerageTransactionHandler.GetOrderByBrokerageId(orderId);

            contract = this.emulator.Contracts.FirstOrDefault(x => x.Symbol == order.Symbol.Value);
            execution.OrderId = orderId;
            this.emulator.Client.execDetails(0, contract, execution);
        }

        public Dictionary<TKey, TValue> CloneDictionaryCloningValues<TKey, TValue>
                    (ConcurrentDictionary<TKey, TValue> original)
        {
            Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>(original.Count);
            foreach (KeyValuePair<TKey, TValue> entry in original)
            {
                ret.Add(entry.Key, entry.Value);
            }
            return ret;
        }
    }
}
