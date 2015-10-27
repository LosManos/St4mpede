//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
namespace St4mpede
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	//	using System;
#endif
	//#	Regular ol' C# classes and code...

	internal interface ISettings
	{
		string ConfigPath { get; set; }
		string InitPathfilename { get; set; }
		string OutputXmlFilename { get; }
		string ConnectionString { get; set; }
		string DatabaseName { get; set; }
		int DatabaseIndex { get; set; }
		string ExcludedTablesRegex { get; set; }
		string DatabaseXmlFile { get; set; }
		string RootFolder { get; set; }
	}
	
	//TODO:Change name to ParserSettings or something alike.
	internal class Settings : ISettings
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

		public string RootFolder { get; set; }

        internal static class XmlElements
		{
			internal const string ConnectionString = "ConnectionString";
			internal const string DatabaseName = "DatabaseName";
			internal const string DatabaseIndex = "DatabaseIndex";
			internal const string ExcludedTablesRegex = "ExcludedTablesRegex";
			internal const string DatabaseXmlFile = "DatabaseXmlFile";
			internal const string RootFolder = "RootFolder";
        }

		internal Settings()
		{
		}

		/// <summary>This constructor takes every needed property as argument.
		/// </summary>
		internal Settings(string configPath, string initPathfilename, 
			string connectionString, string databaseName, int databaseIndex, 
			string excludedTablesRegex, string databaseXmlFile, 
			string rootFolder)
		{
			this.ConfigPath = configPath;
			this.InitPathfilename = initPathfilename;
			this.ConnectionString = connectionString;
			this.DatabaseName = databaseName;
			this.DatabaseIndex = databaseIndex;
			this.ExcludedTablesRegex = excludedTablesRegex;
			this.DatabaseXmlFile = databaseXmlFile;
			this.RootFolder = rootFolder;
		}
	}

#if NOT_IN_T4
} //end the namespace
#endif
//#>