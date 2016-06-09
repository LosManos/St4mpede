namespace St4mpede.Sql
{
	using System;

	public static class Utils
    {
		public static string ToCamelCase(this string identifier)
		{
			if (null == identifier)
			{
				throw new ArgumentNullException(nameof(identifier));
			}
			return identifier.Substring(0, 1).ToLower() + identifier.Substring(1);
		}
	}
}
