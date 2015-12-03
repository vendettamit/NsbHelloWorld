using NServiceBus;

namespace Contracts
{
	public class KickOff : IMessage
	{
		public string RequestId { get; set; }
		public string Phrase { get; set; }
	}
}
