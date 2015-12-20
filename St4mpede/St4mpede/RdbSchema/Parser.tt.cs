//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/

using St4mpede.St4mpede.Core;

#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
namespace St4mpede
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	using Microsoft.SqlServer.Management.Smo;
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Xml.Linq;
#endif
	//#	Regular ol' C# classes and code...

	public class Parser
	{
		#region Private properties and schemas.

		private ILog _log;

		private CoreSettings _coreSettings;

		private IParserSettings _settings;

		private DatabaseData _databaseData;

		private IDatabaseConnection _databaseConnection;

		private IParserLogic _parserLogic;

		private const string ProjectPath = "RdbSchema";

		#endregion

		#region Public methods.

		internal Parser()
			:this(new Log())
		{
			_databaseConnection = new DatabaseConnection();
			_parserLogic = new ParserLogic();
		}

		internal Parser(ILog log)
		{
			_log = log;
		}

		internal void Init( string hostTemplateFile)
		{
			Init(hostTemplateFile, null);
		}

		internal void Init(string hostTemplateFile, string configFilename)
		{
			if (null == hostTemplateFile) { throw new ArgumentNullException("hostTemplateFile"); }

			var configPath = Path.GetDirectoryName(hostTemplateFile);
			configFilename = configFilename ?? Core.DefaultConfigFilename;

            var doc = Core.ReadConfig(
				configPath, 
				configFilename);
			_coreSettings = Core.Init(doc);
			_settings = ParserSettings.Init(configPath, configFilename, doc);
		}

		internal void Generate()
		{
			using (var serverInfo = _databaseConnection.GetServerInfo(_settings.ConnectionString))
			{
				_log.Add("Choose server {0}", serverInfo.Name);

				var database =
					string.IsNullOrWhiteSpace(_settings.DatabaseName) ?
					serverInfo.DatabaseList[_settings.DatabaseIndex] :    //	Uses index, like for SqlCompact.
					serverInfo.Databases[_settings.DatabaseName];
				if (null == database) { throw new UnknownDatabaseException(GetDatabasesInfo(serverInfo.Databases)); }
				_log.Add("Chose database {0}.", database.Name);

				_databaseData = _parserLogic.Parse(
					serverInfo.GetTablesByDatabase(database), 
					_settings.ExcludedTablesRegex);
			}
		}

		internal void Output()
		{
			var xmlPathFile = _settings.DatabaseXmlFile;
			_log.Add(string.Empty);
			_log.Add("Writing database xml {0}.", xmlPathFile);

			var xml = ToXml(_databaseData);

			_log.Add("Created xml:");
			_log.Add(xml);

			xml.Save(Path.Combine(_settings.ConfigPath, ProjectPath, _settings.OutputXmlFilename));
		}

		public string ToInfo()
		{
			return _log.ToInfo();
		}

		#endregion

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

		private static IList<ColumnData> ParseColumns(IList<Column> columns)
		{
			var ret = new List<ColumnData>();
			foreach (var column in columns)
			{
				ret.Add(new ColumnData(column.Name, column.DataType.ToString(), column.InPrimaryKey));
			}
			return ret;
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

		#region Unit testing work arounds.

		internal IParserSettings UT_Settings { get { return _settings; }
			set { _settings = value; } }

		internal DatabaseData UT_ServerData{get{ return _databaseData; } }

		[DebuggerStepThrough]
		internal void UT_SetDatabaseConnection( IDatabaseConnection databaseConnection)
		{
			_databaseConnection = databaseConnection;
		}

		[DebuggerStepThrough]
		internal void UT_SetParserLogic( IParserLogic parserLogic)
		{
			_parserLogic = parserLogic;
		}

		[DebuggerStepThrough]
		internal static XDocument UT_ToXml( DatabaseData serverData)
		{
			return ToXml(serverData);
		}

		#endregion
	}

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
			foreach( var table in tables)
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
			foreach( var column in columns)
			{
				ret.Add(Parse(column));
			}
			return ret;
		}

		public ColumnData Parse( Column column)
		{
			var ret = new ColumnData();
			ret.Name = column.Name;
			ret.DatabaseTypeName =column.DataType.ToString();
			ret.IsInPrimaryKey = column.InPrimaryKey;
			return ret;
		}
	}

#if NOT_IN_T4
} //end the namespace
#endif
//#>