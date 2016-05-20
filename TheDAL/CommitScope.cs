using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace TheDAL
{
	public class CommitScope : IDisposable
	{
		// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
		private readonly IDbConnection _connection;
		private readonly IDbTransaction _transaction;
		private bool _commited;
		private bool _rollbacked;
		private readonly string _callerMemberName;

		internal IDbConnection Connection
		{
			get { return _connection; }
		}

		internal IDbTransaction Transaction
		{
			get { return _transaction; }
		}

		public CommitScope([CallerMemberName] string callerMemberName = "")
		{
			_callerMemberName = callerMemberName;
			_connection = new SqlConnection(Common.ConnectionString);
			_connection.Open();
			_transaction = _connection.BeginTransaction();
		}

		static CommitScope()
		{
		}

		public void Commit()
		{
			_transaction.Commit();
			_commited = true;
		}

		public void Rollback()
		{
			_transaction.Rollback();
			_rollbacked = true;
		}

		public void Dispose()
		{

			//	We have already commited or rollbacked so just dispose and leave.
			if (_commited || _rollbacked)
			{
				//	NOP.
				DoDispose();
				return;
			}

			//	If  the caller has not chosen to explicitly commit we roll back.
			_transaction.Rollback();

			DoDispose();
		}

		private void DoDispose()
		{
			if (_transaction != null)
			{
				_transaction.Dispose();
			}

			if (_connection != null)
			{
				_connection.Close();
				_connection.Dispose();
			}			
		}

	}
}
