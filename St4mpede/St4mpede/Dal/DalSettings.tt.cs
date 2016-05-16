//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
using System.IO;

namespace St4mpede.St4mpede.Poco
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	//using System;
using System.IO;
#endif
	//#	Regular ol' C# classes and code...

	internal class DalSettings
	{
		internal string OutputFolder { get; private set; }
		public string ProjectPath { get; private set; }
		public string XmlOutputFilename { get; private set; }

		public string XmlOutputPathFilename
		{
			get
			{
				return Path.Combine(ProjectPath, XmlOutputFilename);
			}
		}

		public DalSettings()
		{
		}

		public DalSettings(string outputFolder, string projectPath, string xmlOutputFileName)
		{
			this.OutputFolder = outputFolder;
			this.ProjectPath = projectPath;
			this.XmlOutputFilename = xmlOutputFileName;
		}
	}

#if NOT_IN_T4
} //end the namespace
#endif
//#>
