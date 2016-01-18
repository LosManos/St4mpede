using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AutoMapper;
using AutoMapper.Mappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace St4mpede.Settings.Test
{
	[TestClass]
	public class SettingsComplexTest
	{
		internal class SubElementsRootClass
		{
			public SubElementsRootClass()
			{
				Projects = new List<Project>();
			}

			public int CustomerId { get; set; }
			public string Customer { get; set; }
			public IList<Project> Projects { get; set; }
		}

		internal class Project
		{
			public int CustomerId { get; set; }
			public string Name { get; set; }
		}

		[TestMethod]
		public void GetFromXml_given_SubElements_should_Populate()
		{
			//	#	Arrange.
			const string XmlString = @"
<Root>
	<Customer id='42'>Bengt</Customer>
	<Projects>
		<Project customerId='42'>Main</Project>
		<Project customerId='43'>Other</Project>
	</Projects>
</Root>";

			Func<XElement, string, string, List<XElement>> mapItems =
			(src, collectionName, elementName) =>
				(src.Element(collectionName) ?? new XElement(collectionName)).Elements(elementName).ToList();

			var mappingConfigStore = new ConfigurationStore(new TypeMapFactory(), MapperRegistry.Mappers);
			mappingConfigStore.CreateMap<XElement, SubElementsRootClass>()

				.ForMember(dest => dest.Customer,
					opt => opt.ResolveUsing<XElementResolver<string>>()
						.ConstructedBy(() => new XElementResolver<string>("Customer")))

				.ForMember(dest => dest.CustomerId,
					opt => opt.ResolveUsing<XAttributeResolver<int>>()
						.FromMember(src => src.Element("Customer"))
						.ConstructedBy(() => new XAttributeResolver<int>("id")))

				.ForMember(dest => dest.Projects,
					opt => opt.MapFrom(src => mapItems(src, "Projects", "Project")));

			mappingConfigStore.CreateMap<XElement, Project>()

				.ForMember(dest => dest.Name,
					opt => opt.ResolveUsing<XElementResolver<string>>()
						.ConstructedBy(() => new XElementResolver<string>(/*"Project"*/)))

				.ForMember(dest => dest.CustomerId,
					opt => opt.ResolveUsing<XAttributeResolver<int>>()
						.FromMember(src => src /*.Element("Project")*/)
						.ConstructedBy(() => new XAttributeResolver<int>("customerId")));

			//.ForMember(dest => dest.ProjectName,
			//	opt => opt.ResolveUsing<XElementResolver<string>>()
			//		.ConstructedBy(() => new XElementResolver<string>("Project")))

			//.ForMember(dest => dest.CustomerId,
			//	opt => opt.ResolveUsing<XAttributeResolver<int>>()
			//		.FromMember(src => src.Element("Project"))
			//		.ConstructedBy(() => new XAttributeResolver<int>("customerId")));

			var sut = new Settings(new MappingEngine(mappingConfigStore));
			var xml = XDocument.Parse(XmlString);

			//	#	Act.
			var res = sut.GetFromXml<SubElementsRootClass>(xml.Element("Root"));

			//	#	Assert.
			Assert.AreEqual("Bengt", res.Customer);
			Assert.AreEqual(42, res.CustomerId);
			Assert.AreEqual(2, res.Projects.Count);
			Assert.AreEqual(42, res.Projects[0].CustomerId);
			Assert.AreEqual("Main", res.Projects[0].Name);
			Assert.AreEqual(43, res.Projects[1].CustomerId);
			Assert.AreEqual("Other", res.Projects[1].Name);
		}
	}
}
