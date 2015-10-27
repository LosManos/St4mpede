//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
using System.IO;
using System.Xml.Linq;

namespace St4mpede
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	//	using System;
#endif
	//#	Regular ol' C# classes and code...

	internal interface IParserSettings
	{
		string ConfigPath { get; set; }
		string InitPathfilename { get; set; }
		string OutputXmlFilename { get; }
		string ConnectionString { get; set; }
		string DatabaseName { get; set; }
		int DatabaseIndex { get; set; }
		string ExcludedTablesRegex { get; set; }
		string DatabaseXmlFile { get; set; }
		string ProjectPath { get; set; }
	}

	internal class ParserSettings : IParserSettings
	{
		public string ConfigPath { get; set; }

		public string InitPathfilename { get; set; }

		/// <summary>This is the name of the outputed XML file.
		/// By the time of writing it is hard coded but when we have the time to make it settable, with default value fall back we might. Check Github for bug #4 https://github.com/LosManos/St4mpede/issues/4
		/// </summary>
		public string OutputXmlFilename { get { return "St4mpede.RdbSchema.xml"; } }

		public string ConnectionString { get; set; }

		public string DatabaseName { get; set; }

		public int DatabaseIndex { get; set; }

		public string ExcludedTablesRegex { get; set; }

		public string DatabaseXmlFile { get; set; }

		public string ProjectPath { get; set; }

		internal static class XmlElements
		{
			internal const string ConnectionString = "ConnectionString";
			internal const string DatabaseName = "DatabaseName";
			internal const string DatabaseIndex = "DatabaseIndex";
			internal const string ExcludedTablesRegex = "ExcludedTablesRegex";
			internal const string DatabaseXmlFile = "DatabaseXmlFile";
			internal const string ProjectPath = "ProjectPath";
		}

		internal ParserSettings()
		{
		}

		/// <summary>This constructor takes every needed property as argument.
		/// </summary>
		internal ParserSettings(string configPath, string initPathfilename,
			string connectionString, string databaseName, int databaseIndex,
			string excludedTablesRegex, string databaseXmlFile,
			string projectPath)
		{
			this.ConfigPath = configPath;
			this.InitPathfilename = initPathfilename;
			this.ConnectionString = connectionString;
			this.DatabaseName = databaseName;
			this.DatabaseIndex = databaseIndex;
			this.ExcludedTablesRegex = excludedTablesRegex;
			this.DatabaseXmlFile = databaseXmlFile;
			this.ProjectPath = projectPath;
		}

		internal static ParserSettings Init(string configPath, string configFilename, XDocument doc)
		{
			const string RdbSchemaSubElementName = "RdbSchema";

			//	Get the databasename which might be a name or a pathfilename or a number.
			var databaseName = (string)doc.Root.Element(RdbSchemaSubElementName).Element(ParserSettings.XmlElements.DatabaseName);

			//	If the databasename is a number - put it in databaseindex.
			int databaseIndex = 0;
			int.TryParse(databaseName, out databaseIndex);

			//	Create the object with all properties set.
			return new ParserSettings(
				configPath,
				Path.Combine(configPath, configFilename),
				(string)doc.Root.Element(RdbSchemaSubElementName).Element(ParserSettings.XmlElements.ConnectionString),
				databaseName,
				databaseIndex,
				(string)doc.Root.Element(RdbSchemaSubElementName).Element(ParserSettings.XmlElements.ExcludedTablesRegex),
				(string)doc.Root.Element(RdbSchemaSubElementName).Element(ParserSettings.XmlElements.DatabaseXmlFile),
				(string)doc.Root.Element(RdbSchemaSubElementName).Element(ParserSettings.XmlElements.ProjectPath)
			);
		}

	}

#if NOT_IN_T4
} //end the namespace
#endif
//#>