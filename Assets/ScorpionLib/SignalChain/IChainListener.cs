using System;

namespace SignalChain
{
	public interface IChainListener
	{
		void Listen<T>(T message);
	}
}

