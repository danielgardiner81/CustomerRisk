using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using CsvHelper;
using CustomerRisk.Model;

namespace CustomerRisk.Services.Impl
{
    public class BetRepository : IBetRepository
    {
        private readonly IFileSystem _fileSystem;

        public BetRepository(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public IEnumerable<Bet> GetSettledBets()
        {
            return ReadBets("resources/settled.csv");
        }

        public IEnumerable<Bet> GetUnsettledBets()
        {
            return ReadBets("resources/unsettled.csv");
        }

        private IEnumerable<Bet> ReadBets(string resourcesSettledCsv)
        {
            var bets = new List<Bet>();
            using (var reader = _fileSystem.File.OpenText(resourcesSettledCsv))
            using (var csvReader = new CsvReader(reader))
            {
                while (csvReader.Read())
                {
                    bets.Add(csvReader.GetRecord<Bet>());
                }
            }

            return bets;
        }
    }
}