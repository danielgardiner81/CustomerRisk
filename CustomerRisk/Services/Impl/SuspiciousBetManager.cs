using System;
using System.Collections.Generic;
using System.Linq;
using CustomerRisk.Model;

namespace CustomerRisk.Services.Impl
{
    public class SuspiciousBetManager : ISuspiciousBetManager
    {
        private readonly IBetRepository _betRepository;

        public SuspiciousBetManager(IBetRepository betRepository)
        {
            _betRepository = betRepository;
        }

        public decimal WinRateThreshold { get; set; } = (decimal) 0.6;
        public decimal HighWinThreashold { get; set; } = 1000;
        public decimal SuspiciousStakeThreshold { get; set; } = 10;
        public decimal HighlySuspiciousStakeThreshold { get; set; } = 30;

        public IEnumerable<Customer> GetCustomersWithSuspiciousWinRates()
        {
            return GetCustomerMetrics().Values.Where(o => o.WinRate > WinRateThreshold).ToList();
        }

        public IEnumerable<SuspiciousBet> GetUnsettledBetsFromSuspiciousWinRateCustomers()
        {
            var suspiciousCustomerIds = GetCustomersWithSuspiciousWinRates().Select(o => o.CustomerId);

            return _betRepository
                .GetUnsettledBets()
                .Where(o => suspiciousCustomerIds.Contains(o.CustomerId))
                .Select(o => new SuspiciousBet
                    {
                        Bet = o,
                        SuspiciousReason = SuspiciousReason.SuspiciousWinRateCustomer
                    });
        }

        public IEnumerable<SuspiciousBet> GetUnsettledBetsWithSuspiciousStakes()
        {
            var customerMetrics = GetCustomerMetrics();

            return _betRepository
                .GetUnsettledBets()
                .Where(o => customerMetrics.ContainsKey(o.CustomerId))
                .Where(o => o.Stake > customerMetrics[o.CustomerId].StakeAverage * SuspiciousStakeThreshold)
                .Select(o => new SuspiciousBet
                    {
                        Bet = o,
                        SuspiciousReason = SuspiciousReason.SuspiciousStake
                    });
        }

        public IEnumerable<SuspiciousBet> GetUnsettledBetsWithHighlySuspiciousStakes()
        {
            var customerMetrics = GetCustomerMetrics();

            return _betRepository
                .GetUnsettledBets()
                .Where(o => customerMetrics.ContainsKey(o.CustomerId))
                .Where(o => o.Stake > customerMetrics[o.CustomerId].StakeAverage * HighlySuspiciousStakeThreshold)
                .Select(o => new SuspiciousBet
                {
                    Bet = o,
                    SuspiciousReason = SuspiciousReason.HighlySuspiciousStake
                });
        }

        public IEnumerable<SuspiciousBet> GetUnsettledWithHighWinAmount()
        {
            return _betRepository
                .GetUnsettledBets()
                .Where(o => o.Win >= HighWinThreashold)
                .Select(o => new SuspiciousBet
                    {
                        Bet = o,
                        SuspiciousReason = SuspiciousReason.HighWinAmount
                    });
        }

        private Dictionary<int, Customer> GetCustomerMetrics()
        {
            return _betRepository
                .GetSettledBets()
                .GroupBy(o => o.CustomerId, (key, bets) => new Customer
                    {
                        CustomerId = key,
                        WinRate = bets.Average(bet => Math.Min(bet.Win, 1)),
                        StakeAverage = bets.Average(bet => bet.Stake)
                    })
                .ToDictionary(customer => customer.CustomerId, customer => customer);
        }
    }
}