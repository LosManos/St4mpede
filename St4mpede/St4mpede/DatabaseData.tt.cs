//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
namespace St4mpede
{
	using System.Collections.Generic;
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	using System.Runtime.Serialization;
	using System.Xml.Serialization;
#endif
	//#	Regular ol' C# classes and code...

	/// <summary>This class must be public to make de/serialising possible when unit testing. </summary>
	[DataContract]
	[XmlType(TypeName = "Database")]
	public class DatabaseData
	{
		/// <summary>List of Tables for this Database.
		/// <para>Note it must be List and not IList to make serialising work.</para>
		/// </summary>
		public List<TableData> Tables { get; set; }
	}

#if NOT_IN_T4
} //end the namespace
#endif
//#>