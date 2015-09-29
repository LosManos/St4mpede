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
			const string ColumnOneAName = "ColOne";
			const string ColumnOneBName = "ColTwo";
			const string TableNameOne = "One";
			const string TableNameTwo = "Two";
			var databaseData = new DatabaseData
			{
				Tables = new List<TableData>
				{
					new TableData {
						Name=TableNameOne,
						Include=true,
						Columns =new List<ColumnData> {
							new ColumnData(
								ColumnOneAName, "nvarchar"),
							new ColumnData(
								ColumnOneBName, "numeric")
						}
					},
                    new TableData {
						Name = TableNameTwo,
						Include = false,
						Columns = new List<ColumnData>()
					}
                }
			};
			sut.UT_DatabaseData= databaseData;

			//	#	Act.
			sut.Generate();

			//	#	Assert.
			Assert.AreEqual(1, sut.UT_ClassData.Count, 
				"Only 1 Table is included.");
			var theClass = sut.UT_ClassData.Single();
			Assert.AreEqual(TableNameOne, theClass.Name, 
				"The name of the Class should be the same as the Table.");
			Assert.AreEqual(2, theClass.Properties.Count, 
				"Both Columns should be used as 2 Properties.");
			Assert.AreEqual(ColumnOneAName, theClass.Properties[0].Name, 
				"The name of the Property is the same as that of the Column.");
			Assert.AreEqual("System.String", theClass.Properties[0].DotnetTypeName,
				"The Property is a string.");
			Assert.AreEqual(ColumnOneBName, theClass.Properties[1].Name, 
				"The name of the Property should be teh same as the Column.");
			Assert.AreEqual("System.Int32", theClass.Properties[1].DotnetTypeName,
				"The Property type is int.");
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
			//	#	Arrange.
			var sut = new PocoGenerator(new Log(), null);
			sut.UT_ClassData = new List<ClassData>
			{
				new ClassData {
					Name= "Customer",
					Properties = new List<PropertyData>
					{
						new PropertyData(
							"CustomerID",
							typeof(int).ToString())
					}
				}
			};

			//	#	Act.
			sut.Output();

			//	#	Arrange
		}
	}
}
