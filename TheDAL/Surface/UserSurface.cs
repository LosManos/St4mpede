//	This file was generated by St4mpede.Surface 2016-05-21 00:54:34Z.

namespace TheDal.Surface
{
	using System.Collections.Generic;
	using System.Linq;
	using Dapper;

	/// <summary> This is the Surface for the User table.
	/// </summary>
	public class UserSurface
	{
	
		public UserSurface()
		{
		}
	
		/// <summary> This method is used for adding a new record in the database.
		/// </summary>
		private TheDAL.Poco.User Add( TheDAL.CommitScope context, System.String userName, System.String hashedPassword, System.DateTime lastLoggedOnDatetime )
		{
			//TODO:OF:TBA.
			return null;
		}
	
		/// <summary> This method is used for adding a new record in the database.
		/// <para>St4mpede guid:{759FBA46-A462-4E4A-BA2B-9B5AFDA572DE}</para>
		/// </summary>
		private TheDAL.Poco.User Add( TheDAL.CommitScope context, TheDAL.Poco.User user )
		{
			const string Sql = @"
				Insert Into User
				(
					UserName, HashedPassword, LastLoggedOnDatetime
				)
				Values
				(
					@UserName, @HashedPassword, @LastLoggedOnDatetime
				)
				Select * From User Where UserId = Scope_Identity()";
			var ret = context.Connection.Query<TheDAL.Poco.User>(
				Sql, 
				new { /*...*/ },
				context.Transaction,
				false, null, null);
			return ret.Single();
		}
	
		/// <summary> This method is used for deleting a record.
		/// </summary>
		public void Delete( TheDAL.CommitScope context, TheDAL.Poco.User user )
		{
			//TODO:OF:TBA. =>get or throw exception.
		}
	
		/// <summary> This method is used for deleting a record by its primary key.
		/// </summary>
		public void DeleteById( TheDAL.CommitScope context, System.Int32 userId )
		{
			//TODO:OF:TBA. =>get or throw exception.
		}
	
		/// <summary> This method returns every record for this table/surface.
		/// Do not use it on too big tables.
		/// </summary>
		public IList<TheDAL.Poco.User> GetAll( TheDAL.CommitScope context )
		{
			//TODO:OF:TBA.
			return null;
		}
	
		/// <summary> This method is used for getting a record but its Id/primary key.
		/// If nothing is found an exception is thrown. If you want to get null back use LoadById <see cref="LoadById"> instead.
		/// </summary>
		public TheDAL.Poco.User GetById( TheDAL.CommitScope context, System.Int32 userId )
		{
			//TODO:OF:TBA. =>get or throw exception.
			return null;
		}
	
		/// <summary> This method is used for getting a record but its Id/primary key.
		/// If nothing is found an null is returned. If you want to throw an exception use GetById <see cref="GetById"> instead.
		/// </summary>
		public TheDAL.Poco.User LoadById( TheDAL.CommitScope context, System.Int32 userId )
		{
			//TODO:OF:TBA. =>get or return null.
			return null;
		}
	
		/// <summary> This method is used for updating an existing record in the database.
		/// </summary>
		private TheDAL.Poco.User Update( TheDAL.CommitScope context, System.Int32 userId, System.String userName, System.String hashedPassword, System.DateTime lastLoggedOnDatetime )
		{
			//TODO:OF:TBA.
			return null;
		}
	
		/// <summary> This method is used for updating an existing record in the database.
		/// </summary>
		private TheDAL.Poco.User Update( TheDAL.CommitScope context, TheDAL.Poco.User user )
		{
			//TODO:OF:TBA.
			return null;
		}
	
		/// <summary> This method is used for creating a new or  updating an existing record in the database.
		/// If the primary key is 0 (zero) we know it is a new record and try to add it. Otherwise we try to update the record.
		/// </summary>
		public TheDAL.Poco.User Upsert( TheDAL.CommitScope context, System.Int32 userId, System.String userName, System.String hashedPassword, System.DateTime lastLoggedOnDatetime )
		{
			//TODO:OF:TBA. =>if( return 0 == primarykey ? Add(...) : Update(...)
			return null;
		}
	
		/// <summary> This method is used for creating a new or  updating an existing record in the database.
		/// If the primary key is 0 (zero) we know it is a new record and try to add it. Otherwise we try to update the record.
		/// </summary>
		public TheDAL.Poco.User Upsert( TheDAL.CommitScope context, TheDAL.Poco.User user )
		{
			//TODO:OF:TBA. =>if( return 0 == primarykey ? Add(...) : Update(...)
			return null;
		}
	}
}
