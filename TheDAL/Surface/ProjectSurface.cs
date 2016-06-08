//	This file was generated by St4mpede.Surface 2016-06-08 22:27:06Z.

namespace TheDal.Surface
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Dapper;

	/// <summary> This is the Surface for the Project table.
	/// </summary>
	public class ProjectSurface
	{
	
		public ProjectSurface()
		{
		}
	
		/// <summary> This method is used for adding a new record in the database.
		/// <para>St4mpede guid:{BC89740A-CBA5-4EDE-BFF4-9B0D8BA4058F}</para>
		/// </summary>
		private TheDAL.Poco.Project Add( TheDAL.CommitScope context, System.String name )
		{
			return Add( 
				context, 
				new TheDAL.Poco.Project(
					0, 
					name
			));
		}
	
		/// <summary> This method is used for adding a new record in the database.
		/// <para>St4mpede guid:{759FBA46-A462-4E4A-BA2B-9B5AFDA572DE}</para>
		/// </summary>
		private TheDAL.Poco.Project Add( TheDAL.CommitScope context, TheDAL.Poco.Project project )
		{
			const string Sql = @"
				Insert Into Project
				(
					Name
				)
				Values
				(
					@Name
				)
				Select * From Project Where ProjectId = Scope_Identity()";
			var ret = context.Connection.Query<TheDAL.Poco.Project>(
				Sql, 
				new {
					projectId = project.ProjectId
				},
				context.Transaction,
				false, null, null);
			return ret.Single();
		}
	
		/// <summary> This method is used for deleting a record.
		/// <para>St4mpede guid:{F74246AE-0295-4094-AA7F-1D118C11229D}</para>
		/// </summary>
		public void Delete( TheDAL.CommitScope context, TheDAL.Poco.Project project )
		{
			const string Sql = @"
				Delete From Project
				 Where ProjectId = @projectId
			";
			var ret = context.Connection.Execute(
				Sql, 
				new {
					projectId = project.ProjectId
				},
				context.Transaction,
				null, null);
		}
	
		/// <summary> This method is used for deleting a record by its primary key.
		/// </summary>
		public void DeleteById( TheDAL.CommitScope context, System.Int32 projectId )
		{
			//TODO:OF:TBA.
			throw new NotImplementedException("TBA");
		}
	
		/// <summary> This method returns every record for this table/surface.
		/// Do not use it on too big tables.
		/// </summary>
		public IList<TheDAL.Poco.Project> GetAll( TheDAL.CommitScope context )
		{
			//TODO:OF:TBA.
			throw new NotImplementedException("TBA");
		}
	
		/// <summary> This method is used for getting a record but its Id/primary key.
		/// If nothing is found an exception is thrown. If you want to get null back use LoadById <see cref="LoadById"> instead.
		/// </summary>
		public TheDAL.Poco.Project GetById( TheDAL.CommitScope context, System.Int32 projectId )
		{
			//TODO:OF:TBA.
			throw new NotImplementedException("TBA");
		}
	
		/// <summary> This method is used for getting a record but its Id/primary key.
		/// If nothing is found an null is returned. If you want to throw an exception use GetById <see cref="GetById"> instead.
		/// </summary>
		public TheDAL.Poco.Project LoadById( TheDAL.CommitScope context, System.Int32 projectId )
		{
			//TODO:OF:TBA.
			throw new NotImplementedException("TBA");
		}
	
		/// <summary> This method is used for updating an existing record in the database.
		/// </summary>
		private TheDAL.Poco.Project Update( TheDAL.CommitScope context, System.Int32 projectId, System.String name )
		{
			//TODO:OF:TBA.
			throw new NotImplementedException("TBA");
		}
	
		/// <summary> This method is used for updating an existing record in the database.
		/// </summary>
		private TheDAL.Poco.Project Update( TheDAL.CommitScope context, TheDAL.Poco.Project project )
		{
			//TODO:OF:TBA.
			throw new NotImplementedException("TBA");
		}
	
		/// <summary> This method is used for creating a new or  updating an existing record in the database.
		/// If the primary key is 0 (zero) we know it is a new record and try to add it. Otherwise we try to update the record.
		/// </summary>
		public TheDAL.Poco.Project Upsert( TheDAL.CommitScope context, System.Int32 projectId, System.String name )
		{
			//TODO:OF:TBA.
			throw new NotImplementedException("TBA");
		}
	
		/// <summary> This method is used for creating a new or  updating an existing record in the database.
		/// If the primary key is 0 (zero) we know it is a new record and try to add it. Otherwise we try to update the record.
		/// </summary>
		public TheDAL.Poco.Project Upsert( TheDAL.CommitScope context, TheDAL.Poco.Project project )
		{
			//TODO:OF:TBA. =>if( return 0 == primarykey ? Add(...) : Update(...)
			throw new NotImplementedException("TBA");
		}
	}
}
