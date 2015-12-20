//	This file was generated by St4mpede,Poco 2015-12-19 22:23:33Z.

//	ReSharper disable BuiltInTypeReferenceStyle
//	ReSharper disable BuiltInTypeReferenceStyle
//	ReSharper disable NonReadonlyMemberInGetHashCode
//	ReSharper disable ArrangeThisQualifier
//	ReSharper disable PartialTypeWithSinglePart
namespace TheDAL.Poco
{
	public partial class User
	{
		/// <summary> This property is part of the primary key.
		/// </summary>
		public System.Int32 UserId{ get; set; }
	
		public System.String UserName{ get; set; }
	
		public System.Char HashedPassword{ get; set; }
	
		public System.DateTime LastLoggedOnDatetime{ get; set; }
	
		/// <summary> Default constructor needed for instance for de/serialising.
		/// </summary>
		public User()
		{
		}
	
		/// <summary> This constructor takes all properties as parameters.
		/// </summary>
		public User( System.Int32 userId, System.String userName, System.Char hashedPassword, System.DateTime lastLoggedOnDatetime )
		{
			this.UserId = userId;
			this.UserName = userName;
			this.HashedPassword = hashedPassword;
			this.LastLoggedOnDatetime = lastLoggedOnDatetime;
		}
	
		/// <summary> This constructor takes all properties but primary keys as parameters.
		/// </summary>
		public User( System.String userName, System.Char hashedPassword, System.DateTime lastLoggedOnDatetime )
		{
			this.UserName = userName;
			this.HashedPassword = hashedPassword;
			this.LastLoggedOnDatetime = lastLoggedOnDatetime;
		}
	
		/// <summary> This is the copy constructor.
		/// </summary>
		public User( User user )
		{
			this.UserId = user.UserId;
			this.UserName = user.UserName;
			this.HashedPassword = user.HashedPassword;
			this.LastLoggedOnDatetime = user.LastLoggedOnDatetime;
		}
	
		/// <summary> This is the Equals method.
		/// </summary>
		public override System.Boolean Equals( System.Object o )
		{
			var obj = o as User;
			if( obj == null ){
				return false;
			}
			
			return
				this.UserId == obj.UserId &&
				this.UserName == obj.UserName &&
				this.HashedPassword == obj.HashedPassword &&
				this.LastLoggedOnDatetime == obj.LastLoggedOnDatetime;
		}
	
		/// <summary> This is the GetHashCode method.
		/// </summary>
		public override System.Int32 GetHashCode()
		{
			int hash = 13;
			hash = (hash*7) + this.UserId.GetHashCode();
			hash = (hash*7) + ( null == UserName ? 0 : this.UserName.GetHashCode() );
			hash = (hash*7) + this.HashedPassword.GetHashCode();
			hash = (hash*7) + this.LastLoggedOnDatetime.GetHashCode();
			return hash;
		}
	}
}
