using System.Collections.Generic;
using System.Security.Policy;

namespace St4mpede.Sql.Test
{
	public class CustomerTable : ITable
	{
		public string Name => "Customer";

		public static class  Column
		{
			public static IColumn Id =
				new Sql.Column {Name = "Id", TypeString = typeof(int).ToString(), IsInPrimaryKey = true};
			public static IColumn Name =
				new Sql.Column {Name = "Name", TypeString = typeof(string).ToString(), IsInPrimaryKey = false};
			public static IColumn Address =
				new Sql.Column {Name = "Address", TypeString = typeof(string).ToString(), IsInPrimaryKey = false};
		};

	public IEnumerable<IColumn> Columns { get; } = new[]
		{
			Column.Id,
			Column.Address,
			Column.Name
		};
	}
}