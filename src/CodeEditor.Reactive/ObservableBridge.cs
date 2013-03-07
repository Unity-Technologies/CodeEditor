using System;
using System.Linq;

namespace CodeEditor.Reactive
{
	internal static class ObservableBridge
	{
		public static IObservable<T> ToObservable<T>(this IObservableX<T> observableX)
		{
			var adapter = observableX as ObservableX<T>;
			return adapter != null 
				? adapter.Observable
				: Observable.Create<T>(subscribe: observer => observableX.Subscribe(observer.ToObserverX()).Dispose);
		}

		public static IObserverX<T> ToObserverX<T>(this IObserver<T> observer)
		{
			return new ObserverX<T>(observer);
		}

		public static IObservableX<T> ToObservableX<T>(this IObservable<T> observable)
		{
			return new ObservableX<T>(observable);
		}

		class ObservableX<T> : IObservableX<T>
		{
			private readonly IObservable<T> _observable;

			public ObservableX(IObservable<T> observable)
			{
				_observable = observable;
			}

			public IObservable<T> Observable
			{
				get { return _observable; }
			}

			public IDisposable Subscribe(IObserverX<T> observer)
			{
				var observerX = observer as ObserverX<T>;
				if (observerX != null)
					return _observable.Subscribe(observerX.Observer);
				return _observable.Subscribe(
					observer.OnNext,
					observer.OnError,
					observer.OnCompleted);
			}
		}

		public sealed class ObserverX<T> : IObserverX<T>
		{
			private readonly IObserver<T> _observer;

			public ObserverX(IObserver<T> observer)
			{
				_observer = observer;
			}

			public IObserver<T> Observer
			{
				get { return _observer; }
			}

			void IObserverX<T>.OnNext(T value)
			{
				_observer.OnNext(value);
			}

			void IObserverX<T>.OnCompleted()
			{
				_observer.OnCompleted();
			}

			void IObserverX<T>.OnError(Exception exception)
			{
				_observer.OnError(exception);
			}
		}
	}
}