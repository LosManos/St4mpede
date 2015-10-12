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


		//TODO:Change name to ParserSettings or something alike.
	internal class Settings
	{
		internal string ConfigPath { get; set; }

		internal string InitPathfilename { get; set; }

		/// <summary>This is the name of the outputed XML file.
		/// By the time of writing it is hard coded but when we have the time to make it settable, with default value fall back we might. Check Github for bug #4 https://github.com/LosManos/St4mpede/issues/4
		/// </summary>
		internal string OutputXmlFilename { get { return "St4mpede.xml"; } }

		internal string ConnectionString { get; set; }

		internal string DatabaseName { get; set; }

		internal int DatabaseIndex { get; set; }

		internal string ExcludedTablesRegex { get; set; }

		internal string DatabaseXmlFile { get; set; }

		internal string RootFolder { get; set; }

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