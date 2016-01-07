using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Xml.Linq;
using AutoMapper;
using AutoMapper.Mappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace St4mpede.Settings.Test
{
	[TestClass]
	public class SettingsTest
	{
		private const string Xml =
			@"<?xml version='1.0' encoding='utf-8' ?>
<St4mpede>
	<Core>
		<RootFolder>C:\DATA\PROJEKT\St4mpede\St4mpede\St4mpede\St4mpede</RootFolder>
	</Core>
	<RdbSchema>
		<ConnectionString>Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename=C:\DATA\PROJEKT\St4mpede\St4mpede\St4mpede.Test\Database\St4mpede.mdf;Integrated Security = True; Connect Timeout = 30 </ConnectionString>
		<DatabaseName>C:\DATA\PROJEKT\ST4MPEDE\St4mpede\ST4MPEDE.TEST\DATABASE\ST4MPEDE.MDF</DatabaseName>
		<DatabaseIndex/>
		<ExcludedTablesRegex>__RefactorLog</ExcludedTablesRegex>
		<DatabaseXmlFile>St4mpede.RdbSchema.xml</DatabaseXmlFile>
		<ProjectPath>RdbSchema</ProjectPath>
	</RdbSchema>
	<Poco>
		<OutputFolder>..\..\TheDAL\Poco</OutputFolder>
		<NameSpace Name = 'TheDAL.Poco'>
            <Comments>
                <Comment> ReSharper disable BuiltInTypeReferenceStyle</Comment>
				<Comment>ReSharper disable BuiltInTypeReferenceStyle</Comment>
				<Comment>ReSharper disable NonReadonlyMemberInGetHashCode</Comment>
				<Comment>ReSharper disable ArrangeThisQualifier</Comment>
				<Comment>ReSharper disable PartialTypeWithSinglePart</Comment>
			</Comments>
		</NameSpace>
		<XmlOutputFilename>PocoGenerator.xml</XmlOutputFilename>
		<ProjectPath>Poco</ProjectPath>
		<MakePartial>True</MakePartial>
		<Constructors>
			<Default>True</Default>
			<AllProperties>True</AllProperties>
			<AllPropertiesSansPrimaryKey>True</AllPropertiesSansPrimaryKey>
			<CopyConstructor>True</CopyConstructor>
		</Constructors>
		<Methods>
			<Equals Regex = '.*'>True</Equals>
        </Methods>
	</Poco>
</St4mpede >";

		#region MenuXml

		private const string MenuXml = @"<Menu>
  <SubMenus>
    <Menu name='SubMenu 1'>
      <SubMenus>
        <Menu name = 'Sub-SubMenu 1' >
          <MenuItems >
            <MenuItem name='Menu Item 1-1-1' action='Action1.exe' />
            <MenuItem name = 'Menu Item 1-1-2' action='Action2.exe' parameters='-2' />
          </MenuItems>
        </Menu>
        <Menu name = 'Sub-SubMenu 2' >
          <MenuItems >
            <MenuItem name='Menu Item 1-2-1' action='Action3.exe' />
            <MenuItem name = 'Menu Item 1-2-2' action='Action4.exe' parameters='-2' />
          </MenuItems>
        </Menu>
      </SubMenus>
    </Menu>
    <Menu name = 'Sub Menu 2' >
      <MenuItems >
        <MenuItem name='Menu Item 2-1' action='Action5.exe' parameters='-item 2.1' />
      </MenuItems>
    </Menu>
  </SubMenus>
  <MenuItems>
    <MenuItem name = 'Menu Item 1' action='Action6.exe'/>
    <MenuItem name = 'Menu Item 2' action='Action7.exe' parameters='-7' />
  </MenuItems>
</Menu>";

		#endregion

		[TestMethod]
		[Ignore]
		public void GetMenuFromXmlTest()
		{
			var sut = new Settings();
			var res = sut.GetMenuFromXml(MenuXml);

			Assert.AreEqual(2, res.MenuItems.Count);
		}

		[TestMethod]
		public void GetFromXml_given_CoreElement_should_ReturnPopulatedCoreSettings()
		{
			var sut = new Settings();
			var xml = XDocument.Parse(Xml).Element("St4mpede").Element("Core");
			var res = sut.GetCoreFromXml(xml);

			Assert.IsNotNull(res);
			Assert.AreEqual(@"C:\DATA\PROJEKT\St4mpede\St4mpede\St4mpede\St4mpede", res.RootFolder);
		}

		internal class RootClass
		{
			public int Customer { get; set; }
			public string CustomerName { get; set; }
			public string ProjectName { get; set; }
			public int CustomerId { get; set; }
		}

		[TestMethod]
		public void GetFromXml_given_SimpleElements_should_Populate()
		{
			//	#	Arrange.
			const string XmlString = @"
<Root>
	<Customer>42</Customer>
	<Project>Main</Project>
</Root>";
			var mappingConfigStore = new ConfigurationStore(new TypeMapFactory(), MapperRegistry.Mappers);
			mappingConfigStore.CreateMap<XElement, RootClass>()
				.ForMember(dest => dest.Customer,
					opt => opt.ResolveUsing<XElementResolver<string>>()
						.ConstructedBy(() => new XElementResolver<string>("Customer")))
				.ForMember(dest => dest.ProjectName,
					opt => opt.ResolveUsing<XElementResolver<string>>()
						.ConstructedBy(() => new XElementResolver<string>("Project")));

			var sut = new Settings(new MappingEngine(mappingConfigStore));
			var xml = XDocument.Parse(XmlString);

			//	#	Act.
			var res = sut.GetFromXml<RootClass>(xml.Element("Root"));

			//	#	Arrange.
			Assert.AreEqual(42,res.Customer);
			Assert.AreEqual("Main", res.ProjectName);
		}

		[TestMethod]
		public void GetFromXml_given_Attributes_should_Populate()
		{
			//	#	Arrange.
			const string XmlString = @"
<Root>
	<Customer name='Bengt'>42</Customer>
	<Project customerId='42'>Main</Project>
</Root>";

			var mappingConfigStore = new ConfigurationStore(new TypeMapFactory(), MapperRegistry.Mappers);
			mappingConfigStore.CreateMap<XElement, RootClass>()

				.ForMember(dest => dest.Customer,
					opt => opt.ResolveUsing<XElementResolver<string>>()
						.ConstructedBy(() => new XElementResolver<string>("Customer")))

				.ForMember(dest => dest.CustomerName,
					opt => opt.ResolveUsing<XAttributeResolver<string>>()
						.FromMember(src => src.Element("Customer"))
						.ConstructedBy(() => new XAttributeResolver<string>("name")))

				.ForMember(dest => dest.ProjectName,
					opt => opt.ResolveUsing<XElementResolver<string>>()
						.ConstructedBy(() => new XElementResolver<string>("Project")))

				.ForMember(dest => dest.CustomerId,
					opt => opt.ResolveUsing<XAttributeResolver<int>>()
						.FromMember(src => src.Element("Project"))
						.ConstructedBy(() => new XAttributeResolver<int>("customerId")));

			var sut = new Settings(new MappingEngine(mappingConfigStore));
			var xml = XDocument.Parse(XmlString);
			
			//	#	Act.
			var res = sut.GetFromXml<RootClass>(xml.Element("Root"));

			//	#	Arrange.
			Assert.AreEqual(42, res.Customer);
			Assert.AreEqual("Bengt", res.CustomerName);
			Assert.AreEqual("Main", res.ProjectName);
			Assert.AreEqual(42, res.CustomerId);
		}

		#region GetFromXml_given_SubElements_should_Populate test.

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
						
				.ForMember(dest=>dest.Projects, 
					opt=>opt.MapFrom(src=>mapItems(src,"Projects", "Project")));

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

		#endregion
	}
}

