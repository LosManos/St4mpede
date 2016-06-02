namespace St4mpede.Sql
{
	public interface IColumn
	{
		string Name { get; }
		string TypeString { get; }
		bool IsInPrimaryKey { get; }
	}
}