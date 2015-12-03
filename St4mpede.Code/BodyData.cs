using System;
using System.Collections.Generic;

namespace St4mpede.Code
{
	public class BodyData : DataBaseClass, IToCode
	{
		public List<string> Lines { get; set; }

		public override IList<string> ToCode()
		{
			return Lines;
		}
	}
}
