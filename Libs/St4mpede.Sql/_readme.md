# Readme for St4mpede.sql

	var insert = new Insert<CustomerTable, Customer>(customerTable)
		.Columns(customerTable.Columns)
		.Values(customers.Where(c=>c.Id!=0));
	var sql = insert.ToParameterisedSql();

	Insert Into Customer
	(
		Name, 
		Address
	)
	Values
	(
		@name,
		@address
	)

	var select = new Select<CustomerTable>()
		.AllColumns()
		.AllRows();

	Select
	*
	From Customer

	var select = new Select<CustomerTable>()
		.Columns(customerTable.Columns)
		.Where($"{customerTable.Column.Address.Name}=="""MyAddress"""")
		.ToParameterisedSql();