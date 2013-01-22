using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeEditor.Composition.Client.Tests.Fixtures
{
	public interface IPingPonger
	{
		void Ping(int pingerId);
		IObservable<Pong> Pong { get; }
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
		private readonly Subject<Pong> _pongSubject;

		public PingPonger()
		{
			_pongSubject = new Subject<Pong>();
			Pong = _pongSubject.Remotable();
		}

		public IObservable<Pong> Pong { get; private set; }

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
