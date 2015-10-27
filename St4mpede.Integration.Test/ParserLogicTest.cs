using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace St4mpede.Integration.Test
{
	[TestClass]
	public class ParserLogicTest
	{
		private const string DatabasePath = @"ST4MPEDE.TEST\DATABASE\";
		private const string ConnectionStringTemplate = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True;Connect Timeout=30";

		public TestContext TestContext { get; set; }
	
		[TestMethod]
		[TestCategory(Common.Constants.TestCategoryIntegration)]
		public void ParseTablesTest()
		{
			//	#	Arrange.
			const string DatabaseName = "ParseTablesTest.mdf";
			var connectionString = string.Format(ConnectionStringTemplate,
				Path.Combine( Common.Functions.SolutionPath(TestContext), DatabasePath, DatabaseName));
			IList<Table> tables;

			using (var conn = new SqlConnection(connectionString))
			{
				var serverConnection = new ServerConnection(conn);
				var server = new Server(serverConnection);
				var database = server.Databases[
					Path.Combine(Common.Functions.SolutionPath(TestContext), DatabasePath, DatabaseName)];
				tables = ServerInfo.UT_GetTablesByDatabasePrivate(database);
            }
			var sut = new ParserLogic();

			//	#	Act.
			var res = sut.Parse(tables, "Project");

			//	#	Assert.
			Assert.AreEqual(2, res.Tables.Count);
			Assert.AreEqual("Customer", res.Tables[0].Name);
			Assert.IsTrue(res.Tables[0].Include);
			Assert.AreEqual("Project", res.Tables[1].Name);
			Assert.IsFalse(res.Tables[1].Include);
		}

	}
}
