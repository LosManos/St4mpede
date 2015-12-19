using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace St4mpede.Code.Test
{
	[TestClass]
	public class MethodDataTest
	{
		[TestMethod]
		public void ToCode_given_ConstructorDataWithoutParameters_should_ReturnDefaultConstructorCode()
		{
			//	#	Arrange.
			var sut = new MethodData();
			sut.Comment = new CommentData("Default constructor.");
			sut.IsConstructor = true;
			sut.Scope = Common.VisibilityScope.Public;
			sut.Name = "MyClassName";

			//	#	Act.
			var res = sut.ToCode();

			//	#	Assert.
			Assert.AreEqual(5, res.Count);
			CollectionAssert.AreEqual(
				new[]
				{
					"/// <summary> Default constructor.",
					"/// </summary>",
					"public MyClassName()",
					"{",
					"}",
				},
				res.ToList());
		}

		[TestMethod]
		public void ToCode_given_ConstructorDataWithParameters_should_ReturnProperConstrutor()
		{
			//	#	Arrange.
			var sut = new MethodData
			{
				IsConstructor = true,
				Scope = Common.VisibilityScope.Internal,
				Name = "Customer",
				Parameters = new List<ParameterData>
				{
					new ParameterData
					{
						Name="CustomerId",
						SystemTypeString = typeof(int).ToString()
					},
					new ParameterData
					{
						Name="CustomerName",
						SystemTypeString = typeof(string).ToString()
					}
				}
			};

			//	#	Act.
			var res = sut.ToCode();

			//	#	Assert.
			CollectionAssert.AreEqual(
				new[]
				{
					"internal Customer( System.Int32 customerId, System.String customerName )",
					"{",
					"\tthis.CustomerId = customerId;",
					"\tthis.CustomerName = customerName;",
					"}"
				},
				res.ToList());
		}

		[TestMethod]
		public void ToCode_given_ConstructorDataWithBody_should_ReturnProper()
		{
			//	#	Arrange.
			const string ClassName = "Customer";

			var sut = new MethodData
			{
				IsConstructor = true,
				Scope = Common.VisibilityScope.Internal,
				Name = ClassName,
				Parameters = new List<ParameterData>
				{
					new ParameterData
					{
						Name=ClassName,
						SystemTypeString = ClassName
					}
				}, 
				Body = new BodyData
				{
					Lines= new List<string>
					{
						"this.CustomerId = customer.CustomerId;", 
						"this.CustomerName = customer.CustomerName;"
					}
				}
			};

			//	#	Act.
			var res = sut.ToCode();

			//	#	Assert.
			CollectionAssert.AreEqual(
				new[]
				{
					"internal Customer( Customer customer )",
					"{",
					"\tthis.CustomerId = customer.CustomerId;",
					"\tthis.CustomerName = customer.CustomerName;",
					"}"
				},
				res.ToList());
		}
	}
}
