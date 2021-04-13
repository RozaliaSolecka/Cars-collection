using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Linq;

namespace PT_lab9
{
    class Program
    {
        private static List<Car> myCars = new List<Car>()
        {
            new Car("E250", new Engine(1.8, 204, "CGI"), 2009),
            new Car("E350", new Engine(3.5, 292, "CGI"), 2009),
            new Car("A6", new Engine(2.5, 187, "FSI"), 2012),
            new Car("A6", new Engine(2.8, 220, "FSI"), 2012),
            new Car("A6", new Engine(3.0, 295, "TFSI"), 2012),
            new Car("A6", new Engine(2.0, 175, "TDI"), 2011),
            new Car("A6", new Engine(3.0, 309, "TDI"), 2011),
            new Car("S6", new Engine(4.0, 414, "TFSI"), 2012),
            new Car("S8", new Engine(4.0, 513, "TFSI"), 2012)
        };

        static void Main()
        {
            LinqQuery();
            SerializationDeserialization();
            XPath();
            createXmlFromLinq(myCars);
            htmlTable();
            modification();
        }
        //ZAD1
        private static void LinqQuery()
        {
            var myCarsProjection = from c in myCars
                                   where c.model.Equals("A6")
                                   select new
                                   {
                                       engineType = String.Compare(c.motor.model, "TDI") == 0 ? "diesel" : "petrol",
                                       hppl = c.motor.horsePower / c.motor.displacement
                                   };
            foreach(var car in myCarsProjection)
            {
                Console.WriteLine($"enginetype = {car.engineType}, hppl =  {car.hppl}");
            }
            Console.WriteLine();
            var groupingEngineType = from p in myCarsProjection
                                     group p by p.engineType into grouped
                                     select new
                                     {
                                         engine = grouped.First().engineType,
                                         average = grouped.Average(a=>a.hppl).ToString()
                                     };

            foreach(var engine in groupingEngineType)
            {
                Console.WriteLine($"{engine.engine}: {engine.average}");
            }
            Console.WriteLine();
        }

      
        //ZAD2
        private static void SerializationDeserialization()
        {
          
            SerializationAndDeserialization.Serialization(myCars);

            var deserialized = SerializationAndDeserialization.Deserialization();

            Console.Write($"Models after deserialization  ");

            foreach (var element in deserialized)
            {
                Console.Write($" {element.model} "); 
            }

        }
        //ZAD3
        private static void XPath()
        {
            XElement rootNode = XElement.Load("CarsCollection.xml");
            double avgHP = (double)rootNode.XPathEvaluate("sum(//car/engine[@model!=\"TDI\"]/horsePower) div count(//car/engine[@model!=\"TDI\"]/horsePower)");
            Console.WriteLine();
            Console.WriteLine($"Average horsepower (engine different than TDI) {avgHP}");

            IEnumerable<XElement> models = rootNode.XPathSelectElements("car[not(model = following::car/model)]/model"); /////////////

            Console.WriteLine();
            Console.Write($"Models: "); 
            Console.WriteLine(); 
            foreach (var el in models)
            {
                Console.WriteLine($"{(string)el}");
            }      
        }
        //ZAD4
        private static void createXmlFromLinq(List<Car> myCars)
        {
            IEnumerable<XElement> nodes = from car in myCars
                                          select new XElement("car",
                                            new XElement("model", car.model),
                                            new XElement("year", car.year),
                                            new XElement("engine",
                                                new XAttribute("model", car.motor.model),
                                                new XElement("displacement", car.motor.displacement),
                                                new XElement("horsePower", car.motor.horsePower)
                                            )
                                          );

            XElement rootNode = new XElement("cars", nodes); //create a root node to contain the query results
            rootNode.Save("CarsFromLinq.xml");
        }

        //ZAD5

        private static void htmlTable()
        {
            XDocument template = XDocument.Load("template.html");
            var body = template.Root.LastNode as XElement;
            IEnumerable<XElement> rows = from car in myCars
                                          select new XElement("tr", new XAttribute("style", "border: 2px double black"),
                                            new XElement("td", new XAttribute("style", "border: 2px double black"), car.model),
                                            new XElement("td", new XAttribute("style", "border: 2px double black"), car.motor.model),
                                            new XElement("td", new XAttribute("style", "border: 2px double black"), car.motor.displacement),
                                            new XElement("td", new XAttribute("style", "border: 2px double black"), car.motor.horsePower),
                                            new XElement("td", new XAttribute("style", "border: 2px double black"), car.year)
                                          );

            body.Add(new XElement("table", new XAttribute("style", "border: 2px double black"), rows));
            template.Save("htmlTable.html");
        }

        //ZAD6
        private static void modification()
        {
            XElement carsCollection = XElement.Load("CarsCollection.xml");
            foreach (var car in carsCollection.Elements())
            {
                foreach (var el in car.Elements())
                {
                    if (el.Name == "engine")
                    {
                        foreach (var el2 in el.Elements())
                        {
                            if (el2.Name == "horsePower")
                            {
                                el2.Name = "hp";
                            }
                        }
                    }
                    else if (el.Name == "model")
                    {
                        var year = car.Element("year");
                        XAttribute attribute = new XAttribute("year", year.Value);
                        el.Add(attribute);
                        year.Remove();
                    }
                }
            }
            carsCollection.Save("CarsCollectionModified.xml");
        }

    }
}
