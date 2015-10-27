//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
namespace St4mpede
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	using System;
	using System.Collections.Generic;
#endif
	//#	Regular ol' C# classes and code...

	internal class St4mpedeException : Exception
	{
		public St4mpedeException()		{		}

		public St4mpedeException( string message )
			:base(message)
		{		}
	}

	internal class UnknownDatabaseException : St4mpedeException
	{
		public IList<string> DatabaseInfo { get; set; }

		public UnknownDatabaseException()		{		}

		public UnknownDatabaseException( IList<string> databasesInfo )
			:base( string.Format("The database cannot be found. Existing databases in the server are:{0}{1}", "\r\n", string.Join("\r\n", databasesInfo)))
		{
			this.DatabaseInfo = databasesInfo;
		}
	}

#if NOT_IN_T4
} //end the namespace
#endif
//#>