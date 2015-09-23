//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
namespace St4mpede
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	using System.Collections.Generic; 
	using System.Linq;
	using System.Runtime.Serialization;
#endif
	//#	Regular ol' C# classes and code...

	/// <summary>This class must be public to make de/serialising possible when unit testing.
	/// </summary>
	[DataContract]
	public class TableData
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public bool Include { get; set; }

		[DataMember]
		public IList<ColumnData> Columns { get; set; }

		public TableData()		{		}

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

			foreach( var table in tables)
			{
				ret.Add(string.Format("Table {0}:", table.Name));
				ret.AddRange(ColumnDataHelpers.ToInfo(table.Columns));
			}

			return ret;
		}
	}

#if NOT_IN_T4
} //end the namespace
#endif
//#>