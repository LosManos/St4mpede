using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AutoMapper;
using AutoMapper.Mappers;

namespace St4mpede.Settings
{
	/// <summary>This class initialises the automapping.
	/// Read more about how it works here:
	/// http://stackoverflow.com/questions/6543659/automapper-to-create-object-from-xml
	/// http://www.codeproject.com/Articles/706992/Using-AutoMapper-with-Complex-XML-Data
	/// </summary>
	public static class MapInitializer
	{
		private static Func<XElement, string, string, List<XElement>> _mapItems =
			(src, collectionName, elementName) =>
				(src.Element(collectionName) ?? new XElement(collectionName)).Elements(elementName).ToList();

		[Obsolete("For getting stuff in the air only.", true)]
		public static void CreateMenuMap()
		{
			//MenuItem map
			Mapper.CreateMap<XElement, MenuItem>()
				.ForMember(dest => dest.Name,
					opt => opt.ResolveUsing<XAttributeResolver<string>>()
						.ConstructedBy(() => new XAttributeResolver<string>("name")))
				.ForMember(dest => dest.Action,
					opt => opt.ResolveUsing<XAttributeResolver<string>>()
						.ConstructedBy(() => new XAttributeResolver<string>("action")))
				.ForMember(dest => dest.Parameters,
					opt => opt.ResolveUsing<XAttributeResolver<string>>()
						.ConstructedBy(() => new XAttributeResolver<string>("parameters")));
			// Menu map
			Mapper.CreateMap<XElement, Menu>()
				.ForMember(dest => dest.Name,
					opt =>
						opt.ResolveUsing<XAttributeResolver<string>>()
							.ConstructedBy(() => new XAttributeResolver<string>("name")))
				.ForMember(dest => dest.MenuItems,
					opt => opt.MapFrom(src => _mapItems(src, "MenuItems", "MenuItem")))
				.ForMember(dest => dest.SubMenus,
					opt => opt.MapFrom(src => _mapItems(src, "SubMenus", "Menu")));
		}

		public static void CreateSettingsMap()
		{
			Mapper.CreateMap<XElement, Core>()
				.ForMember(dest => dest.RootFolder,
					opt => opt.ResolveUsing<XElementResolver<string>>()
						.ConstructedBy(() => new XElementResolver<string>()));
			//var config = new ConfigurationStore(new TypeMapFactory(), MapperRegistry.Mappers);
			//config.CreateMap<XElement, Core>()
			//	.ForMember(dest => dest.RootFolder,
			//		opt => opt.ResolveUsing<XElementResolver<string>>()
			//			.ConstructedBy(() => new XElementResolver<string>()));

			//var mappingEngine = new MappingEngine();
			//Mapper.Configuration.ad
		}
	}
}