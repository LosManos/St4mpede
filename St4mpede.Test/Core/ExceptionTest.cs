using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace St4mpede.Test.Core
{
	[TestClass]
	public class ExceptionTest
	{
		[TestMethod]
		public void St4mpedeExceptionTest()
		{
			//	#	Arrange.
			var sut = new St4mpedeException();

			Assert.IsTrue(sut is Exception, "Really only testing extistence of exception class.");
		}

		[TestMethod]
		public void UnknownDatabaseExceptionTest()
		{
			//	#	Arrange.
			const string TextOne = "a text";
			const string TextTwo = "another one";

			//	#	Act.
			var sut = new UnknownDatabaseException();	//	Asserting default constructor exists.
			sut = new UnknownDatabaseException(new[] { TextOne, TextTwo });

			//	#	Assert.
			Assert.IsTrue(new UnknownDatabaseException() is St4mpedeException, 
				"All exceptions should inherit from St4mpedeException; so should this.");

			Assert.IsTrue(sut.Message.Contains(TextOne));			
			Assert.IsTrue(sut.Message.Contains(TextTwo));

			Assert.AreEqual(2, sut.DatabaseInfo.Count);
			Assert.AreEqual(TextOne, sut.DatabaseInfo[0]);
			Assert.AreEqual(TextTwo, sut.DatabaseInfo[1]);
		}

	}
}
