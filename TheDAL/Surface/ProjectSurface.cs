//	This file was generated by St4mpede.Surface 2016-05-19 15:24:03Z.

/// <summary> This is the Surface for the Project table.
/// </summary>
public class ProjectSurface
{

	public ProjectSurface()
	{
	}

	/// <summary> This method is used for adding a new record in the database.
	/// </summary>
	private TheDAL.Poco.Project Add( System.String name )
	{
		//TODO:OF:TBA.
		return null;
	}

	/// <summary> This method is used for updating an existing record in the database.
	/// </summary>
	private TheDAL.Poco.Project Update( System.Int32 projectId, System.String name )
	{
		//TODO:OF:TBA.
		return null;
	}

	/// <summary> This method is used for creating a new or  updating an existing record in the database.
	/// If the primary key is 0 (zero) we know it is a new record and try to add it. Otherwise we try to update the record.
	/// </summary>
	public TheDAL.Poco.Project Upsert( System.Int32 projectId, System.String name )
	{
		//TODO:OF:TBA. =>if( return 0 == primarykey ? Add(...) : Update(...)
		return null;
	}
}
