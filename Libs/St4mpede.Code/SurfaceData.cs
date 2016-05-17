using System.Collections.Generic;

namespace St4mpede.Code
{
	public class SurfaceData: DataBaseClass, IToCode
	{
		public string Name { get; set; }

		public List<MethodData> Methods { get; set; }

		public override IList<string> ToCode()
		{
			var ret = new List<string>();
			ret.Add($"public class {Name}{{");
			ret.Add("TBA:Methods goes here...");	//	TODO:OF:implement. Use Class class.
			ret.Add("}");
			return ret;
		}
	}
}
