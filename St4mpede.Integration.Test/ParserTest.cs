﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace St4mpede.Integration.Test
{
	[TestClass]
	public class ParserTest
	{
        [TestMethod]
		[TestCategory(Common.Constants.TestCategoryIntegration)]
		public void ParseDatabaseTest()
		{
			Assert.Inconclusive("TBA");
			var sut = new Parser();
			sut.ParseDatabase();
		}

	}
}

