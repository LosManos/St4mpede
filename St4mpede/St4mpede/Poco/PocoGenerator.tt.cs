//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
namespace St4mpede.Poco
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Xml.Linq;
#endif
	//#	Regular ol' C# classes and code...

	internal class PocoGenerator
	{
		#region Private properties and fields.

		private const string PocoElement = "Poco";
		private const string OutputFolderElement = "OutputFolder";

		private ICore _core;

		private ILog _log;

		private IXDocHandler _xDocHandler;

		private IList<ClassData> _classDataList;

		private DatabaseData _database;

		private string _outputFolder;

		private CoreSettings _coreSettings;

		#endregion

		internal static class DatabaseTypes
		{
			internal const string DateTime = "datetime";
			internal const string NChar = "nchar";
			internal const string NVarChar = "nvarchar";
			internal const string Int = "int";
			internal const string VarChar = "varchar";
		}

		#region Constructors.

		internal PocoGenerator()
			:this(new Core(), new Log(), new XDocHandler())
		{	}

		internal PocoGenerator(ICore core, ILog log, IXDocHandler xDocHandler)
		{
			_core = core;
			_log = log;
			_xDocHandler = xDocHandler;
		}

		#endregion

		internal void Generate()
		{
			_classDataList = new List<ClassData>();
			_database.Tables
				.Where(t=>t.Include)
				.ToList()
				.ForEach(table =>
		   {
			   var classData = new ClassData(table.Name);
			   classData.Properties = new List<PropertyData>();
			   table.Columns
			   .ForEach(column =>
			   {
				   classData.Properties.Add(
					   new PropertyData(column.Name,
					   ConvertDatabaseTypeToDotnetType(column.DatabaseTypeName)));
			   });
			   _classDataList.Add(classData);
		   });

			_log.Add("Included classes are:");
			_log.Add(ClassDataHelpers.ToInfo(_classDataList));
		}

		internal void Init( string hostTemplateFile)
		{
			Init(hostTemplateFile, null, null);
		}

		internal void Init(string hostTemplateFile, string configFilename, 
			Func<string, string, XDocument> readConfigFunction)
		{
			if (null == hostTemplateFile) { throw new ArgumentNullException("configPath"); }

			configFilename = configFilename ?? Core.DefaultConfigFilename;
			readConfigFunction = readConfigFunction ?? Core.ReadConfig;

			var doc = readConfigFunction(
				Path.GetDirectoryName(hostTemplateFile), 
				configFilename);

			var coreSettings = Core.Init(doc);

			var settings = (from c in doc.Descendants(PocoElement) select c).SingleOrDefault();
			if( null == settings)
			{
				_log.Add("Configuration does not contain element {0}", PocoElement);
				return;	//	Bail.
			}
			Init(coreSettings, settings);
		}

		private void Init( CoreSettings settings, XElement doc)
		{
			_coreSettings = settings;
			_outputFolder = doc.Descendants(OutputFolderElement).Single().Value;
		}

		internal void Output()
		{
			//	TODO: Make path and name of xml output file settable.
			var pathFileForXmlOutput =
				Path.Combine(_coreSettings.RootFolder, @"Poco\PocoGenerator.xml");

			_log.Add("Writing the output file {0}.", pathFileForXmlOutput);

			_core.WriteOutput(
				Core.Serialise(_classDataList.ToList()),
				pathFileForXmlOutput);

			//	TODO: Write output pocos.
			//	TODO: Make output path settable.
			var pathForPocoOutput = Path.Combine(_coreSettings.RootFolder, @"..\Poco\");
			_log.Add("Writing {0} classes in {1}.", _classDataList.Count, pathForPocoOutput);

			WritePocoClasses(pathForPocoOutput);

			//	TODO: Add Poco files to project.
        }

		internal void ReadXml()
		{
			//TODO:Change to not rooted path.
			var xmlPathFile = @"C:\DATA\PROJEKT\St4mpede\St4mpede\St4mpede\St4mpede\RdbSchema\St4mpede.RdbSchema.xml";
			_log.Add("Reading xml {0}.", xmlPathFile);

			var doc = _xDocHandler.Load(xmlPathFile);

			var database = Core.Deserialise<DatabaseData>(doc);

			this._database = database;
			_log.Add(string.Format("Read database with tables: {0}.",
				string.Join(", ", database.Tables.Select(t=>t.Name))));
		}

		internal string ToInfo()
		{
			return _log.ToInfo();
		}

		internal interface IXDocHandler
		{
			XDocument Load(string pathfile);
		}
		internal class XDocHandler : IXDocHandler
		{
			XDocument IXDocHandler.Load( string pathfile)
			{
				return XDocument.Load(pathfile);
			}
		}

		#region Private methods.

		internal class TypesTuple
		{
			internal string DatabaseTypeName { get; set; }
			internal string DotnetTypeName { get; set; }
			internal TypesTuple(string databaseTypeName, string dotnetTypeName)
			{
				DatabaseTypeName = databaseTypeName;
				DotnetTypeName = dotnetTypeName;
			}
		}

		/// <summary>This is a dictionary of how the database types match to dotnet types.
		/// TODO:Create a dictionary of this list. Maybe we can get rid of the case of not ubiquitous data too.
		/// <para>It cannot be static as it messes up the test. We have a test that tests if we have a not ubuquitous type conversion and for that we manipulate this dictionar to be in a not correct way. If it is static this erroneous Types stays put before next test that then fails. Run next test by itself and the test succeeds. Hard error to track down that is.</para>
		/// </summary>
		private IList<TypesTuple> Types = new List<TypesTuple>
		{
			new TypesTuple(DatabaseTypes.DateTime, typeof(DateTime).ToString()),
			new TypesTuple(DatabaseTypes.NChar, typeof(char).ToString()),
			new TypesTuple(DatabaseTypes.NVarChar, typeof(string).ToString()),
			new TypesTuple(DatabaseTypes.Int, typeof(int).ToString()),
			new TypesTuple(DatabaseTypes.VarChar, typeof(string).ToString()),
		};

		private string AddSuffix(string name)
		{
			return name + ".cs";
		}

		private string ConvertDatabaseTypeToDotnetType(string databaseTypeName)
		{
			var res = Types.Where(t => t.DatabaseTypeName == databaseTypeName).ToList();
			switch(res.Count)
			{
				case 0:
					return string.Format("ERROR! Unkown database type {0}. This is a technical error and teh dictionary should be updated.", databaseTypeName);
				case 1:
					return res.Single().DotnetTypeName;
				default:
					return string.Format("ERROR! Not ubiquitous database type {0} as it could be referenced to any of [{1}]. This is a technical error and the dictionary should be updated.", databaseTypeName, string.Join(",", res.Select(t=>t.DotnetTypeName)));
			}
		}

		private void WritePocoClasses(string pathForPocoOutput)
		{
			foreach( var classData in _classDataList)
			{
				var res= MakeClass(classData);
				_core.WriteOutput(res, Path.Combine(pathForPocoOutput,
					AddSuffix(classData.Name)));
            }
		}

		private IList<string> MakeClass(ClassData classData)
		{
			var ret = new List<string>();
			ret.Add(string.Format( "//\tThis file was generated by St4mpede {0}.", DateTime.Now.ToString("u")));
			ret.Add(string.Empty);
			ret.Add(string.Format("public class {0}", classData.Name));
			ret.Add("{");
			foreach( var property in classData.Properties)
			{
				ret.Add(
					string.Format(
						"\tpublic {0} {1} {{ get; set; }}", 
						property.DotnetTypeName, 
						property.Name));
			}
			ret.Add("}");
			return ret;
		}

		#endregion

		#region Methods and properties for making automatic testing possible without altering the architecture.

		internal IList<ClassData> UT_ClassData
		{
			get
			{
				return _classDataList;
			}
			set
			{
				_classDataList = value;
			}
		}

		internal string UT_ConvertDatabaseTypeToDotnetType(string databaseType)
		{
			return ConvertDatabaseTypeToDotnetType(databaseType);
        }

		internal DatabaseData UT_DatabaseData
		{
			get { return this._database; }
			set { this._database = value; }
		}

		[DebuggerStepThrough]
		internal void UT_Init(CoreSettings coreSettings, XElement settingsElement)
		{
			Init(coreSettings, settingsElement);
		}

		internal string UT_OutputFolder
		{
			get { return _outputFolder; }
			set { _outputFolder = value; }
		}

		internal CoreSettings UT_CoreSettings
		{
			get { return _coreSettings; }
			set { _coreSettings = value; }
		}

		internal IList<TypesTuple> UT_Types
		{
			get
			{
				return Types;
			}
		}

		#endregion
	}

#if NOT_IN_T4
} //end the namespace
#endif
//#>
