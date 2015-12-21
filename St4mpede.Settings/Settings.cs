﻿using System.Linq;
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
		static Settings()
		{
			//MapInitializer.CreateMenuMap();
			MapInitializer.CreateSettingsMap();
			//Mapper.Initialize(cfg => cfg.CreateMap<Entity, Dto>());
		}

		public Menu GetMenuFromXml(string xmlString)
		{
			var xml = XDocument.Parse(xmlString);
			var menu  = Mapper.Map<XElement,Menu>
			(xml.Element("Menu"));
			return menu;
		}

		public Core GetFromXml(XElement xml)
		{
			var settings = Mapper.Map<XElement, Core>(xml.Elements().Single());
			return settings;
		}

		public T GetFFromElement<T>(string xPath) where T : new()
		{
			var entity = new Entity {CustomerID = 42};
			var dto = Mapper.Map<Dto>(entity);
			return (T) (object) dto;
		}
	}
}