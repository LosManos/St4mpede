using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AutoMapper;

namespace St4mpede.Settings
{
	public static class MapInitializer
	{
		private static Func<XElement, string, string, List<XElement>> _mapItems =
			(src, collectionName, elementName) =>
				(src.Element(collectionName) ?? new XElement(collectionName)).Elements(elementName).ToList();

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
	}
}