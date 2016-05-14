using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace St4mpede.Code.Test
{
	[TestClass]
	public class CommonTest
	{
		#region Safe tests.

		[TestMethod]
		public void Safe_given_KeyWord_should_ReturnSafeString()
		{
			Assert.AreEqual("@public", Common.Safe("public"));
		}

		[TestMethod]
		public void Safe_given_NoKewWord_should_ReturnAsIs()
		{
			Assert.AreEqual("Customer", Common.Safe("Customer"));
		}

		#endregion

		#region ToCamelCase tests.

		[TestMethod]
		public void ToCamelCase_given_NullOrWhiteSpace_should_ReturnTheSame()
		{
			Assert.AreEqual(null, Common.ToCamelCase(null));
			Assert.AreEqual(string.Empty, Common.ToCamelCase(string.Empty));
			Assert.AreEqual(" ", Common.ToCamelCase(" "));
			Assert.AreEqual("\t", Common.ToCamelCase("\t"));
		}

		[TestMethod]
		public void ToCamelCase_given_PropertString_should_ReturnCamelCase()
		{
			Assert.AreEqual("customer", Common.ToCamelCase("customer"));
			Assert.AreEqual("customer", Common.ToCamelCase("Customer"));
			Assert.AreEqual("customerData", Common.ToCamelCase("CustomerData"));
		}

		#endregion
	}
}
