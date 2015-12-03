using System;
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
		IHandleMessages<LetterGenerated>
	{
		protected override void ConfigureHowToFindSaga(SagaPropertyMapper<HelloWorldData> mapper)
		{
			mapper.ConfigureMapping<KickOff>(x => x.RequestId).ToSaga(x => x.RequestId);
			mapper.ConfigureMapping<LetterGenerated>(x => x.RequestId).ToSaga(x => x.RequestId);
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
			Console.WriteLine("received {0}", message.Letter);
			Data.Chars[message.Order] = message.Letter;

			//check if pharase is complete
			if (Data.Chars.Any(x => x == '\0'))
				return;

			Console.WriteLine("*** [{0}]] ***", string.Join("", Data.Chars));

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
