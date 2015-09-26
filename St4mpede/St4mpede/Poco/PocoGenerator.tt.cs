//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
using System.Xml.Linq;
using System.Xml.Serialization;

namespace St4mpede.St4mpede.Poco
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	//	using System.Xml.Serialization;
#endif
	//#	Regular ol' C# classes and code...

	internal class PocoGenerator
	{
		public DatabaseData Database { get; set; }

		internal void Generate()
		{

		}

		internal void Output()
		{

		}

		internal void ReadXml()
		{
			var xmlPathFile = @"C:\DATA\PROJEKT\St4mpede\St4mpede\St4mpede\St4mpede\St4mpede.xml";
			//         AddLog(string.Empty);
			//AddLog("Writing database xml {0}.", xmlPathFile);

			var doc = XDocument.Load(xmlPathFile);

			var database = Deserialise<DatabaseData>(doc);

			this.Database = database;
			//AddLog("Created xml:");
			//AddLog(xml);

			//xml.Save(Path.Combine(_settings.ConfigPath, St4mpedePath, _settings.OutputXmlFilename));
		}

		internal string ToInfo()
		{
			return "TBA";
		}

		//	TODO:Put, with Parser ditto, in common or core lib.
		private static T Deserialise<T>(XDocument doc)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

			using (var reader = doc.Root.CreateReader())
			{
				return (T)xmlSerializer.Deserialize(reader);
			}
		}

	}

#if NOT_IN_T4
} //end the namespace
#endif
//#>
