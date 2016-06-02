namespace St4mpede.Sql
{
	using System.Collections.Generic;

	public class Insert<TMeta, TData> where TMeta : class, ITable
	{
		private readonly ITable _table;

		public Insert(ITable table)
		{
			_table = table;
		}

		public InsertColumns<TMeta, TData> Columns(IEnumerable<IColumn> columns)
		{
			var ret = new InsertColumns<TMeta, TData>(_table, columns);
			return ret;
		}
	}
}