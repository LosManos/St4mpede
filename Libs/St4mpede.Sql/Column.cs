namespace St4mpede.Sql
{
	public class Column:IColumn
	{
		public string Name { get; set; }
		public string TypeString { get; set; }
		public bool IsInPrimaryKey { get; set; }
	}
}