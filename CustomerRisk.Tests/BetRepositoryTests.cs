using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using CustomerRisk.Model;
using CustomerRisk.Services.Impl;
using NUnit.Framework;

namespace CustomerRisk.Tests
{
    [TestFixture]
    public class BetRepositoryTests
    {
        private MockFileSystem _mockFileSystem;

        [SetUp]
        public void Setup()
        {
            _mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
        }

        [Test]
        public void GetSettledBets()
        {
            _mockFileSystem.AddFile("resources/settled.csv", new MockFileData(@"Customer,Event,Participant,Stake,Win
1,1,6,50,250
2,1,3,5,0"));

            var actual = Test(repo => repo.GetSettledBets());

            Assert.That(actual.Count(), Is.EqualTo(4));

            Assert.That(actual.Skip(0).First().CustomerId, Is.EqualTo(1));
            Assert.That(actual.Skip(0).First().EventId, Is.EqualTo(1));
            Assert.That(actual.Skip(0).First().ParticipantId, Is.EqualTo(6));
            Assert.That(actual.Skip(0).First().Stake, Is.EqualTo(50));
            Assert.That(actual.Skip(0).First().Win, Is.EqualTo(250));

            Assert.That(actual.Skip(1).First().CustomerId, Is.EqualTo(2));
            Assert.That(actual.Skip(1).First().EventId, Is.EqualTo(1));
            Assert.That(actual.Skip(1).First().ParticipantId, Is.EqualTo(3));
            Assert.That(actual.Skip(1).First().Stake, Is.EqualTo(5));
            Assert.That(actual.Skip(1).First().Win, Is.EqualTo(0));
        }

        [Test]
        public void GetUnsettledBets()
        {
            _mockFileSystem.AddFile("resources/settled.csv", new MockFileData(@"Customer,Event,Participant,Stake,To Win
1,11,4,50,500
3,11,6,50,400"));

            var actual = Test(repo => repo.GetUnsettledBets());

            Assert.That(actual.Count(), Is.EqualTo(4));

            Assert.That(actual.Skip(0).First().CustomerId, Is.EqualTo(1));
            Assert.That(actual.Skip(0).First().EventId, Is.EqualTo(11));
            Assert.That(actual.Skip(0).First().ParticipantId, Is.EqualTo(4));
            Assert.That(actual.Skip(0).First().Stake, Is.EqualTo(50));
            Assert.That(actual.Skip(0).First().Win, Is.EqualTo(500));

            Assert.That(actual.Skip(1).First().CustomerId, Is.EqualTo(3));
            Assert.That(actual.Skip(1).First().EventId, Is.EqualTo(11));
            Assert.That(actual.Skip(1).First().ParticipantId, Is.EqualTo(6));
            Assert.That(actual.Skip(1).First().Stake, Is.EqualTo(50));
            Assert.That(actual.Skip(1).First().Win, Is.EqualTo(400));
        }

        private IEnumerable<Bet> Test(Func<BetRepository, IEnumerable<Bet>> func)
        {
            var betRepository = new BetRepository(_mockFileSystem);
            return func(betRepository);
        }
    }
}