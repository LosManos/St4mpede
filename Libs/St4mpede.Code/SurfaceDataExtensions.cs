using System.Collections.Generic;

namespace St4mpede.Code
{
	public static class SurfaceDataExtensions
	{
		public static IEnumerable<string> ToInfo(this IList<SurfaceData> surfaceDataList)
		{
			var ret = new List<string>();
			foreach (var s in surfaceDataList)
			{
				//	Even though we are using Dotnet4.6 is seems we cannot use the $ string interpolation. Is it T4?
				//ret.Add($"Name={column.Name}\tType={column.TypeName}");
				//	Well, maybe we can now that the code is compiled. But in T4 we cannot.
				ret.Add(string.Format("Name={0}",
					s.Name));
			}
			return ret;
		}
	}
}
