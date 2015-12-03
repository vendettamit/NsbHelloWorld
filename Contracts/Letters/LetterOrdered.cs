﻿using NServiceBus;

namespace Contracts.Letters
{
	public class LetterOrdered: IEvent
	{
		public string RequestId { get; set; }
		public char Letter { get; set; }
		public int Order { get; set; }
	}
}
