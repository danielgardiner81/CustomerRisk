using System;
using System.Collections.Generic;
using System.Linq;
using CustomerRisk.Model;
using CustomerRisk.Services;
using CustomerRisk.Services.Impl;
using Moq;
using NUnit.Framework;

namespace CustomerRisk.Tests
{
    [TestFixture]
    public class SuspiciousBetManagerTests
    {
        private IBetRepository _betRepository;
        private IEnumerable<Bet> _settledBets;
        private IEnumerable<Bet> _settledBetsSuspiciousWinRates;
        private IEnumerable<Bet> _unsettledBets;
        private IEnumerable<Bet> _unsettledBetsSuspiciousStake;
        private IEnumerable<Bet> _unsettledBetsHighlySuspiciousStake;
        private IEnumerable<Bet> _unsettledBetsHighWinAmount;
        private SuspiciousBetManager _engine;

        [SetUp]
        public void SetUp()
        {
            _unsettledBets = new List<Bet>
                {
                    new Bet { CustomerId = 1, EventId = 10, ParticipantId = 1, Stake = 20, Win = 200 },
                    new Bet { CustomerId = 1, EventId = 12, ParticipantId = 1, Stake = 60, Win = 300 },
                    new Bet { CustomerId = 1, EventId = 13, ParticipantId = 1, Stake = 70, Win = 400 },
                    new Bet { CustomerId = 2, EventId = 1, ParticipantId = 1, Stake = 40, Win = 500 },
                    new Bet { CustomerId = 3, EventId = 1, ParticipantId = 1, Stake = 40, Win = 500 },
                };

            _unsettledBetsSuspiciousStake = new List<Bet>
                {
                    new Bet { CustomerId = 1, EventId = 10, ParticipantId = 1, Stake = 900, Win = 200 },
                    new Bet { CustomerId = 1, EventId = 12, ParticipantId = 1, Stake = 800, Win = 300 },
                    new Bet { CustomerId = 1, EventId = 13, ParticipantId = 1, Stake = 1200, Win = 400 },
                    new Bet { CustomerId = 2, EventId = 1, ParticipantId = 1, Stake = 1100, Win = 500 },
                    new Bet { CustomerId = 3, EventId = 1, ParticipantId = 1, Stake = 40, Win = 500 },
                };

            _unsettledBetsHighlySuspiciousStake = new List<Bet>
                {
                    new Bet { CustomerId = 1, EventId = 10, ParticipantId = 1, Stake = 9000, Win = 200 },
                    new Bet { CustomerId = 1, EventId = 12, ParticipantId = 1, Stake = 8000, Win = 300 },
                    new Bet { CustomerId = 1, EventId = 13, ParticipantId = 1, Stake = 12000, Win = 400 },
                    new Bet { CustomerId = 2, EventId = 1, ParticipantId = 1, Stake = 11000, Win = 500 },
                    new Bet { CustomerId = 3, EventId = 1, ParticipantId = 1, Stake = 400, Win = 500 },
                };

            _unsettledBetsHighWinAmount = new List<Bet>
                {
                    new Bet { CustomerId = 1, EventId = 10, ParticipantId = 1, Stake = 9000, Win = 2000 },
                    new Bet { CustomerId = 1, EventId = 12, ParticipantId = 1, Stake = 8000, Win = 3000 },
                    new Bet { CustomerId = 1, EventId = 13, ParticipantId = 1, Stake = 12000, Win = 4000 },
                    new Bet { CustomerId = 2, EventId = 1, ParticipantId = 1, Stake = 11000, Win = 5000 },
                    new Bet { CustomerId = 3, EventId = 1, ParticipantId = 1, Stake = 400, Win = 500 },
                };

            _settledBets = new List<Bet>
                {
                    new Bet { CustomerId = 1, EventId = 1, ParticipantId = 1, Stake = 50, Win = 0 },
                    new Bet { CustomerId = 1, EventId = 2, ParticipantId = 1, Stake = 50, Win = 500 },
                    new Bet { CustomerId = 1, EventId = 3, ParticipantId = 1, Stake = 50, Win = 0 },
                    new Bet { CustomerId = 2, EventId = 1, ParticipantId = 1, Stake = 50, Win = 0 },
                    new Bet { CustomerId = 3, EventId = 1, ParticipantId = 1, Stake = 50, Win = 0 },
                };

            _settledBetsSuspiciousWinRates = new List<Bet>
                {
                    new Bet { CustomerId = 1, EventId = 1, ParticipantId = 1, Stake = 50, Win = 0 },
                    new Bet { CustomerId = 1, EventId = 2, ParticipantId = 1, Stake = 50, Win = 500 },
                    new Bet { CustomerId = 1, EventId = 3, ParticipantId = 1, Stake = 50, Win = 500 },
                    new Bet { CustomerId = 2, EventId = 1, ParticipantId = 1, Stake = 50, Win = 500 },
                    new Bet { CustomerId = 3, EventId = 1, ParticipantId = 1, Stake = 50, Win = 500 },
                };
        }

        [Test]
        public void GetSuspiciousWinRates_When_None_Suspicious()
        {
            SetBetData(_settledBets, null);

            var actual = Test(manager => manager.GetCustomersWithSuspiciousWinRates());

            Assert.That(actual.Count(), Is.EqualTo(0));
        }

        [Test]
        public void GetCustomersWithSuspiciousWinRates_When_Two_Suspicious()
        {
            SetBetData(_settledBetsSuspiciousWinRates, null);

            var actual = Test(manager => manager.GetCustomersWithSuspiciousWinRates());

            Assert.That(actual.Count(), Is.EqualTo(2));
            Assert.That(actual.Single(o => o.CustomerId == 1).WinRate, Is.EqualTo((decimal) 3 / (decimal) 2));
            Assert.That(actual.Single(o => o.CustomerId == 2).WinRate, Is.EqualTo(1));
        }

        [Test]
        public void GetUnsettledBetsFromSuspiciousCustomers_When_None_Suspicious()
        {
            SetBetData(_settledBets, _unsettledBets);

            var actual = Test(manager => manager.GetUnsettledBetsFromSuspiciousWinRateCustomers());

            Assert.That(actual.Count(), Is.EqualTo(0));
        }

        [Test]
        public void GetUnsettledBetsFromSuspiciousCustomers_When_Two_Suspicious()
        {
            SetBetData(_settledBetsSuspiciousWinRates, _unsettledBets);

            var actual = Test(manager => manager.GetUnsettledBetsFromSuspiciousWinRateCustomers());

            Assert.That(actual.Count(), Is.EqualTo(4));
            Assert.That(actual.Select(o => o.SuspiciousReason), Is.All.EqualTo(SuspiciousReason.SuspiciousWinRateCustomer));
            Assert.That(actual.Count(o => o.Bet.CustomerId == 1), Is.EqualTo(3));
            Assert.That(actual.Count(o => o.Bet.CustomerId == 2), Is.EqualTo(1));
        }

        [Test]
        public void GetUnsettledWithSuspiciousStakes_When_None_Suspicious()
        {
            SetBetData(_settledBets, _unsettledBets);

            var actual = Test(manager => manager.GetUnsettledBetsWithSuspiciousStakes());

            Assert.That(actual.Count(), Is.EqualTo(0));
        }

        [Test]
        public void GetUnsettledWithSuspiciousStakes_When_Four_Suspicious()
        {
            SetBetData(_settledBets, _unsettledBetsSuspiciousStake);

            var actual = Test(manager => manager.GetUnsettledBetsWithSuspiciousStakes());

            Assert.That(actual.Count(), Is.EqualTo(4));
            Assert.That(actual.Select(o => o.SuspiciousReason), Is.All.EqualTo(SuspiciousReason.SuspiciousStake));
            Assert.That(actual.Count(o => o.Bet.CustomerId == 1), Is.EqualTo(3));
            Assert.That(actual.Count(o => o.Bet.CustomerId == 2), Is.EqualTo(1));
        }

        [Test]
        public void GetUnsettledWithHighlySuspiciousStakes_When_None_HighlySuspicious()
        {
            SetBetData(_settledBets, _unsettledBets);

            var actual = Test(manager => manager.GetUnsettledBetsWithHighlySuspiciousStakes());

            Assert.That(actual.Count(), Is.EqualTo(0));
        }

        [Test]
        public void GetUnsettledWithHighlySuspiciousStakes_When_Only_Suspicious()
        {
            SetBetData(_settledBets, _unsettledBetsSuspiciousStake);

            var actual = Test(manager => manager.GetUnsettledBetsWithHighlySuspiciousStakes());

            Assert.That(actual.Count(), Is.EqualTo(0));
        }

        [Test]
        public void GetUnsettledWithHighlySuspiciousStakes_When_Four_HighlySuspicious()
        {
            SetBetData(_settledBets, _unsettledBetsHighlySuspiciousStake);

            var actual = Test(manager => manager.GetUnsettledBetsWithHighlySuspiciousStakes());

            Assert.That(actual.Count(), Is.EqualTo(4));
            Assert.That(actual.Select(o => o.SuspiciousReason), Is.All.EqualTo(SuspiciousReason.HighlySuspiciousStake));
            Assert.That(actual.Count(o => o.Bet.CustomerId == 1), Is.EqualTo(3));
            Assert.That(actual.Count(o => o.Bet.CustomerId == 2), Is.EqualTo(1));
        }

        [Test]
        public void GetUnsettledWithHighWinAmount_When_None_Suspicious()
        {
            SetBetData(_settledBets, _unsettledBets);

            var actual = Test(manager => manager.GetUnsettledWithHighWinAmount());

            Assert.That(actual.Count(), Is.EqualTo(0));
        }

        [Test]
        public void GetUnsettledWithWinAmount_When_Four_Suspicious()
        {
            SetBetData(_settledBets, _unsettledBetsHighWinAmount);

            var actual = Test(manager => manager.GetUnsettledWithHighWinAmount());

            Assert.That(actual.Count(), Is.EqualTo(4));
            Assert.That(actual.Select(o => o.SuspiciousReason), Is.All.EqualTo(SuspiciousReason.HighWinAmount));
            Assert.That(actual.Count(o => o.Bet.CustomerId == 1), Is.EqualTo(3));
            Assert.That(actual.Count(o => o.Bet.CustomerId == 2), Is.EqualTo(1));
        }

        private void SetBetData(IEnumerable<Bet> settledBets, IEnumerable<Bet> unsettledBets)
        {
            _betRepository = Mock.Of<IBetRepository>(o =>
                o.GetSettledBets() == settledBets &&
                o.GetUnsettledBets() == unsettledBets);
        }

        public IEnumerable<SuspiciousBet> Test(Func<SuspiciousBetManager, IEnumerable<SuspiciousBet>> testFunc)
        {
            _engine = new SuspiciousBetManager(_betRepository);
            return testFunc(_engine);
        }

        public IEnumerable<Customer> Test(Func<SuspiciousBetManager, IEnumerable<Customer>> testFunc)
        {
            _engine = new SuspiciousBetManager(_betRepository);
            return testFunc(_engine);
        }
    }
}