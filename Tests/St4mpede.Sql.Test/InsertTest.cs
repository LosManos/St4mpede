namespace St4mpede.Sql.Test
{
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using System.Linq;

	[TestClass]
	public class InsertTest
	{
		[TestMethod]
		public void Insert_Items_ReturnAParameterisedQuery()
		{
			var customerTable = new CustomerTable();
			var customers = new Customer[]
			{
				new Customer {Id = 0, Name = "MyName", Address = "MyAddress"},
				new Customer {Id = 42, Name = "MyOtherName", Address = "MyOtherAddress"},
			};
			
			var insert = new Insert<CustomerTable, Customer>(customerTable)
				.Columns(customerTable.Columns)
				.Values(customers.Where(c=>c.Id!=0));
			var sql = insert.ToParameterisedSql();

			Assert.AreEqual(
				@"Insert Into Customer
(
Name,
Address
)
Values
(
@name,
@address
)", sql);
		}
	}
}
