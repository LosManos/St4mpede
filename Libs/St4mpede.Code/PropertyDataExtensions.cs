﻿using System.Collections.Generic;
using System.Linq;

namespace St4mpede.Code
{
	public static class PropertyDataExtensions
	{
		public static IList<string> ToCode( this IEnumerable<DataBaseClass> lst, Indent indent)
		{
			var ret = new List<string>();
			foreach( var item in lst)
			{
				ret.AddRange(item.ToCode(indent));
				ret.Add(string.Empty);
			}

			//	Remove the last empty line.
			if(ret.Any())
			{
				ret.RemoveAt(ret.Count - 1);
			}

			return ret;
		}
	}
}
