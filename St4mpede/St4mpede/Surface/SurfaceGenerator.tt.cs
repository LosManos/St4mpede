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
	using Sql;
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

		//private readonly IXDocHandler _xDocHandler;

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
			//_xDocHandler = xDocHandler;
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
				surfaceClass.Methods.Add(new MethodData
				{
					Name = "Add",
					Comment = new CommentData( new[] {
						"This method is used for adding a new record in the database.",
						"<para>St4mpede guid:{BC89740A-CBA5-4EDE-BFF4-9B0D8BA4058F}</para>"}),
					Scope = Common.VisibilityScope.Private,
					ReturnTypeString = $"TheDAL.Poco.{table.Name}", //	TODO:OF:Fetch namespace from Poco project.
					Parameters = new[] {
							new ParameterData
							{
								Name = "context",
								SystemTypeString = "TheDAL.CommitScope"	//	TODO:OF:Fetch namespace from settings.
							}
						}
						.Concat(
								table.NonPrimaryKeyColumns
									.Select(p =>
										new ParameterData
									   {
										   Name = p.Name,
										   SystemTypeString = parserLogic2.ConvertDatabaseTypeToDotnetTypeString(p.DatabaseTypeName)
									   }))
						.ToList(),
					Body = new BodyData(new[]
					{
						$"return Add( ",
						"\tcontext, ",
						$"	new TheDAL.Poco.{table.Name}(", //	TODO:OF:Fetch namespace from Poco project.
						//	TODO:OF:We should set default value and not 0 to the primary keys.
						"\t\t" + string.Join(", ", table.PrimaryKeyColumns.Select(c=>"0")) + ", ",
						"\t\t" + string.Join(", ", table.NonPrimaryKeyColumns.Select(c=>c.Name.ToCamelCase()).ToList()),
						"));"
					})
				});

		//	##	Create the Add method.
		surfaceClass.Methods.Add(new MethodData
				{
					Name = "Add",
					Comment = new CommentData(new[] {
						"This method is used for adding a new record in the database.",
						"<para>St4mpede guid:{759FBA46-A462-4E4A-BA2B-9B5AFDA572DE}</para>"}),
					Scope = Common.VisibilityScope.Private,
					ReturnTypeString = $"TheDAL.Poco.{table.Name}", //	TODO:OF:Fetch namespace from Poco project.
					Parameters =
							new[] { new ParameterData
							{
								Name="context", 
								SystemTypeString = "TheDAL.CommitScope"	//	TODO:OF:Fetch namespace from settings.
							} }
						.Concat(
							new[] {	new ParameterData
							   {
								   Name = table.Name,	//	TODO:OF:Make camelcase.
								   SystemTypeString = $"TheDAL.Poco.{table.Name}", //	TODO:OF:Fetch namespace from Poco project.
								}
							})
						.ToList(),
					Body = new BodyData(new[]
					{
						$"const string Sql = @" + "\"",
						$"	Insert Into {table.Name}",
						$"	(",
						$"		{string.Join(", ",table.Columns.Where(c=>false==c.IsInPrimaryKey).Select(c=>c.Name))}",
						$"	)",
						$"	Values",
						$"	(",
						//	TODO:OF:Make lowercase.
						$"		{string.Join(", ",table.Columns.Where(c=>false==c.IsInPrimaryKey).Select(c=>"@" + c.Name))}",
						$"	)",
						$"	Select * From {table.Name} Where {table.Columns.Single(c=>c.IsInPrimaryKey).Name} = Scope_Identity()" + "\";",
						$"var ret = context.Connection.Query<TheDAL.Poco.{table.Name}>(", //	TODO:OF:Fetch namespace from Poco project.
						"	Sql, ",
						"	new {",
						$"	{string.Join(", ", table.Columns.Where(c=>c.IsInPrimaryKey).Select(c=> "\t" + c.Name.ToCamelCase() + " = " + table.Name.ToCamelCase() + "." + c.Name  ))}",
						"	},",
						"	context.Transaction,", 
						"	false, null, null);", 
						"return ret.Single();",
					})
				});

				//	##	Create the Delete method.
				surfaceClass.Methods.Add(new MethodData
				{
					Name = "Delete",
					Comment = new CommentData(new[] {
						"This method is used for deleting a record.",
						"<para>St4mpede guid:{F74246AE-0295-4094-AA7F-1D118C11229D}</para>"
						}),
					Scope = Common.VisibilityScope.Public,
					ReturnTypeString = $"void",
					Parameters =
							new[] { new ParameterData
							{
								Name="context",
								SystemTypeString = "TheDAL.CommitScope"	//	TODO:OF:Fetch namespace from settings.
							} }
						.Concat(
							new [] { new ParameterData
							   {
								   Name = table.Name,
								   SystemTypeString = $"TheDAL.Poco.{table.Name}", //	TODO:OF:Fetch namespace from Poco project.
							   }
					}).ToList(),
					Body = new BodyData(new[]
					{
						"DeleteById( context, " +
						string.Join(", ", table.PrimaryKeyColumns.Select(c=>
						$"{table.Name.ToCamelCase()}.{c.Name}")) + ");"
					})
				});

				//	##	Create the DeleteById method.
				surfaceClass.Methods.Add(new MethodData
				{
					Name = "DeleteById",
					Comment = new CommentData(new[] {
						"This method is used for deleting a record by its primary key.",
						"<para>St4mpede guid:{F0137E62-1D45-4B92-A48E-05954850FFE8}</para>"
						}),
					Scope = Common.VisibilityScope.Public,
					ReturnTypeString = $"void",
					Parameters =
							new[] { new ParameterData
							{
								Name="context",
								SystemTypeString = "TheDAL.CommitScope"	//	TODO:OF:Fetch namespace from settings.
							} }
						.Concat(
							table.PrimaryKeyColumns
								.Select(p =>
								   new ParameterData
								   {
									   Name = p.Name,
									   SystemTypeString = parserLogic2.ConvertDatabaseTypeToDotnetTypeString(p.DatabaseTypeName)
								   }))
						.ToList(),
					Body = new BodyData(new[]
						{
							$"const string Sql = @" + "\"",
							$"	Delete From {table.Name}",
							"\t Where " + string.Join(" And ", table.PrimaryKeyColumns.Select(c=>$"{c.Name} = @{c.Name.ToCamelCase()}")),
							"\";",
							$"context.Connection.Execute(",
							"	Sql, ",
							"	new {",
							$"	{string.Join(", ", table.Columns.Where(c=>c.IsInPrimaryKey).Select(c=> "\t" +$"{c.Name.ToCamelCase()} = {c.Name.ToCamelCase()}"  ))}",
							"	},",
							"	context.Transaction,",
							"	null, null);",
						})
				});

				//	##	Create the GetAll method.
				surfaceClass.Methods.Add(new MethodData
				{
					Name = "GetAll",
					Comment = new CommentData(new[] {
						"This method returns every record for this table/surface.",
						"Do not use it on too big tables.",
						"<para>St4mpede guid:{4A910A99-9A50-4977-B2A2-404240CDDC73}</para>"
						}),
					Scope = Common.VisibilityScope.Public,
					ReturnTypeString = $"IList<TheDAL.Poco.{table.Name}>", //	TODO:OF:Fetch namespace from Poco project.
					Parameters =
						new[] { new ParameterData
							{
								Name="context",
								SystemTypeString = "TheDAL.CommitScope"	//	TODO:OF:Fetch namespace from settings.
							} }.ToList(),
					Body = new BodyData(new[]
						{
							$"const string Sql = @" + "\"",
							$"	Select * From {table.Name}",
							"\";",
							$"var res = context.Connection.Query<TheDAL.Poco.{table.Name}>(",	//	TODO:OF:Fetch namespace from settings.
							"	Sql, ",
							"	null, ",
							"	context.Transaction,",
							"	false, null, null);",
							"return res.ToList();"
					})
				});

				//	##	Create the GetById method.
				surfaceClass.Methods.Add(new MethodData
				{
					Name = "GetById",
					Comment = new CommentData(new[] {
						"This method is used for getting a record but its Id/primary key.",
						"If nothing is found an exception is thrown. If you want to get null back use LoadById <see cref=\"LoadById\"/> instead.",
						"<para>St4mpede guid:{71CF185E-0DD1-4FAE-9721-920B5C3C12D9}</para>"
						}),
					Scope = Common.VisibilityScope.Public,
					ReturnTypeString = $"TheDAL.Poco.{table.Name}", //	TODO:OF:Fetch namespace from Poco project.
					Parameters =
						new[] { new ParameterData
							{
								Name="context",
								SystemTypeString = "TheDAL.CommitScope"	//	TODO:OF:Fetch namespace from settings.
							} }
						.Concat(
							table.PrimaryKeyColumns
								.Select(p =>
								   new ParameterData
								   {
									   Name = p.Name,
									   SystemTypeString = parserLogic2.ConvertDatabaseTypeToDotnetTypeString(p.DatabaseTypeName)
								   }))
						.ToList(),
					Body = new BodyData(new[]
						{
							$"const string Sql = @" + "\"",
							$"	Select * From {table.Name}",
							"\t Where " + string.Join(" And ", table.PrimaryKeyColumns.Select(c => $"{c.Name} = @{c.Name.ToCamelCase()}")),
							"\";",
							$"var res = context.Connection.Query<TheDAL.Poco.{table.Name}>(",	//	TODO:OF:Fetch namespace from settings.
							"	Sql, ",
							"	new {",
							$"		{string.Join(", ", table.Columns.Where(c => c.IsInPrimaryKey).Select(c =>$"{c.Name.ToCamelCase()} = {c.Name.ToCamelCase()}"))}",
							"	},",
							"	context.Transaction,",
							"	false, null, null);",
							"return res.Single();"
						})
				});

				//	##	Create the LoadById method.
				surfaceClass.Methods.Add(new MethodData
				{
					Name = "LoadById",
					Comment = new CommentData(new[] {
						"This method is used for getting a record but its Id/primary key.",
						"If nothing is found an null is returned. If you want to throw an exception use GetById <see cref=\"GetById\"> instead.",
						"<para>St4mpede guid:{BC171F29-81F2-41ED-AC5C-AD6884EC9718}</para>"
						}),
					Scope = Common.VisibilityScope.Public,
					ReturnTypeString = $"TheDAL.Poco.{table.Name}", //	TODO:OF:Fetch namespace from Poco project.
					Parameters =
						new[] { new ParameterData
						{
							Name="context",
							SystemTypeString = "TheDAL.CommitScope"	//	TODO:OF:Fetch namespace from settings.
						} }
						.Concat(
							table.Columns
								.Where(c => c.IsInPrimaryKey)
								.Select(p =>
								   new ParameterData
								   {
									   Name = p.Name,
									   SystemTypeString = parserLogic2.ConvertDatabaseTypeToDotnetTypeString(p.DatabaseTypeName)
								   }))
						.ToList(),
					Body = new BodyData(new[]
						{
							$"const string Sql = @" + "\"",
							$"	Select * From {table.Name}",
							"\t Where " + string.Join(" And ", table.PrimaryKeyColumns.Select(c => $"{c.Name} = @{c.Name.ToCamelCase()}")),
							"\";",
							$"var res = context.Connection.Query<TheDAL.Poco.{table.Name}>(",	//	TODO:OF:Fetch namespace from settings.
							"	Sql, ",
							"	new {",
							$"		{string.Join(", ", table.Columns.Where(c => c.IsInPrimaryKey).Select(c =>$"{c.Name.ToCamelCase()} = {c.Name.ToCamelCase()}"))}",
							"	},",
							"	context.Transaction,",
							"	false, null, null);",
							"return res.SingleOrDefault();"
						})
				});

				//	##	Create the Update method.
				surfaceClass.Methods.Add(new MethodData
				{
					Name = "Update",
					Comment = new CommentData(new [] {
						"This method is used for updating an existing record in the database.",
						"<para>St4mpede guid:{5A4CE926-447C-4F3F-ADFC-8CA9229C60BF}</para>"
						}),
					Scope = Common.VisibilityScope.Private,
					ReturnTypeString = $"TheDAL.Poco.{table.Name}", //	TODO:OF:Fetch namespace from Poco project.
					Parameters =
							new[] { new ParameterData
							{
								Name="context",
								SystemTypeString = "TheDAL.CommitScope"	//	TODO:OF:Fetch namespace from settings.
							} }
						.Concat(
							table.Columns
								.Select(p =>
								   new ParameterData
								   {
									   Name = p.Name,
									   SystemTypeString = parserLogic2.ConvertDatabaseTypeToDotnetTypeString(p.DatabaseTypeName)
								   }))
						.ToList(),
					Body = new BodyData(new[]
						{
							$"return Update( ",
							"\tcontext, ",
							$"\tnew TheDAL.Poco.{table.Name}(",	//	TODO:OF:Fetch namespace from settings.
							"\t\t" + string.Join(", ",table.Columns.Select( c=> $"{c.Name.ToCamelCase()}")),
							"));"
						})
				});

				//	##	Create the Update method.
				surfaceClass.Methods.Add(new MethodData
				{
					Name = "Update",
					Comment = new CommentData(new []{
						"This method is used for updating an existing record in the database.",
						"<para>St4mpede guid:{B2B1B845-5F93-4A5C-9F90-FBA570228542}</para>"
						}),
					Scope = Common.VisibilityScope.Private,
					ReturnTypeString = $"TheDAL.Poco.{table.Name}", //	TODO:OF:Fetch namespace from Poco project.
					Parameters = new[] {
							new ParameterData
							{
								Name="context",
								SystemTypeString = "TheDAL.CommitScope"	//	TODO:OF:Fetch namespace from settings.
							},
							new ParameterData
							{
								Name = table.Name,	//	TODO:OF:Make camelcase.
								SystemTypeString = $"TheDAL.Poco.{table.Name}", //	TODO:OF:Fetch namespace from Poco project.
							} }
						.ToList(),
					Body = new BodyData(new[]
						{
						$"const string Sql = @" + "\"",
						$"	Update {table.Name}",
						$"	Set",
						"\t\t" + string.Join(", ",table.NonPrimaryKeyColumns.Select(c=>
							$"{c.Name} = @{c.Name.ToCamelCase()}"
						)),
						$"\tWhere",
						"\t\t" + string.Join(" And ",table.PrimaryKeyColumns.Select(c=>
							$"{c.Name} = @{c.Name}"))
						,
						$"\t Select * From {table.Name} Where ", 
						$"\t\t" + string.Join(" And ", table.PrimaryKeyColumns.Select(c=>$"{c.Name} = ${c.Name.ToCamelCase()}")),
						"\t\";",
						$"var ret = context.Connection.Query<TheDAL.Poco.{table.Name}>(", //	TODO:OF:Fetch namespace from Poco project.
						"	Sql, ",
						"	new {",
						$"	{string.Join(", ", table.Columns.Select(c=> "\t" + c.Name.ToCamelCase() + " = " + table.Name.ToCamelCase() + "." + c.Name  ))}",
						"	},",
						"	context.Transaction,",
						"	false, null, null);",
						"return ret.Single();",
					})
				});

				//	##	Create the Upsert method.
				surfaceClass.Methods.Add(new MethodData
				{
					Name = "Upsert",
					Comment = new CommentData(new[] {
						"This method is used for creating a new or  updating an existing record in the database.",
						"If the primary key is 0 (zero) we know it is a new record and try to add it. Otherwise we try to update the record.",
						"<para>St4mpede guid:{A821E709-7333-4ABA-9F38-E85617C906FE}</para>"
						}),
					Scope = Common.VisibilityScope.Public,
					ReturnTypeString = $"TheDAL.Poco.{table.Name}", //	TODO:OF:Fetch namespace from Poco project.
					Parameters =
						new[] { new ParameterData
							{
								Name="context",
								SystemTypeString = "TheDAL.CommitScope"	//	TODO:OF:Fetch namespace from settings.
							} }
						.Concat(
						table.Columns
							.Select(c =>
							   new ParameterData
							   {
								   Name = c.Name,
								   SystemTypeString = parserLogic2.ConvertDatabaseTypeToDotnetTypeString(c.DatabaseTypeName)
							   }))
						.ToList(),
					Body = new BodyData(new[]
						{
						$"return Upsert(",
						"\tcontext,",
						$"\tnew TheDAL.Poco.{table.Name}(", //	TODO:OF:Fetch namespace from settings.
						"\t\t" + string.Join( ", ", table.Columns.Select(c=>$"{c.Name.ToCamelCase()}")),
						"));"
					})
				});

				//	##	Create the Upsert method.
				surfaceClass.Methods.Add(new MethodData
				{
					Name = "Upsert",
					Comment = new CommentData(new[] {
						"This method is used for creating a new or  updating an existing record in the database.",
						"If the primary key is default value (typically null or zero) we know it is a new record and try to add it. Otherwise we try to update the record.",
						"<para>St4mpede guid:{97D67E96-7C3E-4D8B-8984-104896646077}</para>"
						}),
					Scope = Common.VisibilityScope.Public,
					ReturnTypeString = $"TheDAL.Poco.{table.Name}", //	TODO:OF:Fetch namespace from Poco project.
					Parameters = new[] {
							new ParameterData
							{
								Name="context",
								SystemTypeString = "TheDAL.CommitScope"	//	TODO:OF:Fetch namespace from settings.
							},
							new ParameterData
							{
								Name = table.Name,	//	TODO:OF:Make camelcase.
								SystemTypeString = $"TheDAL.Poco.{table.Name}", //	TODO:OF:Fetch namespace from Poco project.
							} }
						.ToList(),
					Body = new BodyData(new[]
						{
						"if(" + string.Join(" &&", table.PrimaryKeyColumns.Select(c=> $"{table.Name.ToCamelCase()}.{c.Name} == default({parserLogic2.ConvertDatabaseTypeToDotnetTypeString(c.DatabaseTypeName)})") ) + "){",
						$"\treturn Add(context, {table.Name.ToCamelCase()});", 
						"}else{",
						$"\treturn Update(context, {table.Name.ToCamelCase()});", 
						"}"
					})
				});
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
			ret.Add($"//\tThis file was generated by St4mpede.Surface {DateTime.Now.ToString("u")}.");
			ret.Add(string.Empty);

//			ret.AddRange(_pocoSettings.NameSpaceComments.Select(c => string.Format("//\t{0}", c)));
			ret.Add($"namespace TheDal.Surface");
			ret.Add("{");
			ret.Add("	using System;");
			ret.Add("	using System.Collections.Generic;");
			ret.Add("	using System.Linq;");
			ret.Add("	using Dapper;");
			ret.Add(string.Empty);

			ret.AddRange(classData.ToCode(new Indent(1)));

			ret.Add("}");

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
