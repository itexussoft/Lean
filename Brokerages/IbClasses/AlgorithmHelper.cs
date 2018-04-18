using QuantConnect.Data.Market;
using QuantConnect.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wtpEnum = WTP.BLL.Infrastructure.DTO.Enums;
using wtpDto = WTP.BLL.Infrastructure.DTO;

namespace QuantConnect.Brokerages.IbClasses
{
    public class AlgorithmHelper
    {
        public static WTP.BLL.Infrastructure.DTO.Enums.OrderType GetOrderType(OrderType qcType)
        {
            switch (qcType)
            {
                case OrderType.Market:
                    return wtpEnum.OrderType.Market;
                case OrderType.Limit:
                    return wtpEnum.OrderType.Limit;
                case OrderType.StopMarket:
                    return wtpEnum.OrderType.StopMarket;
                case OrderType.StopLimit:
                    return wtpEnum.OrderType.StopLimit;
                case OrderType.MarketOnOpen:
                    return wtpEnum.OrderType.MarketOnOpen;
                case OrderType.MarketOnClose:
                    return wtpEnum.OrderType.MarketOnClose;
                default:
                    throw new ArgumentOutOfRangeException(nameof(qcType), qcType, null);
            }
        }

        public static wtpEnum.Position GetPosition(string position)
        {
            switch (position.ToUpper())
            {
                case "S":
                    return wtpEnum.Position.S;
                case "L":
                    return wtpEnum.Position.L;
                case "C":
                    return wtpEnum.Position.C;
                default:
                    return wtpEnum.Position.NotSet;
            }
        }

        public static decimal GetEP(TradeBar tradeBar, wtpEnum.Position position)
        {
            return position == wtpEnum.Position.L
                ? RoundDown(tradeBar.Low)
                : RoundUp(tradeBar.High);
        }

        public static PRule CalculateP1Loss(decimal entry, wtpEnum.Position position, wtpDto.RuleSettings ruleSettings)
        {
            switch (position)
            {
                case wtpEnum.Position.L:
                    var levelL = entry < 20
                        ? ruleSettings.AdjustedStopPriceLevelLess20Long
                        : ruleSettings.AdjustedStopPriceLevelGreater20Long;
                    var p1LossL = RoundDown(entry * (decimal)levelL);
                    return new PRule(p1LossL, p1LossL - 0.01m);
                case wtpEnum.Position.S:
                case wtpEnum.Position.C:
                    var levelS = entry < 20
                        ? ruleSettings.AdjustedStopPriceLevelLess20Short
                        : ruleSettings.AdjustedStopPriceLevelGreater20Short;
                    var p1LossS = RoundDown(entry * (decimal)levelS);
                    return new PRule(p1LossS, p1LossS + 0.01m);
                default:
                    throw new Exception("Position invalid");
            }
        }

        public static PRule CalculateP2Loss(decimal entry, wtpEnum.Position position, wtpDto.RuleSettings ruleSettings)
        {
            switch (position)
            {
                case wtpEnum.Position.L:
                    var p2LossL = RoundDown(entry * (decimal)ruleSettings.InitialStopPriceMaxLong);
                    return new PRule(p2LossL, p2LossL - 0.01m);
                case wtpEnum.Position.S:
                case wtpEnum.Position.C:
                    var p2LossS = RoundUp(entry * (decimal)ruleSettings.InitialStopPriceMaxShort); ;
                    return new PRule(p2LossS, p2LossS + 0.01m);
                default:
                    throw new Exception("Position invalid");
            }
        }

        public static PRule CalculateP3Loss(decimal entry, wtpEnum.Position position, wtpDto.RuleSettings ruleSettings)
        {
            switch (position)
            {
                case wtpEnum.Position.L:
                    var p3LossL = RoundDown(entry * (decimal)ruleSettings.InitialStopAbsoluteMaxLong);
                    return new PRule(p3LossL, p3LossL + 0.01m);
                case wtpEnum.Position.S:
                case wtpEnum.Position.C:
                    var p3LossS = RoundUp(entry * (decimal)ruleSettings.InitialStopAbsoluteMaxShort);
                    return new PRule(p3LossS, p3LossS - 0.01m);
                default:
                    throw new Exception("Position invalid");
            }
        }

        public static double CalculatePPdlh(SymbolData symbolData, decimal entry)
        {
            var ppdlh = 0.0;
            if (entry != 0)
            {
                ppdlh = (double)(((entry - symbolData.PdlH) / entry) * 100);
            }
            return Round(ppdlh);
        }

        public static void CalculatePdlhRange(SymbolData symbolData, decimal entry, wtpDto.RuleSettings ruleSettings)
        {
            if (symbolData.Position == wtpEnum.Position.L)
            {
                var level = entry < 20
                    ? ruleSettings.AdjustedStopPDLRangeLess20Long
                    : ruleSettings.AdjustedStopPDLRangeGreater20Long;
                symbolData.PdlHRange = RoundDown(symbolData.PdlH * (decimal)level);
            }
            else
            {
                var level = entry < 20
                    ? ruleSettings.AdjustedStopPDLRangeLess20Short
                    : ruleSettings.AdjustedStopPDLRangeGreater20Short;

                symbolData.PdlHRange = RoundUp(symbolData.PdlH * (decimal)level);
            }
        }

        public static double Round(double value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }

        public static decimal Round(decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }

        public static double RoundDown(double value)
        {
            var s1 = value.ToString("0.0000");
            s1 = s1.Substring(0, s1.IndexOf(".") + 3);
            return Convert.ToDouble(s1);
        }

        public static double RoundUp(double value)
        {
            return Math.Round(value, 2);
        }

        public static decimal RoundDown(decimal value)
        {
            var s1 = value.ToString("0.0000");
            s1 = s1.Substring(0, s1.IndexOf(".") + 3);
            return Convert.ToDecimal(s1);
        }

        public static decimal RoundUp(decimal value)
        {
            return Math.Round(value, 2);
        }

        public static TimeSpan GetExecutionTime(string time)
        {
            var timeParts = time.Split(':');
            if (timeParts.Length != 3)
            {
                throw new Exception($"GetExecutionTime: {time} parsin failed");
            }
            return new TimeSpan(Convert.ToInt32(timeParts[0]), Convert.ToInt32(timeParts[1]), Convert.ToInt32(timeParts[2]));
        }

        public static TradeBar GetJoinBar(List<TradeBar> bars)
        {
            if (bars.Count == 0)
            {
                return null;
            }
            var firstBar = bars.FirstOrDefault();
            var lastBar = bars.LastOrDefault();

            if (firstBar == null || lastBar == null)
            {
                return null;
            }
            var bar = new TradeBar
            {
                Open = firstBar.Open,
                High = firstBar.High,
                Low = firstBar.Low,
                Close = lastBar.Close
            };

            foreach (var tradeBar in bars)
            {
                if (tradeBar.High > bar.High)
                {
                    bar.High = tradeBar.High;
                }
                if (tradeBar.Low < bar.Low)
                {
                    bar.Low = tradeBar.Low;
                }
            }

            return bar;
        }
    }
}
