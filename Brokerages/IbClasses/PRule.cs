namespace QuantConnect.Brokerages.IbClasses
{
    public class PRule
    {
        public PRule(decimal loss, decimal rule)
        {
            this.Loss = loss;
            this.Rule = rule;
        }

        public decimal Loss { get; }

        public decimal Rule { get; }
    }
}
