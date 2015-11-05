//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
namespace St4mpede.Poco
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.Xml.Serialization;
#endif
	//#	Regular ol' C# classes and code...

	/// <summary>This class must be public to make de/serialising possible when unit testing. </summary>
	[DataContract]
	[XmlType(TypeName = "Class")]
	public class ClassData
	{
		[DataMember]
		public string Name { get; set; }

		public ClassData() { }

		public ClassData(string name)
		{
			this.Name = name;
		}

		/// <summary>List of Properties for this Class.
		/// <para>Note it must be List and not IList to make serialising work.</para>
		/// </summary>
		public List<PropertyData> Properties { get; set; }
	}

	/// <summary>This class must be public to make de/serialising possible when unit testing. </summary>
	[DataContract]
	[XmlType(TypeName = "Property")]
	public class PropertyData
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string DotnetTypeName { get; set; }

		[DataMember]
		public bool IsInPrimaryKey { get; set; }

		public PropertyData()		{		}

		public PropertyData(string name, string dotnetTypeName, bool isInPrimaryKey)
		{
			this.Name = name;
			this.DotnetTypeName = dotnetTypeName;
			this.IsInPrimaryKey = isInPrimaryKey;
		}
	}

	#region Helper methods.
	internal static class ClassDataHelpers
	{
		internal static IEnumerable<string> ToInfo(IList<ClassData> classes)
		{
			var ret = new List<string>();
			foreach (var c in classes)
			{
				//	Even though we are using Dotnet4.6 is seems we cannot use the $ string interpolation. Is it T4?
				//ret.Add($"Name={column.Name}\tType={column.TypeName}");
				ret.Add(string.Format("Name={0}", 
					c.Name));
			}

			return ret;
		}

		internal static IEnumerable<string> ToInfo(IList<PropertyData> properties)
		{
			return properties
				//	Even though we are using Dotnet4.6 is seems we cannot use the $ string interpolation. Is it T4?
				.Select(p => string.Format("Name={0},DotnetTypeName={1}",
				p.Name, p.DotnetTypeName));
		}
	}

	#endregion

#if NOT_IN_T4
} //end the namespace
#endif
//#>