//	This file was generated by St4mpede 2015-12-02 20:44:39Z.

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
	public User( System.Int32 UserId, System.String UserName, System.Char HashedPassword, System.DateTime LastLoggedOnDatetime )
	{
		this.UserId = UserId;
		this.UserName = UserName;
		this.HashedPassword = HashedPassword;
		this.LastLoggedOnDatetime = LastLoggedOnDatetime;
	}

	/// <summary> This constructor takes all properties but primary keys as parameters.
	/// </summary>
	public User( System.String UserName, System.Char HashedPassword, System.DateTime LastLoggedOnDatetime )
	{
		this.UserName = UserName;
		this.HashedPassword = HashedPassword;
		this.LastLoggedOnDatetime = LastLoggedOnDatetime;
	}
}
