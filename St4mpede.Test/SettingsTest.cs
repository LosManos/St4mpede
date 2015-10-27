using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace St4mpede.Test
{
	[TestClass]
	public class SettingsTest
	{
		[TestMethod]
		public void Settings_given_ObjectExists_should_HaveAllProperties()
		{
			//	#	Arrange and Act
			const string MyConfigPath = "myConfigPath";
			const string MyInitPathfilename = "myInitPathFileName";
			const string MyConnectionString = "myConnectionString";
			const string MyDatabaseName = "myDatabaseName";
			const int MyDatabaseIndex = 42;
			const string MyExcludedTablesRegex = "myExcludedTablesRegex";
			const string MyDatabaseXmlFile = "myDatabaseXmlFile";
			const string MyProjectPath = "MyProjectPath";
            var sut = new ParserSettings(
				MyConfigPath,
				MyInitPathfilename,
				MyConnectionString,
				MyDatabaseName,
				MyDatabaseIndex,
				MyExcludedTablesRegex,
				MyDatabaseXmlFile,
				MyProjectPath);

			//	#	Assert.
			Assert.AreEqual(MyConfigPath, sut.ConfigPath);
			Assert.AreEqual(MyInitPathfilename, sut.InitPathfilename);
			Assert.AreEqual(MyConnectionString, sut.ConnectionString);
			Assert.AreEqual(MyDatabaseName, sut.DatabaseName);
			Assert.AreEqual(MyDatabaseIndex, sut.DatabaseIndex);
			Assert.AreEqual(MyExcludedTablesRegex, sut.ExcludedTablesRegex);
			Assert.AreEqual(MyDatabaseXmlFile, sut.DatabaseXmlFile);
			Assert.AreEqual(MyProjectPath, sut.ProjectPath);

			Assert.IsFalse(
				sut.GetType().GetProperties().Any(p => p.GetValue(sut, null) == null),
				"All properties should be set for this test to work properly.");
		}
	}
}