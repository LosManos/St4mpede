//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace St4mpede.St4mpede.Poco
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...

#endif
	//#	Regular ol' C# classes and code...

	internal class DalGenerator
	{
		#region Private properties and fields.

		private const string DalElement = "Dal";
		private const string OutputFolderElement = "OutputFolder";
		private const string ProjectPathElement = "ProjectPath";
		private const string XmlOutputFilenameElement = "XmlOutputFilename";

        private readonly ICore _core;

		private readonly ILog _log;

		private readonly IXDocHandler _xDocHandler;

		private CoreSettings _coreSettings;
		private DalSettings _dalSettings;
		private IParserSettings _rdbSchemaSettings;

		#endregion

		#region Constructors.

		public DalGenerator()
			:this(new Core(), new Log(), new XDocHandler())
		{	}

		internal DalGenerator(ICore core, ILog log, IXDocHandler xDocHandler)
		{
			_core = core;
			_log = log;
			_xDocHandler = xDocHandler;
		}

		#endregion

		internal void Generate()
		{
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

			var settings = (from c in doc.Descendants(DalElement) select c).SingleOrDefault();
			if (null == settings)
			{
				_log.Add("Configuration does not contain element {0}", DalElement);
				return; //	Bail.
			}
			Init(Core.Init(doc), ParserSettings.Init(configPath, configFilename, doc), settings);
		}

		internal void Output()
		{
			var pathFileForXmlOutput =
				Path.Combine(_coreSettings.RootFolder,
				Path.Combine(_dalSettings.XmlOutputPathFilename    //	@"Ddal\St4mpede.Dal.xml"
				));

			_log.Add("Writing the output file {0}.", pathFileForXmlOutput);

			//_core.WriteOutput(
			//	Core.Serialise(_classDataList.ToList()),
			//	pathFileForXmlOutput);

			var pathForPocoOutput = Path.Combine(_coreSettings.RootFolder, _dalSettings.OutputFolder);
			//_log.Add("Writing {0} classes in {1}.", _classDataList.Count, pathForPocoOutput);

			//WriteDalClasses(pathForPocoOutput);

			//	TODO: Add Dal files to project.
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
			_log.Add("XmlOutputFilename={0}.", outputFolder);

			_dalSettings = new DalSettings(
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
