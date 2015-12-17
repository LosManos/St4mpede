//	This file was generated by St4mpede,Poco 2015-12-17 22:28:15Z.

//	ReSharper disable BuiltInTypeReferenceStyle
//	ReSharper disable BuiltInTypeReferenceStyle
//	ReSharper disable NonReadonlyMemberInGetHashCode
//	ReSharper disable ArrangeThisQualifier
//	ReSharper disable PartialTypeWithSinglePart
namespace TheDAL.Poco
{
	public partial class Customer
	{
		/// <summary> This property is part of the primary key.
		/// </summary>
		public System.Int32 CustomerID{ get; set; }
	
		public System.String Name{ get; set; }
	
		/// <summary> Default constructor needed for instance for de/serialising.
		/// </summary>
		public Customer()
		{
		}
	
		/// <summary> This constructor takes all properties as parameters.
		/// </summary>
		public Customer( System.Int32 customerID, System.String name )
		{
			this.CustomerID = customerID;
			this.Name = name;
		}
	
		/// <summary> This constructor takes all properties but primary keys as parameters.
		/// </summary>
		public Customer( System.String name )
		{
			this.Name = name;
		}
	
		/// <summary> This is the copy constructor.
		/// </summary>
		public Customer( Customer customer )
		{
			this.CustomerID = customer.CustomerID;
			this.Name = customer.Name;
		}
	
		/// <summary> This is the Equals method.
		/// </summary>
		public override System.Boolean Equals( System.Object o )
		{
			var obj = o as Customer;
			if( obj == null ){
				return false;
			}
			
			return
				this.CustomerID == obj.CustomerID &&
				this.Name == obj.Name;
		}
	
		/// <summary> This is the GetHashCode method.
		/// </summary>
		public override System.Int32 GetHashCode()
		{
			int hash = 13;
			hash = (hash*7) + this.CustomerID.GetHashCode();
			hash = (hash*7) + ( null == Name ? 0 : this.Name.GetHashCode() );
			return hash;
		}
	}
}
