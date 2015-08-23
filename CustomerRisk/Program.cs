using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Autofac;
using CustomerRisk.Model;
using CustomerRisk.Services;
using CustomerRisk.Services.Impl;

namespace CustomerRisk
{
    internal class Program
    {
        private static IContainer _container;
        private static ISuspiciousBetManager _suspiciousBetManager;
        private static IBetRepository _betRepository;

        private static void SetupIoC()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<FileSystem>().AsImplementedInterfaces();
            builder.RegisterType<BetRepository>().AsImplementedInterfaces();
            builder.RegisterType<SuspiciousBetManager>().AsImplementedInterfaces();
            _container = builder.Build();

            _suspiciousBetManager = _container.Resolve<ISuspiciousBetManager>();
            _betRepository = _container.Resolve<IBetRepository>();
        }

        private static void Main()
        {
            SetupIoC();

            WriteSuspiciousWinRateCustomers();

            WriteSuspiciousBets();

            Console.ResetColor();
            Console.WriteLine("");
            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();
        }

        private static void WriteSuspiciousBets()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n\n\nSuspicious unsettled bets\n");           
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\t{"CustomerId",-20}{"EventId",-20}{"ParticipantId",-20}{"Stake",-20}{"Win",-20}{"Reason"}");
            Console.ResetColor();

            // TODO: if there was more time and the assumption holds that all the data in settled / unsettled
            // TODO: represents unique bets we could add and id to the bets to group suspicious bets to remove the rows

            var suspiciousBets = _suspiciousBetManager
                .GetUnsettledBetsFromSuspiciousWinRateCustomers()
                .Union(_suspiciousBetManager.GetUnsettledWithHighWinAmount())
                .Union(_suspiciousBetManager.GetUnsettledBetsWithSuspiciousStakes())
                .Union(_suspiciousBetManager.GetUnsettledBetsWithHighlySuspiciousStakes())
                .ToList();

            WriteSuspiciousBets(suspiciousBets);
        }

        private static void WriteSuspiciousWinRateCustomers()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\nCustomers with suspicious win rates\n");            
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\t{"CustomerId",-20}{"Avg Stake",-20}{"WinRate",-20}{"Reason"}");
            Console.ResetColor();

            foreach (var customer in _suspiciousBetManager.GetCustomersWithSuspiciousWinRates().OrderBy(o => o.CustomerId))
            {
                Console.Write($"\t{customer.CustomerId,-20}{customer.StakeAverage,-20:C}{customer.WinRate,-20:P}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Suspicious Win Rate");
            }
        }

        private static void WriteSuspiciousBets(IEnumerable<SuspiciousBet> suspiciousBets)
        {
            foreach (var suspicious in suspiciousBets.OrderBy(o => o.Bet.CustomerId).ThenByDescending(o => o.Bet.Win))
            {
                Console.Write($"\t{suspicious.Bet.CustomerId,-20}{suspicious.Bet.EventId,-20}{suspicious.Bet.ParticipantId,-20}{suspicious.Bet.Stake,-20:C}{suspicious.Bet.Win,-20:C}");

                Console.ForegroundColor = GetReasonColor(suspicious);

                Console.WriteLine($"{MakeFriendlyReason(suspicious.SuspiciousReason.ToString()),-20}");
                Console.ResetColor();
            }
        }

        private static ConsoleColor GetReasonColor(SuspiciousBet suspicious)
        {
            switch (suspicious.SuspiciousReason)
            {
                case SuspiciousReason.SuspiciousWinRateCustomer:
                    return ConsoleColor.Yellow;
                case SuspiciousReason.SuspiciousStake:
                    return ConsoleColor.DarkRed;
                case SuspiciousReason.HighlySuspiciousStake:
                    return ConsoleColor.Red;
                case SuspiciousReason.HighWinAmount:
                    return ConsoleColor.DarkYellow;
                default:
                    throw new ArgumentOutOfRangeException(nameof(suspicious.SuspiciousReason));
            }
        }

        private static string MakeFriendlyReason(string text)
        {
            for (var i = 1; i < text.Length - 1; i++)
            {
                if (char.IsLower(text[i - 1]) && char.IsUpper(text[i]) || text[i - 1] != ' ' && char.IsUpper(text[i]) && char.IsLower(text[i + 1]))
                {
                    text = text.Insert(i, " ");
                }
            }

            return text;
        }
    }
}