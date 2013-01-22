using System;
using System.Runtime.Remoting;

namespace CodeEditor.Composition.Client.Implementation
{
	[Export(typeof(ICompositionClientProvider))]
	public class CompositionClientProvider : ICompositionClientProvider
	{
		static CompositionClientProvider()
		{
			// callback channel
			TcpChannelServices.RegisterTcpChannelOnPort(8889);
		}

		public IServiceProvider CompositionClientFor(string address)
		{
			var proxy = RemotingServices.Connect(typeof(IServiceProvider), address);
			return (IServiceProvider)proxy;
		}
	}
}
