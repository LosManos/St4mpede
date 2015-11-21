using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace St4mpede.Code.Test
{
	[TestClass]
	public class MethodDataTest
	{
		[TestMethod]
		public void ToCode_given_ConstrutorData_should_ReturnConstructorCode()
		{
			//	#	Arrange.
			var sut = new MethodData();
			sut.Comment = new CommentData("Default constructor.");
			sut.IsConstructor = true;
			sut.Scope = Common.VisibilityScope.Public;
			sut.Name = "MyClassName";

			//	#	Act.
			var res = sut.ToCode();

			//	#	Assert.
			Assert.AreEqual(5, res.Count);
			CollectionAssert.AreEqual(
				new[]
				{
					"/// <summary> Default constructor.",
					"/// </summary>",
					"public MyClassName()",
					"{",
					"}",
				},
				res.ToList());
		}
	}
}
