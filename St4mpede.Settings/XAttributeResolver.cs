using System;
using System.Xml.Linq;
using AutoMapper;

namespace St4mpede.Settings
{
	public class XAttributeResolver<T> : ValueResolver<XElement, T>
	{
		public string Name { get; set; }

		public XAttributeResolver(string attributeName)
		{
			Name = attributeName;
		}

		protected override T ResolveCore(XElement source)
		{
			if (source == null)
				return default(T);
			var attribute = source.Attribute(Name);
			if (attribute == null || String.IsNullOrEmpty(attribute.Value))
				return default(T);

			return (T)Convert.ChangeType(attribute.Value, typeof(T));
		}
	}

	public class XElementResolver<T> : ValueResolver<XElement, T>
	{
		protected override T ResolveCore(XElement source)
		{
			if (source == null || string.IsNullOrEmpty(source.Value))
				return default(T);
			return (T)Convert.ChangeType(source.Value, typeof(T));
		}
	}
}