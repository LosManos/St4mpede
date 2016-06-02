using System.Collections.Generic;

namespace St4mpede.Sql
{
	public interface ITable
	{
		string Name { get; }
		IEnumerable<IColumn> Columns { get; }
	}
}