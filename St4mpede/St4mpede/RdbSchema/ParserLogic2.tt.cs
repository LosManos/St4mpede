//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible

namespace St4mpede.RdbSchema
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	//	using System;
#endif
	//#	Regular ol' C# classes and code...

	internal interface IParserLogic2
	{
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
