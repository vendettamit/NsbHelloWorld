using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Contracts.Letters;
using NServiceBus;
using NServiceBus.Saga;

namespace StartMe
{
	class HelloWorldSaga : Saga<HelloWorldData>,
		IAmStartedByMessages<KickOff>,
		IHandleMessages<LetterGenerated>,
		IHandleMessages<LetterOrdered>
	{
		protected override void ConfigureHowToFindSaga(SagaPropertyMapper<HelloWorldData> mapper)
		{
			mapper.ConfigureMapping<KickOff>(x => x.RequestId).ToSaga(x => x.RequestId);
			mapper.ConfigureMapping<LetterGenerated>(x => x.RequestId).ToSaga(x => x.RequestId);
			mapper.ConfigureMapping<LetterOrdered>(x => x.RequestId).ToSaga(x => x.RequestId);
		}

		public void Handle(KickOff message)
		{
			Data.RequestId = message.RequestId;
			Data.Chars = new char[message.Phrase.Length];

			for(var i = 0; i < message.Phrase.Length; i++)
			{
				var n = i;
				Task.Run(() =>
				{
					Task.Delay(new Random(DateTime.Now.Millisecond).Next(1, 420));
					Bus.Send(new LetterRequest {RequestId = message.RequestId, Letter = message.Phrase[n], Order = n});
				});
			}

			Console.WriteLine("Saga Kicked Off");
		}

		public void Handle(LetterGenerated message)
		{
			Bus.Send(new OrderLetter {RequestId = message.RequestId, Letter = message.Letter});
		}

		public void Handle(LetterOrdered message)
		{
			//discard unnecessary letters
			if(message.Order < 0)
				return;

			//discard duplicates
			if (Data.Chars[message.Order] != '\0')
				return;

			Data.Chars[message.Order] = message.Letter;
			Console.WriteLine("received {0}, [{1}]", message.Letter, string.Join("", Data.Chars));

			//check if pharase is complete
			if (Data.Chars.Any(x => x == '\0'))
				return;

			Console.WriteLine("*** [{0}]] ***", string.Join("", Data.Chars));

			Bus.Send(new KillGenerator());

			MarkAsComplete();
		}
	}

	class HelloWorldData : ContainSagaData
	{
		[Unique]
		public string RequestId { get; set; }
		public char[] Chars { get; set; }
	}
}
