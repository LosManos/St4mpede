using System.Collections.Generic;

namespace St4mpede.Code
{
	public static class ParameterDataExtensions
	{
		public static IList<string> ToCode( this IList<ParameterData> me)
		{
			var ret = new List<string>();

			string.Join(
				", ",
				me.ToCode());

			return ret;
		}
	}
}
