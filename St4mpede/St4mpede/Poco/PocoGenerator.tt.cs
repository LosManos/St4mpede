//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace St4mpede.Poco
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	//	using System.Xml.Serialization;
#endif
	//#	Regular ol' C# classes and code...

	internal class PocoGenerator
	{
		private ILog _log;

		private IXDocHandler _xDocHandler;

		private IList<ClassData> _classDataList;

		private DatabaseData _database;

		internal PocoGenerator()
			:this(new Log(), new XDocHandler())
		{	}

		internal PocoGenerator(ILog log, IXDocHandler xDocHandler)
		{
			_log = log;
			_xDocHandler = xDocHandler;
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

		internal void Output()
		{
			_log.Add("Writing {0} classes.", _classDataList.Count);
			//xml.Save(Path.Combine(_settings.ConfigPath, St4mpedePath, _settings.OutputXmlFilename));
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

		private class TypesTuple
		{
			internal string DatabaseTypeName { get; set; }
			internal string DotnetTypeName { get; set; }
			public TypesTuple(string databaseTypeName, string dotnetTypeName)
			{
				DatabaseTypeName = databaseTypeName;
				DotnetTypeName = dotnetTypeName;
			}
		}

		//	TODO:Create a dictionary.
		private IList<TypesTuple> Types = new List<TypesTuple>
		{
			new TypesTuple("nvarchar", typeof(string).ToString()),
			new TypesTuple("numeric", typeof(int).ToString())
		};

		private string ConvertDatabaseTypeToDotnetType(string databaseTypeName)
		{
			return Types.Single(t => t.DatabaseTypeName == databaseTypeName).DotnetTypeName;
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

		internal DatabaseData UT_DatabaseData
		{
			get { return this._database; }
			set { this._database = value; }
		}

		#endregion
	}

#if NOT_IN_T4
} //end the namespace
#endif
//#>
