//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible

namespace St4mpede.RdbSchema
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	//	using System;
	using Microsoft.SqlServer.Management.Smo;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;
#endif
	//#	Regular ol' C# classes and code...

	internal interface IParserLogic
	{
		DatabaseData Parse(IList<Table> tables, string excludedTablesRegex);
		TableData Parse(Table table, string excludedTablesRegex);
		IList<ColumnData> Parse(IEnumerable<Column> columns);
		ColumnData Parse(Column column);
	}

	internal class ParserLogic : IParserLogic
	{
		public DatabaseData Parse(IList<Table> tables, string excludedTablesRegex)
		{
			var tableDataList = new List<TableData>();
			foreach (var table in tables)
			{
				tableDataList.Add(Parse(table, excludedTablesRegex));
			}
			var ret = new DatabaseData
			{
				Tables = tableDataList
			};
			return ret;
		}

		public TableData Parse(Table table, string excludedTablesRegex)
		{
			var ret = new TableData();
			ret.Name = table.Name;
			ret.Include = false == Regex.IsMatch(table.Name, excludedTablesRegex);
			ret.Columns = Parse(table.Columns.Cast<Column>()).ToList();
			return ret;
		}

		public IList<ColumnData> Parse(IEnumerable<Column> columns)
		{
			var ret = new List<ColumnData>();
			foreach (var column in columns)
			{
				ret.Add(Parse(column));
			}
			return ret;
		}

		public ColumnData Parse(Column column)
		{
			var ret = new ColumnData();
			ret.Name = column.Name;
			ret.DatabaseTypeName = column.DataType.ToString();
			ret.IsInPrimaryKey = column.InPrimaryKey;
			return ret;
		}

	}

#if NOT_IN_T4
} //end the namespace
#endif
//#>