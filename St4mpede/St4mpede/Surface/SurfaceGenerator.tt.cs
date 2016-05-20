//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
namespace St4mpede.St4mpede.Surface
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	using Code;
	using RdbSchema;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Xml.Linq;

#endif
	//#	Regular ol' C# classes and code...

	internal class SurfaceGenerator
	{
		#region Private properties and fields.

		private const string SurfaceElement = "Surface";
		private const string OutputFolderElement = "OutputFolder";
		private const string ProjectPathElement = "ProjectPath";
		private const string XmlOutputFilenameElement = "XmlOutputFilename";

        private readonly ICore _core;

		private readonly ILog _log;

		private IList<ClassData> _surfaceDataList;

		private DatabaseData _database;

		private readonly IXDocHandler _xDocHandler;

		private CoreSettings _coreSettings;
		private SurfaceSettings _surfaceSettings;
		private IParserSettings _rdbSchemaSettings;

		#endregion

		#region Constructors.

		public SurfaceGenerator()
			:this(new Core(), new Log(), new XDocHandler())
		{	}

		internal SurfaceGenerator(ICore core, ILog log, IXDocHandler xDocHandler)
		{
			_core = core;
			_log = log;
			_xDocHandler = xDocHandler;
		}

		#endregion

		internal void Generate()
		{
			_surfaceDataList = new List<ClassData>();

			var xmlRdbSchemaPathfile = Path.Combine(
				//@"C:\DATA\PROJEKT\St4mpede\St4mpede\St4mpede",
				_coreSettings.RootFolder,
				//	\RdbSchema",
				_rdbSchemaSettings.ProjectPath,
				//St4mpede.RdbSchema.xml";
				_rdbSchemaSettings.DatabaseXmlFile);

			IParserLogic2 parserLogic2 = new ParserLogic2();

			_log.Add("Reading xml {0}.", xmlRdbSchemaPathfile);

			_database = parserLogic2.GetResult(xmlRdbSchemaPathfile);

			_database.Tables.ForEach(table =>
			{
			   //	Set data for the very [class].
				var surfaceClass = new ClassData
				{
					Name = $"{table.Name}Surface",
					 Comment = new CommentData($"This is the Surface for the {table.Name} table."),
				    IsPartial = false,
					Properties = new List<PropertyData>(),
					Methods = new List<MethodData>()
				};
				_surfaceDataList.Add(surfaceClass);

				//	#	Create the properties.
				//	Well... we don't have any properties.
				
				//	#	Create the constructor.
				var constructorMethod = new MethodData
				{
					IsConstructor = true,
					Name = surfaceClass.Name
				};
				surfaceClass.Methods.Add(constructorMethod);

				//	#	Create the methods.
				//	##	Create the Add method.
				var addMethod = new MethodData
				{
					Name = "Add",
					Comment = new CommentData("This method is used for adding a new record in the database."),
					Scope = Common.VisibilityScope.Private,
					ReturnTypeString = $"TheDAL.Poco.{table.Name}",	//	TODO:OF:Fetch namespace from Poco project.
					Parameters = table.Columns
						.Where(p => false == p.IsInPrimaryKey)
						.Select(p =>
						   new ParameterData
						   {
							   Name = p.Name,
							   SystemTypeString = parserLogic2.ConvertDatabaseTypeToDotnetTypeString(p.DatabaseTypeName)
						   }).ToList(), 
					Body = new BodyData(new[]
					{
						"//TODO:OF:TBA.",
						"return null;"
					})
				};
				surfaceClass.Methods.Add(addMethod);

				var updateMethod = new MethodData
				{
					Name = "Update",
					Comment = new CommentData("This method is used for updating an existing record in the database."),
					Scope = Common.VisibilityScope.Private,
					ReturnTypeString = $"TheDAL.Poco.{table.Name}", //	TODO:OF:Fetch namespace from Poco project.
					Parameters = table.Columns
						.Select(p =>
						   new ParameterData
						   {
							   Name = p.Name,
							   SystemTypeString = parserLogic2.ConvertDatabaseTypeToDotnetTypeString(p.DatabaseTypeName)
						   }).ToList(),
					Body = new BodyData(new[]
					{
						"//TODO:OF:TBA.",
						"return null;"
					})
				};
				surfaceClass.Methods.Add(updateMethod);

				var upsertMethod = new MethodData
				{
					Name = "Upsert",
					Comment = new CommentData( new[] {
						"This method is used for creating a new or  updating an existing record in the database.",
						"If the primary key is 0 (zero) we know it is a new record and try to add it. Otherwise we try to update the record."
						}),
					Scope = Common.VisibilityScope.Public,
					ReturnTypeString = $"TheDAL.Poco.{table.Name}", //	TODO:OF:Fetch namespace from Poco project.
					Parameters = table.Columns
						.Select(p =>
						   new ParameterData
						   {
							   Name = p.Name,
							   SystemTypeString = parserLogic2.ConvertDatabaseTypeToDotnetTypeString(p.DatabaseTypeName)
						   }).ToList(),
					Body = new BodyData(new[]
					{
						"//TODO:OF:TBA. =>if( return 0 == primarykey ? Add(...) : Update(...)", 
						"return null;"
					})
				};
				surfaceClass.Methods.Add(upsertMethod);
			});

			_log.Add("Included surfaces are:");
			_log.Add(_surfaceDataList.ToInfo());
		}

		internal void Init( string hostTemplateFile)
		{
			_log.Add("HostTemplateFile={0}.", hostTemplateFile);
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

			var settings = (from c in doc.Descendants(SurfaceElement) select c).SingleOrDefault();
			if (null == settings)
			{
				_log.Add("Configuration does not contain element {0}", SurfaceElement);
				return; //	Bail.
			}
			Init(Core.Init(doc), ParserSettings.Init(configPath, configFilename, doc), settings);
		}

		internal void Output()
		{
			var pathFileForXmlOutput =
				Path.Combine(_coreSettings.RootFolder,
				Path.Combine(_surfaceSettings.XmlOutputPathFilename    //	@"Surface\St4mpede.Surface.xml"
				));

			_log.Add("Writing the output file {0}.", pathFileForXmlOutput);

			_core.WriteOutput(
				Core.Serialise(_surfaceDataList.ToList()),
				pathFileForXmlOutput);

			var pathForSurfaceOutput = Path.Combine(_coreSettings.RootFolder, _surfaceSettings.OutputFolder);
			//_log.Add("Writing {0} classes in {1}.", _classDataList.Count, pathForPocoOutput);

			WriteSurfaceClasses(pathForSurfaceOutput);

			//	TODO: Add Surface files to project.
		}

		internal string ToInfo()
		{
			return _log.ToInfo();
		}

		#region Classes and interfaces.

		internal interface IXDocHandler
		{
			XDocument Load(string pathfile);
		}

		internal class XDocHandler : IXDocHandler
		{
			XDocument IXDocHandler.Load(string pathfile)
			{
				return XDocument.Load(pathfile);
			}
		}

		#endregion

		#region Private methods.

		private void Init(CoreSettings settings, IParserSettings rdbSchemaSettings, XElement doc)
		{
			_coreSettings = settings;
			_rdbSchemaSettings = rdbSchemaSettings;

			var outputFolder =
				doc
					.Descendants()
					.Single(e => e.Name == OutputFolderElement)
					.Value;
			_log.Add("OutputFolder={0}.", outputFolder);

			var projectPath =
				doc
					.Descendants()
					.Single(e => e.Name == ProjectPathElement)
					.Value;
			_log.Add("ProjectPath={0}.", projectPath);

			var xmlOutputFilename =
				doc
					.Descendants()
					.Single(e => e.Name == XmlOutputFilenameElement)
					.Value;
			_log.Add("XmlOutputFilename={0}.", xmlOutputFilename);

			_surfaceSettings = new SurfaceSettings(
				outputFolder, 
				projectPath, 
				xmlOutputFilename
			);
		}

		private void WriteSurfaceClasses(string pathForSurfaceOutput)
		{
			foreach (var classData in _surfaceDataList)
			{
				var cls = MakeClass(classData);
				_core.WriteOutput(cls, Path.Combine(pathForSurfaceOutput,
					_core.AddSuffix(classData.Name)));
			}
		}

		private IList<string> MakeClass(ClassData classData)
		{
			var ret = new List<string>();
			ret.Add(string.Format("//\tThis file was generated by St4mpede.Surface {0}.", DateTime.Now.ToString("u")));
			ret.Add(string.Empty);

//			ret.AddRange(_pocoSettings.NameSpaceComments.Select(c => string.Format("//\t{0}", c)));
//			ret.Add(string.Format("namespace {0}", _pocoSettings.NameSpace));
//			ret.Add("{");

			ret.AddRange(classData.ToCode());

//			ret.Add("}");

			return ret;
		}

		#endregion

		#region Methods and properties for making automatic testing possible without altering the architecture.

		#endregion
	}

#if NOT_IN_T4
} //end the namespace
#endif
//#>
