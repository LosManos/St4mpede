//	This file was generated by St4mpede.Poco 2016-06-05 19:32:48Z.

//	ReSharper disable BuiltInTypeReferenceStyle
//	ReSharper disable BuiltInTypeReferenceStyle
//	ReSharper disable NonReadonlyMemberInGetHashCode
//	ReSharper disable ArrangeThisQualifier
//	ReSharper disable PartialTypeWithSinglePart
namespace TheDAL.Poco
{
	public partial class Project
	{
		/// <summary> This property is part of the primary key.
		/// </summary>
		public System.Int32 ProjectId{ get; set; }
	
		public System.String Name{ get; set; }
	
		/// <summary> Default constructor needed for instance for de/serialising.
		/// </summary>
		public Project()
		{
		}
	
		/// <summary> This constructor takes all properties as parameters.
		/// </summary>
		public Project( System.Int32 projectId, System.String name )
		{
			this.ProjectId = projectId;
			this.Name = name;
		}
	
		/// <summary> This constructor takes all properties but primary keys as parameters.
		/// </summary>
		public Project( System.String name )
		{
			this.Name = name;
		}
	
		/// <summary> This is the copy constructor.
		/// </summary>
		public Project( Project project )
		{
			this.ProjectId = project.ProjectId;
			this.Name = project.Name;
		}
	
		/// <summary> This is the Equals method.
		/// </summary>
		public override System.Boolean Equals( System.Object o )
		{
			var obj = o as Project;
			if( obj == null ){
				return false;
			}
			
			return
				this.ProjectId == obj.ProjectId &&
				this.Name == obj.Name;
		}
	
		/// <summary> This is the GetHashCode method.
		/// </summary>
		public override System.Int32 GetHashCode()
		{
			int hash = 13;
			hash = (hash*7) + this.ProjectId.GetHashCode();
			hash = (hash*7) + ( null == Name ? 0 : this.Name.GetHashCode() );
			return hash;
		}
	}
}
