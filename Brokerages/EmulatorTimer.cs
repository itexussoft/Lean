using IBApi;
using QuantConnect.Brokerages.IbClasses;
using QuantConnect.Brokerages.InteractiveBrokers;
using QuantConnect.Interfaces;
using QuantConnect.Securities;
using System;
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

        private int currentTickType = (int)TickType.LastPrice;

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

        private static readonly TimeSpan StartTickerPrice = AlgorithmHelper.GetExecutionTime("11:34:00");
        private static readonly TimeSpan StartExecutionDetails = AlgorithmHelper.GetExecutionTime("9:30:00");



        public EmulatorTimer(Emulator emulator, IOrderProvider brokerageTransactionHandler)
        {
            this.emulator = emulator;
            this.brokerageTransactionHandler = brokerageTransactionHandler;
        }

        public void AddTimerEvents()
        {
            this.scheduledEventHandler.AddScheduledEvent(StartTickerPrice, TickerPriceInvoke);
            this.scheduledEventHandler.AddScheduledEvent(StartExecutionDetails, ExecutionDetailsInvoke);
        }

        public void InitializeTimer()
        {
            this.scheduledEventHandler.Initialize();
        }

        public void TickerPriceInvoke()
        {
            foreach (var symbol in this.emulator.SubscribedSymbols)
            {
                this.emulator.Client.tickPrice(symbol.Value, currentTickType, new Random().NextDouble() * 100, new TickAttrib());
            }
        }

        public void ExecutionDetailsInvoke()
        {
            Contract contract;
            var orders = this.brokerageTransactionHandler.GetOrders();
            foreach (var order in orders)
            {
                contract = this.emulator.Contracts.FirstOrDefault(x => x.Symbol == order.Symbol.Value);
                execution.OrderId = order.Id;
                this.emulator.Client.execDetails(0, contract, execution);
            }
        }
    }
}
