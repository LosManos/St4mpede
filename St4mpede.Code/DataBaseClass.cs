using System.Collections.Generic;
using System.Linq;

namespace St4mpede.Code
{
	public abstract class DataBaseClass 
	{
		internal int _indentLevel = 0;

		internal string _indentString = "\t";

		public abstract IList<string> ToCode();

		public IList<string> ToCode(int indentLevel)
		{
			return ToCode().Select(row => _indentString + row).ToList();
		}

	}
}
