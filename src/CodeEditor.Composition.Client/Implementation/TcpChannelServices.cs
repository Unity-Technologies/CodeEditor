using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;

namespace CodeEditor.Composition.Client.Implementation
{
	public static class TcpChannelServices
	{
		public static void RegisterTcpChannelOnPort(int port)
		{
			var serverProvider = new BinaryServerFormatterSinkProvider { TypeFilterLevel = TypeFilterLevel.Full };
			var clientProvider = new BinaryClientFormatterSinkProvider();
			var properties = new Hashtable { { "port", port } };
			ChannelServices.RegisterChannel(new TcpChannel(properties, clientProvider, serverProvider), false);
		}
	}
}