using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace St4mpede.Test
{
	[TestClass]
	public class SettingsTest
	{
		[TestMethod]
		public void Settings_given_ObjectExists_should_HaveAllProperties()
		{
			//	#	Arrange and Act.
			var sut = new St4mpede.Settings();

			//	#	Assert.
			Assert.IsNull(sut.InitPathfilename);
			Assert.IsNull(sut.ConnectionString);
			Assert.IsNull(sut.DatabaseName);
			Assert.IsNull(sut.DatabaseIndex);
			Assert.AreEqual(0, sut.ExcludedTablesRegex);
			Assert.IsNull(sut.DatabaseXmlFile);
		}
	}
}