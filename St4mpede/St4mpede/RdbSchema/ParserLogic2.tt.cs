//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible

using System;
using System.Collections.Generic;
using System.Linq;

namespace St4mpede.RdbSchema
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	//	using System;
#endif
	//#	Regular ol' C# classes and code...

	internal interface IParserLogic2
	{
		Type ConvertDatabaseTypeToDotnetType(string databaseTypeName);
		string ConvertDatabaseTypeToDotnetTypeString(string databaseTypeName);
		DatabaseData GetResult(string pathfile);
	}

	/// <summary>This is a stupid name for a class containing functionality for Parser.
	/// We cannot use <see cref="ParserLogic"/> because it contains 
	/// Microsoft.SqlServer.Management.Smo references in the method signatures
	/// and that means the callers have to konw about SqlServer stuff 
	/// which is something we really don't want; only the resulting data.
	/// </summary>
	internal class ParserLogic2 : IParserLogic2
	{
		internal static class DatabaseTypes
		{
			internal const string DateTime = "datetime";
			internal const string NChar = "nchar";
			internal const string NVarChar = "nvarchar";
			internal const string Int = "int";
			internal const string VarChar = "varchar";
			internal const string Char = "char";
		}

		/// <summary>This is a dictionary of how the database types match to dotnet types.
		/// TODO:Create a dictionary of this list. Maybe we can get rid of the case of not ubiquitous data too.
		/// <para>
		/// NOTE: It cannot be static as it messes up the test. 
		/// We have a test that tests if we have a not ubuquitous type conversion and for that we manipulate this dictionary to be in a not correct way. If it is static this erroneous Types stays put before next test that then fails. Run next test by itself and the test succeeds. Hard error to track down that is.</para>
		/// </summary>
		private IList<TypesTuple> Types = new List<TypesTuple>
		{
			new TypesTuple(DatabaseTypes.DateTime, typeof(DateTime).ToString()),
			new TypesTuple(DatabaseTypes.NChar, typeof(char).ToString()),
			new TypesTuple(DatabaseTypes.NVarChar, typeof(string).ToString()),
			new TypesTuple(DatabaseTypes.Int, typeof(int).ToString()),
			new TypesTuple(DatabaseTypes.VarChar, typeof(string).ToString()),
			new TypesTuple(DatabaseTypes.Char, typeof(string).ToString()),
		};

		internal class TypesTuple
		{
			internal string DatabaseTypeName { get; set; }
			internal string DotnetTypeName { get; set; }
			internal TypesTuple(string databaseTypeName, string dotnetTypeName)
			{
				DatabaseTypeName = databaseTypeName;
				DotnetTypeName = dotnetTypeName;
			}
		}

		public Type ConvertDatabaseTypeToDotnetType(string databaseTypeName)
		{
			var res = Types.Where(t => t.DatabaseTypeName == databaseTypeName).ToList();
			switch (res.Count)
			{
				case 0:
					throw new ArgumentException(string.Format("ERROR! Unkown database type {0}. This is a technical error and the dictionary should be updated.", databaseTypeName), "databaseTypeName");
				case 1:
					return Type.GetType(res.Single().DotnetTypeName);
				default:
					throw new ArgumentException(string.Format("ERROR! Not ubiquitous database type {0} as it could be referenced to any of [{1}]. This is a technical error and the dictionary should be updated.", databaseTypeName, string.Join(",", res.Select(t => t.DotnetTypeName))
						),
					"databaseTypeName");
			}
		}

		public string ConvertDatabaseTypeToDotnetTypeString(string databaseTypeName)
		{
			var res = Types.Where(t => t.DatabaseTypeName == databaseTypeName).ToList();
			switch (res.Count)
			{
				case 0:
					return string.Format("ERROR! Unkown database type {0}. This is a technical error and teh dictionary should be updated.", databaseTypeName);
				case 1:
					return res.Single().DotnetTypeName;
				default:
					return string.Format("ERROR! Not ubiquitous database type {0} as it could be referenced to any of [{1}]. This is a technical error and the dictionary should be updated.", databaseTypeName, string.Join(",", res.Select(t => t.DotnetTypeName)));
			}
		}

		/// <summary>This method returns the result of Parser.
		/// Parser's "main" output is the XML describing the files it creates.
		/// <para>
		/// Find a way to get rid of the Pathfile parameters. 
		/// This class should know itself where it stores its output.</para>
		/// </summary>
		/// <param name="pathfile"></param>
		/// <returns></returns>
		public DatabaseData GetResult(string pathfile)
		{
			return new Core().ReadFromXmlPathfile<DatabaseData>(pathfile);
		}
	}

#if NOT_IN_T4
} //end the namespace
#endif
//#>
