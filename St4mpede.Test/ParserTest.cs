using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Microsoft.SqlServer.Management.Smo;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using System.Collections.Generic;

namespace St4mpede.Test
{
	[TestClass]
	public class ParserTest
	{
		private Settings m_settings;
		private const string DatabasePath = @"C:\DATA\PROJEKT\ST4MPEDE\ST4MPEDE.TEST\DATABASE\";
		private const string ConnectionStringTemplate = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True;Connect Timeout=30";

		[TestInitialize]
		public void Initialize()
		{
			m_settings = new Settings();
		}

		[TestMethod]
		public void GetDatabasesInfoTest()
		{
			Assert.Inconclusive("TBA");
		}

		[TestMethod]
		public void AddLogTest()
		{
			Assert.Inconclusive("TBA");
		}

		[TestMethod]
		public void Init_given_Null_should_ThrowArgumentNullException()
		{
			//	#	Arrange.
			var sut = new Parser();

			//	#	Act and Assert.
			try
			{
				sut.Init(null);
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
			try {
				sut.Init("ThisFileDoesNotExist");
				Assert.Fail("Should have thrown an exception.");
			}catch( FileNotFoundException)
			{
				//	Ok.
			}
		}

		[TestMethod]
		public void Init_givenValidFile_should_CreateSettings()
		{
			//	#	Arrange.
			const string PathFilename = @"..\..\ValidConfigFile.xml";
            var sut = new Parser();

			//	#	Act.
			sut.Init(PathFilename);

			//	#	Assert.
			Assert.IsInstanceOfType(sut.UT_Settings, typeof(Settings));
			Assert.AreEqual("myConnectionString", sut.UT_Settings.ConnectionString);
			Assert.AreEqual("myDatabaseName", sut.UT_Settings.DatabaseName);
			Assert.AreEqual(0, sut.UT_Settings.DatabaseIndex);
			Assert.AreEqual("myExcludedTablesRegex", sut.UT_Settings.ExcludedTablesRegex);
			Assert.AreEqual(PathFilename, sut.UT_Settings.InitPathfilename);
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
			Assert.AreEqual(2, sut.UT_TablesData.Count);
			Assert.AreEqual("Customer", sut.UT_TablesData[0].Name);
			Assert.IsTrue(sut.UT_TablesData[0].Include);
			Assert.AreEqual("Project", sut.UT_TablesData[1].Name);
			Assert.IsFalse(sut.UT_TablesData[1].Include);
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
			var sut = new Parser();
			var tables = new List<TableData>();

			//	#	Act.
			//sut.UT_ToXml(tables);

			Assert.Fail("Not Implemented");

			//	#	Assert.
		}
	}
}
