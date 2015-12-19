using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace St4mpede.Code.Test
{
	[TestClass]
	public class ClassDataTest
	{
		[TestMethod]
		public void ToCode_given_TwoPropertiesAndConstructor_should_ReturnCode()
		{
			//	#	Arrange.
			var sut = new ClassData();
			sut.Comment = new CommentData("Simple DTO class.");
			sut.Name = "Customer";
			sut.Properties = new List<PropertyData>
			{
				new PropertyData
				{
					Name="CustomerID",
					SystemType=typeof(int),
					Scope=Common.VisibilityScope.Internal
				},
				new PropertyData
				{
					Name="Name",
					SystemType =typeof(string),
					Scope=Common.VisibilityScope.Public
				}
			};
			sut.Methods = new List<MethodData>
			{
				new MethodData
				{
					Name="Customer",
					IsConstructor= true,
					Scope=Common.VisibilityScope.Internal
				}
			};

			//	#	Act.
			var res = sut.ToCode();

			//	#	Assert.
			Assert.AreEqual(12, res.Count);
			CollectionAssert.AreEqual(
				new[] {
				"/// <summary> Simple DTO class.",
				"/// </summary>",
				"public class Customer",
				"{",
				"\tinternal System.Int32 CustomerID{ get; set; }",
				string.Empty,
				"\tpublic System.String Name{ get; set; }",
				string.Empty,
				"\tinternal Customer()",
				"\t{",
				"\t}",
				"}" },
				res.ToList());
		}
	}
}
