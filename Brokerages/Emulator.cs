/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using QuantConnect.Configuration;
using QuantConnect.Data;
using QuantConnect.Data.Market;
using QuantConnect.Interfaces;
using QuantConnect.Logging;
using QuantConnect.Orders;
using QuantConnect.Packets;
using QuantConnect.Securities;
using QuantConnect.Util;
using Order = QuantConnect.Orders.Order;
using IB = QuantConnect.Brokerages.InteractiveBrokers.Client;
using IBApi;
using NodaTime;
using Bar = QuantConnect.Data.Market.Bar;
using HistoryRequest = QuantConnect.Data.HistoryRequest;

namespace QuantConnect.Brokerages.InteractiveBrokers
{
    /// <summary>
    /// The Interactive Brokers brokerage
    /// </summary>
    public sealed class Emulator : InteractiveBrokersBrokerage
    {
        private List<Contract> contracts { get; set; } = new List<Contract>();

        public event Action<int> OrderPlaced;

        public List<Contract> Contracts
        {
            get
            {
                return this.contracts;
            }
        }

        /// <summary>
        /// Returns true if we're currently connected to the broker
        /// </summary>
        public override bool IsConnected
        {
            get
            {
                return base._client != null && base._client.Connected && !base._disconnected1100Fired;
            }
        }

        public Emulator(IAlgorithm algorithm, IOrderProvider orderProvider, ISecurityProvider securityProvider)
            : this(
                algorithm,
                orderProvider,
                securityProvider,
                Config.Get("ib-account"),
                Config.Get("ib-host", "LOCALHOST"),
                Config.GetInt("ib-port", 4001),
                Config.GetValue("ib-agent-description", IB.AgentDescription.Individual)
                )
        {
        }

        /// <summary>
        /// Creates a new InteractiveBrokersBrokerage for the specified account
        /// </summary>
        /// <param name="algorithm">The algorithm instance</param>
        /// <param name="orderProvider">An instance of IOrderProvider used to fetch Order objects by brokerage ID</param>
        /// <param name="securityProvider">The security provider used to give access to algorithm securities</param>
        /// <param name="account">The account used to connect to IB</param>
        public Emulator(IAlgorithm algorithm, IOrderProvider orderProvider, ISecurityProvider securityProvider, string account)
            : this(
                algorithm,
                orderProvider,
                securityProvider,
                account,
                Config.Get("ib-host", "LOCALHOST"),
                Config.GetInt("ib-port", 4001),
                Config.GetValue("ib-agent-description", IB.AgentDescription.Individual)
                )
        {
        }

      

        /// <summary>
        /// Creates a new InteractiveBrokersBrokerage from the specified values
        /// </summary>
        /// <param name="algorithm">The algorithm instance</param>
        /// <param name="orderProvider">An instance of IOrderProvider used to fetch Order objects by brokerage ID</param>
        /// <param name="securityProvider">The security provider used to give access to algorithm securities</param>
        /// <param name="account">The Interactive Brokers account name</param>
        /// <param name="host">host name or IP address of the machine where TWS is running. Leave blank to connect to the local host.</param>
        /// <param name="port">must match the port specified in TWS on the Configure&gt;API&gt;Socket Port field.</param>
        /// <param name="agentDescription">Used for Rule 80A describes the type of trader.</param>
        public Emulator(IAlgorithm algorithm, IOrderProvider orderProvider, ISecurityProvider securityProvider, string account, string host, int port, string agentDescription = IB.AgentDescription.Individual)
            : base(algorithm, orderProvider, securityProvider, account, host, port, agentDescription)
        {
        }

        public ConcurrentDictionary<Symbol, int> SubscribedSymbols
        {
            get
            {
                return subscribedSymbolsStatic;
            }
        }

        /// <summary>
        /// Places the order with InteractiveBrokers
        /// </summary>
        /// <param name="order">The order to be placed</param>
        /// <param name="needsNewId">Set to true to generate a new order ID, false to leave it alone</param>
        /// <param name="exchange">The exchange to send the order to, defaults to "Smart" to use IB's smart routing</param>
        private void IBPlaceOrder(Order order, bool needsNewId, string exchange = null)
        {
            // connect will throw if it fails
            Connect();

            if (!base.IsConnected)
            {
                throw new InvalidOperationException("InteractiveBrokersBrokerage.IBPlaceOrder(): Unable to place order while not connected.");
            }

            // MOO/MOC require directed option orders
            if (exchange == null &&
                order.Symbol.SecurityType == SecurityType.Option &&
                (order.Type == OrderType.MarketOnOpen || order.Type == OrderType.MarketOnClose))
            {
                exchange = Market.CBOE.ToUpper();
            }

            var contract = CreateContract(order.Symbol, exchange);
            this.contracts.Add(contract);

            int ibOrderId;
            if (needsNewId)
            {
                // the order ids are generated for us by the SecurityTransactionManaer
                var id = GetNextBrokerageOrderId();
                order.BrokerId.Add(id.ToString());
                ibOrderId = id;
            }
            else if (order.BrokerId.Any())
            {
                // this is *not* perfect code
                ibOrderId = int.Parse(order.BrokerId[0]);
            }
            else
            {
                throw new ArgumentException("Expected order with populated BrokerId for updating orders.");
            }

            _requestInformation[ibOrderId] = "IBPlaceOrder: " + contract;

            if (order.Type == OrderType.OptionExercise)
            {
                _client.ClientSocket.exerciseOptions(ibOrderId, contract, 1, decimal.ToInt32(order.Quantity), base._account, 0);
            }
            else
            {
                var ibOrder = ConvertOrder(order, contract, ibOrderId);
                Thread.Sleep(20);

                this.OrderPlaced?.Invoke(ibOrder.OrderId);
            }
        }

        /// <summary>
        /// Places a new order and assigns a new broker ID to the order
        /// </summary>
        /// <param name="order">The order to be placed</param>
        /// <returns>True if the request for a new order has been placed, false otherwise</returns>
        public override bool PlaceOrder(Order order)
        {
            try
            {
                Log.Trace("InteractiveBrokersBrokerage.PlaceOrder(): Symbol: " + order.Symbol.Value + " Quantity: " + order.Quantity);

                IBPlaceOrder(order, true);
                return true;
            }
            catch (Exception err)
            {
                Log.Error("InteractiveBrokersBrokerage.PlaceOrder(): " + err);
                return false;
            }
        }

        /// <summary>
        /// Updates the order with the same id
        /// </summary>
        /// <param name="order">The new order information</param>
        /// <returns>True if the request was made for the order to be updated, false otherwise</returns>
        public override bool UpdateOrder(Order order)
        {
            try
            {
                Log.Trace("InteractiveBrokersBrokerage.UpdateOrder(): Symbol: " + order.Symbol.Value + " Quantity: " + order.Quantity + " Status: " + order.Status);

                IBPlaceOrder(order, false);
            }
            catch (Exception err)
            {
                Log.Error("InteractiveBrokersBrokerage.UpdateOrder(): " + err);
                return false;
            }
            return true;
        }        
    }
}
