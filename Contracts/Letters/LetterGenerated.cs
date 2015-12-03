﻿using NServiceBus;

namespace Contracts.Letters
{
	public class LetterGenerated : IEvent
	{
		public string RequestId { get; set; }
		public char Letter { get; set; }
	}
}
