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
			var serverProperties = new Hashtable { { "includeVersions", false }, { "strictBinding", false } };
			var serverProvider = new BinaryServerFormatterSinkProvider(serverProperties, null) { TypeFilterLevel = TypeFilterLevel.Full };
			var clientProvider = new BinaryClientFormatterSinkProvider((IDictionary) serverProperties.Clone(), null);
			var channelProperties = new Hashtable { {"port", port} };
			ChannelServices.RegisterChannel(new TcpChannel(channelProperties, clientProvider, serverProvider), false);
		}
	}
}