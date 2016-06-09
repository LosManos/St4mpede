namespace St4mpede.Sql.Test
{
	using System;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class UtilsTests
	{
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ToCameLCase_null_ThrowException()
		{
			((string) null).ToCamelCase();
		}

		[TestMethod]
		public void ToCamelCase_PascalCase_ReturnCamelCase()
		{
			Assert.AreEqual("a", "a".ToCamelCase());
			Assert.AreEqual("a", "A".ToCamelCase());
			Assert.AreEqual("abc", "abc".ToCamelCase());
			Assert.AreEqual("abc", "Abc".ToCamelCase());
			Assert.AreEqual("aBC", "ABC".ToCamelCase());
			Assert.AreEqual("myCustomer", "MyCustomer".ToCamelCase());
		}
	}
}
