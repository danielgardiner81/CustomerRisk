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
        private Bets _settledBets;
        private Bets _settledBetsSuspiciousRates;
        private Bets _unsettledBets;
        private Bets _unsettledBetsSuspiciousStake;
        private Bets _unsettledBetsHighlySuspiciousStake;
        private Bets _unsettledBetsHighWinAmount;

        [SetUp]
        public void SetUp()
        {
            _unsettledBets = new Bets
                {
                    new Bet { CustomerId = 1, EventId = 10, ParticipantId = 1, Stake = 20, Win = 200 },
                    new Bet { CustomerId = 1, EventId = 12, ParticipantId = 1, Stake = 60, Win = 300 },
                    new Bet { CustomerId = 1, EventId = 13, ParticipantId = 1, Stake = 70, Win = 400 },
                    new Bet { CustomerId = 2, EventId = 1, ParticipantId = 1, Stake = 40, Win = 500 },
                    new Bet { CustomerId = 3, EventId = 1, ParticipantId = 1, Stake = 40, Win = 500 },
                };

            _unsettledBetsSuspiciousStake = new Bets
                {
                    new Bet { CustomerId = 1, EventId = 10, ParticipantId = 1, Stake = 900, Win = 200 },
                    new Bet { CustomerId = 1, EventId = 12, ParticipantId = 1, Stake = 800, Win = 300 },
                    new Bet { CustomerId = 1, EventId = 13, ParticipantId = 1, Stake = 1200, Win = 400 },
                    new Bet { CustomerId = 2, EventId = 1, ParticipantId = 1, Stake = 1100, Win = 500 },
                    new Bet { CustomerId = 3, EventId = 1, ParticipantId = 1, Stake = 40, Win = 500 },
                };

            _unsettledBetsHighlySuspiciousStake = new Bets
                {
                    new Bet { CustomerId = 1, EventId = 10, ParticipantId = 1, Stake = 9000, Win = 200 },
                    new Bet { CustomerId = 1, EventId = 12, ParticipantId = 1, Stake = 8000, Win = 300 },
                    new Bet { CustomerId = 1, EventId = 13, ParticipantId = 1, Stake = 12000, Win = 400 },
                    new Bet { CustomerId = 2, EventId = 1, ParticipantId = 1, Stake = 11000, Win = 500 },
                    new Bet { CustomerId = 3, EventId = 1, ParticipantId = 1, Stake = 400, Win = 500 },
                };

            _unsettledBetsHighWinAmount = new Bets
                {
                    new Bet { CustomerId = 1, EventId = 10, ParticipantId = 1, Stake = 9000, Win = 2000 },
                    new Bet { CustomerId = 1, EventId = 12, ParticipantId = 1, Stake = 8000, Win = 3000 },
                    new Bet { CustomerId = 1, EventId = 13, ParticipantId = 1, Stake = 12000, Win = 4000 },
                    new Bet { CustomerId = 2, EventId = 1, ParticipantId = 1, Stake = 11000, Win = 5000 },
                    new Bet { CustomerId = 3, EventId = 1, ParticipantId = 1, Stake = 400, Win = 500 },
                };

            _settledBets = new Bets
                {
                    new Bet { CustomerId = 1, EventId = 1, ParticipantId = 1, Stake = 50, Win = 0 },
                    new Bet { CustomerId = 1, EventId = 2, ParticipantId = 1, Stake = 50, Win = 500 },
                    new Bet { CustomerId = 1, EventId = 3, ParticipantId = 1, Stake = 50, Win = 0 },
                    new Bet { CustomerId = 2, EventId = 1, ParticipantId = 1, Stake = 50, Win = 0 },
                    new Bet { CustomerId = 3, EventId = 1, ParticipantId = 1, Stake = 50, Win = 0 },
                };

            _settledBetsSuspiciousRates = new Bets
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
            _betRepository = Mock.Of<IBetRepository>(o =>
                o.GetSettledBets() == _settledBets);

            var engine = new SuspiciousBetManager(_betRepository);
            var actual = engine.GetCustomersWithSuspiciousWinRates();

            Assert.That(actual.Count(), Is.EqualTo(0));
        }

        [Test]
        public void GetCustomersWithSuspiciousWinRates_When_Two_Suspicious()
        {
            _betRepository = Mock.Of<IBetRepository>(o =>
                o.GetSettledBets() == _settledBetsSuspiciousRates);

            var engine = new SuspiciousBetManager(_betRepository);
            var actual = engine.GetCustomersWithSuspiciousWinRates();

            Assert.That(actual.Count(), Is.EqualTo(2));
            Assert.That(actual.Single(o => o.CustomerId == 1).WinRate, Is.EqualTo((decimal) 3 / (decimal) 2));
            Assert.That(actual.Single(o => o.CustomerId == 2).WinRate, Is.EqualTo(1));
        }

        [Test]
        public void GetUnsettledBetsFromSuspiciousCustomers_When_None_Suspicious()
        {
            _betRepository = Mock.Of<IBetRepository>(o =>
                o.GetSettledBets() == _settledBets &&
                o.GetUnsettledBets() == _unsettledBets);

            var engine = new SuspiciousBetManager(_betRepository);
            var actual = engine.GetUnsettledBetsFromSuspiciousWinRateCustomers();

            Assert.That(actual.Count(), Is.EqualTo(0));
        }

        [Test]
        public void GetUnsettledBetsFromSuspiciousCustomers_When_Two_Suspicious()
        {
            _betRepository = Mock.Of<IBetRepository>(o =>
                o.GetSettledBets() == _settledBetsSuspiciousRates &&
                o.GetUnsettledBets() == _unsettledBets);

            var engine = new SuspiciousBetManager(_betRepository);
            var actual = engine.GetUnsettledBetsFromSuspiciousWinRateCustomers();

            Assert.That(actual.Count(), Is.EqualTo(4));
            Assert.That(actual.Select(o => o.SuspiciousReason), Is.All.EqualTo(SuspiciousReason.SuspiciousWinRateCustomer));
            Assert.That(actual.Count(o => o.Bet.CustomerId == 1), Is.EqualTo(3));
            Assert.That(actual.Count(o => o.Bet.CustomerId == 2), Is.EqualTo(1));
        }

        [Test]
        public void GetUnsettledWithSuspiciousStakes_When_None_Suspicious()
        {
            _betRepository = Mock.Of<IBetRepository>(o =>
                o.GetSettledBets() == _settledBets &&
                o.GetUnsettledBets() == _unsettledBets);

            var engine = new SuspiciousBetManager(_betRepository);
            var actual = engine.GetUnsettledBetsWithSuspiciousStakes();

            Assert.That(actual.Count(), Is.EqualTo(0));
        }

        [Test]
        public void GetUnsettledWithSuspiciousStakes_When_Four_Suspicious()
        {
            _betRepository = Mock.Of<IBetRepository>(o =>
                o.GetSettledBets() == _settledBets &&
                o.GetUnsettledBets() == _unsettledBetsSuspiciousStake);

            var engine = new SuspiciousBetManager(_betRepository);
            var actual = engine.GetUnsettledBetsWithSuspiciousStakes();

            Assert.That(actual.Count(), Is.EqualTo(4));
            Assert.That(actual.Select(o => o.SuspiciousReason), Is.All.EqualTo(SuspiciousReason.SuspiciousStake));
            Assert.That(actual.Count(o => o.Bet.CustomerId == 1), Is.EqualTo(3));
            Assert.That(actual.Count(o => o.Bet.CustomerId == 2), Is.EqualTo(1));
        }

        [Test]
        public void GetUnsettledWithHighlySuspiciousStakes_When_None_HighlySuspicious()
        {
            _betRepository = Mock.Of<IBetRepository>(o =>
                o.GetSettledBets() == _settledBets &&
                o.GetUnsettledBets() == _unsettledBets);

            var engine = new SuspiciousBetManager(_betRepository);
            var actual = engine.GetUnsettledBetsWithHighlySuspiciousStakes();

            Assert.That(actual.Count(), Is.EqualTo(0));
        }

        [Test]
        public void GetUnsettledWithHighlySuspiciousStakes_When_Only_Suspicious()
        {
            _betRepository = Mock.Of<IBetRepository>(o =>
                o.GetSettledBets() == _settledBets &&
                o.GetUnsettledBets() == _unsettledBetsSuspiciousStake);

            var engine = new SuspiciousBetManager(_betRepository);
            var actual = engine.GetUnsettledBetsWithHighlySuspiciousStakes();

            Assert.That(actual.Count(), Is.EqualTo(0));
        }

        [Test]
        public void GetUnsettledWithHighlySuspiciousStakes_When_Four_HighlySuspicious()
        {
            _betRepository = Mock.Of<IBetRepository>(o =>
                o.GetSettledBets() == _settledBets &&
                o.GetUnsettledBets() == _unsettledBetsHighlySuspiciousStake);

            var engine = new SuspiciousBetManager(_betRepository);
            var actual = engine.GetUnsettledBetsWithHighlySuspiciousStakes();

            Assert.That(actual.Count(), Is.EqualTo(4));
            Assert.That(actual.Select(o => o.SuspiciousReason), Is.All.EqualTo(SuspiciousReason.HighlySuspiciousStake));
            Assert.That(actual.Count(o => o.Bet.CustomerId == 1), Is.EqualTo(3));
            Assert.That(actual.Count(o => o.Bet.CustomerId == 2), Is.EqualTo(1));
        }

        [Test]
        public void GetUnsettledWithHighWinAmount_When_None_Suspicious()
        {
            _betRepository = Mock.Of<IBetRepository>(o =>
                o.GetSettledBets() == _settledBets &&
                o.GetUnsettledBets() == _unsettledBets);

            var engine = new SuspiciousBetManager(_betRepository);
            var actual = engine.GetUnsettledWithHighWinAmount();

            Assert.That(actual.Count(), Is.EqualTo(0));
        }

        [Test]
        public void GetUnsettledWithWinAmount_When_Four_Suspicious()
        {
            _betRepository = Mock.Of<IBetRepository>(o =>
                o.GetSettledBets() == _settledBets &&
                o.GetUnsettledBets() == _unsettledBets);

            var engine = new SuspiciousBetManager(_betRepository);
            var actual = engine.GetUnsettledWithHighWinAmount();

            Assert.That(actual.Count(), Is.EqualTo(4));
            Assert.That(actual.Select(o => o.SuspiciousReason), Is.All.EqualTo(SuspiciousReason.HighWinRate));
            Assert.That(actual.Count(o => o.Bet.CustomerId == 1), Is.EqualTo(3));
            Assert.That(actual.Count(o => o.Bet.CustomerId == 2), Is.EqualTo(1));
        }
    }
}