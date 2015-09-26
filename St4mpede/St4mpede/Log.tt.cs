//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
namespace St4mpede
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	using System;
#endif
	//#	Regular ol' C# classes and code...

	interface ILog
	{

	}

	internal class Log : ILog
	{

	}
#if NOT_IN_T4
} //end the namespace
#endif
//#>