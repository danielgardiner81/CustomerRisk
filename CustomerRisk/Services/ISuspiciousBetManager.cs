using System.Collections.Generic;
using CustomerRisk.Model;

namespace CustomerRisk.Services
{
    public interface ISuspiciousBetManager
    {
        decimal WinRateThreshold { get; set; }
        decimal HighWinThreashold { get; set; }
        decimal SuspiciousStakeThreshold { get; set; }
        decimal HighlySuspiciousStakeThreshold { get; set; }
        IEnumerable<Customer> GetCustomersWithSuspiciousWinRates();
        IEnumerable<SuspiciousBet> GetUnsettledBetsFromSuspiciousWinRateCustomers();
        IEnumerable<SuspiciousBet> GetUnsettledBetsWithSuspiciousStakes();
        IEnumerable<SuspiciousBet> GetUnsettledBetsWithHighlySuspiciousStakes();
        IEnumerable<SuspiciousBet> GetUnsettledWithHighWinAmount();
    }
}