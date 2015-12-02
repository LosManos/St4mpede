using System.Collections.Generic;
using System.Linq;

namespace St4mpede.Code
{
	public static class ParameterDataExtensions
	{
		public static IList<string> ToMethodParameterDeclaration(this IList<ParameterData> me)
		{
			return me.Select(p => p.ToDeclaration()).ToList();
		}

		public static string ToMethodParameterDeclarationString(this IList<ParameterData> me)
		{
			return string.Join(", ", me.ToMethodParameterDeclaration());
		}
	}
}
