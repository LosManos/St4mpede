//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
using System.IO;

namespace St4mpede.Poco
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	//using System;
#endif
	//#	Regular ol' C# classes and code...

	internal class PocoSettings
	{
		public bool CreateDefaultConstructor { get; set; }
		public bool CreateAllPropertiesConstructor { get; set; }
		public bool CreateAllPropertiesSansPrimaryKeyConstructor { get; set; }
		public bool CreateCopyConstructor { get; set; }

		public bool CreateMethodEquals { get; set; }
		public string CreateMethodEqualsRegex { get; set; }

		internal bool MakePartial { get; set; }
		internal string OutputFolder { get; set; }
		public string ProjectPath { get; private set; }
		public string XmlOutputFilename { get; private set; }

		public string XmlOutputPathFilename
		{
			get
			{
				return Path.Combine(ProjectPath, XmlOutputFilename);
			}
		}

		public PocoSettings()
		{
		}

		public PocoSettings(
			bool makePartial,
			bool createDefaultConstructor, 
			bool createAllPropertiesConstructor, 
			bool createAllPropertiesSansPrimaryKeyConstructor,
			bool createCopyConstructor,
			bool createMethodEquals, 
			string createMethodEqualsRegex,
            string outputFolder, 
			string projectPath, 
			string xmlOutputFilename)
		{
			this.MakePartial = makePartial;
			this.CreateDefaultConstructor = createDefaultConstructor;
			this.CreateAllPropertiesConstructor = createAllPropertiesConstructor;
			this.CreateAllPropertiesSansPrimaryKeyConstructor = createAllPropertiesSansPrimaryKeyConstructor;
			this.CreateCopyConstructor = createCopyConstructor;

			this.CreateMethodEquals = createMethodEquals;
			this.CreateMethodEqualsRegex = createMethodEqualsRegex;

			this.OutputFolder = outputFolder;
			this.ProjectPath = projectPath;
			this.XmlOutputFilename = xmlOutputFilename;
		}
	}

#if NOT_IN_T4
} //end the namespace
#endif
//#>
