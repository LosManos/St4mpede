using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace St4mpede.Code.Test
{
	[TestClass]
	public class PropertyDataTest
	{
		[TestMethod]
		public void ToCode_given_NameAndType_should_ReturnCode()
		{
			//	#	Arrange.
			var sut = new PropertyData
			{
				Scope = Common.VisibilityScope.Public,
				Name = "MyName",
				SystemType = typeof(string)
			};

			//	#	Act.
			var res = sut.ToCode();

			//	#	Assert.
			Assert.AreEqual(1, res.Count);
			Assert.AreEqual(
				@"public System.String MyName{ get; set; }",
				res[0]);
		}

		[TestMethod]
		public void ToCode_given_CommentAndNameAndType_should_ReturnCode()
		{
			//	#	Arrange.
			var sut = new PropertyData
			{
				Comment = new CommentData("MyComment"),
				Scope = Common.VisibilityScope.Private,
				Name = "CustomerID",
				SystemType = typeof(int)
			};

			//	#	Act.
			var res = sut.ToCode();

			//	#	Assert.
			Assert.AreEqual(3, res.Count);
			CollectionAssert.AreEqual(
				new[]
				{
					"/// <summary> MyComment",
					"/// </summary>",
					"private System.Int32 CustomerID{ get; set; }"
				}, 
				res.ToList());
		}
	}
}
