//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
using System.Xml.Linq;

namespace St4mpede.St4mpede.Core
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...

#endif
	//#	Regular ol' C# classes and code...

	internal interface IXDocHandler
	{
		XDocument Load(string pathfile);
	}

	internal class XDocHandler : IXDocHandler
	{
		XDocument IXDocHandler.Load(string pathfile)
		{
			return XDocument.Load(pathfile);
		}
	}
#if NOT_IN_T4
} //end the namespace
#endif
//#>
