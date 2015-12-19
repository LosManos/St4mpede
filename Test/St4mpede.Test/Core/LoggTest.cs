using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Xml.Linq;

namespace St4mpede.Test.Core
{
	[TestClass]
	public class LoggTest
	{
		[TestMethod]
		public void Add_given_FormatAndArgs_should_AddItem()
		{
			//	#	Arrange.
			ILog sut = new Log();
			Assert.IsFalse(sut.Logs.Any(), "There should be no log when we start.");

			//	#	Act.
			sut.Add("a{0}c{1}", "b", "d");

			//	#	Assert.
			Assert.AreEqual("abcd", sut.Logs.Single());
		}

		[TestMethod]
		public void Add_given_LogRows_should_AddThem()
		{
			//	#	Arrange.
			ILog sut = new Log();
			Assert.IsFalse(sut.Logs.Any(), "There should be no log when we start.");
			const string RowOne = "My first row";
			const string RowTwo = "My second row";

			//	#	Act.
			sut.Add(new[] { RowOne, RowTwo });

			//	#	Assert.
			Assert.AreEqual(2, sut.Logs.Count());
			Assert.AreEqual(RowOne, sut.Logs[0]);
			Assert.AreEqual(RowTwo, sut.Logs[1]);
		}

		[TestMethod]
		public void Add_given_XDocument_should_AddIt()
		{
			//	#	Arrange.
			ILog sut = new Log();
			Assert.IsFalse(sut.Logs.Any(), "There should be no log when we start.");
			var xml = new XDocument();

			//	#	Act.
			sut.Add(xml);

			//	#	Assert.
			Assert.AreEqual(1, sut.Logs.Count(),
			"Right now we don't test the formatting, only the existence of the row.");
		}


	}
}
