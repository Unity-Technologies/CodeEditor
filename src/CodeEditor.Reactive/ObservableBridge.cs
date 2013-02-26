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
				Console.Error.WriteLine("Subscribing {0}", observer);
				return _observable.Subscribe(
					value =>
					{
						Console.Error.WriteLine("sending {0}", value);
						try
						{
							observer.OnNext(value);
						}
						catch (Exception x)
						{
							Console.Error.WriteLine("error: {0}", x);
						}
					},
					exception => observer.OnError(exception),
					() =>
					{
						Console.Error.WriteLine("sending OnCompleted");
						try
						{
							observer.OnCompleted();
						}
						catch (Exception x)
						{
							Console.Error.WriteLine("error: {0}", x);
						}
					});
			}
		}

		public sealed class ObserverX<T> : MarshalByRefObject, IObserverX<T>
		{
			private readonly IObserver<T> _observer;

			public ObserverX(IObserver<T> observer)
			{
				_observer = observer;
			}

			void IObserverX<T>.OnNext(T value)
			{
				Console.Error.WriteLine("ObserverX.OnNext({0})", value);
				_observer.OnNext(value);
			}

			void IObserverX<T>.OnCompleted()
			{
				Console.Error.WriteLine("ObserverX.OnCompleted()");
				_observer.OnCompleted();
			}

			void IObserverX<T>.OnError(Exception exception)
			{
				Console.Error.WriteLine("ObserverX.OnError() -> {0}", exception);
				_observer.OnError(exception);
			}
		}
	}
}