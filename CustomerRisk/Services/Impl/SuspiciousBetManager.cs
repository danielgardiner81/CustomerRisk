using System.Collections.Generic;
using CustomerRisk.Model;

namespace CustomerRisk.Services.Impl
{
    public class SuspiciousBetManager
    {
        private readonly IBetRepository _betRepository;

        public SuspiciousBetManager(IBetRepository betRepository)
        {
            _betRepository = betRepository;
        }

        public IEnumerable<Customer> GetCustomersWithSuspiciousWinRates()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<SuspiciousBet> GetUnsettledBetsFromSuspiciousWinRateCustomers()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<SuspiciousBet> GetUnsettledWithHighWinAmount()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<SuspiciousBet> GetUnsettledBetsWithSuspiciousStakes()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<SuspiciousBet> GetUnsettledBetsWithHighlySuspiciousStakes()
        {
            throw new System.NotImplementedException();
        }
    }
}