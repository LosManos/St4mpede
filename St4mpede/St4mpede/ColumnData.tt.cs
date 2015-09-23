//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
namespace St4mpede
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	using System.Collections.Generic; 
	using System.Runtime.Serialization;
#endif
	//#	Regular ol' C# classes and code...

	[DataContract]
	public class ColumnData
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string TypeName { get; set; }

		public ColumnData()		{		}

		public ColumnData(string name, string typeName)
		{
			this.Name = name;
			this.TypeName = typeName;
		}
	}

	internal static class ColumnDataHelpers
	{
		internal static IEnumerable<string> ToInfo( IList<ColumnData> columns)
		{
			var ret = new List<string>();
			foreach( var column in columns)
			{
				//	Even though we are using Dotnet4.6 is seems we cannot use the $ string interpolation.
				//ret.Add($"Name={column.Name}\tType={column.TypeName}");
				ret.Add(string.Format("Name={0}\tType={1}", column.Name, column.TypeName));
            }

			return ret;
		}
	}

#if NOT_IN_T4
} //end the namespace
#endif
//#>