using System.IO.Abstractions;
using Autofac;
using Autofac.Core;
using CustomerRisk.Services.Impl;

namespace CustomerRisk
{
    internal class Program
    {
        private IContainer _container;

        public Program(Container container)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<FileSystem>().AsImplementedInterfaces();
            builder.RegisterType<BetRepository>().AsImplementedInterfaces();
            builder.RegisterType<SuspiciousBetManager>().AsImplementedInterfaces();
            _container = builder.Build();
        }

        private static void Main(string[] args)
        {
        }
    }
}