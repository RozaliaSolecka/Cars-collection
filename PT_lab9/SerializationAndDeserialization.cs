using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace PT_lab9
{
    class SerializationAndDeserialization
    {
        public static void Serialization(List<Car> myCars)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Car>), new XmlRootAttribute("cars"));
            using (TextWriter textWriter = new StreamWriter("CarsCollection.xml"))
            {
                serializer.Serialize(textWriter, myCars);
            }
        }

        public static List<Car> Deserialization()
        {
            List<Car> list = new List<Car>();
            XmlSerializer serializer = new XmlSerializer(typeof(List<Car>), new XmlRootAttribute("cars"));
            using (Stream reader = new FileStream("CarsCollection.xml", FileMode.Open))
            {
                list = (List<Car>)serializer.Deserialize(reader);
            }
            return list;
        }
    }
}
