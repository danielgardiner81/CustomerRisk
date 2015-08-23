using System.Collections.Generic;
using CustomerRisk.Model;

namespace CustomerRisk.Services
{
    public interface IBetRepository
    {
        IEnumerable<Bet> GetSettledBets();
        IEnumerable<Bet> GetUnsettledBets();
    }
}