//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
namespace St4mpede
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	using Microsoft.SqlServer.Management.Common;
	using Microsoft.SqlServer.Management.Smo;
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
#endif
	//#	Regular ol' C# classes and code...

	internal interface IDatabaseConnection
	{
		IServerInfo GetServerInfo(string connectionString);
    }

	internal class DatabaseConnection : IDatabaseConnection 
	{
		public IServerInfo GetServerInfo( string connectionString)
		{
			return new ServerInfo(connectionString);
		}

	}

	internal interface IServerInfo : IDisposable
	{
		string Name { get; }
		DatabaseCollection Databases { get; }
		/// <summary>I have a hard time mocking DatabaseCollection as it is sealed and abstract and whatnot. So I created a list instead. Maybe it should be a Dictionary with the database name as key so we can use it for fetching databases by name too.
		/// </summary>
		IList<Database> DatabaseList { get; }
		IList<Table> GetTablesByDatabase(Database database);
		IList<Column> GetColumnsByTable(Table table);
	}

	internal class ServerInfo : IServerInfo, IDisposable
	{
		private SqlConnection _connection;
		private Server _server;

		internal ServerInfo( string connectionString)
		{
			_connection = new SqlConnection(connectionString);
			_serverConnection = new ServerConnection(_connection);
			_server = new Server(_serverConnection);
		}

		public string Name
		{
			get
			{
				return _server.Name;
			}
		}

		public DatabaseCollection Databases
		{
			get
			{
				return _server.Databases;
			}
		}

		public IList<Database> DatabaseList
		{
			get
			{
				var ret = new List<Database>();
				for( var i = 0; i < _server.Databases.Count; ++i)
				{
					ret.Add(_server.Databases[i]);
				}
				return ret;
			}
		}

		public IList<Table>GetTablesByDatabase( Database database)
		{
			return GetTablesByDatabasePrivate(database);
		}

		public IList<Column> GetColumnsByTable(Table table)
		{
			var ret = new List<Column>();
			for( var i=0; i<table.Columns.Count; ++i)
			{
				ret.Add(table.Columns[i]);
			}
			return ret;
		}

		#region Private methods

		private static IList<Table> GetTablesByDatabasePrivate(Database database)
		{
			var ret = new List<Table>();
			for (var i = 0; i < database.Tables.Count; ++i)
			{
				ret.Add(database.Tables[i]);
			}
			return ret;
		}

		#endregion

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls
		private ServerConnection _serverConnection;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// Dispose managed state (managed objects) here.
					_serverConnection.Disconnect();
					_connection.Dispose();
				}

				// Free unmanaged resources (unmanaged objects) here
				//	and override a finalizer below.
				// Set large fields to null here.

				disposedValue = true;
			}
		}

		// Override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~ServerInfo() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// Uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion

		#region Methods for making testing possible without changing the architecture.

		internal static IList<Table> UT_GetTablesByDatabasePrivate(Database database)
		{
			return GetTablesByDatabasePrivate(database);
		}

		#endregion
	}

#if NOT_IN_T4
} //end the namespace
#endif
//#>