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
		private IList<string> m_Log = new List<string>();

		private Settings _settings;

		private ServerData _serverData;

		internal static string GetExecutingPath()
		{
			var ret = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			return ret;
		}

		internal void Init(string configPath, string configFilename)
		{
			if (null == configPath) { throw new ArgumentNullException("configPath"); }
			if (null == configFilename) { throw new ArgumentNullException("configFilename"); }

			var configPathfilename = Path.Combine(configPath, configFilename);
            AddLog("Reading config file {0}.", configPathfilename);
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
					AddLog("Chose server {0}", server.Name);

					var database =
						string.IsNullOrWhiteSpace(_settings.DatabaseName) ?
						server.Databases[_settings.DatabaseIndex] :    //	Uses index, like for SqlCompact.
						server.Databases[_settings.DatabaseName];
					if (null == database) { throw new UnknownDatabaseException(GetDatabasesInfo(server.Databases)); }
					AddLog("Chose database {0}.", database.Name);

					var tables = database.Tables;
					AddLog("Number of tables:{0}.", tables.Count);

					ParseTables(tables, _settings.ExcludedTablesRegex);
					AddLog("Tables parsed:{0}", string.Join(",", _serverData.Tables.Select(t => t.Name)));
					AddLog(TableDataHelpers.ToInfo(_serverData.Tables));
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
			AddLog(string.Empty);
			AddLog("Writing database xml {0}.", xmlPathFile);

			var xml = ToXml(_serverData);

			AddLog("Created xml:");
			AddLog(xml);

			//	TODO:	Write Xml to file.
			xml.Save(Path.Combine(_settings.ConfigPath, _settings.OutputXmlFilename));
		}

		#region Private methods.

		private void AddLog(string format, params object[] args)
		{
			m_Log.Add(string.Format(format, args));
		}

		private void AddLog(IEnumerable<string> logRows)
		{
			foreach (var row in logRows)
			{
				AddLog(row);
			}
		}

		private void AddLog(XDocument xml)
		{
			//	TODO:	Make the output better.
			AddLog(xml.ToString());
		}

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
			_serverData = new ServerData
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
		private static XDocument ToXml(ServerData serverData)
		{
			return Serialise(serverData);
		}

		/// <summary>This method deserialises an XDocument to an object.
		/// See complementary method <see cref="Serialise{T}(T)"/>.
		/// <para>
		/// Copied with pride from:
		///	http://stackoverflow.com/questions/1295046/use-xdocument-as-the-source-for-xmlserializer-deserialize
		/// </para>
		/// <para>
		/// If there ever will be a Util or Common library, maybe we should move it there.
		/// </para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="doc"></param>
		/// <returns></returns>
		private static T Deserialise<T>(XDocument doc)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

			using (var reader = doc.Root.CreateReader())
			{
				return (T)xmlSerializer.Deserialize(reader);
			}
		}

		/// <summary>This method serialises an object into XDocument.
		/// See complementary method <see cref="Deserialise{T}(XDocument)"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		private static XDocument Serialise<T>(T value)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

			XDocument doc = new XDocument();
			using (var writer = doc.CreateWriter())
			{
				xmlSerializer.Serialize(writer, value);
			}

			return doc;
		}

		public override string ToString()
		{
			return string.Join("\r\n", m_Log);
		}

		#region Unit testing work arounds.

		internal IList<string> UT_Log { get { return m_Log; } }

		[DebuggerStepThrough]
		internal void UT_AddLog( string format, params object[] args)
		{
			AddLog(format, args);
		}

		[DebuggerStepThrough]
		internal void UT_AddLog( IEnumerable<string> logRows)
		{
			AddLog(logRows);
		}

		[DebuggerStepThrough]
		internal void UT_AddLog(XDocument xml)
		{
			AddLog(xml);
		}

		internal static T UT_Deserialise<T>(XDocument res)
		{
			return Deserialise<T>(res);
		}

		[DebuggerStepThrough]
		internal void UT_ParseTables(TableCollection tables, string excludedTablesRegex)
		{
			ParseTables(tables, excludedTablesRegex);
		}

		internal Settings UT_Settings { get { return _settings; } }

		internal ServerData UT_ServerData{get{ return _serverData; } }

		[DebuggerStepThrough]
		internal static XDocument UT_ToXml( ServerData serverData)
		{
			return ToXml(serverData);
		}

		#endregion
	}

#if NOT_IN_T4
} //end the namespace
#endif
//#>