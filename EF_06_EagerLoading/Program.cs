/* Im Entity Framework kann man mittels Eager Loading bereits beim Erstellen einer
 * Abfrage angeben, welche Detaildatensätze gleich mitgeladen werden sollen. In der
 * Abfrage wird dazu die Include-Methode verwendet. Dabei verhindert man wiederholte
 * Anfragen an den Server wie das beim Lazy Loading der Fall ist.
 * 
 * Mit der Include-Methode können die Detaildatensätze mehrerer Ebenen von Navigations-
 * eigenschaften auf einmal gelesen werden. Das Entity Framework generiert ein SQL-
 * Statement mit den zugrundeliegenden Tabellen und den entsprechenden Joins.
 * 
 * Standardmäßig kann die Include-Methode nur mit einem String-Parameter verwendet
 * werden, der den Namen einer Navigationseigenschaft angibt. Um statt mit dem String-
 * Parameter mit einem Lambda-Ausdruck arbeiten zu können, muss der Namespace
 * 
 *    System.Data.Entity
 *    
 * eingebunden werden. Dieser enthält die Klasse QueryableExtensions, die eine Reihe
 * von Erweiterungsmethoden für Entity Framework LINQ Abfragen zur Verfügung stellt,
 * siehe MSDN QueryableExtensions Class. Die Include-Methode mit Lambda-Ausdruck als
 * Parameter hat den Vorteil, dass man IntelliSense-Unterstützung hat, was beim String-
 * Parameter nicht der Fall ist.
 * 
 * Bei Referenzdetaildaten (Navigationseigenschaft zu einer einzelnen anderen Entität)
 * können die Eigenschaften der Detaildaten überall in der Query ohne Einschränkung
 * verwendet werden.
 * 
 * Bei Detaildatenlisten (Navigationseigenschaft zu einer Collection anderer Entitäten)
 * kann man die Eigenschaften der Detaildaten zwar in der Query verwenden, allerdings
 * nur mit Methoden, die sich auf eine Collection beziehen, wie z.B. die Aggregations-
 * Operatoren (Count, Min, Max, ...) oder die Quantifizierungs-Operatoren (Any, All,
 * Contains). Die Detaildatenlisten können aber nicht gefiltert oder sortiert werden.
 * Das ist nur mittels Projektion oder Umdrehen der Query möglich.
 */

using System;
using System.Linq;
using System.Text;
using System.Data.Entity;     // Namespace der Klasse QueryableExtensions einbinden

namespace EF_06_EagerLoading
{
    class Program
    {
        static void Main(string[] args)
        {
            // -> Tabellen "Employees", "EmployeeTerritories", "Territories" und "Region"
            //    importieren
            // -> Die Verbindungstabelle "EmployeeTerritories" wird nicht in das EDM
            //    aufgenommen. Stattdessen gibt es die Navigationseigenschaften
            //    "Employee.Territories" und "Territory.Employees".

            Console.WriteLine("\nEager Loading - Include mit Lambda-Ausdruck");
            Console.WriteLine("-------------------------------------------");

            using (NorthwindEntities ctx = new NorthwindEntities())
            {
                // Ohne "using System.Data.Entity;" kann als Parameter für die Include-Methode
                // nur der Name einer Navigationseigenschaft (string), aber kein Lambda-Ausdruck
                // verwendet werden.
                // -> Include() kann auch nicht hinter Where() verwendet werden!

                var query = ctx.Territories.Include(t => t.Region);

                Console.WriteLine("\n" + query.ToString());  // SQL-Statement ausgeben

                Console.WriteLine("\nTerritorien mit zugehöriger Region:\n");
                foreach (Territory t in query)
                    Console.WriteLine($"{t.TerritoryID} {t.TerritoryDescription} {t.RegionID} {t.Region.RegionDescription}");
            }


            Console.WriteLine("\nLazy Loading - 2 Ebenen");
            Console.WriteLine("-----------------------");

            using (NorthwindEntities ctx = new NorthwindEntities())
            {
                var query = ctx.Employees;

                Console.WriteLine("\nAngestellte mit zugehörigen Territorien und Regionen:\n");
                foreach (Employee e in query)
                {
                    Console.WriteLine($"{e.EmployeeID} {e.FirstName + " " + e.LastName}");

                    // Die Detaildaten werden beim Zugriff auf die Navigationseigenschaften
                    // mit separaten SQL Statements nachgeladen.
                    foreach (Territory t in e.Territories)
                        Console.WriteLine($"   {t.TerritoryDescription} {t.Region.RegionDescription}");
                }
            }

            Console.WriteLine("\nEager Loading - 2 Ebenen");
            Console.WriteLine("------------------------");

            using (NorthwindEntities ctx = new NorthwindEntities())
            {
                // Die Detaildatensätze der Navigationseigenschaften "Territories" und "Region"
                // gleich mit den "Employees"-Daten laden.
                var query = ctx.Employees
                   .Include("Territories.Region")   // Ebenen voneinander durch Punkte trennen!
                                                    // ODER Include() mit Lambda-Ausdruck
                   .Include(e => e.Territories.Select(r => r.Region));

                // Allgemein gilt für Include() mit Lambda-Ausdruck als Parameter:
                // Nach Referenz-Navigationseigenschaften kommt man mit . zur nächsten Ebene,
                // nach Collection-Navigationseigenschaften muss man .Select() verwenden.
                // z.B.: ctx.Products.Include(p => p.Order_Details.Select(od => od.Order.Employee.Territories.Select(t => t.Region)));

                Console.WriteLine("\n" + query.ToString());

                // -> Ausgabe wie im vorigen Beispiel (Lazy Loading)
                Console.WriteLine("\nAngestellte mit zugehörigen Territorien und Regionen:\n");
                foreach (Employee e in query)
                {
                    Console.WriteLine($"{e.EmployeeID} {e.FirstName + " " + e.LastName}");

                    foreach (Territory t in e.Territories)
                        Console.WriteLine($"   {t.TerritoryDescription} {t.Region.RegionDescription}");
                }
            }


            Console.WriteLine("\nEager Loading mit Bedingung");
            Console.WriteLine("Navigationseigenschaft zu einer einzelnen anderen Entität");
            Console.WriteLine("---------------------------------------------------------");

            using (NorthwindEntities ctx = new NorthwindEntities())
            {
                var query = ctx.Territories
                   .Include(t => t.Region)
                   .Where(t => t.Region.RegionDescription == "Northern");

                Console.WriteLine("\n" + query.ToString());

                Console.WriteLine("\nTerritorien mit Region \"Northern\":\n");
                foreach (Territory t in query)
                {
                    Console.WriteLine($"{t.TerritoryID} {t.TerritoryDescription} {t.RegionID} {t.Region.RegionDescription}");
                }
            }

            Console.WriteLine("\nEager Loading mit Bedingung");
            Console.WriteLine("Navigationseigenschaft zu einer Collection anderer Entitäten");
            Console.WriteLine("------------------------------------------------------------");

            using (NorthwindEntities ctx = new NorthwindEntities())
            {
                var query = ctx.Regions
                   .Include(r => r.Territories)
                   .Where(r => r.Territories.Count() < 12);
                // Bedingung, die sich auf die gesamte Collection bezieht (-> Count)

                Console.WriteLine("\n" + query.ToString());

                Console.WriteLine("\nRegionen mit weniger als 12 Territorien:\n");
                foreach (Region r in query)
                {
                    Console.WriteLine($"Region {r.RegionID} {r.RegionDescription.Trim()} ({r.Territories.Count()} Territorien)");
                    foreach (Territory t in r.Territories)
                        Console.WriteLine($"   {t.TerritoryID} {t.TerritoryDescription}");
                }
            }

            using (NorthwindEntities ctx = new NorthwindEntities())
            {
                var query = ctx.Regions
                   .Include(r => r.Territories)
                   .Where(r => r.Territories.Any(t => t.TerritoryDescription.StartsWith("A")));
                // Bedingung mit einer Eigenschaft der Detaildatenliste. Die Eigenschaft der
                // Collection kann aber nur in einer Methode verwendet werden, die sich auf die
                // gesamte Collection bezieht (hier z.B. Any).

                Console.WriteLine("\n" + query.ToString());

                Console.WriteLine("\nRegionen mit zumindest einem Territorium, das mit \"A\" beginnt:\n");
                foreach (Region r in query)
                {
                    Console.WriteLine($"Region {r.RegionID} {r.RegionDescription.Trim()} ({r.Territories.Count()} Territorien)");
                    foreach (Territory t in r.Territories)
                    {
                        Console.WriteLine($"   {t.TerritoryID} {t.TerritoryDescription}");
                    }
                }

                // Achtung:
                // Any prüft, ob es unter den Territorien zumindest 1 Element gibt, das mit "A"
                // beginnt. Wenn das zutrifft, dann wird die Region in das Resultset aufgenommen.
                // Für diese Region werden aber ALLE zugehörigen Territorien geladen!

                // Man kann die Eigenschaften der inkludierten Daten in der Query verwenden, man
                // kann aber nicht die inkludierten Daten filtern oder sortieren!

                //var query1 = ctx.Regions
                //   .Include(r => r.Territories)
                //   .Where(r => r.Territories.TerritoryDescription.StartsWith("A"));
                //
                // führt zu einem Compiler-Fehler.
            }


            Console.WriteLine("\nInkludierte Detaildatenlisten mittels Projektion filtern");
            Console.WriteLine("--------------------------------------------------------");

            using (NorthwindEntities ctx = new NorthwindEntities())
            {
                var query = ctx.Regions
                   .Include(r => r.Territories)
                   // Es ist egal, ob mit oder ohne Include(), die SQLs sind identisch!
                   .Select(region => new
                   {
                       region,
                       Ters = region.Territories.Where(t => t.TerritoryDescription.Trim().EndsWith("s"))
                   });
                // region.Territories enthält alle Territorien, Ters nur jene, die auf "s" enden

                Console.WriteLine("\n" + query.ToString());

                Console.WriteLine("\nAlle Regionen mit allen Territorien:\n");
                foreach (var item in query)
                {
                    Console.WriteLine($"Region {item.region.RegionID} {item.region.RegionDescription}");
                    foreach (Territory t in item.region.Territories)
                    {
                        Console.WriteLine($"   {t.TerritoryID} {t.TerritoryDescription}");
                    }
                }

                Console.WriteLine("\nRegionen mit Territorien, die auf \"s\" enden:\n");
                foreach (var item in query)
                {
                    if (item.Ters.Count() > 0)
                        Console.WriteLine($"Region {item.region.RegionID} {item.region.RegionDescription}");
                    foreach (Territory t in item.Ters)
                    {
                        Console.WriteLine($"   {t.TerritoryID} {t.TerritoryDescription}");
                    }
                }
            }

            Console.WriteLine("\nInkludierte Detaildaten durch Umdrehen der Query filtern");
            Console.WriteLine("--------------------------------------------------------");

            // Query umdrehen: Von den Territorien ausgehen -> man bekommt ein flaches Resultat.

            using (NorthwindEntities ctx = new NorthwindEntities())
            {
                var query = ctx.Territories
                   .Include(t => t.Region)          // ohne Include() -> Lazy Loading!
                   .Where(t => t.TerritoryDescription.TrimEnd().EndsWith("s"))
                   .OrderBy(t => t.Region.RegionID);      // -> erst in einem 2. Schritt

                Console.WriteLine("\n" + query.ToString());

                Console.WriteLine("\nTerritorien, die auf \"s\" enden mit zugehöriger Region:\n");
                foreach (Territory t in query)
                {
                    Console.WriteLine($"Territorium {t.TerritoryID} {t.TerritoryDescription} Region {t.Region.RegionID} {t.Region.RegionDescription} ");
                }
            }


            // ToDo -> Ev. Vergleich mit Join

            // Join

            //   var cats2 = ctx.Categories.Join(ctx.Products,
            //      c => c.CategoryID,
            //      p => p.CategoryID,
            //      (c, p) => new
            //      {
            //         c.CategoryID,
            //         c.CategoryName,
            //         p.ProductID,
            //         p.ProductName
            //      })
            //      ;

            //   Console.WriteLine("\n" + cats2.ToString());

            // Projektion

            //   var cats3 = ctx.Categories
            //      .Select(c => new { c, c.Products });

            //   Console.WriteLine("\n" + cats3.ToString());

            Console.ReadKey();
        }
    }
}