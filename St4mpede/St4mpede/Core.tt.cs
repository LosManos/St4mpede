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
#endif
	//#	Regular ol' C# classes and code...
	internal class Core
	{
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

	}
#if NOT_IN_T4
} //end the namespace
#endif
//#>