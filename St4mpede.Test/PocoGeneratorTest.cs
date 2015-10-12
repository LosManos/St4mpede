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
			var sut = new PocoGenerator(new Log(), null, null);
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
								ColumnOneBName, "numeric")
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
				Assert.AreEqual("configPath", exc.ParamName);
			}
		}

		[TestMethod]
		public void Init_given_NoConfigFileName_should_ThrowException()
		{
			//	#	Arrange.
			var sut = new PocoGenerator(null, null, null);

			//	#	Act.
			try
			{
				sut.Init("whatever", null, null);
				Assert.Fail("Should not come here.");
			}
			catch (ArgumentNullException exc)
			{
				Assert.AreEqual("configFilename", exc.ParamName);
			}
		}

		[TestMethod]
		public void Init_given_NoReadConfigFunction_should_ThrowException()
		{
			//	#	Arrange.
			var sut = new PocoGenerator(null, null, null);

			//	#	Act.
			try
			{
				sut.Init("whatever", "whatevar", null);
				Assert.Fail("Should not come here.");
			}
			catch (ArgumentNullException exc)
			{
				Assert.AreEqual("readConfigFunction", exc.ParamName);
			}
		}

		[TestMethod]
		public void Init_given_ProperData_should_DoItsMagic()
		{
			//	#	Arrange.
			Func<string, string, XDocument> func = (string configPath, string configFilename) => { return XDocument.Parse(@"
				<St4mpede>
					<RootFolder>MyRootFolder</RootFolder>
					<Poco>
						<OutputFolder>MyOutputFolder</OutputFolder>
					</Poco>
				</St4mpede>
"); };
			var mockLog = new Mock<ILog>();
			var sut = new PocoGenerator(mockLog.Object, null, null);

			//	#	Act.
			sut.Init("whatever", "whatevar", func);

			//	#	Assert.
			Assert.AreEqual("MyOutputFolder", sut.UT_OutputFolder);
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
	</Poco>");

			//	#	Act.
			sut.UT_Init(coreSettings, doc.Elements().Single());

			//	#	Assert.
			Assert.AreEqual(@"MyFolder\WithBackslash", sut.UT_OutputFolder);
		}

		#endregion

		[TestMethod]
		public void OutputTest()
		{
			//	#	Arrange.
			int callCount = 0;
			XDocument resultDoc = null;
			Action<XDocument, string> writeOutputFunction =
				(XDocument doc, string pathFilename) =>
				{
					++callCount;
					resultDoc = doc;
				};
            var sut = new PocoGenerator(new Log(), null, writeOutputFunction);
			sut.UT_OutputFolder = @"path\path";
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
			Assert.AreEqual(1, callCount, "We should have called the function once.");
			Assert.IsNotNull(resultDoc, "We have at least got a xdoc. Feel free to check further for its contents.");
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
			var sut = new PocoGenerator(log, mockXDocHandler.Object, null);

			//	#	Act.
			sut.ReadXml();

			//	#	Assert.
			Assert.AreEqual(2, sut.UT_DatabaseData.Tables.Count);
		}

	}
}
