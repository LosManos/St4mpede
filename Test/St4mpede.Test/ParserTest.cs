using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Microsoft.SqlServer.Management.Smo;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Linq;
using St4mpede.Test.Extensions;
using Moq;

namespace St4mpede.Test
{
	[TestClass]
	public class ParserTest
	{
		private const string DatabasePath = @"C:\DATA\PROJEKT\ST4MPEDE\ST4MPEDE\ST4MPEDE.TEST\DATABASE\";
		private const string ConnectionStringTemplate = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True;Connect Timeout=30";

		[TestMethod]
		public void Generate_given_HappyPath_should_Succeed()
		{
			//	#	Arrange.
			const string ExcludedTablesRegex = "MyExcludedTablesRegex";
            var mockedLog = new Mock<ILog>();
			var mockedDatabaseConnection = new Mock<IDatabaseConnection>();
			var mockedServerInfo = new Mock<IServerInfo>();
			var mockedParserLogic = new Mock<IParserLogic>();
			var databaseList = new List<Database>
			{
				new Database()
			};
			var tableList = new List<Table> {
				new Table { Name = "MyTableName"}
			};
			//var columnList = new List<Column>
			//{
			//	new Column {Name="MyColumnName"}
			//};
			mockedServerInfo
				.SetupGet(m => m.DatabaseList)
				.Returns(databaseList);
			mockedServerInfo
				.Setup(m => m.GetTablesByDatabase(It.IsAny<Database>()))
				.Returns(tableList);
			//mockedServerInfo
			//	.Setup( m=>m.GetColumnsByTable(It.IsAny<Table>()))
			//	.Returns(columnList);
			var mockedSettings = new Mock<IParserSettings>();
			mockedSettings
				.SetupGet(m => m.ConnectionString)
				.Returns("whatever");
			mockedSettings
				.Setup(m => m.DatabaseName)
				.Returns((string)null);
			mockedSettings
				.SetupGet(m => m.DatabaseIndex)
				.Returns(0);
			mockedSettings
				.Setup(m => m.ExcludedTablesRegex)
				.Returns(ExcludedTablesRegex);
			var sut = new Parser(mockedLog.Object);
			mockedDatabaseConnection
				.Setup(
				m => m.GetServerInfo(It.IsAny<string>()))
				.Returns(mockedServerInfo.Object);
			mockedParserLogic
				.Setup(m => m.Parse(It.IsAny<IList<Table>>(), ExcludedTablesRegex))
				.Returns(new DatabaseData());

			sut.UT_SetDatabaseConnection(mockedDatabaseConnection.Object);
			sut.UT_Settings = mockedSettings.Object;
			sut.UT_SetParserLogic(mockedParserLogic.Object);

			//	#	Act.
			sut.Generate();

			//	#	Assert.
			//	Success.
			//	What are we really testing here? It is like only happy path and nothing more. Rewriteit to test the different logic, which database to get and so forth and in the end call ParserLogic.Parse.
		}

		[TestInitialize]
		public void Initialize()
		{
			//m_settings = new Settings();
		}

		#region Init(string,string) tests.

		[TestMethod]
		public void Init_given_Null_should_ThrowArgumentNullException()
		{
			//	#	Arrange.
			var sut = new Parser();

			//	#	Act and Assert.
			try
			{
				sut.Init(null, null);
				Assert.Fail("Should have thrown an exception.");
			}
			catch (ArgumentNullException)
			{
				//	Ok.
			}
		}

		[TestMethod]
		public void Init_given_InvalidFile_should_ThrowException()
		{
			//	#	Arrange.
			var sut = new Parser();

			//	#	Act and Assert.
			try
			{
				sut.Init("ThisPathDoesNotExist", "ThisFileDoesNotExist");
				Assert.Fail("Should have thrown an exception.");
			}
			catch (FileNotFoundException)
			{
				//	Ok.
			}
			catch (DirectoryNotFoundException)
			{
				//	Ok.
			}
		}

		[TestMethod]
		public void Init_givenValidFile_should_CreateSettings()
		{
			//	#	Arrange.
			const string MyPath = @"..\..\";
			const string MyFilename = @"ValidConfigFile.xml";
            var sut = new Parser();

			//	#	Act.
			sut.Init(MyPath, MyFilename);

			//	#	Assert.
			Assert.IsInstanceOfType(sut.UT_Settings, typeof(ParserSettings));
			Assert.AreEqual("myConnectionString", sut.UT_Settings.ConnectionString);
			Assert.AreEqual("myDatabaseName", sut.UT_Settings.DatabaseName);
			Assert.AreEqual(0, sut.UT_Settings.DatabaseIndex);
			Assert.AreEqual("myExcludedTablesRegex", sut.UT_Settings.ExcludedTablesRegex);
			Assert.AreEqual(@"..\..", sut.UT_Settings.ConfigPath);
			Assert.AreEqual(Path.Combine( MyPath,MyFilename), sut.UT_Settings.InitPathfilename);
		}

		#endregion

		#region Init(string,string.xdoc) tests.

		[TestMethod]
		public void Init_given_ValidData_should_ReturnSettings()
		{
			//	#	Arrange.
			const string ConfigPath = "MyConfigPath";
			const string ConfigFilename = "MyConfigFilename";
			const string Xml =
				@"
<St4mpede>
	<Core>
		<RootFolder>MyRootFolder</RootFolder>
	</Core>
	<RdbSchema>
		<ConnectionString>MyConnectionString</ConnectionString>
		<DatabaseName>MyDatabaseName</DatabaseName>
		<ExcludedTablesRegex>MyExcludedTablesRegex</ExcludedTablesRegex>
		<DatabaseXmlFile>MyDatabaseXmlFile</DatabaseXmlFile>
	</RdbSchema>
	<Poco>
		<OutputFolder>MyOutputFolder</OutputFolder>
	</Poco>
</St4mpede>																		
";
			var doc = XDocument.Parse(Xml);

			//	#	Act.
			var res = ParserSettings.Init(ConfigPath, ConfigFilename, doc);

			//	#	Assert.
			Assert.AreEqual(ConfigPath, res.ConfigPath);
			Assert.AreEqual(string.Format(@"{0}\{1}", ConfigPath, ConfigFilename), res.InitPathfilename);
			Assert.AreEqual("MyConnectionString", res.ConnectionString);
			Assert.AreEqual("MyDatabaseName", res.DatabaseName);
			Assert.AreEqual(0, res.DatabaseIndex);
			Assert.AreEqual("MyExcludedTablesRegex", res.ExcludedTablesRegex);
			Assert.AreEqual("MyDatabaseXmlFile", res.DatabaseXmlFile);
		}

		#endregion

		[TestMethod]
		public void ToInfo_should_ReturnLog()
		{
			//	#	Arrange.
			const string LogMessage = "my Message";
			ILog log = new Log();
			var sut = new Parser(log);
			log.Add(LogMessage + "1");
			log.Add(LogMessage + "2");

			//	#	Act.
			var res = sut.ToInfo();

			//	#	Assert.
			Assert.AreEqual( LogMessage + "1" + "\r\n" + LogMessage + "2", res);
		}

		[TestMethod]
		public void ToXml()
		{
			//	#	Arrange.
			var database = new DatabaseData
			{
				Tables = new List<TableData>()
			};
			var table = database.Tables.AddItem(new TableData("MyTableName", true));
			table.Columns = new List<ColumnData>
			{
				new ColumnData("MyIdColumnName", "MyColType", true), 
				new ColumnData("MyOtherColumnName", "OtherColType", false)
			};

			//	#	Act.
			var res = Parser.UT_ToXml(database);

			//	#	Assert.
			var resServer =global::St4mpede.Core.Deserialise<DatabaseData>(res);

			Assert.AreEqual(1, resServer.Tables.Count);

			Assert.AreEqual(
				@"<Database xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <Tables>
    <Table>
      <Name>MyTableName</Name>
      <Include>true</Include>
      <Columns>
        <Column IsInPrimaryKey=""true"">
          <Name>MyIdColumnName</Name>
          <DatabaseTypeName>MyColType</DatabaseTypeName>
        </Column>
        <Column IsInPrimaryKey=""false"">
          <Name>MyOtherColumnName</Name>
          <DatabaseTypeName>OtherColType</DatabaseTypeName>
        </Column>
      </Columns>
    </Table>
  </Tables>
</Database>", 
				res.ToString());
	

	//			Assert.AreEqual(


	//				@"<ArrayOfTableData xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.datacontract.org/2004/07/St4mpede\">
 // < TableData >
	//    < Include > true </ Include >
	//    < Name > MyName </ Name >
	//  </ TableData >
	//</ ArrayOfTableData > ",
	//                res.ToString());

	//		Assert.Fail("Not Implemented");

	//		//	#	Assert.
		}

		//	http://stackoverflow.com/questions/1295046/use-xdocument-as-the-source-for-xmlserializer-deserialize
		public static class SerializationUtil
		{
			//public static T Deserialize<T>(XDocument doc)
			//{
			//	XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

			//	using (var reader = doc.Root.CreateReader())
			//	{
			//		return (T)xmlSerializer.Deserialize(reader);
			//	}
			//}

			public static XDocument Serialize<T>(T value)
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

				XDocument doc = new XDocument();
				using (var writer = doc.CreateWriter())
				{
					xmlSerializer.Serialize(writer, value);
				}

				return doc;
			}
		}
	}
}
