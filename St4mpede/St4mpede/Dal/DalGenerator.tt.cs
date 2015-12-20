//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using St4mpede.St4mpede.Core;

namespace St4mpede.St4mpede.Dal
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	
#endif
	//#	Regular ol' C# classes and code...

	internal class DalGenerator
	{
		#region Private properties and fields.

		private const string DalElement = "Dal";

		private readonly ICore _core;

		private readonly ILog _log;

		private readonly IXDocHandler _xDocHandler;

		#endregion

		internal void Init(string hostTemplateFile)
		{
			Init(hostTemplateFile, null, null);
		}

		internal void ReadXml()
		{
			
		}

		internal void Generate()
		{
		}

		internal void Output()
		{
			
		}

		#region Private methods.

		#region Constructors.

		internal DalGenerator()
			:this(new Core.Core(), new Log(), new XDocHandler())
		{ }

		internal DalGenerator(ICore core, ILog log, IXDocHandler xDocHandler)
		{
			_core = core;
			_log = log;
			_xDocHandler = xDocHandler;
		}

		#endregion

		private void Init(string hostTemplateFile, string configFilename,
	Func<string, string, XDocument> readConfigFunction)
		{
			if (null == hostTemplateFile) { throw new ArgumentNullException("hostTemplateFile"); }

			var configPath = Path.GetDirectoryName(hostTemplateFile);
			configFilename = configFilename ?? Core.Core.DefaultConfigFilename;
			readConfigFunction = readConfigFunction ?? Core.Core.ReadConfig;

			var doc = readConfigFunction(
				configPath,
				configFilename);

			var settings = (from c in doc.Descendants(DalElement) select c).SingleOrDefault();
			if (null == settings)
			{
				_log.Add("Configuration does not contain element {0}", DalElement);
				return; //	Bail.
			}
			Init(Core.Core.Init(doc), ParserSettings.Init(configPath, configFilename, doc), settings);
		}

		private void Init(CoreSettings settings, IParserSettings rdbSchemaSettings, XElement doc)
		{
			throw new NotImplementedException("TBA:Init for CoreSettings.");
		}

		#endregion

			#region Methods for making unit testing possible without affecting the architecure.

			#endregion
		}

#if NOT_IN_T4
	} //end the namespace
#endif
//#>
