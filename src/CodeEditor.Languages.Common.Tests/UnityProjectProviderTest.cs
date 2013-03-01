using System;
using System.IO;
using CodeEditor.Composition;
using CodeEditor.Composition.Client;
using CodeEditor.IO;
using CodeEditor.Testing;
using NUnit.Framework;

namespace CodeEditor.Languages.Common.Tests
{
	[TestFixture]
	public class UnityProjectProviderTest : MockBasedTest
	{
		[Test]
		public void GetsProjectLocationFromLocationProviderAndAddressFromPidFile()
		{
			const string projectFolder = "/Project";
			const string serverAddress = "tcp://localhost:4242/IServiceProvider";

			var projectPathProvider = MockFor<IUnityProjectPathProvider>();
			projectPathProvider
				.SetupGet(_ => _.Location)
				.Returns(projectFolder);

			var pidFilePath = Path.Combine(projectFolder, "Library/CodeEditor/Server/CodeEditor.Composition.Server.pid");

			// provider tries to delete pid file to decide if it needs
			// to start the server
			var fileSystem = MockFor<IFileSystem>();
			var pidFile = MockFor<IFile>();
			fileSystem
				.Setup(_ => _.FileFor(pidFilePath))
				.Returns(pidFile.Object);

			pidFile
				.Setup(_ => _.Delete())
				.Throws(new IOException());

			pidFile
				.Setup(_ => _.ReadAllText())
				.Returns(serverAddress);
			
			var subject = new ObservableServiceClientProvider
			{
				ProjectPathProvider = projectPathProvider.Object,
				FileSystem = fileSystem.Object,
				Logger = new StandardLogger()
			};
			Assert.IsNotNull(subject.Client);

			VerifyAllMocks();
		}
	}
}