using System;
using System.Collections.Generic;

namespace CodeEditor.Reactive.Subjects
{
	public interface ISubjectX<T> : IObservableX<T>, IObserverX<T>
	{
	}

	public static class SubjectX
	{
		public static ISubjectX<T> Create<T>()
		{
			var subject = new Subject<T>();
			return Create(subject.ToObservableX(), subject.ToObserverX());
		}

		private static ISubjectX<T> Create<T>(IObservableX<T> observable, IObserverX<T> observer)
		{
			return new SubjectX<T>(observable, observer);
		}
	}

	class SubjectX<T> : ISubjectX<T>
	{
		private readonly IObservableX<T> _observable;
		private readonly IObserverX<T> _observer;

		public SubjectX(IObservableX<T> observable, IObserverX<T> observer)
		{
			_observable = observable;
			_observer = observer;
		}

		public IDisposable Subscribe(IObserverX<T> observer)
		{
			return _observable.Subscribe(observer);
		}

		public void OnNext(T value)
		{
			_observer.OnNext(value);
		}

		public void OnCompleted()
		{
			_observer.OnCompleted();
		}

		public void OnError(Exception exception)
		{
			_observer.OnError(exception);
		}
	}
}