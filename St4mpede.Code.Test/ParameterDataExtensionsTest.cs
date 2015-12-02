﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace St4mpede.Code.Test
{
	[TestClass]
	public class ParameterDataExtensionsTest
	{
		[TestMethod]
		public void ToMethodParameterDeclaration_given_Parameters_should_ReturnProper()
		{
			//	#	Arrange.
			var parameters = new List<ParameterData>
			{
				new ParameterData
				{
					Name="MyName1",
					SystemTypeString="MyType1"
				},
				new ParameterData
				{
					Name="MyName2",
					SystemTypeString="MyType2"
				}
			};

			//	#	Act.
			var res = parameters.ToMethodParameterDeclaration();

			//	#	Assert.
			CollectionAssert.AreEqual(
				new[] {
					"MyType1 MyName1",
					"MyType2 MyName2"
					},
				res.ToList());
		}

		[TestMethod]
		public void ToMethodParameterDeclarationString_given_Parameters_should_ReturnProper()
		{
			//	#	Arrange.
			var parameters = new List<ParameterData>
			{
				new ParameterData
				{
					Name="MyName1",
					SystemTypeString="MyType1"
				},
				new ParameterData
				{
					Name="MyName2",
					SystemTypeString="MyType2"
				}
			};

			//	#	Act.
			var res = parameters.ToMethodParameterDeclarationString();

			//	#	Assert.
			Assert.AreEqual(
				"MyType1 MyName1, MyType2 MyName2",
				res);
		}
	}
}