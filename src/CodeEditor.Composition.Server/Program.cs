using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using CodeEditor.Composition.Client.Implementation;

namespace CodeEditor.Composition.Server
{
	/// <summary>
	/// Generic service server.
	/// 
	/// Exposes a single entry point of type IServiceProvider allowing clients
	/// to ask for any service available in the server's container.
	/// </summary>
	public class Program
	{
		static void Main()
		{
			Trace.Listeners.Add(new ConsoleTraceListener(true));

			TcpChannelServices.RegisterTcpChannelOnPort(8888);

			var container = new CompositionContainerServer();
			RemotingServices.Marshal(container, "IServiceProvider");

			Console.WriteLine("Press <ENTER> to quit");
			Console.ReadLine();
		}
	}
}
