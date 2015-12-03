using NServiceBus;

namespace Contracts.Letters
{
	public class OrderLetter : IMessage
	{
		public string RequestId { get; set; }
		public char Letter { get; set; }
	}
}
