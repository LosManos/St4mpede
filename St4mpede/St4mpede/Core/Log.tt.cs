//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
namespace St4mpede
{
	using System;
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	using System.Collections.Generic;
	using System.Xml.Linq;
#endif
	//#	Regular ol' C# classes and code...

	internal interface ILog
	{
		IList<string> Logs { get; }
		void Add(string format, params object[] args);
		void Add(IEnumerable<string> logRows);
		void Add(XDocument xml);
		void AddError(string format, params object[] args);
		string ToInfo();
	}

	internal class Log : ILog
	{
		private IList<string> _log = new List<string>();

		IList<string> ILog.Logs { get { return _log; } }
		
		void ILog.Add(string format, params object[] args)
		{
			_log.Add(string.Format(format, args));
		}

		void ILog.Add(IEnumerable<string> logRows)
		{
			foreach (var row in logRows)
			{
				((ILog)this).Add(row);
			}
		}

		void ILog.Add(XDocument xml)
		{
			((ILog)this).Add(xml.ToString());
		}

		void ILog.AddError(string format, params object[] args)
		{
			((ILog)this).Add("ERROR! : " + format, args);
		}

		string ILog.ToInfo()
		{
			return string.Join(Environment.NewLine, _log);
		}

		#region Unit testing work arounds.

		//internal IList<string> UT_Log { get { return _log; } }

		//[DebuggerStepThrough]
		//internal void UT_AddLog(string format, params object[] args)
		//{
		//	AddLog(format, args);
		//}

		//[DebuggerStepThrough]
		//internal void UT_AddLog(IEnumerable<string> logRows)
		//{
		//	AddLog(logRows);
		//}

		//[DebuggerStepThrough]
		//internal void UT_AddLog(XDocument xml)
		//{
		//	AddLog(xml);
		//}

		#endregion
	}
#if NOT_IN_T4
} //end the namespace
#endif
//#>