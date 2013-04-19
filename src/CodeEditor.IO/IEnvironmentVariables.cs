using CodeEditor.Composition;

namespace CodeEditor.IO
{
	public interface IEnvironmentVariables
	{
		/// <returns>null if the variable is not set</returns>
		string ValueOf(string variable);
	}

	[Export(typeof(IEnvironmentVariables))]
	class EnvironmentVariables : IEnvironmentVariables
	{
		public string ValueOf(string variable)
		{
			return System.Environment.GetEnvironmentVariable(variable);
		}
	}
}