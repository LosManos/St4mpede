//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
namespace St4mpede
{
	using System;
	using System.Collections.Generic;
#endif
	//#	Regular ol' C# classes and code...

	//	TODO:Create proper Exceptions.
	internal class St4mpedeException : Exception
	{
		public St4mpedeException()		{		}

		public St4mpedeException( string message )
			:base(message)
		{		}
	}

	internal class UnknownDatabaseException : St4mpedeException
	{
		public UnknownDatabaseException()		{		}

		public UnknownDatabaseException( IList<string> databasesInfo )
			:base( string.Format("The database cannot be found. Exising databases in the server are:{0}{1}", "\r\n", string.Join("\r\n", databasesInfo)))
		{		}
	}

#if NOT_IN_T4
} //end the namespace
#endif
//#>