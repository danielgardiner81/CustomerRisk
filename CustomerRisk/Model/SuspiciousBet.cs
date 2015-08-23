namespace CustomerRisk.Model
{
    public class SuspiciousBet
    {
        public Bet Bet { get; set; }
        public SuspiciousReason SuspiciousReason { get; set; }
    }
}