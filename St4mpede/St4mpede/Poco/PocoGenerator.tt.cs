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
		private const string PocoElement = "Poco";
		private const string OutputFolderElement = "OutputFolder";

		private ILog _log;

		private IXDocHandler _xDocHandler;

		private IList<ClassData> _classDataList;

		private DatabaseData _database;

		private string _outputFolder;

		private CoreSettings _coreSettings;

		private Action<XDocument, string> _writeOutputFunction;

        internal PocoGenerator()
			:this(new Log(), new XDocHandler(), Core.WriteOutput)
		{	}

		internal PocoGenerator(ILog log, IXDocHandler xDocHandler,
			Action<XDocument, string> writeOutputFunction)
		{
			_log = log;
			_xDocHandler = xDocHandler;
			_writeOutputFunction = writeOutputFunction;
		}

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

		internal void Init(string configPath, string configFilename,
			Func<string, string, XDocument> readConfigFunction)
		{
			if (null == configPath) { throw new ArgumentNullException("configPath"); }
			if (null == configFilename) { throw new ArgumentNullException("configFilename"); }
			if (null == readConfigFunction) { throw new ArgumentNullException("readConfigFunction"); }

			var doc = readConfigFunction(configPath, configFilename);

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
			_log.Add("Writing {0} classes.", _classDataList.Count);

			_writeOutputFunction(
				Core.Serialise(_classDataList.ToList()),
				Path.Combine(_coreSettings.RootFolder, "PocoGenerator.xml"));
		}

		internal void ReadXml()
		{
			var xmlPathFile = @"C:\DATA\PROJEKT\St4mpede\St4mpede\St4mpede\St4mpede\St4mpede.xml";
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
			new TypesTuple("nvarchar", typeof(string).ToString()),
			new TypesTuple("numeric", typeof(int).ToString())
		};

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
