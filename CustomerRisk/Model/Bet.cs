namespace CustomerRisk.Model
{
    public class Bet
    {
        public int CustomerId { get; set; }
        public int EventId { get; set; }
        public int ParticipantId { get; set; }
        public decimal Stake { get; set; }
        public decimal Win { get; set; }
    }
}