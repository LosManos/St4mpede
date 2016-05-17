using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace St4mpede.Test.Core
{
	[TestClass]
	public class CoreTest
	{
		[TestMethod]
		public void DeserialiseTest()
		{
			Assert.Inconclusive("TBA");
		}

		[TestMethod]
		[Ignore]
		public void ReadFromXmlPathfile_given_XDoxWith2Tables_should_ReturnPocosWith2Tables()
		{
			//	#	Arrange.
			var sut = new global::St4mpede.Core();// null, log, mockXDocHandler.Object);

			//	#	Act.
			var res = sut.ReadFromXmlPathfile<DatabaseData>("TBA");

			//	#	Assert.
			Assert.AreEqual(2, res.Tables.Count);
			Assert.AreEqual("one", res.Tables[0].Name);
		}

		[TestMethod]
		public void SerialiseTest()
		{
			Assert.Inconclusive("TBA");
		}
	}
}
