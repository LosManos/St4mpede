using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace St4mpede.Integration.Test.Common
{
	internal class Functions
	{
		/// <summary>This method returns the path to the solution.
		/// </summary>
		/// <param name="testContext"></param>
		/// <returns></returns>
		internal static string SolutionPath( TestContext testContext )
		{
			//	The path is something like "C:\DATA\PROJEKT\St4mpede\St4mpede\TestResults\Deploy_LosManos 2015-10-27 11_22_36" so we remove everything to the right of "TestResults".
			var dir = testContext.TestDir;
			const string Token = @"\TestResults";
			var path = dir.Substring(0, dir.Length - dir.IndexOf(Token) - Token.Length - 2);
			return path;
		}
	}
}
