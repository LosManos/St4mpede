using System.Linq;
using System.Xml.Linq;
using AutoMapper;

namespace St4mpede.Settings
{
	public class Entity
	{
		public int CustomerID { get; set; }
	}

	public class MenuItem
	{
		public string Name { get; set; }
		public string Action { get; set; }
		public string Parameters { get; set; }
	}

	public class Dto
	{
		public int CustomerID { get; set; }
	}

	public class Settings
	{
		private readonly MappingEngine _mappingEngine;

		static Settings()
		{
			//MapInitializer.CreateMenuMap();
			MapInitializer.CreateSettingsMap();
			//Mapper.Initialize(cfg => cfg.CreateMap<Entity, Dto>());
		}

		public Settings()
		{ }

		public Settings(MappingEngine mappingEngine )
		{
			//	http://www.damirscorner.com/blog/posts/20141222-UsingNonStaticAutoMapperConfiguration.html
			_mappingEngine = mappingEngine;
		}

		public Menu GetMenuFromXml(string xmlString)
		{
			var xml = XDocument.Parse(xmlString);
			var menu  = Mapper.Map<XElement,Menu>
			(xml.Element("Menu"));
			return menu;
		}

		public Core GetCoreFromXml(XElement xml)
		{
			var settings = Mapper.Map<XElement, Core>(xml.Elements().Single());
			return settings;
		}

		public T GetFromXml<T>(XElement xmlElement)
		{
			var res = ((null == _mappingEngine)
				? Mapper.Engine
				: _mappingEngine)
				.Map<XElement, T>(xmlElement);

			return res;
		} 
	}
}