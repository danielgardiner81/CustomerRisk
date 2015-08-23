using CustomerRisk.Model;

namespace CustomerRisk.Services
{
    public interface IBetRepository
    {
        Bets GetSettledBets();
        Bets GetUnsettledBets();
    }
}