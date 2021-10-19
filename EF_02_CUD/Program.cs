using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EF_02_CUD;

namespace EF_02_LINQ2Entities
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("\n***Beispiele zu LINQ2Entities***");

                Console.WriteLine("1 - Alle Produkte auflisten");
                Console.WriteLine("2 - Produkte in Flaschen die mehr als 10$ kosten");
                Console.WriteLine("3 - Alle Bestellungen von Kunden aus Madrid");
                Console.WriteLine("4 - Kunden nach Stadt gruppiert");
                Console.WriteLine("5 - Bestellungen je Stadt mit Anzahl Bestellungen je Stadt");
                Console.WriteLine("6 - Produkte mit bestimmten ProductIDs (mit Contains)");
                Console.WriteLine("7 - Produkte japanischer Lieferanten, von denen weniger als 30 Einheiten lagernd sind");
                Console.WriteLine();
                Console.Write("      Auswahl:");
                string auswahl = Console.ReadLine();

                switch (auswahl)
                {
                    case "1": Option1(); break;
                    case "2": Option2(); break;
                    case "3": Option3(); break;
                    case "4": Option4(); break;
                    case "5": Option5(); break;
                    case "6": Option6(); break;
                    case "7": Option7(); break;
                    default:
                        return;
                }
                Console.ReadKey();
            }
        }

        private static void Option7()
        // Produkte japanischer Lieferanten, von denen weniger als 30 Einheiten lagernd sind
        {
            using (var context = new NorthwindEntities())
            {
                var query = from sup in context.Suppliers
                            from prod in context.Products
                            where sup.SupplierID == prod.SupplierID
                            && sup.Country == "Japan"
                            && prod.UnitsInStock < 30
                            select new
                            {
                                prod.ProductID,
                                prod.ProductName,
                                prod.UnitsInStock,
                                sup.CompanyName,
                            };

                Console.WriteLine("\nProdukte japanischer Lieferanten, von denen weniger als 30 Einheiten lagernd sind:");
                foreach (var item in query)
                {
                    Console.WriteLine($"{item.ProductID,3} {item.ProductName,-30}" +
                        $" {item.CompanyName,-20} {item.UnitsInStock,4}");
                }

                Console.WriteLine($"\n{query.Count()} von insgesamt {context.Products.Count()} Produkten gefunden.");
            }
        }

        private static void Option6()
        // Produkte mit bestimmten ProductIDs (analaog zu SQL IN Klausel)
        {
            int[] productIds = { 22, 29, 69 };

            using (var context = new NorthwindEntities())
            {
                var query = context.Products
                    .Where(p => productIds.Contains(p.ProductID));

                Console.WriteLine($"\nProdukte mit den IDs {string.Join(", ", productIds)}:");
                foreach (var p in query)
                {
                    Console.WriteLine($"{p.ProductID} {p.ProductName}");
                }
            }

        }

        private static void Option5()
        // Bestellungen (ID und Datum) je Stadt mit Anzahl der Bestellungen je Stadt.
        // Am Ende der Städte noch die Anzahl der Städte, für die es Bestellungen gibt, ausgeben.
        {
            using (var context = new NorthwindEntities())
            {
                var queryOrder = from order in context.Orders
                                 from cust in context.Customers
                                 where order.CustomerID == cust.CustomerID
                                 select new { order.OrderID, order.OrderDate, cust.City };

                var cityGroupQuery = from order in queryOrder
                                     group order by order.City into gr
                                     select new
                                     {
                                         City = gr.Key,
                                         Group = gr.ToList()
                                     };

                Console.WriteLine("\nBestellungen je Stadt mit Anzahl Bestellungen je Stadt:");
                foreach (var g in cityGroupQuery)
                {
                    Console.WriteLine($"\n{g.City}, {g.Group.Count} Bestellung(en)");   // Gruppen-Header
                    foreach (var o in g.Group)                                          // Schleife über alle Gruppenmember (Bestellungen)
                    {
                        Console.WriteLine($"    {o.OrderID} {o.OrderDate:d}");
                    }
                }
                Console.WriteLine($"\nAnzahl Städte mit Bestellungen: {cityGroupQuery.Count()}");
            }
        }

        private static void Option4()
        // Kunden (Firmennamen) nach Stadt gruppiert, mit der Anzahl der Kunden je Stadt
        // und der Anzahl der Städte (=Anzahl der Kundengruppen)
        {
            using (var context = new NorthwindEntities())
            {
                var query = from cust in context.Customers
                            group cust by cust.City into gr
                            select new
                            {
                                City = gr.Key,
                                Group = gr.ToList()     // IGrouping<string, Customer> -> ListCustomer>
                                                        // Mit der Liste kann man leichter weiterarbeiten
                            };

                Console.WriteLine("\nKunden (Firmennamen) nach Stadt gruppiert:");
                foreach (var gr in query)
                {
                    Console.WriteLine($"\n{gr.City}({gr.Group.Count}:");      // Gruppen-Header
                    foreach (var c in gr.Group)                         // Schleife über alle Gruppenmember (Kunden)
                    {
                        Console.WriteLine(c.CompanyName);
                    }
                }
                Console.WriteLine($"\n{query.Count()} (Kundengruppen)");
            }
        }


        // Hier werden, mittels join, zwei Tabellen abgefragt
        private static void Option3()
        // Alle Bestellungen von Kunden aus Madrid
        {
            using (var context = new NorthwindEntities())
            {
                var query = from order in context.Orders
                            join cust in context.Customers
                            on order.CustomerID equals cust.CustomerID
                            where cust.City == "Madrid"
                            select new
                            {
                                order.OrderID,
                                order.OrderDate,
                                cust.City,
                                Company = cust.CompanyName
                            };

                Console.WriteLine("\nAlle Bestellungen von Kunden aus Madrid:");
                foreach (var item in query)
                {
                    Console.WriteLine($"{item.OrderID} {item.OrderDate} - {item.City} - {item.Company}");
                }
            }
        }

        public static void Option2()
        // Produkte in Flaschen die mehr als 10$ kosten
        {
            using (var context = new NorthwindEntities())
            {
                var query = context.Products
                    .Where(p => p.QuantityPerUnit.Contains("bottle") && p.UnitPrice > 10)
                    .OrderByDescending(p => p.UnitPrice);

                Console.WriteLine("\nProdukte in Flaschen die mehr als 10$ kosten:");
                foreach (var item in query)
                {
                    Console.WriteLine($"{item.ProductID,3} {item.ProductName,-35} " +
                        $"{item.QuantityPerUnit,-20} {item.UnitPrice,8:n}");
                }
            }
        }

        public static void Option1()
        // Alle Produkte mit Namen und ID auflisten
        {
            using (NorthwindEntities context = new NorthwindEntities())
            {
                // LINQ to Entities Query, um alle Produkte abzufragen (Methodensyntax)
                var query = context.Products
                    //.OrderBy(p => p.ProductID)
                    .Select(x => new { x.ProductID, x.ProductName }) // ID und Name selektieren
                    .OrderBy(p => p.ProductID);

                Console.WriteLine("\nListe aller Produkte:");
                foreach (var item in query)
                {
                    Console.WriteLine($"{item.ProductID,3} {item.ProductName}");
                }
            }
        }
    }
}
