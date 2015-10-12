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
	using System.Xml.Serialization;
#endif
	//#	Regular ol' C# classes and code...

	/// <summary>This class must be public to make de/serialising possible when unit testing. </summary>
	[DataContract]
	[XmlType(TypeName = "Column")]
	public class ColumnData
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string DatabaseTypeName { get; set; }

		public ColumnData() { }

		public ColumnData(string name, string typeName)
		{
			this.Name = name;
			this.DatabaseTypeName = typeName;
		}
	}

	/// <summary>This class must be public to make de/serialising possible when unit testing. </summary>
	[DataContract]
	[XmlType(TypeName = "Database")]
	public class DatabaseData
	{
		/// <summary>List of Tables for this Database.
		/// <para>Note it must be List and not IList to make serialising work.</para>
		/// </summary>
		public List<TableData> Tables { get; set; }
	}

	/// <summary>This class must be public to make de/serialising possible when unit testing. </summary>
	[DataContract]
	[XmlType(TypeName = "Table")]
	public class TableData
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public bool Include { get; set; }

		/// <summary>List of Columns for this Table.
		/// <para>Note it must be List and not IList to make serialising work.</para>
		/// </summary>
		[DataMember]
		public List<ColumnData> Columns { get; set; }

		public TableData() { }

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

	#region Helper methods.
	internal static class ColumnDataHelpers
	{
		internal static IEnumerable<string> ToInfo(IList<ColumnData> columns)
		{
			var ret = new List<string>();
			foreach (var column in columns)
			{
				//	Even though we are using Dotnet4.6 is seems we cannot use the $ string interpolation. Is it T4?
				//ret.Add($"Name={column.Name}\tType={column.TypeName}");
				ret.Add(string.Format("Name={0},\tDatabaseTypeName={1}", column.Name, column.DatabaseTypeName));
			}

			return ret;
		}
	}

	internal static class TableDataHelpers
	{
		internal static IEnumerable<string> ToInfo(IList<TableData> tables)
		{
			var excludedTables =
				tables.Where(t => false == t.Include).Select(t => t.Name);
			var includedTables =
				tables.Where(t => t.Include).Select(t => t.Name);
			var ret = new List<string>();

			ret.Add(string.Format("Excluded tables are {0}:{1}.",
				excludedTables.Count(), string.Join(", ", excludedTables)));

			ret.Add(string.Format("Included tables are {0}:{1}.",
				includedTables.Count(), string.Join(", ", includedTables)));

			ret.AddRange(tables.Select(t => t.ToString()));

			foreach (var table in tables)
			{
				ret.Add(string.Format("Table {0}:", table.Name));
				ret.AddRange(ColumnDataHelpers.ToInfo(table.Columns));
			}

			return ret;
		}
	}

	#endregion

#if NOT_IN_T4
} //end the namespace
#endif
//#>