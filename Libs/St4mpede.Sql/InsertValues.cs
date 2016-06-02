using System.Collections.Generic;
using System.Linq;

namespace St4mpede.Sql
{
	public class InsertValues<TData>
	{
		private readonly IEnumerable<IColumn> _columns;
		private readonly IEnumerable<TData> _rows;
		private readonly ITable _table;

		public InsertValues(ITable table, IEnumerable<IColumn> columns, IEnumerable<TData> rows)
		{
			_table = table;
			_columns = columns;
			_rows = rows;
		}

		public string ToParameterisedSql()
		{
			var sql = new List<string>();
			foreach (var row in _rows)
			{
				sql.Add($"Insert Into {_table.Name}");
				sql.Add("(");
				sql.Add(string.Join(",\r\n", _columns.Where(c => c.IsInPrimaryKey == false).Select(c => c.Name)));
				sql.Add(")");
				sql.Add("Values");
				sql.Add("(");
				sql.Add(string.Join(",\r\n", _columns.Where(c => c.IsInPrimaryKey == false).Select(c => "@" + c.Name.ToLower())));
				sql.Add(")");
			}
			return string.Join("\r\n", sql);
		}
	}
}