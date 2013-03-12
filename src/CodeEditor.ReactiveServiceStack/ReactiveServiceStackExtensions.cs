using System;
using System.Collections.Generic;
using System.IO;
using CodeEditor.Reactive;
using ServiceStack.Common.Web;
using ServiceStack.Service;
using ServiceStack.ServiceHost;
using ServiceStack.Text;

namespace CodeEditor.ReactiveServiceStack
{
	public static class ReactiveServiceStackExtensions
	{
		public static IStreamWriter ToJsonStreamWriter<T>(this IObservableX<T> source)
		{
			return new ObservableStreamWriter<T>(source);
		}

		public class ObservableStreamWriter<T> : IStreamWriter, IHasOptions
		{
			readonly IObservableX<T> _source;
			readonly JsonSerializer<T> _serializer;
			readonly IDictionary<string, string> _options;

			public ObservableStreamWriter(IObservableX<T> source)
			{
				_source = source;
				_serializer = new JsonSerializer<T>();
				_options = new Dictionary<string, string>
				{
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

		public static IObservableX<T> DeserializeMany<T>(this Stream stream)
		{
			return ObservableX.Using(
			  () => new StreamReader(stream),
			  reader =>
			  {
				  var firstLine = reader.ReadLine();
				  if (firstLine != "[")
					  return ObservableX.Throw<T>(new InvalidOperationException("Expecting '[', got '{0}'".Fmt(firstLine)));
				  return ObservableX.Generate(
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
						_ =>
						{
							var line = _.ReadLine();
							if (line == "]")
								return null;
							return line;
						})
						.TakeWhile(line => line != null)
					  .Select(new JsonSerializer<T>().DeserializeFromString);
			  });
		}
	}
}