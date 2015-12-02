using System;
using System.Collections.Generic;

namespace St4mpede.Code
{
	public static class Common
	{
		/// <summary>This is a list of all keywords in C#.
		/// If you think the language seems too simple to use it is alright. Just add more keywords.
		/// </summary>
		public static readonly IList<string> KeyWords = new List<string>
		{
			"public",
			"static",
			"string"
		};

		/// <summary>This enum contains the visibility scopes there are.
		/// </summary>
		public enum VisibilityScope
		{
			Public, 
			Internal, 
			Private
		}

		/// <summary>This method return a visibilityscope as a c# string.
		/// </summary>
		/// <param name="me"></param>
		/// <returns></returns>
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

		/// <summary>This method makes sure any variable that does not compile will.
		/// For instance are keywords prefixed with an amersand.
		/// </summary>
		/// <param name="variableName"></param>
		/// <returns></returns>
		public static string Safe(string variableName)
		{
			return (KeyWords.Contains(variableName) ? "@" : "") +variableName;
		}

		/// <summary>This method returns the string as camel case.
		/// It has no knowledge about words but just lower cases the first letter.
		/// If null or whitespace is sent along the same string is returned.
		/// <para>
		/// The method does still not handle a string with spaces before e.g. "  Customer".
		/// </para>
		/// </summary>
		/// <param name="variable"></param>
		/// <returns></returns>
		public static string ToCamelCase(string variable)
		{
			if(string.IsNullOrWhiteSpace(variable))
			{
				return variable;
			}
			return Char.ToLowerInvariant(variable[0]) + variable.Substring(1);
        }
	}
}