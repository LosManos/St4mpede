using System.Collections.Generic;
using System.Linq;

namespace St4mpede.Code
{
	public abstract class DataBaseClass 
	{
		internal Indent _indent = new Indent
		{
			Level = 0,
			WhiteSpace = "\t"
		};

		public abstract IList<string> ToCode();

		public IList<string> ToCode(Indent indent)
		{
			return ToCode().Select(row => _indent.IndentString(indent.Level) + row).ToList();
		}

	}
}
