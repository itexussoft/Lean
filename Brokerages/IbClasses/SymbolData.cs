namespace QuantConnect.Brokerages.IbClasses
{
    using System.Collections.Generic;
    using QuantConnect.Data;
    using QuantConnect.Orders;
    using WTP.BLL.Infrastructure.DTO.Enums;
    using WTP.BLL.Infrastructure.DTO.Ticker;

    public class SymbolData : BaseData
    {
        public string TickerSymbol { get; set; }
        public Position Position { get; set; }
        public ReactiveStop ReactiveStop { get; set; }
        public DashboardData DashboardData { get; set; }
        public MOOOrderTicket MOOOrderTicket { get; set; }
        public List<OrderTicket> ScaleByGainOrderTickets { get; set; }

        public decimal Entry { get; set; }
        public decimal P1 { get; set; }
        public decimal P2 { get; set; }
        public decimal P3 { get; set; }
        public decimal PdlH { get; set; }
        public decimal PdlHRange { get; set; }
        public decimal RangeLow { get; set; }
        public decimal RangeHigh { get; set; }
        public decimal Plb { get; set; }
        public decimal Phb { get; set; }
        public decimal PrevDayMidpoint { get; set; }

        public decimal Quantity { get; set; }
        public decimal FilledQuantity { get; set; }
        public decimal ExitedChgFill { get; set; }

        public decimal PreviousClose { get; set; }
        public decimal PrevDayLow { get; set; }
        public decimal PrevDayHigh { get; set; }
        
        public decimal T15Min { get; set; }
        public decimal AdjustedT15Min { get; set; }

        public decimal InitialOrderPrice { get; set; }
        public decimal AdditionalInitialOrderPrice { get; set; }
        public InitialOrderType InitialOrderType { get; set; }

        public decimal InitialStopLoss { get; set; }
        public decimal AdditionalStopLoss{ get; set; }

        public bool IsOpenInRange { get; set; }
        public bool IsGainStop { get; set; }
        public bool IsMultiAdjustedStoped { get; set; }
        public bool IsMOOInitialFilled { get; set; }
        public bool IsGapRule { get; set; }
        public bool IsFillInRange { get; set; }
        public bool IsGain1NeedToUpdate { get; set; }
        public bool IsMOOFirstFill { get; set; }
        public bool IsMOOSecondPartFirstFill { get; set; }
        public bool IsMaxStopApplied { get; set; }
        public bool IsAdjustedSkipped { get; set; }
        public bool IsUserCancelledLogic { get; set; }
    }

    public class ReactiveStop
    {
        public decimal StopPrice { get; set; }

        public OrderTag OrderTag { get; set; }

        public bool IsHit { get; set; }
    }
}