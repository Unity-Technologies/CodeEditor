using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
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
			
			using (var pidFileWriter = new StreamWriter(File.Open(PidFile, FileMode.Create, FileAccess.Write, FileShare.Read)))
			{
				pidFileWriter.Write("tcp://localhost:8888/IServiceProvider");
				pidFileWriter.Flush();

				TcpChannelServices.RegisterTcpChannelOnPort(8888);

				var container = new CompositionContainerServer();
				RemotingServices.Marshal(container, "IServiceProvider");

				Console.WriteLine("Press <ENTER> to quit");
				Console.ReadLine();
			}
		}

		protected static string PidFile
		{
			get { return Path.ChangeExtension(typeof(Program).Module.FullyQualifiedName, "pid"); }
		}
	}
}
