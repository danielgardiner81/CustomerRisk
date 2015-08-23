namespace CustomerRisk.Model
{
    internal class Customer
    {
        public int CustomerId { get; set; }
        public int EventId { get; set; }
        public int ParticipantId { get; set; }
        public decimal Stake { get; set; }
        public decimal Win { get; set; }
    }
}