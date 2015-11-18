using System.Collections.Generic;

namespace St4mpede.Code
{
	public interface IToCode
	{
		IList<string> ToCode();

		//IList<string> ToCode(int indentLevel);

	}
}