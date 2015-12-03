using System;
using System.Threading;
using Contracts.Letters;
using NServiceBus;

namespace NsbHelloWorld.LetterGenerator
{
	class LetterGenerator : IWantToRunWhenBusStartsAndStops, IHandleMessages<KillGenerator>
	{
		private readonly IBus _bus;
		private Thread _thread;

		public LetterGenerator(IBus bus)
		{
			_bus = bus;
		}

		public void Start()
		{
			_thread = new Thread(ThreadProc);
			_thread.Start("FetchHelloWorld");
		}

		public void Stop()
		{
			if (_thread != null)
			{
				_thread.Interrupt();
				_thread = null;
			}
		}

		private void ThreadProc(object param)
		{
			var requestId = param.ToString();

			var rnd = new Random(DateTime.Now.Millisecond);
			while (true)
			{
				try
				{
					var n = rnd.Next('A', 'z');

					//remap char to compensate for space
					if (n == 96) n = 32;

					//prioritize some chars
					if (n >= 58 && n <= 64) n = 'l';
					if (n >= 91 && n < 96) n = 'o';

					var letterGenerated = new LetterGenerated { RequestId = requestId, Letter = (char)n };
					_bus.Publish(letterGenerated);

					Console.WriteLine("published {0}", letterGenerated.Letter);

					Thread.Sleep(n);
				}
				catch (ThreadInterruptedException)
				{
					return;
				}
			}
		}

		public void Handle(KillGenerator message)
		{
			Stop();
		}
	}
}
