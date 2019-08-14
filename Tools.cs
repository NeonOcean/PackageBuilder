using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PackageBuilder {
	public static class Tools {
		public static object ReadXML<T> (string filePath) {
			using(XmlTextReader reader = new XmlTextReader(filePath)) {
				return new XmlSerializer(typeof(T)).Deserialize(reader);
			}
		}

		public static void WriteXML (string filePath, object target) {
			Directory.CreateDirectory(Path.GetDirectoryName(filePath));

			using(XmlWriter writer = XmlWriter.Create(filePath, new XmlWriterSettings() { Indent = true, IndentChars = "\t" })) {
				new XmlSerializer(target.GetType()).Serialize(writer, target);
			}
		}
	}
}
