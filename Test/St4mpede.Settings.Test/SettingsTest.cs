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
		<ConnectionString>Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename=C:\DATA\PROJEKT\St4mpede\St4mpede\St4mpede.Test\Database\St4mpede.mdf;Integrated Security = True; Connect Timeout = 30 </ ConnectionString >
		<DatabaseName>C:\DATA\PROJEKT\ST4MPEDE\St4mpede\ST4MPEDE.TEST\DATABASE\ST4MPEDE.MDF</DatabaseName>
		<DatabaseIndex/>
		<ExcludedTablesRegex>__RefactorLog</ExcludedTablesRegex>
		<DatabaseXmlFile>St4mpede.RdbSchema.xml</DatabaseXmlFile>
		<ProjectPath>RdbSchema</ProjectPath>
	</RdbSchema>
	<Poco>
		<OutputFolder>..\..\TheDAL\Poco</OutputFolder>
		<NameSpace Name = 'TheDAL.Poco' >
            < Comments >
                < Comment > R	eSharper disable BuiltInTypeReferenceStyle</Comment>
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
			<Equals Regex = '.*' > True </ Equals >
        </ Methods >
	</ Poco >
</ St4mpede >";

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


		[TestMethod]
		public void TestMethod1()
        {
	        var sut = new Settings();
			var res = sut.GetFFromElement<Dto> (null);
			Assert.AreEqual(42, res.CustomerID);
        }

		[TestMethod]
		public void GetFromXmlTest()
		{
			var sut = new Settings();
			var res = sut.GetFromXml(MenuXml);

			Assert.AreEqual(2, res.MenuItems.Count);
		}
	}
}
