//<#+
/*That line above is very carefully constructed to be awesome and make it so this works!*/
#if NOT_IN_T4
//Apparently T4 places classes into another class, making namespaces impossible
namespace St4mpede
{
	//	Note that when adding namespaces here we also have to add the namespaces to the TT file  import namespace=...
	//	The same way any any new assembly reference must be added to the TT file assembly. name=...
	using System.Xml.Linq;
	using System.Xml.Serialization;
	using System;
	using System.IO;
	using System.Linq;
	using System.Collections.Generic;
#endif
	internal interface ICore
	{
		//XDocument LoadXDocumentFromPathfile(string pathfile);

		string AddSuffix(string name);
		T ReadFromXmlPathfile<T>(string pathfile);
		void WriteOutput(XDocument doc, string pathFilename);
		void WriteOutput(IList<string> rows, string pathFilename);
	}

	//#	Regular ol' C# classes and code...
	internal class Core : ICore
	{
		private const string ElementRootFolder = "RootFolder";

		internal static readonly string DefaultConfigFilename = "St4mpede.config.xml";
		internal static readonly string DefaultSt4mpedePath = "St4mpede";

		internal static XDocument ReadConfig( string configPath, string configFilename)
		{
			if (null == configPath) { throw new ArgumentNullException("configPath"); }
			if (null == configFilename) { throw new ArgumentNullException("configFilename"); }

			var configPathfilename = Path.Combine(configPath, configFilename);
			var doc = XDocument.Load(new FileStream(configPathfilename, FileMode.Open));
			return doc;
		}

		internal static CoreSettings Init( XDocument doc)
		{
			return new CoreSettings(
				doc.Descendants().Where(e =>
			   e.Name == ElementRootFolder).Single().Value);
		}

		//public XDocument LoadXDocumentFromPathfile(string pathfile)
		//{
		//	return XDocument.Load(pathfile);
		//}

		public string AddSuffix(string name)
		{
			return name + ".cs";
		}

		public T ReadFromXmlPathfile<T>(string pathfile)
		{
			var doc = XDocument.Load(pathfile);

			var res = Core.Deserialise<T>(doc);

			return res;
		}

		public void WriteOutput(XDocument doc, string pathFilename)
		{
			doc.Save(pathFilename, SaveOptions.None);
		}

		public void WriteOutput( IList<string> rows, string pathFilename)
		{
			using (var sw = File.CreateText(pathFilename))
			{
				foreach( var row in rows)
				{
					sw.WriteLine(row);
				}
			}
		}

		#region Serialise/deserialise methods.

		/// <summary>This method deserialises an XDocument to an object.
		/// See complementary method <see cref="Serialise{T}(T)"/>.
		/// <para>
		/// Copied with pride from:
		///	http://stackoverflow.com/questions/1295046/use-xdocument-as-the-source-for-xmlserializer-deserialize
		/// </para>
		/// <para>
		/// If there ever will be a Util or Common library, maybe we should move it there.
		/// </para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="doc"></param>
		/// <returns></returns>
		internal static T Deserialise<T>(XDocument doc)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

			using (var reader = doc.Root.CreateReader())
			{
				return (T)xmlSerializer.Deserialize(reader);
			}
		}

		/// <summary>This method serialises an object into XDocument.
		/// See complementary method <see cref="Deserialise{T}(XDocument)"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		internal static XDocument Serialise<T>(T value)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

			XDocument doc = new XDocument();
			using (var writer = doc.CreateWriter())
			{
				xmlSerializer.Serialize(writer, value);
			}

			return doc;
		}

		#endregion
	}
#if NOT_IN_T4
} //end the namespace
#endif
//#>