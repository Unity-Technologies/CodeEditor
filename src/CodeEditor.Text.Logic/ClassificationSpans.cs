using System.Collections.Generic;

namespace CodeEditor.Text.Logic
{
	public static class ClassificationSpans
	{
		public static IEnumerable<ClassificationSpan> Merge(IEnumerable<ClassificationSpan> classificationSpans)
		{
			var enumerator = classificationSpans.GetEnumerator();
			while (enumerator.MoveNext())
			{
				var merged = enumerator.Current;
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Classification != merged.Classification)
					{
						yield return merged;
						merged = enumerator.Current;
					}
					else
						merged = new ClassificationSpan(merged.Classification, merged.Span + enumerator.Current.Span);
				}
				yield return merged;
			}
		}
	}
}