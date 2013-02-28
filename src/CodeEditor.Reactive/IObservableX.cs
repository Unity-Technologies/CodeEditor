using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Linq;

namespace CodeEditor.Reactive
{
	public interface IObservableX<out T>
	{
		IDisposable Subscribe(IObserverX<T> observer);
	}

	public interface IObserverX<in T>
	{
		void OnNext(T value);
		void OnError(Exception exception);
		void OnCompleted();
	}

	public static class ObservableX
	{
		public static IObservableX<T> ObserveOnThreadPool<T>(this IObservableX<T> source)
		{
			return source.Map(_ => _.ObserveOn(Scheduler.ThreadPool));
		}

		public static IObservableX<T> Empty<T>()
		{
			return Observable.Empty<T>().ToObservableX();
		}

		public static IObservableX<T> Start<T>(Func<T> func)
		{
			return Observable.Start(func).ToObservableX();
		}

		public static IObservableX<T> Return<T>(T value)
		{
			return Observable.Return(value).ToObservableX();
		}

		public static IObservableX<T> Throw<T>(Exception exception)
		{
			return Observable.Throw<T>(exception).ToObservableX();
		}

		public static IObservableX<T> Catch<T>(this IObservableX<T> source, IObservableX<T> second)
		{
			return source.Map(_ => _.Catch(second.ToObservable()));
		}

		public static IDisposable Subscribe<T>(this IObservableX<T> source, Action<T> onNext)
		{
			return source.ToObservable().Subscribe(onNext);
		}

		public static IObservableX<TResult> Select<T, TResult>(this IObservableX<T> source, Func<T, TResult> selector)
		{
			return source.Map(_ => _.Select(selector));
		}

		public static IObservableX<TResult> SelectMany<T, TResult>(this IObservableX<T> source, Func<T, IEnumerable<TResult>> selector)
		{
			return source.Map(_ => _.SelectMany(selector));
		}

		public static IObservableX<TResult> SelectMany<T, TResult>(this IObservableX<T> source, Func<T, IObservableX<TResult>> selector)
		{
			Func<T, IObservable<TResult>> observableSelector = t => selector(t).ToObservable();
			return source.Map(_ => _.SelectMany(observableSelector));
		}

		public static IObservableX<T> Where<T>(this IObservableX<T> source, Func<T, bool> predicate)
		{
			return source.Map(_ => _.Where(predicate));
		}

		public static IObservableX<T> Do<T>(this IObservableX<T> source, Action<T> action)
		{
			return source.Map(_ => _.Do(action));
		}

		public static IObservableX<T> Remotable<T>(this IObservableX<T> source)
		{
			return new MarshalByRefObservableX<T>(source);
		}

		public class MarshalByRefObservableX<T> : MarshalByRefObject, IObservableX<T>
		{
			private readonly IObservableX<T> _source;

			public MarshalByRefObservableX(IObservableX<T> source)
			{
				_source = source;
			}

			public IDisposable Subscribe(IObserverX<T> observer)
			{
				return new MarshalByRefDisposable(_source.Subscribe(observer));
			}
		}

		internal class MarshalByRefDisposable : MarshalByRefObject, IDisposable
		{
			private readonly IDisposable _disposable;

			public MarshalByRefDisposable(IDisposable disposable)
			{
				_disposable = disposable;
			}

			public void Dispose()
			{
				_disposable.Dispose();
			}
		}

		public static IObservableX<T> Merge<T>(this IEnumerable<IObservableX<T>> sources)
		{
			return sources.Select(_ => _.ToObservable()).Merge().ToObservableX();
		}

		public static T FirstOrDefault<T>(this IObservableX<T> source)
		{
			return source.ToObservable().FirstOrDefault();
		}

		public static T FirstOrTimeout<T>(this IObservableX<T> source, TimeSpan timeout)
		{
			return source.ToObservable().Timeout(timeout, Observable.Throw<T>(new TimeoutException())).First();
		}

		public static IObservableX<IList<T>> ToList<T>(this IObservableX<T> source)
		{
			return source.Map(_ => _.ToList());
		}

		public static IObservableX<T> ToObservableX<T>(this IEnumerable<T> source)
		{
			return source.ToObservable().ToObservableX();
		}

		public static IObservableX<TResult> Map<T, TResult>(this IObservableX<T> source, Func<IObservable<T>, IObservable<TResult>> selector)
		{
			return selector(source.ToObservable()).ToObservableX();
		}
	}
}
