using System.Collections.Generic;

namespace St4mpede.Test.Extensions
{
	internal static class ObjectExtensions
	{
		internal static T AddItem<T>(this IList<T> lst, T item)
		{
			lst.Add(item);
			return item;
		}
	}
}
