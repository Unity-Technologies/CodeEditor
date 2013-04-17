using System;
using System.Collections.Generic;
using System.Net;
using CodeEditor.Reactive;
using CodeEditor.Reactive.Disposables;
using ServiceStack.Service;
using ServiceStack.ServiceClient.Web;
using ServiceStack.ServiceHost;

namespace CodeEditor.ReactiveServiceStack
{
	public interface IObservableServiceClient
	{
		IObservableX<TResponse> Observe<TResponse>(IReturn<TResponse> request);
		IObservableX<TResponse> ObserveMany<TResponse>(IReturn<IEnumerable<TResponse>> request);
	}

	public class ObservableServiceClient : IObservableServiceClient
	{
		static readonly TimeSpan Timeout = TimeSpan.FromSeconds(3);

		readonly string _baseUri;

		public ObservableServiceClient(string baseUri)
		{
			_baseUri = baseUri;
		}

		public IObservableX<TResponse> Observe<TResponse>(IReturn<TResponse> request)
		{
			return SendAsync<TResponse, TResponse>(request, onSuccess: (response, observer, disposable) =>
			{
				disposable.Disposable = null;
				observer.OnNext(response);
				observer.OnCompleted();
			});
		}

		public IObservableX<TResponse> ObserveMany<TResponse>(IReturn<IEnumerable<TResponse>> request)
		{
			return SendAsync<HttpWebResponse, TResponse>(request, onSuccess: (response, observer, disposable) =>
			{
				var responseStream = response.GetResponseStream();
				disposable.Disposable = responseStream.DeserializeMany<TResponse>().Subscribe(observer);
			});
		}

		IObservableX<TResult> SendAsync<TResponse, TResult>(object request, Action<TResponse, IObserverX<TResult>, MultipleAssignmentDisposable> onSuccess)
		{
			return ObservableX.CreateWithDisposable<TResult>(observer =>
			{
				var client = NewJsonServiceClient();
				var disposable = MultipleAssignmentDisposableFor(client);
				client.SendAsync<TResponse>(
					request,
					onSuccess: response => onSuccess(response, observer, disposable),
					onError: (response, exception) =>
					{
						disposable.Disposable = null;
						observer.OnError(exception);
					});
				return disposable;
			});
		}

		JsonServiceClient NewJsonServiceClient()
		{
			return new JsonServiceClient(_baseUri)
			{
				Timeout = Timeout
			};
		}

		static MultipleAssignmentDisposable MultipleAssignmentDisposableFor(IRestClientAsync client)
		{
			return new MultipleAssignmentDisposable
			{
				Disposable = Disposable.Create(client.CancelAsync)
			};
		}
	}
}