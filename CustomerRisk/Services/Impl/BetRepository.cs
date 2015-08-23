using System;
using System.IO.Abstractions;
using CustomerRisk.Model;

namespace CustomerRisk.Services.Impl
{
    public class BetRepository : IBetRepository
    {
        private readonly IFileSystem fileSystem;

        public BetRepository(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public Bets GetSettledBets()
        {
            throw new NotImplementedException();
        }

        public Bets GetUnsettledBets()
        {
            throw new NotImplementedException();
        }
    }
}