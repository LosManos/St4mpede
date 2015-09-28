using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using St4mpede.Poco;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace St4mpede.Test
{
	[TestClass]
	public class PocoGeneratorTest
	{
		[TestMethod]
		public void Generate_given_Tables_should_CreateOnlyIncludedAsClass()
		{
			//	#	Arrange.
			var sut = new PocoGenerator(new Log(), null);
			const string TableNameOne = "One";
			const string TableNameTwo = "Two";
			var databaseData = new DatabaseData
			{
				Tables = new List<TableData>
				{
					new TableData(TableNameOne, true),
					new TableData(TableNameTwo, false)
				}
			};
			sut.UT_DatabaseData= databaseData;

			//	#	Act.
			sut.Generate();

			//	#	Assert.
			Assert.AreEqual(1, sut.UT_Get_ClassData().Count);
			var theClass = sut.UT_Get_ClassData().Single();
			Assert.AreEqual(TableNameOne, theClass.Name);

		}

		[TestMethod]
		public void ReadXml_given_XDoxWith2Tables_should_ReturnPocosWith2Tables()
		{
			//	#	Arrange.
			var log = new Log();
			var xml = XDocument.Parse(@"
<Database>
  <Tables>
    <Table>
		<Name>Table one</Name>
    </Table>
    <Table>
		<Name>Table two</Name>
    </Table>
  </Tables>
</Database>
");
            var mockXDocHandler = new Mock< PocoGenerator.IXDocHandler>();
			mockXDocHandler
				.Setup(m => m.Load(It.IsAny<string>()))
				.Returns(xml);
			var sut = new PocoGenerator(log, mockXDocHandler.Object);

			//	#	Act.
			sut.ReadXml();

			//	#	Assert.
			Assert.AreEqual(2, sut.UT_DatabaseData.Tables.Count);
		}

		[TestMethod]
		public void OutputTest()
		{
			Assert.Fail("TBA");
			var sut = new PocoGenerator();
			sut.Output();
		}
	}
}
