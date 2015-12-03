using System;
using Contracts.Letters;
using NServiceBus;

namespace NsbHelloWorld.LetterGenerator
{
	class LetterHandler : IHandleMessages<LetterRequest>
	{
		private readonly IBus _bus;

		public LetterHandler(IBus bus)
		{
			_bus = bus;
		}

		public void Handle(LetterRequest message)
		{
			var rnd = new Random(DateTime.Now.Millisecond);

			if(rnd.Next(0, 42) % 2 == 0)
				throw new RankException();

			_bus.Publish(new LetterGenerated { RequestId = message.RequestId, Letter = message.Letter, Order = message.Order });

			Console.WriteLine("generated {0}", message.Letter);
		}
	}
}
