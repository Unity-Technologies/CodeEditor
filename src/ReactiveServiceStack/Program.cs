using System;
using System.Collections.Generic;
using System.Disposables;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Funq;
using ServiceStack.Common.Web;
using ServiceStack.Service;
using ServiceStack.ServiceClient.Web;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Providers;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;

namespace ReactiveServiceStack
{
	[Route("/search")]
	public class SearchRequest : IReturn<IEnumerable<SearchResponse>>
	{
		public string Filter { get; set; }
	}

	public class SearchResponse
	{
		public string DisplayText { get; set; }
		public MatchKind MatchKind { get; set; }
	}

	public enum MatchKind
	{
		Exact,
		Prefix,
		Substring,
		Pattern
	}

	class SearchService : AsyncServiceBase<SearchRequest>
	{
		protected override object Run(SearchRequest request)
		{
			return Observable
				.Timer(DateTimeOffset.Now, TimeSpan.FromSeconds(1))
				.Take(5)
				.Select(tick => new SearchResponse { DisplayText = "*{0}* {1}".Fmt(request.Filter, tick), MatchKind = (MatchKind)(tick % Enum.GetValues(typeof(MatchKind)).Length)})
				.Do(_ => "SENDING *{0}*".Fmt(_.ToJsv()).Print())
				.ToJsonStreamWriter();
		}
	}

	class Client
	{
		public static void Run()
		{
			var client = new ObservableServiceClient("http://localhost:1338/");
			client
				.ObserveMany(new SearchRequest { Filter = "f" })
				.Take(3)
				.Subscribe(
					onNext: response => response.PrintDump(),
					onCompleted: () => "COMPLETED".Print(),
					onError: error => error.PrintDump());
		}
	}

	public class ObservableServiceClient
	{
		readonly string _baseUri;

		public ObservableServiceClient(string baseUri)
		{
			_baseUri = baseUri;
		}

		public IObservable<TResponse> ObserveMany<TResponse>(IReturn<IEnumerable<TResponse>> request)
		{
			return Observable.Create<TResponse>(observer =>
			{
				var client = new JsonServiceClient(_baseUri);
				var disposable = new MutableDisposable
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
						response.Close();
						observer.OnError(exception);
					});

				return disposable.Dispose;
			});
		}
	}

	public static class ObservableServiceStackExtensions
	{
		public static IStreamWriter ToJsonStreamWriter<T>(this IObservable<T> source)
		{
			return new ObservableStreamWriter<T>(source);
		}

		public class ObservableStreamWriter<T> : IStreamWriter, IHasOptions
		{
			readonly IObservable<T> _source;
			readonly JsonSerializer<T> _serializer;
			readonly IDictionary<string, string> _options;

			public ObservableStreamWriter(IObservable<T> source)
			{
				_source = source;
				_serializer = new JsonSerializer<T>();
				_options = new Dictionary<string, string> {
					{ HttpHeaders.ContentType, "application/json" }
				};
			}

			public IDictionary<string, string> Options
			{
				get { return _options; }
			}

			public void WriteTo(Stream responseStream)
			{
				using (var writer = new StreamWriter(responseStream))
				{
					writer.WriteLine('[');
					writer.Flush();
					using (var enumerator = _source.ToEnumerable().GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							WriteTo(writer, enumerator.Current);
							while (enumerator.MoveNext())
							{
								writer.Write(',');
								WriteTo(writer, enumerator.Current);
							}
						}
					}
					writer.WriteLine(']');
				}
			}

			private void WriteTo(TextWriter writer, T value)
			{
				_serializer.SerializeToWriter(value, writer);
				writer.WriteLine();
				writer.Flush();
			}
		}

		public static IObservable<T> DeserializeMany<T>(this Stream stream)
		{
			return Observable.Using(
			  () => new StreamReader(stream),
			  reader =>
			  {
				  var firstLine = reader.ReadLine();
				  if (firstLine != "[")
					  return Observable.Throw<T>(new InvalidOperationException("Expecting '[', got '{0}'".Fmt(firstLine)));
				  return Observable.Generate(
				    reader,
				    _ => _ != null,
				    _ =>
				    {
					    var separator = _.Read();
					    if (separator == ']')
						    return null;
					    if (separator != ',')
						    throw new InvalidOperationException("Expecting ',', got '{0}'".Fmt((char)separator));
					    return _;
				    },
				    _ => _.ReadLine())
					  .Do(_ => "DTO: *{0}*".Fmt(_).Print())
					  .Select(new JsonSerializer<T>().DeserializeFromString);
			  });
		}
	}

	class Program
	{
		static void Main()
		{
			using (var appHost = new AppHost(typeof(Program).Assembly))
			{
				appHost.Init();
				appHost.Start("http://localhost:1337/");

				Client.Run();

				Console.WriteLine("Press <ENTER> to quit");
				Console.ReadLine();
			}
		}

		class AppHost : AppHostHttpListenerBase
		{
			public AppHost(params Assembly[] assemblies) : base("ServiceStack + Rx", assemblies)
			{
			}

			public override void Configure(Container container)
			{
				EndpointHostConfig.Instance.DebugMode = true;
				container.Register<IRequestLogger>(new InMemoryRollingRequestLogger());
				this.AddPluginsFromAssembly(typeof(AuthFeature).Assembly);
			}
		}
	}
}
