//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
namespace St4mpede
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	using Microsoft.SqlServer.Management.Common;
	using Microsoft.SqlServer.Management.Smo;
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Text.RegularExpressions;
	using System.Xml.Linq;
	using System.Xml.Serialization;
#endif
	//#	Regular ol' C# classes and code...

	public class Parser
	{
		//	TODO:Move to settings with St4mpede as fallback.
		private const string St4mpedePath = "St4mpede";

		private ILog _log;

		private Settings _settings;

		private DatabaseData _databaseData;

		internal static string GetExecutingPath()
		{
			var ret = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			return ret;
		}

		internal Parser()
			:this(new Log())
		{
		}

		internal Parser(ILog log)
		{
			_log = log;
		}

		internal void Init(string configPath, string configFilename)
		{
			if (null == configPath) { throw new ArgumentNullException("configPath"); }
			if (null == configFilename) { throw new ArgumentNullException("configFilename"); }

			var configPathfilename = Path.Combine(configPath, configFilename);
            _log.Add("Reading config file {0}.", configPathfilename);
			var doc = XDocument.Load(new FileStream(configPathfilename, FileMode.Open));
			Init(configPath, configFilename, doc);
		}

		internal void ParseDatabase()
		{
			using (var conn = new SqlConnection(_settings.ConnectionString))
			{
				var serverConnection = new ServerConnection(conn);
				try
				{
					var server = new Server(serverConnection);
					_log.Add("Chose server {0}", server.Name);

					var database =
						string.IsNullOrWhiteSpace(_settings.DatabaseName) ?
						server.Databases[_settings.DatabaseIndex] :    //	Uses index, like for SqlCompact.
						server.Databases[_settings.DatabaseName];
					if (null == database) { throw new UnknownDatabaseException(GetDatabasesInfo(server.Databases)); }
					_log.Add("Chose database {0}.", database.Name);

					var tables = database.Tables;
					_log.Add("Number of tables:{0}.", tables.Count);

					ParseTables(tables, _settings.ExcludedTablesRegex);
					_log.Add("Tables parsed:{0}", string.Join(",", _databaseData.Tables.Select(t => t.Name)));
					_log.Add(TableDataHelpers.ToInfo(_databaseData.Tables));
				}
				finally
				{
					serverConnection.Disconnect();
				}
			}
		}

		internal void WriteDatabaseXml()
		{
			var xmlPathFile = _settings.DatabaseXmlFile;
			_log.Add(string.Empty);
			_log.Add("Writing database xml {0}.", xmlPathFile);

			var xml = ToXml(_databaseData);

			_log.Add("Created xml:");
			_log.Add(xml);

			xml.Save(Path.Combine(_settings.ConfigPath, St4mpedePath	,  _settings.OutputXmlFilename));
		}

		#region Private methods.

		private static IList<string> GetDatabasesInfo(DatabaseCollection databases)
		{
			var lst = new List<string>();
			for (var i = 0; i < databases.Count; ++i)
			{
				lst.Add(string.Format("Name:{0}", databases[i].Name));
			}
			return lst;
		}

		private void Init(string configPath, string configFilename, System.Xml.Linq.XDocument doc)
		{
			var databaseName = (string)doc.Root.Element(Settings.XmlElements.DatabaseName);
			int databaseIndex = 0;
			int.TryParse(databaseName, out databaseIndex);
			_settings = new Settings(
				configPath,
				Path.Combine(configPath, configFilename),
				(string)doc.Root.Element(Settings.XmlElements.ConnectionString),
				databaseName,
				databaseIndex,
				(string)doc.Root.Element(Settings.XmlElements.ExcludedTablesRegex),
				(string)doc.Root.Element(Settings.XmlElements.DatabaseXmlFile)
			);
		}

		private static IList<ColumnData> ParseColumns(IList<Column> columns)
		{
			var ret = new List<ColumnData>();
			foreach (var column in columns)
			{
				ret.Add(new ColumnData(column.Name, column.DataType.ToString()));
			}
			return ret;
		}

		private void ParseTables(TableCollection tables, string excludedTablesRegex)
		{
			var tablesData = new List<TableData>();
			foreach (Table table in tables)
			{
				ParseTable(excludedTablesRegex, tablesData, table);
			}
			_databaseData = new DatabaseData
			{
				Tables = tablesData
			};
		}

		private static void ParseTable(string excludedTablesRegex, IList<TableData> tablesData, Table table)
		{
			var tableData = 
			new TableData(
				table.Name,
				false == Regex.IsMatch(table.Name, excludedTablesRegex)
			);
			tablesData.Add(tableData);

			tableData.Columns = ParseColumns(table.Columns.Cast<Column>().ToList()).ToList();
        }

		#endregion

		/// <summary>This method takes a List and not an IList since serialising requires List (not IList).
		/// </summary>
		/// <param name="tables"></param>
		/// <returns></returns>
		private static XDocument ToXml(DatabaseData databaseData)
		{
			return Core.Serialise(databaseData);
		}

		public string ToInfo()
		{
			return _log.ToInfo();
		}

		#region Unit testing work arounds.

		[DebuggerStepThrough]
		internal void UT_ParseTables(TableCollection tables, string excludedTablesRegex)
		{
			ParseTables(tables, excludedTablesRegex);
		}

		internal Settings UT_Settings { get { return _settings; } }

		internal DatabaseData UT_ServerData{get{ return _databaseData; } }

		[DebuggerStepThrough]
		internal static XDocument UT_ToXml( DatabaseData serverData)
		{
			return ToXml(serverData);
		}

		#endregion
	}

#if NOT_IN_T4
} //end the namespace
#endif
//#>