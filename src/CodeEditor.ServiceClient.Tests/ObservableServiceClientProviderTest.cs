using System;
using System.IO;
using CodeEditor.IO;
using CodeEditor.Logging;
using CodeEditor.Reactive;
using CodeEditor.Testing;
using NUnit.Framework;

namespace CodeEditor.ServiceClient.Tests
{
	[TestFixture]
	public class ObservableServiceClientProviderTest : MockBasedTest
	{
		[Test]
		public void GetsServerAddressFromUriFile()
		{
			const string serverExecutable = "server.exe";
			const string uriFilePath = "server.uri";
			const string serverAddress = "http://localhost:1337/";

			// provider tries to delete pid file to decide if it needs
			// to start the server
			var fileSystem = MockFor<IFileSystem>();
			var uriFile = MockFor<IFile>();
			fileSystem
				.Setup(_ => _.FileFor(uriFilePath))
				.Returns(uriFile.Object);

			// throw IOException when provider tries to delete the file
			// to signal server is already running
			uriFile
				.Setup(_ => _.Delete())
				.Throws(new IOException());

			uriFile
				.Setup(_ => _.ReadAllText())
				.Returns(serverAddress);
			
			var subject = new ObservableServiceClientProvider
			{
				FileSystem = fileSystem.Object,
				Logger = new StandardLogger()
			};
			Assert.IsNotNull(subject.ClientFor(serverExecutable).FirstOrTimeout(TimeSpan.FromSeconds(1)));

			VerifyAllMocks();
		}
	}
}