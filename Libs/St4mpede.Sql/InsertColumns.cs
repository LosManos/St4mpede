using System.Collections.Generic;

namespace St4mpede.Sql
{
	public class InsertColumns<TMeta, TData>
	{
		private readonly IEnumerable<IColumn> _columns;
		private readonly ITable _table;

		internal InsertColumns(ITable table, IEnumerable<IColumn> columns)
		{
			_table = table;
			_columns = columns;
		}

		public InsertValues<TData> Values(IEnumerable<TData> items)
		{
			var ret = new InsertValues<TData>(_table, _columns, items);
			return ret;
		}
	}
}