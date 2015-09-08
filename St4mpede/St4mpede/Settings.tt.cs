//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
namespace St4mpede
{
	//	using System;
#endif
	//#	Regular ol' C# classes and code...

	internal class Settings
	{
		internal string InitPathfilename { get; set; }

		internal string ConnectionString { get; set; }

		internal string DatabaseName { get; set; }

		internal int DatabaseIndex { get; set; }

		internal string ExcludedTablesRegex { get; set; }

		internal string DatabaseXmlFile { get; set; }

        internal static class XmlElements
		{
			internal const string ConnectionString = "ConnectionString";
			internal const string DatabaseName = "DatabaseName";
			internal const string DatabaseIndex = "DatabaseIndex";
			internal const string ExcludedTablesRegex = "ExcludedTablesRegex";
			internal const string DatabaseXmlFile = "DatabaseXmlFile";
        }
	}

#if NOT_IN_T4
} //end the namespace
#endif
//#>