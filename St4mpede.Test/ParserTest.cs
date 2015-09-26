using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Microsoft.SqlServer.Management.Smo;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using St4mpede.Test.Extensions;

namespace St4mpede.Test
{
	[TestClass]
	public class ParserTest
	{
		//private Settings m_settings;
		private const string DatabasePath = @"C:\DATA\PROJEKT\ST4MPEDE\ST4MPEDE\ST4MPEDE.TEST\DATABASE\";
		private const string ConnectionStringTemplate = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True;Connect Timeout=30";

		[TestInitialize]
		public void Initialize()
		{
			//m_settings = new Settings();
		}

		[TestMethod]
		public void AddLog_given_FormatAndArgs_should_AddItem()
		{
			//	#	Arrange.
			var sut = new Parser();
			Assert.IsFalse(sut.UT_Log.Any(), "There should be no log when we start.");

			//	#	Act.
			sut.UT_AddLog("a{0}c{1}", "b", "d");

			//	#	Assert.
			Assert.AreEqual("abcd", sut.UT_Log.Single());
		}

		[TestMethod]
		public void AddLog_given_LogRows_should_AddThem()
		{
			//	#	Arrange.
			var sut = new Parser();
			Assert.IsFalse(sut.UT_Log.Any(), "There should be no log when we start.");
			const string RowOne = "My first row";
			const string RowTwo = "My second row";

			//	#	Act.
			sut.UT_AddLog(new[] { RowOne, RowTwo });

			//	#	Assert.
			Assert.AreEqual(2, sut.UT_Log.Count());
			Assert.AreEqual(RowOne, sut.UT_Log[0]);	
			Assert.AreEqual(RowTwo, sut.UT_Log[1]);	
		}

		[TestMethod]
		public void AddLog_given_XDocument_should_AddIt()
		{
			//	#	Arrange.
			var sut = new Parser();
			Assert.IsFalse(sut.UT_Log.Any(), "There should be no log when we start.");
			var xml = new XDocument();

			//	#	Act.
			sut.UT_AddLog(xml);

			//	#	Assert.
			Assert.AreEqual(1, sut.UT_Log.Count(), 
			"Right now we don't test the formatting, only the existence of the row.");
		}

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
			Assert.IsInstanceOfType(sut.UT_Settings, typeof(Settings));
			Assert.AreEqual("myConnectionString", sut.UT_Settings.ConnectionString);
			Assert.AreEqual("myDatabaseName", sut.UT_Settings.DatabaseName);
			Assert.AreEqual(0, sut.UT_Settings.DatabaseIndex);
			Assert.AreEqual("myExcludedTablesRegex", sut.UT_Settings.ExcludedTablesRegex);
			Assert.AreEqual(MyPath, sut.UT_Settings.ConfigPath);
			Assert.AreEqual(Path.Combine( MyPath,MyFilename), sut.UT_Settings.InitPathfilename);
		}

		[TestMethod]
		public void ParseTablesTest()
		{
			//	#	Arrange.
			const string DatabaseName = "ParseTablesTest.mdf";
			var connectionString = string.Format(ConnectionStringTemplate,
				Path.Combine(DatabasePath, DatabaseName));
			TableCollection tables;

			using (var conn = new SqlConnection(connectionString))
			{
				var serverConnection = new ServerConnection(conn);
				var server = new Server(serverConnection);
				var db = server.Databases[
					Path.Combine(DatabasePath, DatabaseName)];
				tables = db.Tables;
			}
			var sut = new Parser();

			//	#	Act.
			sut.UT_ParseTables(tables, "Project");

			//	#	Assert.
			Assert.AreEqual(2, sut.UT_ServerData.Tables.Count);
			Assert.AreEqual("Customer", sut.UT_ServerData.Tables[0].Name);
			Assert.IsTrue(sut.UT_ServerData.Tables[0].Include);
			Assert.AreEqual("Project", sut.UT_ServerData.Tables[1].Name);
			Assert.IsFalse(sut.UT_ServerData.Tables[1].Include);
		}

		[TestMethod]
		public void ToString_should_ReturnLog()
		{
			//	#	Arrange.
			const string LogMessage = "my Message";
			var sut = new Parser();
			sut.UT_Log.Add(LogMessage + "1");
			sut.UT_Log.Add(LogMessage + "2");

			//	#	Act.
			var res = sut.ToString();

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
				new ColumnData("MyColumnName", "MyColType")
			};

			//	#	Act.
			var res = Parser.UT_ToXml(database);

			//	#	Assert.
			var resServer = Core.Deserialise<DatabaseData>(res);

			Assert.AreEqual(1, resServer.Tables.Count);

			Assert.AreEqual(
				@"<Database xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <Tables>
    <Table>
      <Name>MyTableName</Name>
      <Include>true</Include>
      <Columns>
        <Column>
          <Name>MyColumnName</Name>
          <DatabaseTypeName>MyColType</DatabaseTypeName>
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
