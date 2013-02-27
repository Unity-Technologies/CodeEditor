using System;
using CodeEditor.Reactive;
using CodeEditor.Reactive.Subjects;

namespace CodeEditor.Composition.Client.Tests.Fixtures
{
	public interface IPingPonger
	{
		void Ping(int pingerId);
		IObservableX<Pong> Pong { get; }
	}
	
	[Serializable]
	public struct Pong
	{
		public int PingerId;
		public int PongerId;
	}

	[Export(typeof(IPingPonger))]
	class PingPonger : MarshalByRefObject, IPingPonger
	{
		private readonly ISubjectX<Pong> _pongSubject;

		public PingPonger()
		{
			_pongSubject = SubjectX.Create<Pong>();
			Pong = _pongSubject.Remotable();
		}

		public IObservableX<Pong> Pong { get; private set; }

		public void Ping(int pingerId)
		{
			_pongSubject.OnNext(
				new Pong
				{
					PingerId = pingerId,
					PongerId = CurrentProcessId
				});
		}

		private static int CurrentProcessId
		{
			get { return System.Diagnostics.Process.GetCurrentProcess().Id; }
		}
	}
}
