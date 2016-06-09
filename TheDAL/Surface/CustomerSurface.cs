//	This file was generated by St4mpede.Surface 2016-06-09 22:41:54Z.

namespace TheDal.Surface
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Dapper;

	/// <summary> This is the Surface for the Customer table.
	/// </summary>
	public class CustomerSurface
	{
	
		public CustomerSurface()
		{
		}
	
		/// <summary> This method is used for adding a new record in the database.
		/// <para>St4mpede guid:{BC89740A-CBA5-4EDE-BFF4-9B0D8BA4058F}</para>
		/// </summary>
		private TheDAL.Poco.Customer Add( TheDAL.CommitScope context, System.String name )
		{
			return Add( 
				context, 
				new TheDAL.Poco.Customer(
					0, 
					name
			));
		}
	
		/// <summary> This method is used for adding a new record in the database.
		/// <para>St4mpede guid:{759FBA46-A462-4E4A-BA2B-9B5AFDA572DE}</para>
		/// </summary>
		private TheDAL.Poco.Customer Add( TheDAL.CommitScope context, TheDAL.Poco.Customer customer )
		{
			const string Sql = @"
				Insert Into Customer
				(
					Name
				)
				Values
				(
					@Name
				)
				Select * From Customer Where CustomerId = Scope_Identity()";
			var ret = context.Connection.Query<TheDAL.Poco.Customer>(
				Sql, 
				new {
					customerId = customer.CustomerId
				},
				context.Transaction,
				false, null, null);
			return ret.Single();
		}
	
		/// <summary> This method is used for deleting a record.
		/// <para>St4mpede guid:{F74246AE-0295-4094-AA7F-1D118C11229D}</para>
		/// </summary>
		public void Delete( TheDAL.CommitScope context, TheDAL.Poco.Customer customer )
		{
			DeleteById( context, customer.CustomerId);
		}
	
		/// <summary> This method is used for deleting a record by its primary key.
		/// <para>St4mpede guid:{F0137E62-1D45-4B92-A48E-05954850FFE8}</para>
		/// </summary>
		public void DeleteById( TheDAL.CommitScope context, System.Int32 customerId )
		{
			const string Sql = @"
				Delete From Customer
				 Where CustomerId = @customerId
			";
			context.Connection.Execute(
				Sql, 
				new {
					customerId = customerId
				},
				context.Transaction,
				null, null);
		}
	
		/// <summary> This method returns every record for this table/surface.
		/// Do not use it on too big tables.
		/// <para>St4mpede guid:{4A910A99-9A50-4977-B2A2-404240CDDC73}</para>
		/// </summary>
		public IList<TheDAL.Poco.Customer> GetAll( TheDAL.CommitScope context )
		{
			const string Sql = @"
				Select * From Customer
			";
			var res = context.Connection.Query<TheDAL.Poco.Customer>(
				Sql, 
				null, 
				context.Transaction,
				false, null, null);
			return res.ToList();
		}
	
		/// <summary> This method is used for getting a record but its Id/primary key.
		/// If nothing is found an exception is thrown. If you want to get null back use LoadById <see cref="LoadById"/> instead.
		/// <para>St4mpede guid:{71CF185E-0DD1-4FAE-9721-920B5C3C12D9}</para>
		/// </summary>
		public TheDAL.Poco.Customer GetById( TheDAL.CommitScope context, System.Int32 customerId )
		{
			const string Sql = @"
				Select * From Customer
				 Where CustomerId = @customerId
			";
			var res = context.Connection.Query<TheDAL.Poco.Customer>(
				Sql, 
				new {
					customerId = customerId
				},
				context.Transaction,
				false, null, null);
			return res.Single();
		}
	
		/// <summary> This method is used for getting a record but its Id/primary key.
		/// If nothing is found an null is returned. If you want to throw an exception use GetById <see cref="GetById"> instead.
		/// <para>St4mpede guid:{BC171F29-81F2-41ED-AC5C-AD6884EC9718}</para>
		/// </summary>
		public TheDAL.Poco.Customer LoadById( TheDAL.CommitScope context, System.Int32 customerId )
		{
			const string Sql = @"
				Select * From Customer
				 Where CustomerId = @customerId
			";
			var res = context.Connection.Query<TheDAL.Poco.Customer>(
				Sql, 
				new {
					customerId = customerId
				},
				context.Transaction,
				false, null, null);
			return res.SingleOrDefault();
		}
	
		/// <summary> This method is used for updating an existing record in the database.
		/// <para>St4mpede guid:{5A4CE926-447C-4F3F-ADFC-8CA9229C60BF}</para>
		/// </summary>
		private TheDAL.Poco.Customer Update( TheDAL.CommitScope context, System.Int32 customerId, System.String name )
		{
			return Update( 
				context, 
				new TheDAL.Poco.Customer(
					customerId, name
			));
		}
	
		/// <summary> This method is used for updating an existing record in the database.
		/// <para>St4mpede guid:{B2B1B845-5F93-4A5C-9F90-FBA570228542}</para>
		/// </summary>
		private TheDAL.Poco.Customer Update( TheDAL.CommitScope context, TheDAL.Poco.Customer customer )
		{
			const string Sql = @"
				Update Customer
				Set
					Name = @name
				Where
					CustomerId = @CustomerId
				 Select * From Customer Where 
					CustomerId = $customerId
				";
			var ret = context.Connection.Query<TheDAL.Poco.Customer>(
				Sql, 
				new {
					customerId = customer.CustomerId, 	name = customer.Name
				},
				context.Transaction,
				false, null, null);
			return ret.Single();
		}
	
		/// <summary> This method is used for creating a new or  updating an existing record in the database.
		/// If the primary key is 0 (zero) we know it is a new record and try to add it. Otherwise we try to update the record.
		/// <para>St4mpede guid:{A821E709-7333-4ABA-9F38-E85617C906FE}</para>
		/// </summary>
		public TheDAL.Poco.Customer Upsert( TheDAL.CommitScope context, System.Int32 customerId, System.String name )
		{
			return Upsert(
				context,
				new TheDAL.Poco.Customer(
					customerId, name
			));
		}
	
		/// <summary> This method is used for creating a new or  updating an existing record in the database.
		/// If the primary key is default value (typically null or zero) we know it is a new record and try to add it. Otherwise we try to update the record.
		/// <para>St4mpede guid:{97D67E96-7C3E-4D8B-8984-104896646077}</para>
		/// </summary>
		public TheDAL.Poco.Customer Upsert( TheDAL.CommitScope context, TheDAL.Poco.Customer customer )
		{
			if(customer.CustomerId == default(System.Int32)){
				return Add(context, customer);
			}else{
				return Update(context, customer);
			}
		}
	}
}
