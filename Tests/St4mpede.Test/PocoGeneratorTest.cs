using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using St4mpede.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using St4mpede.St4mpede.Poco;

namespace St4mpede.Test
{
	[TestClass]
	public class PocoGeneratorTest
	{
		[TestMethod]
		public void ConvertDatabaseTypeToDotnetType_given_UnknownDatabaseType_should_ReturnErrorString()
		{
			//	#	Arrange.
			var sut = new PocoGenerator();

			//	#	Act.
			var res = sut.UT_ConvertDatabaseTypeToDotnetType("unkown");
			
			//	#	Assert.
			Assert.IsTrue(res.Contains("ERROR"));
		}

		[TestMethod]
		public void ConvertDatabaseTypeToDotnetType_given_KnownDatabaseType_should_ConvertedType()
		{
			//	#	Arrange.
			var sut = new PocoGenerator();

			//	#	Act.
			var res = sut.UT_ConvertDatabaseTypeToDotnetType("nvarchar");

			//	#	Assert.
			Assert.AreEqual(typeof(string).ToString(), res);
		}

		[TestMethod]
		public void ConvertDatabaseTypeToDotnetType_given_NotUbiquitousDatabaseType_should_ConvertedType()
		{
			//	#	Arrange.
			//	Manipulate Types dictionary to be incorrect.
			var sut = new PocoGenerator();
			sut.UT_Types.Add(
				new PocoGenerator.TypesTuple("nvarchar", typeof(char).ToString()));

			//	#	Act.
			var res = sut.UT_ConvertDatabaseTypeToDotnetType("nvarchar");

			//	#	Assert.
			Assert.IsTrue(res.Contains("ERROR"));
		}

		#region CreateBodyForEqualsMethod tests.

		[TestMethod]
		public void CreateBodyForEqualsMethod_given_HappyPath_should_ReturnProper()
		{
			//	#	Arrange.
			var table = new TableData
			{
				Columns = new List<ColumnData>
				{
					new ColumnData
					{
						Name = "CustomerID"
					}, 
					new ColumnData
					{
						Name="Name"
					}
				}
			};
			var classData = new ClassData
			{
				Name = "Customer"
			};
			const string ParameterName = "o";

			//	#	Act.
			var res = PocoGenerator.UT_CreateBodyForEqualsMethod(table, classData, ParameterName);

			//	# Assert.
			CollectionAssert.AreEqual(new[]
			{
				"var obj = o as Customer;",
				"if( obj == null ){",
				"\treturn false;",
				"}",
				string.Empty,
				"return",
				"\tthis.CustomerID == obj.CustomerID &&",
				"\tthis.Name == obj.Name;"
			},
			res.ToList());
        }

		#endregion

		#region CreateBodyForGetHashCodeMethod tests.

		[TestMethod]
		public void CreateBodyForGetHashCodeMethod_given_HappyPath_should_ReturnProper()
		{
			//	#	Arrange.
			var table = new TableData
			{
				Columns = new List<ColumnData>
				{
					new ColumnData
					{
						Name = "CustomerID", 
						DatabaseTypeName="int"
					},
					new ColumnData
					{
						Name="Name", 
						DatabaseTypeName="varchar"
					}
				}
			};
			var sut = new PocoGenerator();

			//	#	Act.
			var res = sut.UT_CreateBodyForGetHashCodeMethod(table);

			//	#	Assert.
			CollectionAssert.AreEqual(new[]
			{
				"int hash = 13;",
				"hash = (hash*7) + this.CustomerID.GetHashCode();",
				"hash = (hash*7) + ( null == Name ? 0 : this.Name.GetHashCode() );",
				"return hash;"
			},
			res.ToList());
		}

		#endregion

		#region DefaultValueIsNull tests.

		class MyCustomClass { }

		struct MyCustomStruct { }


		[TestMethod]
		public void DefaultValueIsNull_given_Class_should_ReturnTrue()
		{
			//	#	Act.
			var res = PocoGenerator.UT_DefaultValueIsNull(typeof(Tuple<int>));

			//	#	Assert.
			Assert.IsTrue(res);
        }

		[TestMethod]
		public void DefaultValueIsNull_given_CustomClass_should_ReturnFalse()
		{
			//	#	Arrange.

			//	#	Act.
			var res = PocoGenerator.UT_DefaultValueIsNull(typeof(MyCustomClass));

			//	#	Assert.
			Assert.IsTrue(res);
		}

		[TestMethod]
		public void DefaultValueIsNull_given_CustomStruct_should_ReturnFalse()
		{
			//	#	Arrange.

			//	#	Act.
			var res = PocoGenerator.UT_DefaultValueIsNull(typeof(MyCustomStruct));

			//	#	Assert.
			Assert.IsFalse(res);
		}

		[TestMethod]
		public void DefaultValueIsNull_given_Int_should_ReturnFalse()
		{
			//	#	Act.
			var res = PocoGenerator.UT_DefaultValueIsNull(typeof(int));

			//	#	Assert.
			Assert.IsFalse(res);
		}

		[TestMethod]
		public void DefaultValueIsNull_given_Nullable_should_ReturnFalse()
		{
			//	#	Arrange.

			//	#	Act.
			var res = PocoGenerator.UT_DefaultValueIsNull(typeof(Nullable<int>));

			//	#	Assert.
			Assert.IsTrue(res);
		}


		[TestMethod]
		public void DefaultValueIsNull_given_Object_should_ReturnFalse()
		{
			//	#	Act.
			var res = PocoGenerator.UT_DefaultValueIsNull(typeof(object));

			//	#	Assert.
			Assert.IsTrue(res);
		}

		[TestMethod]
		public void DefaultValueIsNull_given_String_should_ReturnTrue()
		{
			//	#	Act.
			var res = PocoGenerator.UT_DefaultValueIsNull(typeof(string));

			//	#	Assert.
			Assert.IsTrue(res);
		}

		#endregion

		[TestMethod]
		public void Generate_given_Tables_should_CreateOnlyIncludedAsClass()
		{
			//	#	Arrange.
			var sut = new PocoGenerator(null, new Log());
			const string ColumnOneAName = "ColOne";
			const string ColumnOneBName = "ColTwo";
			const string TableNameOne = "One";
			const string TableNameTwo = "Two";
			var databaseData = new DatabaseData
			{
				Tables = new List<TableData>
				{
					new TableData {
						Name=TableNameOne,
						Include=true,
						Columns =new List<ColumnData> {
							new ColumnData(
								ColumnOneAName, "nvarchar", true),
							new ColumnData(
								ColumnOneBName, "int", true)
						}
					},
                    new TableData {
						Name = TableNameTwo,
						Include = false,
						Columns = new List<ColumnData>()
					}
                }
			};
			sut.UT_DatabaseData= databaseData;
			sut.UT_PocoSettings = new PocoSettings
			{
				MakePartial = true
			};

			//	#	Act.
			sut.Generate();

			//	#	Assert.
			Assert.AreEqual(1, sut.UT_ClassData.Count, 
				"Only 1 Table is included.");
			var theClass = sut.UT_ClassData.Single();
			Assert.AreEqual(TableNameOne, theClass.Name, 
				"The name of the Class should be the same as the Table.");
			Assert.AreEqual(2, theClass.Properties.Count, 
				"Both Columns should be used as 2 Properties.");
			Assert.AreEqual(ColumnOneAName, theClass.Properties[0].Name, 
				"The name of the Property is the same as that of the Column.");
			Assert.AreEqual(typeof(string), theClass.Properties[0].SystemType,
				"The Property is a string.");
			Assert.AreEqual(ColumnOneBName, theClass.Properties[1].Name, 
				"The name of the Property should be teh same as the Column.");
			Assert.AreEqual(typeof(System.Int32), theClass.Properties[1].SystemType,
				"The Property type is int.");
		}

		#region Init with path test.

		[TestMethod]
		public void Init_given_NoConfigPath_should_ThrowExeption()
		{
			//	#	Arrange.
			var sut = new PocoGenerator(null, null);

			//	#	Act.
			try
			{
				sut.Init(null, "whatever", null);
				Assert.Fail("Should not come here.");
			}catch(ArgumentNullException exc)
			{
				Assert.AreEqual("hostTemplateFile", exc.ParamName);
			}
		}

		[TestMethod]
		public void Init_given_ProperData_should_DoItsMagic()
		{
			//	#	Arrange.
			Func<string, string, XDocument> func = (string configPath, string configFilename) => { return XDocument.Parse(@"
				<St4mpede>
					<Core>
						<RootFolder>MyRootFolder</RootFolder>
					</Core>
					<RdbSchema>
						<DatabaseXmlFile>MyDatabaseXmlFile</DatabaseXmlFile>
					</RdbSchema>
					<Poco>
						<OutputFolder>MyOutputFolder</OutputFolder>
						<NameSpace Name='MyNameSpace'>
							<Comments>
								<Comment>Resharper comment.</Comment>
								<Comment>Other comment.</Comment>
							</Comments>
						</NameSpace>
						<ProjectPath>MyProjectPath</ProjectPath>
						<XmlOutputFilename>MyXmlOutputFilename</XmlOutputFilename>
						<MakePartial>True</MakePartial>
						<Constructors>
							<Default>True</Default>
							<AllProperties>True</AllProperties>
							<AllPropertiesSansPrimaryKey>True</AllPropertiesSansPrimaryKey>
							<CopyConstructor>True</CopyConstructor>
						</Constructors>
						<Methods>
							<Equals Regex='.*'>True</Equals>
						</Methods>
					</Poco>
				</St4mpede>
"); };
			var mockLog = new Mock<ILog>();
			var sut = new PocoGenerator(null, mockLog.Object);

			//	#	Act.
			sut.Init("whatever", "whatevar", func);

			//	#	Assert.
			Assert.AreEqual("MyOutputFolder", sut.UT_PocoSettings.OutputFolder);
			Assert.AreEqual("MyNameSpace", sut.UT_PocoSettings.NameSpace);
			Assert.AreEqual(2, sut.UT_PocoSettings.NameSpaceComments.Count);
			Assert.AreEqual("Resharper comment.", sut.UT_PocoSettings.NameSpaceComments[0]);
			Assert.AreEqual("Other comment.", sut.UT_PocoSettings.NameSpaceComments[1]);
			Assert.AreEqual("MyProjectPath", sut.UT_PocoSettings.ProjectPath);
			Assert.AreEqual("MyXmlOutputFilename", sut.UT_PocoSettings.XmlOutputFilename);
			Assert.IsTrue( sut.UT_PocoSettings.MakePartial);
			Assert.IsTrue( sut.UT_PocoSettings.CreateDefaultConstructor);
			Assert.IsTrue( sut.UT_PocoSettings.CreateAllPropertiesConstructor);
			Assert.IsTrue(sut.UT_PocoSettings.CreateAllPropertiesSansPrimaryKeyConstructor);
			Assert.IsTrue(sut.UT_PocoSettings.CreateCopyConstructor);
			Assert.IsTrue(sut.UT_PocoSettings.CreateMethodEquals);
			Assert.AreEqual(".*", sut.UT_PocoSettings.CreateMethodEqualsRegex);

			Assert.AreEqual("MyRootFolder", sut.UT_CoreSettings.RootFolder);
		}

		#endregion

		#region Init with XElement tests.

		[TestMethod]
		public void Init_given_ProperXml_should_PopulateFields()
		{
			//	#	Arrange.
			var sut = new PocoGenerator(null, null);
			var coreSettings = new CoreSettings();
			var rdbSchemaSettings = new ParserSettings();
			var doc = XDocument.Parse(
				@"	<Poco>
		<OutputFolder>MyFolder\WithBackslash</OutputFolder>
		<NameSpace Name='MyNameSpace'>
			<Comments>
				<Comment>Resharper comment.</Comment>
				<Comment>Other comment.</Comment>
			</Comments>
		</NameSpace>
		<ProjectPath>MyProjectPath</ProjectPath>
		<XmlOutputFilename>MyXmlOutputFilename</XmlOutputFilename>
		<MakePartial>True</MakePartial>
		<Constructors>
			<Default>True</Default>
			<AllProperties>True</AllProperties>
			<AllPropertiesSansPrimaryKey>True</AllPropertiesSansPrimaryKey>
			<CopyConstructor>True</CopyConstructor>
		</Constructors>
		<Methods>
			<Equals Regex='.*'>True</Equals>
		</Methods>
	</Poco>");

			//	#	Act.
			sut.UT_Init(coreSettings, rdbSchemaSettings, doc.Elements().Single());

			//	#	Assert.
			Assert.AreEqual(@"MyFolder\WithBackslash", sut.UT_PocoSettings.OutputFolder);
			Assert.AreEqual("MyNameSpace", sut.UT_PocoSettings.NameSpace);
			Assert.AreEqual(2, sut.UT_PocoSettings.NameSpaceComments.Count);
			Assert.AreEqual("Resharper comment.", sut.UT_PocoSettings.NameSpaceComments[0]);
			Assert.AreEqual("Other comment.", sut.UT_PocoSettings.NameSpaceComments[1]);
			Assert.AreEqual("MyProjectPath", sut.UT_PocoSettings.ProjectPath);
			Assert.AreEqual("MyXmlOutputFilename", sut.UT_PocoSettings.XmlOutputFilename);
			Assert.AreEqual(@"MyProjectPath\MyXmlOutputFilename", sut.UT_PocoSettings.XmlOutputPathFilename);
			Assert.IsTrue(sut.UT_PocoSettings.MakePartial);
			Assert.IsTrue(sut.UT_PocoSettings.CreateDefaultConstructor);
			Assert.IsTrue(sut.UT_PocoSettings.CreateAllPropertiesConstructor);
			Assert.IsTrue(sut.UT_PocoSettings.CreateAllPropertiesSansPrimaryKeyConstructor);
			Assert.IsTrue(sut.UT_PocoSettings.CreateCopyConstructor);
			Assert.IsTrue(sut.UT_PocoSettings.CreateMethodEquals);
			Assert.AreEqual(".*", sut.UT_PocoSettings.CreateMethodEqualsRegex);
		}

		#endregion

		[TestMethod]
		public void OutputTest()
		{
			//	#	Arrange.
			var mockedCore = new Mock<ICore>();
			mockedCore.Setup(m => m.WriteOutput(It.IsAny<IList<string>>(), It.IsAny<string> ()));
			var sut = new PocoGenerator(mockedCore.Object, new Log());
			sut.UT_PocoSettings = new PocoSettings(
				true,
				"MyNameSpace",
				new List<string> {"Resharper comment.","Other comment."},
				true,
				true,
				true,
				true,
				true, 
				".*",
				@"path\path", 
				"Poco", 
				"PocoGenerator.xml");
			sut.UT_CoreSettings = new CoreSettings()
			{
				RootFolder = "MyRootFolder"
			};
			sut.UT_ClassData = new List<ClassData>
			{
				new ClassData {
					Name= "Customer",
					Properties = new List<PropertyData>
					{
						new PropertyData {
							Name =                          "CustomerID",
							SystemType= typeof(int) 
							 //isinprmarykey = true)
						}
					}
				}
			};

			//	#	Act.
			sut.Output();

			//	#	Assert.
			mockedCore.Verify(m => m.WriteOutput(It.IsAny<IList<string>>(), It.IsAny<string>()), Times.Once());
		}

		[TestMethod]
		public void TypesPropertyMustNotBeStatic()
		{
			//	Here we use UT_Types for a workaround that Type should not be static.
			//	To be honest it doesn't work - UT_Types can be non-static while Types can.
			//	But the code is kept as a memory and a, albeit dysfunctional, gate to keep someone from tyrning Types to static.
			//	An easy solution would be to make Types internal and remove UT_Types.
			//	Another solution could be using refletion and search for private "Types".
			//	A third solution would be to create 2 instances of SUT and add an item to Types and see if one instance affected the other; in that case it would be the static we are trying to avoid.
			//	Let's save this for a rainy day.

			//	#	Arrange.
			var sut = new PocoGenerator();
			var typeType = sut.UT_Types.GetType();
			var typeName = GetMemberInfo((PocoGenerator pg) => pg.UT_Types).Name;

			//	#	Act.
			var res = typeType.GetProperties(System.Reflection.BindingFlags.Static).FirstOrDefault(p => p.Name == typeName);

			//	#	Assert.
			Assert.IsNull(res, "We are retrieving all static properties and Types should not be one of them.");	
		}

		private static MemberInfo GetMemberInfo<T, U>(Expression<Func<T, U>> expression)
		{
			var member = expression.Body as MemberExpression;
			if (member != null)
				return member.Member;

			throw new ArgumentException("Expression is not a member access", "expression");
		}

		//private static string GetName(Expression<Func<object>> exp)
		//{
		//	MemberExpression body = exp.Body as MemberExpression;

		//	if (body == null)
		//	{
		//		UnaryExpression ubody = (UnaryExpression)exp.Body;
		//		body = ubody.Operand as MemberExpression;
		//	}

		//	return body.Member.Name;
		//}

	}
}
