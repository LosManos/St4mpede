//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
using System.Collections.Generic;
using System.Linq;

namespace St4mpede
{
	//	using System;
#endif
	//#	Regular ol' C# classes and code...

	internal class TableData
	{
		internal string Name { get; set; }
		internal bool Include { get; set; }

		internal TableData()		{		}

		internal TableData(string name, bool include)
		{
			this.Name = name;
			this.Include = include;
		}

		public override string ToString()
		{
			//TODO:Return json format.
			return string.Format("Name:{0}, Include:{1}.", Name, Include);
		}
	}

	internal static class TableDataHelpers
	{
		internal static IEnumerable<string> ToInfo( IList<TableData>tables)
		{
			var excludedTables =
				tables.Where(t => false == t.Include).Select(t => t.Name);
			var includedTables =
				tables.Where(t => t.Include).Select(t => t.Name);
            var ret = new List<string>();
            ret.Add(string.Format("Excluded tables are {0}:{1}.",
				excludedTables.Count(), string.Join(", ",excludedTables)));
			ret.Add(string.Format("Included tables are {0}:{1}.", 
				includedTables.Count(), string.Join(", ",includedTables)));
			ret.AddRange(tables.Select(t => t.ToString()));
			return ret;
		}
	}

#if NOT_IN_T4
} //end the namespace
#endif
//#>