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

		private IList<SurfaceData> _surfaceDataList;

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
			_surfaceDataList = new List<SurfaceData>();

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

			_database.Tables.ForEach(t =>
			{
				var surface = new SurfaceData
				{
					Name = t.Name
				};
				_surfaceDataList.Add(surface);
			});
			_log.Add("Included surfaces are");
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

			var pathForPocoOutput = Path.Combine(_coreSettings.RootFolder, _surfaceSettings.OutputFolder);
			//_log.Add("Writing {0} classes in {1}.", _classDataList.Count, pathForPocoOutput);

			//WriteSurfaceClasses(pathForPocoOutput);

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

		#endregion

		#region Methods and properties for making automatic testing possible without altering the architecture.

		#endregion
	}

#if NOT_IN_T4
} //end the namespace
#endif
//#>
