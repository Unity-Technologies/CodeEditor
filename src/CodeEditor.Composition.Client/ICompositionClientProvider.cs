using System;

namespace CodeEditor.Composition.Client
{
	public interface ICompositionClientProvider
	{
		IServiceProvider CompositionClientFor(string address);
	}

	public static class ServiceProviderExtensions
	{
		public static T GetService<T>(this IServiceProvider serviceProvider)
		{
			return (T) serviceProvider.GetService(typeof(T));
		}
	}
}
