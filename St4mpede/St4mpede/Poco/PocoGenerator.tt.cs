//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
namespace St4mpede.Poco
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	using Code;
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

		private const string ConstructorsElement = "Constructors";
		private const string ConstructorsDefaultElement = "Default";
		private const string ConstructorsAllPropertiesElement = "AllProperties";
		private const string ConstructorsAllPropertiesSansPrimaryKeyElement = "AllPropertiesSansPrimaryKey";
		private const string MakePartialElement = "MakePartial";
        private const string OutputFolderElement = "OutputFolder";
		private const string ProjectPathElement = "ProjectPath";
		private const string XmlOutputFilenameElement = "XmlOutputFilename";
		
        private ICore _core;

		private ILog _log;

		private IXDocHandler _xDocHandler;

		private IList<ClassData> _classDataList;

		private DatabaseData _database;

		private CoreSettings _coreSettings;

		private PocoSettings _pocoSettings;

		private IParserSettings _rdbSchemaSettings;

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
				.Where(t => t.Include)
				.ToList()
				.ForEach(table =>
		   {
			   //	Set data for the very [class].
			   var classData = new ClassData
			   {
				   Name = table.Name,
				   IsPartial = _pocoSettings.MakePartial,
				   Properties = new List<PropertyData>(),
                   Methods = new List<MethodData>()
			   };

			   //	Create the properties.
			   table.Columns
			   .ForEach(column =>
			   {
				   classData.Properties.Add(
					   new PropertyData
					   {
						   Name = column.Name,
						   Scope = Common.VisibilityScope.Public,
						   SystemType = ConvertDatabaseTypeToDotnetType(column.DatabaseTypeName),
						   Comment = column.IsInPrimaryKey ?
								new CommentData("This property is part of the primary key.")
								: null
					   });
			   });

			   //	Create the constructors.
			   if (_pocoSettings.CreateDefaultConstructor)
			   {
				   classData.Methods.Add(new MethodData
				   {
					   IsConstructor = true,
					   Name = classData.Name,
					   Comment = new CommentData("Default constructor needed for instance for de/serialising.")
				   });
			   }
			   if (_pocoSettings.CreateAllPropertiesConstructor)
			   {
				   classData.Methods.Add( new MethodData
				   {
					   IsConstructor = true,
					   Name = classData.Name,
					   Comment = new CommentData("This constructor takes all properties as parameters."),
					   Parameters = table.Columns.Select(p =>
					   new ParameterData
					   {
						   Name = p.Name,
						   SystemTypeString =ConvertDatabaseTypeToDotnetTypeString( p.DatabaseTypeName)
					   }).ToList()
				   });
               }
			   if (_pocoSettings.CreateAllPropertiesSansPrimaryKeyConstructor)
			   {
				   classData.Methods.Add(new MethodData
				   {
					   IsConstructor = true,
					   Name = classData.Name,
					   Comment = new CommentData("This constructor takes all properties but primary keys as parameters."),
					   Parameters = table.Columns
						.Where(p=>false == p.IsInPrimaryKey)
						.Select(p =>
						   new ParameterData
						   {
							   Name = p.Name,
							   SystemTypeString = ConvertDatabaseTypeToDotnetTypeString( p.DatabaseTypeName)
						   }).ToList()
				   });
			   }

			   _classDataList.Add(classData);
		   });

			_log.Add("Included classes are:");
			_log.Add(_classDataList.ToInfo());
		}

		internal void Init( string hostTemplateFile)
		{
			Init(hostTemplateFile, null, null);
		}

		internal void Init(string hostTemplateFile, string configFilename, 
			Func<string, string, XDocument> readConfigFunction)
		{
			if (null == hostTemplateFile) { throw new ArgumentNullException("hostTemplateFile"); }

			var configPath = Path.GetDirectoryName(hostTemplateFile);
			configFilename = configFilename ?? Core.DefaultConfigFilename;
			readConfigFunction = readConfigFunction ?? Core.ReadConfig;

			var doc = readConfigFunction(
				configPath,
				configFilename);

			var settings = (from c in doc.Descendants(PocoElement) select c).SingleOrDefault();
			if( null == settings)
			{
				_log.Add("Configuration does not contain element {0}", PocoElement);
				return;	//	Bail.
			}
			Init(Core.Init(doc), ParserSettings.Init(configPath, configFilename, doc), settings);
		}

		private void Init( CoreSettings settings, IParserSettings rdbSchemaSettings, XElement doc)
		{
			_coreSettings = settings;
			_rdbSchemaSettings = rdbSchemaSettings;
			_pocoSettings = new PocoSettings(
				bool.Parse( doc.Descendants(MakePartialElement).Single().Value ),
				bool.Parse(doc.Descendants(ConstructorsElement).Single().Descendants(ConstructorsDefaultElement).Single().Value),
				bool.Parse(doc.Descendants(ConstructorsElement).Single().Descendants(ConstructorsAllPropertiesElement).Single().Value),
				bool.Parse(doc.Descendants(ConstructorsElement).Single().Descendants(ConstructorsAllPropertiesSansPrimaryKeyElement).Single().Value),
				doc.Descendants(OutputFolderElement).Single().Value, 
				doc.Descendants(ProjectPathElement).Single().Value,
                doc.Descendants(XmlOutputFilenameElement).Single().Value
			);
		}

		internal void Output()
		{
			var pathFileForXmlOutput =
				Path.Combine(_coreSettings.RootFolder,
				Path.Combine( _pocoSettings.XmlOutputPathFilename	//	@"Poco\PocoGenerator.xml"
				));

			_log.Add("Writing the output file {0}.", pathFileForXmlOutput);

			_core.WriteOutput(
				Core.Serialise(_classDataList.ToList()),
				pathFileForXmlOutput);

			var pathForPocoOutput = Path.Combine(_coreSettings.RootFolder, _pocoSettings.OutputFolder);
			_log.Add("Writing {0} classes in {1}.", _classDataList.Count, pathForPocoOutput);

			WritePocoClasses(pathForPocoOutput);

			//	TODO: Add Poco files to project.
        }

		internal void ReadXml()
		{
			var xmlPathFile = Path.Combine(
				//@"C:\DATA\PROJEKT\St4mpede\St4mpede\St4mpede\St4mpede",
				_coreSettings.RootFolder,
				//	\RdbSchema",
				_rdbSchemaSettings.ProjectPath,
				//St4mpede.RdbSchema.xml";
				_rdbSchemaSettings.DatabaseXmlFile);
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

		private Type ConvertDatabaseTypeToDotnetType(string databaseTypeName)
		{
			var res = Types.Where(t => t.DatabaseTypeName == databaseTypeName).ToList();
			switch (res.Count)
			{
				case 0:
					throw new ArgumentException( string.Format( "ERROR! Unkown database type {0}. This is a technical error and the dictionary should be updated.", databaseTypeName), "databaseTypeName");
				case 1:
					return Type.GetType( res.Single().DotnetTypeName);
				default:
						throw new ArgumentException( string.Format("ERROR! Not ubiquitous database type {0} as it could be referenced to any of [{1}]. This is a technical error and the dictionary should be updated.", databaseTypeName, string.Join(",", res.Select(t => t.DotnetTypeName)) 
							), 
						"databaseTypeName");
			}
		}

		private string ConvertDatabaseTypeToDotnetTypeString(string databaseTypeName)
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
				var cls= MakeClass(classData);
				_core.WriteOutput(cls, Path.Combine(pathForPocoOutput,
					AddSuffix(classData.Name)));
            }
		}

		private IList<string> MakeClass(ClassData classData)
		{
			var ret = new List<string>();
			ret.Add(string.Format( "//\tThis file was generated by St4mpede {0}.", DateTime.Now.ToString("u")));
			ret.Add(string.Empty);

			ret.AddRange(classData.ToCode());

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
			return ConvertDatabaseTypeToDotnetTypeString(databaseType);
        }

		internal DatabaseData UT_DatabaseData
		{
			get { return this._database; }
			set { this._database = value; }
		}

		[DebuggerStepThrough]
		internal void UT_Init(CoreSettings coreSettings, IParserSettings rdbSchemaSettings, XElement settingsElement)
		{
			Init(coreSettings, rdbSchemaSettings, settingsElement);
		}

		internal PocoSettings UT_PocoSettings
		{
			get { return _pocoSettings; }
			set { _pocoSettings = value; }
		}

		internal CoreSettings UT_CoreSettings
		{
			get { return _coreSettings; }
			set { _coreSettings = value; }
		}

		internal ParserSettings UT_RdbSchema {
			set
			{
				_rdbSchemaSettings = value;
			}
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
