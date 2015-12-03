using System;
using NServiceBus;
using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;
using NServiceBus.Logging;

namespace NsbHelloWorld.LetterGenerator
{
	class Program
	{
		static void Main(string[] args)
		{
			LogManager.Use<DefaultFactory>().Level(LogLevel.Error);

			var busConfiguration = new BusConfiguration();
			busConfiguration.UsePersistence<InMemoryPersistence>();

			busConfiguration.EnableInstallers();

			using (var bus = Bus.Create(busConfiguration))
			{
				bus.Start();

				Console.WriteLine("Press any key to exit");

				Console.ReadKey();

			}

		}
	}

	class CustomConfig : IProvideConfiguration<MessageForwardingInCaseOfFaultConfig>,
	IProvideConfiguration<AuditConfig>
	{
		AuditConfig IProvideConfiguration<AuditConfig>.GetConfiguration()
		{
			return new AuditConfig
			{
				QueueName = "audit"
			};
		}

		public MessageForwardingInCaseOfFaultConfig GetConfiguration()
		{
			return new MessageForwardingInCaseOfFaultConfig
			{
				ErrorQueue = "error"
			};
		}
	}
}
