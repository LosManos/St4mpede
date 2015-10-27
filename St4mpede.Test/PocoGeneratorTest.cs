using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using St4mpede.Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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

		[TestMethod]
		public void Generate_given_Tables_should_CreateOnlyIncludedAsClass()
		{
			//	#	Arrange.
			var sut = new PocoGenerator(null, new Log(), null);
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
								ColumnOneAName, "nvarchar"),
							new ColumnData(
								ColumnOneBName, "int")
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
			Assert.AreEqual("System.String", theClass.Properties[0].DotnetTypeName,
				"The Property is a string.");
			Assert.AreEqual(ColumnOneBName, theClass.Properties[1].Name, 
				"The name of the Property should be teh same as the Column.");
			Assert.AreEqual("System.Int32", theClass.Properties[1].DotnetTypeName,
				"The Property type is int.");
		}

		#region Init with path test.

		[TestMethod]
		public void Init_given_NoConfigPath_should_ThrowExeption()
		{
			//	#	Arrange.
			var sut = new PocoGenerator(null, null, null);

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
					<Poco>
						<OutputFolder>MyOutputFolder</OutputFolder>
						<ProjectPath>MyProjectPath</ProjectPath>
						<XmlOutputFilename>MyXmlOutputFilename</XmlOutputFilename>
					</Poco>
				</St4mpede>
"); };
			var mockLog = new Mock<ILog>();
			var sut = new PocoGenerator(null, mockLog.Object, null);

			//	#	Act.
			sut.Init("whatever", "whatevar", func);

			//	#	Assert.
			Assert.AreEqual("MyOutputFolder", sut.UT_PocoSettings.OutputFolder);
			Assert.AreEqual("MyProjectPath", sut.UT_PocoSettings.ProjectPath);
			Assert.AreEqual("MyXmlOutputFilename", sut.UT_PocoSettings.XmlOutputFilename);
			Assert.AreEqual("MyRootFolder", sut.UT_CoreSettings.RootFolder);
		}

		#endregion

		#region Init with XElement tests.

		[TestMethod]
		public void Init_given_ProperXml_should_PopulateFields()
		{
			//	#	Arrange.
			var sut = new PocoGenerator(null, null, null);
			var coreSettings = new CoreSettings();
			var doc = XDocument.Parse(
				@"	<Poco>
		<OutputFolder>MyFolder\WithBackslash</OutputFolder>
		<ProjectPath>MyProjectPath</ProjectPath>
		<XmlOutputFilename>MyXmlOutputFilename</XmlOutputFilename>
	</Poco>");

			//	#	Act.
			sut.UT_Init(coreSettings, doc.Elements().Single());

			//	#	Assert.
			Assert.AreEqual(@"MyFolder\WithBackslash", sut.UT_PocoSettings.OutputFolder);
			Assert.AreEqual("MyProjectPath", sut.UT_PocoSettings.ProjectPath);
			Assert.AreEqual("MyXmlOutputFilename", sut.UT_PocoSettings.XmlOutputFilename);
			Assert.AreEqual(@"MyProjectPath\MyXmlOutputFilename", sut.UT_PocoSettings.XmlOutputPathFilename);
		}

		#endregion

		[TestMethod]
		public void OutputTest()
		{
			//	#	Arrange.
			var mockedCore = new Mock<ICore>();
			mockedCore.Setup(m => m.WriteOutput(It.IsAny<IList<string>>(), It.IsAny<string> ()));
			var sut = new PocoGenerator(mockedCore.Object, new Log(), null);
			sut.UT_PocoSettings = new PocoSettings(
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
						new PropertyData(
							"CustomerID",
							typeof(int).ToString())
					}
				}
			};

			//	#	Act.
			sut.Output();

			//	#	Assert.
			mockedCore.Verify(m => m.WriteOutput(It.IsAny<IList<string>>(), It.IsAny<string>()), Times.Once());
		}

		[TestMethod]
		public void ReadXml_given_XDoxWith2Tables_should_ReturnPocosWith2Tables()
		{
			//	#	Arrange.
			var log = new Log();
			var xml = XDocument.Parse(@"
<Database>
  <Tables>
    <Table>
		<Name>Table one</Name>
    </Table>
    <Table>
		<Name>Table two</Name>
    </Table>
  </Tables>
</Database>
");
			var mockXDocHandler = new Mock<PocoGenerator.IXDocHandler>();
			mockXDocHandler
				.Setup(m => m.Load(It.IsAny<string>()))
				.Returns(xml);
			var sut = new PocoGenerator(null, log, mockXDocHandler.Object);

			//	#	Act.
			sut.ReadXml();

			//	#	Assert.
			Assert.AreEqual(2, sut.UT_DatabaseData.Tables.Count);
		}

	}
}
