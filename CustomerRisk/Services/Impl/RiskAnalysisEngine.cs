using System.Collections.Generic;
using CustomerRisk.Model;

namespace CustomerRisk.Services.Impl
{
    public class RiskAnalysisEngine
    {
        private readonly IBetRepository _betRepository;

        public RiskAnalysisEngine(IBetRepository betRepository)
        {
            _betRepository = betRepository;
        }

        public IEnumerable<Customer> GetCustomersWithSuspiciousWinRates()
        {
            throw new System.NotImplementedException();
        }
    }
}