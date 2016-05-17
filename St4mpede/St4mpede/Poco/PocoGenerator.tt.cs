//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
namespace St4mpede.St4mpede.Poco
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Xml.Linq;
	using Code;
	using RdbSchema;
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
		private const string ConstructorCopy = "CopyConstructor";

		private const string MethodsElement = "Methods";
		private const string MethodsEqualsElement = "Equals";
		private const string MethodsEqualsRegexAttribute = "Regex";

		private const string MakePartialElement = "MakePartial";

		private const string NameSpaceElement = "NameSpace";
		private const string NameSpaceNameAttribute = "Name";
		private const string NameSpaceCommentElement = "Comment";
		private const string NameSpaceCommentsElement = "Comments";

        private const string OutputFolderElement = "OutputFolder";
		private const string ProjectPathElement = "ProjectPath";
		private const string XmlOutputFilenameElement = "XmlOutputFilename";
		
        private readonly ICore _core;

		private readonly ILog _log;

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
			internal const string Char = "char";
		}

		#region Constructors.

		internal PocoGenerator()
			:this(new Core(), new Log() )
		{	}

		internal PocoGenerator(ICore core, ILog log)
		{
			_core = core;
			_log = log;
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

			   //	#	Create the properties.
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

			   //	#	Create the constructors.
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
						   SystemTypeString = ConvertDatabaseTypeToDotnetTypeString( p.DatabaseTypeName)
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

			   if(_pocoSettings.CreateCopyConstructor)
			   {
				   classData.Methods.Add(new MethodData
				   {
					   IsConstructor = true,
					   Name = classData.Name,
					   Comment = new CommentData("This is the copy constructor."),
					   Parameters = new List<ParameterData>
					   {
						   new ParameterData
						   {
							   Name = classData.Name,
							   SystemTypeString = classData.Name
						   }
					   },
					   Body = new BodyData
					   {
						   Lines = table.Columns
							.Select(p => string.Format("this.{0} = {1}.{2};",
							p.Name, Common.Safe(Common.ToCamelCase(table.Name)), p.Name))
							.ToList()
					   }
				   });
			   }

			   //	#	Create the methods.
			   if (_pocoSettings.CreateMethodEquals && Regex.IsMatch(classData.Name, _pocoSettings.CreateMethodEqualsRegex))
			   {
				   classData.Methods.Add(
					   CreateEqualsMethod(table, classData, "o"));

				   classData.Methods.Add(
					   CreateHashMethod(table, classData));
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
			var xmlRdbSchemaPathfile = Path.Combine(
				//@"C:\DATA\PROJEKT\St4mpede\St4mpede\St4mpede",
				_coreSettings.RootFolder,
				//	\RdbSchema",
				_rdbSchemaSettings.ProjectPath,
				//St4mpede.RdbSchema.xml";
				_rdbSchemaSettings.DatabaseXmlFile);

			IParserLogic2 parserLogic2 = new ParserLogic2();

			_log.Add("Reading xml {0}.", xmlRdbSchemaPathfile);

			//_database = _core.ReadFromXmlPathfile<DatabaseData>(xmlRdbSchemaPathfile);
			_database = parserLogic2.GetResult(xmlRdbSchemaPathfile);

			_log.Add(string.Format("Read database with tables: {0}.",
				string.Join(", ", _database.Tables.Select(t=>t.Name))));
		}

		internal string ToInfo()
		{
			return _log.ToInfo();
		}

		#region Private methods.

		private static IList<string> CreateBodyForEqualsMethod(TableData table, ClassData classData, string parameterName)
		{
			var bodyLines = new List<string>
					{
						string.Format("var obj = {0} as {1};", parameterName, classData.Name),
						"if( obj == null ){",
						"\treturn false;",
						"}",
						string.Empty,
						"return"
					};
			bodyLines.AddRange(table.Columns.Select(c =>
				string.Format("\tthis.{0} == obj.{0} &&",
				c.Name,
				parameterName
			)));
			bodyLines[bodyLines.Count - 1] = bodyLines.Last().Replace(" &&", ";");
			return bodyLines;
		}

		private IList<string> CreateBodyForGetHashCodeMethod(TableData table)
		{
			var bodyLines = new List<string>
			{
				"int hash = 13;"
			};
			bodyLines.AddRange(table.Columns.Select(c =>
				"hash = (hash*7) + " +
				(
					DefaultValueIsNull( ConvertDatabaseTypeToDotnetType(c.DatabaseTypeName))
				?   //	It is a parameter that cannot be null.
                    string.Format("( null == {0} ? 0 : this.{0}.GetHashCode() );", c.Name)
				:	//	It is a value that can be null.
					string.Format("this.{0}.GetHashCode();", c.Name)
			)));
			bodyLines.Add("return hash;");
			return bodyLines;
		}

		private static MethodData CreateEqualsMethod(TableData table, ClassData classData, string ParameterName)
		{
			return new MethodData
			{
				IsConstructor = false,
				Name = "Equals",
				Override = true,
				ReturnTypeString = typeof(bool).ToString(),
				Comment = new CommentData("This is the Equals method."),
				Parameters = new List<ParameterData>
				{
					new ParameterData
					{
						Name = ParameterName,
						SystemTypeString = typeof(object).ToString()
					}
				},
				Body = new BodyData(
					CreateBodyForEqualsMethod(table, classData, ParameterName)
					)
			};
		}

		private MethodData CreateHashMethod(TableData table, ClassData classData)
		{
			return new MethodData
			{
				IsConstructor = false,
				Name = "GetHashCode",
				Override = true,
				ReturnTypeString = typeof(int).ToString(),
				Comment = new CommentData("This is the GetHashCode method."),
				Parameters = null,
				Body = new BodyData(CreateBodyForGetHashCodeMethod(table))
			};
		}

		/// <summary>This method returns true if the default value of the parameter type is null. The reason this is in a method of its own (instead of calling (Type).IsValueType is that I have read, but not verified, that Nullable(of) might behave as a value type while it is still null for St4mpede.Poco logic.
		/// This way we can unit test.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static bool DefaultValueIsNull( Type type)
		{
			if(type.IsGenericType)
			{
				return true;
			}
			return false == type.IsValueType;
		}

		private void Init(CoreSettings settings, IParserSettings rdbSchemaSettings, XElement doc)
		{
			_coreSettings = settings;
			_rdbSchemaSettings = rdbSchemaSettings;
			_pocoSettings = new PocoSettings(
				bool.Parse(doc.Descendants(MakePartialElement).Single().Value),

				doc.Descendants(NameSpaceElement).Single().Attributes(NameSpaceNameAttribute).Single().Value,
				
				doc.Descendants(NameSpaceElement).Single().Descendants(NameSpaceCommentsElement).Single().Descendants(NameSpaceCommentElement).Select(e=>e.Value).ToList(),

				bool.Parse(doc.Descendants(ConstructorsElement).Single().Descendants(ConstructorsDefaultElement).Single().Value),
				bool.Parse(doc.Descendants(ConstructorsElement).Single().Descendants(ConstructorsAllPropertiesElement).Single().Value),
				bool.Parse(doc.Descendants(ConstructorsElement).Single().Descendants(ConstructorsAllPropertiesSansPrimaryKeyElement).Single().Value),
				bool.Parse(doc.Descendants(ConstructorsElement).Single().Descendants(ConstructorCopy).Single().Value),

				bool.Parse(doc.Descendants(MethodsElement).Single().Descendants(MethodsEqualsElement).Single().Value),
				doc.Descendants(MethodsElement).Single().Descendants(MethodsEqualsElement).Single().Attributes(MethodsEqualsRegexAttribute).Single().Value,

				doc.Descendants(OutputFolderElement).Single().Value,
				doc.Descendants(ProjectPathElement).Single().Value,
				doc.Descendants(XmlOutputFilenameElement).Single().Value
			);
		}

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
		/// <para>
		/// NOTE: It cannot be static as it messes up the test. 
		/// We have a test that tests if we have a not ubuquitous type conversion and for that we manipulate this dictionary to be in a not correct way. If it is static this erroneous Types stays put before next test that then fails. Run next test by itself and the test succeeds. Hard error to track down that is.</para>
		/// </summary>
		private IList<TypesTuple> Types = new List<TypesTuple>
		{
			new TypesTuple(DatabaseTypes.DateTime, typeof(DateTime).ToString()),
			new TypesTuple(DatabaseTypes.NChar, typeof(char).ToString()),
			new TypesTuple(DatabaseTypes.NVarChar, typeof(string).ToString()),
			new TypesTuple(DatabaseTypes.Int, typeof(int).ToString()),
			new TypesTuple(DatabaseTypes.VarChar, typeof(string).ToString()),
			new TypesTuple(DatabaseTypes.Char, typeof(string).ToString()),
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
			ret.Add(string.Format( "//\tThis file was generated by St4mpede,Poco {0}.", DateTime.Now.ToString("u")));
			ret.Add(string.Empty);

			ret.AddRange(_pocoSettings.NameSpaceComments.Select(c => string.Format("//\t{0}",c)));
			ret.Add(string.Format("namespace {0}", _pocoSettings.NameSpace));
			ret.Add("{");

			ret.AddRange(classData.ToCode(new Indent(1)));

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
			return ConvertDatabaseTypeToDotnetTypeString(databaseType);
        }

		internal DatabaseData UT_DatabaseData
		{
			get { return this._database; }
			set { this._database = value; }
		}

		[DebuggerStepThrough]
		internal static IList<string> UT_CreateBodyForEqualsMethod(TableData table, ClassData classData, string parameterName)
		{
			return CreateBodyForEqualsMethod(table, classData,parameterName);
        }

		[DebuggerStepThrough]
		internal IList<string> UT_CreateBodyForGetHashCodeMethod(TableData table)
		{
			return CreateBodyForGetHashCodeMethod(table);
        }

		[DebuggerStepThrough]
		internal static bool UT_DefaultValueIsNull(Type type)
		{
			return DefaultValueIsNull(type);
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
