using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace St4mpede.Integration.Test
{
	[TestClass]
	public class ParserLogicTest
	{
		private const string DatabasePath = @"ST4MPEDE.INTEGRATION.TEST\DATABASE\";
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
			Assert.AreEqual(3, res.Tables.Count);

			var projectTable = res.Tables.Single(t => t.Name == "Project");
			Assert.IsFalse(projectTable.Include);

			var customerTable = res.Tables.Single(t => t.Name == "Customer");
			Assert.IsTrue(customerTable.Include);

			var userTable = res.Tables.Single(t => t.Name == "User");
			Assert.IsTrue(userTable.Include);
		}

	}
}
