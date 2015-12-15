using System;
using System.Collections.Generic;
using System.Linq;

namespace St4mpede.Code
{
	public class BodyData : DataBaseClass, IToCode
	{
		public List<string> Lines { get; set; }

		public BodyData()
		{
		}

		public BodyData( IList<string> lines)
		{
			Lines = lines.ToList();
		}

		public override IList<string> ToCode()
		{
			return Lines;
		}
	}
}
