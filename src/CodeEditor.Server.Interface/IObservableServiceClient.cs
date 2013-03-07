using System;
using System.Collections.Generic;
using System.Net;
using CodeEditor.Reactive;
using ServiceStack.ServiceHost;
using ServiceStack.Text;

namespace CodeEditor.Server.Interface
{
	public interface IObservableServiceClient
	{
		IObservableX<TResponse> ObserveMany<TResponse>(IReturn<IEnumerable<TResponse>> request);
	}

	public class ObservableServiceClient : IObservableServiceClient
	{
		private const int DefaultTimeout = 1000;

		private readonly string _baseUri;

		public ObservableServiceClient(string baseUri)
		{
			_baseUri = baseUri.WithTrailingSlash();
		}

		public IObservableX<TResponse> ObserveMany<TResponse>(IReturn<IEnumerable<TResponse>> request)
		{
			var absoluteUri = _baseUri + DefaultRouteFor(request).TrimStart('/');
			var queryString = QueryStringSerializer.SerializeToString(request);
			var requestUri = string.IsNullOrEmpty(queryString) ? absoluteUri : absoluteUri + "?" + queryString;

			var webRequest = WebRequest.Create(requestUri);
			webRequest.Timeout = DefaultTimeout;

			var observableResponse = ObservableX.FromAsyncPattern(webRequest.BeginGetResponse, webRequest.EndGetResponse);
			return observableResponse()
				.SelectMany(_ => _.GetResponseStream().DeserializeMany<TResponse>());
		}

		private static string DefaultRouteFor(object request)
		{
			return ((RouteAttribute)Attribute.GetCustomAttribute(request.GetType(), typeof(RouteAttribute))).Path;
		}
	}

	/* 
	 * ServiceStack async API doesn't play nice with unity
	 * because of its Timer + WebRequest.Abort approach to timeouts
	 * 
	public class ObservableServiceClient : IObservableServiceClient
	{
		readonly string _baseUri;

		public ObservableServiceClient(string baseUri)
		{
			_baseUri = baseUri;
		}

		public IObservableX<TResponse> ObserveMany<TResponse>(IReturn<IEnumerable<TResponse>> request)
		{
			return ObservableX.Create<TResponse>(observer =>
			{
				var client = new JsonServiceClient(_baseUri) {Timeout = TimeSpan.FromSeconds(1)};
				var disposable = new MultipleAssignmentDisposable
				{
					Disposable = Disposable.Create(client.CancelAsync)
				};

				client.SendAsync<HttpWebResponse>(
					request,
					onSuccess: response =>
					{
						var responseStream = response.GetResponseStream();
						disposable.Disposable = responseStream.DeserializeMany<TResponse>().Subscribe(observer);
					},
					onError: (response, exception) =>
					{
						disposable.Disposable = null;
						observer.OnError(exception);
					});

				return disposable.Dispose;
			});
		}
	}*/
}