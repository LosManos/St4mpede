using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace St4mpede.Code.Test
{
	[TestClass]
	public class CommentDataTest
	{
		[TestMethod]
		public void ToCode_given_OneRowSummary_should_ReturnCommentOnTwoRows()
		{
			//	#	Arrange.
			var sut = new CommentData("MyComment");

			//	#	Act.
			var res = sut.ToCode();

			//	#	Arrange.
			Assert.AreEqual(2, res.Count);
			CollectionAssert.AreEqual(
				new[] {
					"/// <summary> MyComment",
					"/// </summary>"},
			res.ToList());
		}

		[TestMethod]
		public void ToCode_given_TwoRowsSummary_should_returnThreeRows()
		{
			//	#	Arrange.
			var sut = new CommentData();
			sut.Summary = new List<string>
		{
			"First row",
			"second row."
		};

			//	#	Act.
			var res = sut.ToCode();

			//	#	Assert.
			Assert.AreEqual(3, res.Count);
			CollectionAssert.AreEqual(
				new[]
				{
				"/// <summary> First row",
				"/// second row.",
				"/// </summary>"
				},
				res.ToList());
		}
	}
}
