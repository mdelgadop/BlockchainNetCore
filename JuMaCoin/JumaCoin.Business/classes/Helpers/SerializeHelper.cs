using System.Xml.Serialization;

namespace JumaCoin.Business.classes.Helpers
{
    public class SerializeHelper<T>
        where T: class
    {
        public string Serialize(T objectInstanciated)
        {
            string objectString = string.Empty;

            using(var stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(objectInstanciated.GetType());
                serializer.Serialize(stringwriter, objectInstanciated);
                objectString = stringwriter.ToString();
            }

            return objectString.Trim();
        }

        public virtual T Deserialize(string objectString)
        {
            T objectInstanciated = null;
            using(var stringReader = new System.IO.StringReader(objectString.Trim()))
            {
                var serializer = new XmlSerializer(typeof(T));
                objectInstanciated = serializer.Deserialize(stringReader) as T;
            }

            return objectInstanciated;
        }

    }
}