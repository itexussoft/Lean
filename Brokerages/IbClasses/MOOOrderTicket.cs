namespace QuantConnect.Brokerages.IbClasses
{
    using System.Collections.Generic;
    using System.Linq;
    using QuantConnect.Orders;

    public class MOOOrderTicket
    {
        public MOOOrderTicket(List<OrderTicket> orderTickets)
        {
            this.Tickets = new List<OrderTicket>(orderTickets);
            this.InitialOrderTickets = new List<OrderTicket>(orderTickets);
        }

        public List<OrderTicket> Tickets { get; set; }

        public List<OrderTicket> InitialOrderTickets { get; }

        public bool IsFilled()
        {
            return this.Tickets.All(orderTicket => orderTicket.Status == OrderStatus.Filled);
        }
    }
}
