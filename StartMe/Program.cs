using System;
using Contracts;
using NServiceBus;
using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;
using NServiceBus.Logging;

namespace StartMe
{
	class Program
	{
		// ReSharper disable once UnusedParameter.Local
		private static void Main(string[] args)
		{
			LogManager.Use<DefaultFactory>().Level(LogLevel.Error);

			var busConfiguration = new BusConfiguration();
			busConfiguration.UsePersistence<InMemoryPersistence>();

			busConfiguration.EnableInstallers();

			using (var bus = Bus.Create(busConfiguration))
			{
				bus.Start();

				bus.Send(new KickOff {RequestId = "FetchHelloWorld", Phrase = "Hello World"});

				Console.WriteLine("Hello World is being over-engineered, hold tight.\r\nPress any key to exit if you give up.\r\n\r\n");

				Console.ReadKey();

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
}
