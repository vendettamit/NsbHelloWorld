using System;
using System.Linq;
using Contracts.Letters;
using NServiceBus;

namespace NsbHelloWorld.LetterGenerator
{
	class LetterOrderHandler : IHandleMessages<OrderLetter>
	{
		private readonly IBus _bus;
		private string _s = "Hello World";

		public LetterOrderHandler(IBus bus)
		{
			_bus = bus;
		}

		public void Handle(OrderLetter message)
		{
			var requestId = message.RequestId;

			var index = _s.IndexOf(message.Letter);
			if (index == _s.LastIndexOf(message.Letter))
			{
				if (index < 0) return;

				_bus.Publish(new LetterOrdered { Letter = message.Letter, Order = index, RequestId = requestId});
				return;
			}

			var rnd = new Random(DateTime.Now.Millisecond);
			var letterCount = _s.ToCharArray().Where(x => x == message.Letter).Count();
			var sequence = rnd.Next(0, letterCount+1);
			var c = 0;
			for(var i = 0; i< _s.Length; i++)
			{
				if (_s[i] == message.Letter) c++;

				if (c == sequence && message.Letter == _s[i])
				{
					_bus.Publish(new LetterOrdered { Letter = message.Letter, Order = i, RequestId = requestId });
				}
			}

		}
	}
}
