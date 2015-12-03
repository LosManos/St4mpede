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

		public static IList<string> ToPropertyAssignmentList(this IList<ParameterData> me, Indent indent)
		{
			return me
				.Select(p => string.Format("{0}this.{1} = {2};",indent.IndentString(), p.Name, Common.ToCamelCase(p.Name)))
				.ToList();
		}
    }
}
