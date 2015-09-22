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

		private Settings m_settings;

		private List<TableData> m_tablesData;

		internal static string GetExecutingPath()
		{
			var ret = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			return ret;
		}

		internal void Init(string configPathfilename)
		{
			if (null == configPathfilename) { throw new ArgumentNullException("pathfilename"); }

			AddLog("Reading config file {0}.", configPathfilename);
			var doc = XDocument.Load(new FileStream(configPathfilename, FileMode.Open));
			Init(configPathfilename, doc);
		}

		internal void ParseDatabase()
		{
			using (var conn = new SqlConnection(m_settings.ConnectionString))
			{
				var serverConnection = new ServerConnection(conn);
				var server = new Server(serverConnection);
				AddLog("Choosing server {0}", server.Name);
				var database =
					string.IsNullOrWhiteSpace(m_settings.DatabaseName) ?
					server.Databases[m_settings.DatabaseIndex] :    //	Uses index, like for SqlCompact.
					server.Databases[m_settings.DatabaseName];
				if (null == database) { throw new UnknownDatabaseException(GetDatabasesInfo(server.Databases)); }
				AddLog("Choosing database {0}.", database.Name);

				var tables = database.Tables;
				AddLog("Number of tables:{0}.", tables.Count);
				ParseTables(tables, m_settings.ExcludedTablesRegex);
				AddLog("Tables parsed:{0}", string.Join(",", m_tablesData.Select(t => t.Name)));
				AddLog(TableDataHelpers.ToInfo(m_tablesData));
			}
		}

		internal void WriteDatabaseXml()
		{
			var xmlPathFile = m_settings.DatabaseXmlFile;
			AddLog("Writing database xml {0}.", xmlPathFile);

			var xml = ToXml(m_tablesData);

			AddLog("Created xml:");
			AddLog(xml);	//	TODO:	Make Xml output better.

			//	TODO:	Write Xml to file.
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

		private void Init(string pathfilename, System.Xml.Linq.XDocument doc)
		{
			var databaseName = (string)doc.Root.Element(Settings.XmlElements.DatabaseName);
			int databaseIndex = 0;
			int.TryParse(databaseName, out databaseIndex);
			m_settings = new Settings(
				pathfilename,
				(string)doc.Root.Element(Settings.XmlElements.ConnectionString),
				databaseName,
				databaseIndex,
				(string)doc.Root.Element(Settings.XmlElements.ExcludedTablesRegex),
				(string)doc.Root.Element(Settings.XmlElements.DatabaseXmlFile)
			);
		}

		private void ParseTables(TableCollection tables, string excludedTablesRegex)
		{
			var tablesData = new List<TableData>();
			foreach (Table table in tables)
			{
				tablesData.Add(new TableData(
					table.Name,
					false == Regex.IsMatch(table.Name, excludedTablesRegex)
				));
			}
			m_tablesData = tablesData;
		}

		#endregion

		private XDocument ToXml(List<TableData> tables)
		{
			return Serialize(tables);

			//\var doc = new XDocument();
			//using (var writer = doc.CreateWriter())
			//{
			//	// write xml into the writer
			//\	var serializer = new System.Runtime.Serialization.DataContractSerializer(tables.GetType());
			//	serializer.WriteObject(writer, tables);
			//}
			//return doc;

			//var document = new XDocument();
			//var root = new XElement("Root");
			//document.Add(root);
			//tables.ToList().ForEach(x => root.Add(new XElement("TableData", x)));

			//return document;

			////	We don't do any direct xml serialising of tables here 
			////	because we want to hand craft the xml to look exacly as we want.
			//var ret = new XDocument();
			//ret.AddFirst( new ("asdf");

			//return ret;

			//var xsSubmit = new XmlSerializer(typeof(IList<TableData>));
			//using (var sww = new StringWriter())
			//{
			//	using (var writer = XmlWriter.Create(sww))
			//	{
			//		xsSubmit.Serialize(writer, tables);
			//		var xml = sww.ToString();
			//		return xml;
			//	}
			//}

			//	TODO:	Serialise the tables parameter to an Xml document, not necessary XDocument.
			throw new NotImplementedException();
		}

		//	Copied with pride from:
		//	http://stackoverflow.com/questions/1295046/use-xdocument-as-the-source-for-xmlserializer-deserialize

		//TODO:Make private and a UT_Deserialize method.
		//TODO:Change to BrEng.
		internal static T Deserialize<T>(XDocument doc)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

			using (var reader = doc.Root.CreateReader())
			{
				return (T)xmlSerializer.Deserialize(reader);
			}
		}

		private static XDocument Serialize<T>(T value)
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

		[DebuggerStepThrough]
		internal void UT_ParseTables(TableCollection tables, string excludedTablesRegex)
		{
			ParseTables(tables, excludedTablesRegex);
		}

		internal Settings UT_Settings { get { return m_settings; } }

		internal IList<TableData> UT_TablesData{get{return m_tablesData;} }

		[DebuggerStepThrough]
		internal XDocument UT_ToXml( List<TableData> tables)
		{
			return ToXml(tables);
		}

		#endregion
	}

#if NOT_IN_T4
} //end the namespace
#endif
//#>