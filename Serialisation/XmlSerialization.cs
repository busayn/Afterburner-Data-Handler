using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AfterburnerDataHandler.Serialisation
{
    public class XmlSerialization
    {
        /// <summary>
        /// Serializes an object to a string.
        /// </summary>
        public static string ToXMLString<T>(T obj)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using (StringWriter stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, obj);
                return stringWriter.ToString();
            }
        }

        /// <summary>
        /// Deserializes an object from a string.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static T FromXMLString<T>(string xmlString)
        {
            if (string.IsNullOrEmpty(xmlString))
                throw new ArgumentException();

            T newObj;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using (StringReader stringReader = new StringReader(xmlString))
            {
                newObj = (T)xmlSerializer.Deserialize(stringReader);
            }

            return newObj;
        }
    }
}
