using System;

namespace St4mpede.Code
{
	public static class Common
	{
		public enum VisibilityScope
		{
			Public, 
			Internal, 
			Private
		}

		public static string ToCode( this VisibilityScope me)
		{
			switch (me)
			{
				case VisibilityScope.Public: return "public";
				case VisibilityScope.Internal: return "internal";
				case VisibilityScope.Private: return "private";
				default: throw new NotImplementedException(string.Format("VisibilityScope {0} is not yet implemented or totally wrong.", me));
			}
		}
	}
}