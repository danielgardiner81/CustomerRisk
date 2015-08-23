using System.Linq;
using CustomerRisk.Model;
using CustomerRisk.Services;
using CustomerRisk.Services.Impl;
using Moq;
using NUnit.Framework;

namespace CustomerRisk.Tests
{
    [TestFixture]
    public class RiskAnalysisEngineTests
    {
        private IBetRepository _betRepository;

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void GetSuspiciousWinRates_None_Suspicious()
        {
            _betRepository = Mock.Of<IBetRepository>(o => o.GetSettledBets() == new Bets
                {
                    new Bet { CustomerId = 1, EventId = 1, ParticipantId = 1, Stake = 50, Win = 0 },
                    new Bet { CustomerId = 1, EventId = 2, ParticipantId = 1, Stake = 50, Win = 500 },
                    new Bet { CustomerId = 1, EventId = 3, ParticipantId = 1, Stake = 50, Win = 0 },
                });

            var engine = new RiskAnalysisEngine(_betRepository);
            var actual = engine.GetCustomersWithSuspiciousWinRates();

            Assert.That(actual.Count(), Is.EqualTo(0));
        }

        [Test]
        public void GetSuspiciousWinRates_Two_Suspicious()
        {
            _betRepository = Mock.Of<IBetRepository>(o => o.GetSettledBets() == new Bets
                {
                    new Bet { CustomerId = 1, EventId = 1, ParticipantId = 1, Stake = 50, Win = 0 },
                    new Bet { CustomerId = 1, EventId = 2, ParticipantId = 1, Stake = 50, Win = 500 },
                    new Bet { CustomerId = 1, EventId = 3, ParticipantId = 1, Stake = 50, Win = 500 },
                    new Bet { CustomerId = 2, EventId = 1, ParticipantId = 1, Stake = 50, Win = 500 },
                });

            var engine = new RiskAnalysisEngine(_betRepository);
            var actual = engine.GetCustomersWithSuspiciousWinRates();

            Assert.That(actual.Count(), Is.EqualTo(2));
            Assert.That(actual.Single(o => o.CustomerId == 1).WinRate, Is.EqualTo((decimal) 3 / (decimal) 2));
            Assert.That(actual.Single(o => o.CustomerId == 2).WinRate, Is.EqualTo(1));
        }
    }
}