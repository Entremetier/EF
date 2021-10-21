using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/* Detaildaten, die über die Navigationseigenschaften zur Verfügung gestellt werden,
 * können auf verschiedene Arten geladen werden:
 *    - Lazy Loading      ("faules", verzögertes Laden)
 *    - Explizites Laden
 *    - Eager Loading     (vorzeitiges Laden)
 *
 * Beim Lazy Loading werden die Detaildaten automatisch abgerufen und zwar erst dann,
 * wenn auf eine Navigationseigenschaft zugegriffen wird. Dabei wird ein SQL-Statement
 * an den Server geschickt, um die zugehörigen Detaildaten abzufragen.
 * 
 * Standardmäßig ist im Entity Framework Lazy Loading eingeschaltet. Man kann das aber
 * entweder für das gesamte Modell (im Eigenschaften-Fenster des EDM-Designers) oder
 * auch nur für eine einzelne Kontext-Instanz (im Code) ändern.
 * 
 * Wenn man über lange Listen von Entitäten iteriert und dabei auf Detaildaten
 * zugreift, dann sollte man Lazy Loading abschalten, damit nicht bei jeder Iteration
 * eine Abfrage an den Server geschickt wird.
 * 
 * Wenn Lazy Loading abgeschaltet ist, dann werden die Detaildaten nicht mehr
 * automatisch vom Entity Framework geladen, sondern man muss sich selbst darum
 * kümmern. Diese Art des Ladens heißt Explizites Laden und es kommt dabei die Methode
 * Load() zum Einsatz. Beim Expliziten Laden hat man zwar die Kontrolle über das Laden
 * der Detaildaten, aber bei jedem Load()-Aufruf wird, wie beim Lazy Loading, eine
 * SQL-Anweisung an den Server geschickt, um die Detaildaten abzufragen. Hinsichtlich
 * der Performance ist das explizite Laden von Detaildaten in einer Schleife auch nicht
 * besser als das Lazy Loading (u.U. sogar schlechter, wenn man nicht prüft, ob die
 * Detaildaten bereits geladen wurden).
 * 
 * Im Entity Framework gibt es daher noch das sog. Eager Loading, bei dem man bereits
 * beim Erstellen der Abfrage mittels der Include-Methode angeben kann, welche Detail-
 * daten gleich mitgeladen werden sollen. Dabei verhindert man wiederholte Anfragen an
 * den Server.
 * 
 * Beim Eager Loading werden aber alle Detaildaten geladen, die in der(n) Include-
 * Methode(n) angegeben sind. Das kann zu sehr komplexen Queries und/oder zu großen
 * Datenmengen führen, die vom DB-Server zum Client übertragen und danach noch
 * materialisiert (in Objekte umgewandelt) werden müssen. Das ist vor allem dann
 * problematisch, wenn bestimmte Detaildaten in weiterer Folge gar nicht verwendet
 * werden. Es ist für jeden Einzelfall abzuwägen, ob Eager Loading oder Explizites
 * Laden performanter bzw. resourcenschonender ist, oder ob eine Kombination aus beiden
 * (Include und Load) günstiger ist (siehe dazu auch Projekt EF_06_EagerLoading).
 */

namespace EF_05_DetaildatenLaden
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Implizites Laden");
            Console.WriteLine("----------------\n");

            //TODO:
            // Die Navigationseigenschaft ist eine Navigation Reference
            // (Die Multiplizität der Assoziation zur anderen Entität ist 1 (oder 0).
            // Siehe Model5.edmx (Diagram1) - Beziehung Category zu Product!!!!


            Console.WriteLine("Produkte mit ID >= 75:");
            using (var ctx = new NorthwindEntities())
            {
                var prods = ctx.Products.Where(p => p.ProductID >= 75);

                Console.WriteLine("\n" + prods.ToString());     // SQL-Statement ausgeben

                foreach (Product p in prods)
                {
                    Console.WriteLine($"{p.ProductID} - {p.ProductName}  (CategoryID = {p.CategoryID})");
                    
                    // Beim Zugriff auf die Navigationseigenschaft Category werden die zugehörigen
                    // Detaildaten (z.B. CategoryName) automatisch (nach)geladen. Wenn der Name
                    // schon einmal benutzt wurde bleibt er im Speicher und wird wiederverwendet.
                    // Das bezeichnet man als "Lazy Loading", Standardmäßig ist es eingeschaltet.
                    Console.WriteLine($"    {p.Category.CategoryName}");
                }
                Console.WriteLine();
            }


            //TODO:
            // Die Navigationseigenschaft ist eine Navigation Collection
            // (Die Multiplizität der Assoziation zur anderen Entität ist * (many).
            // Siehe Model5.edmx (Diagram1) - Beziehung Product zu Category !!!!


            Console.WriteLine("Kategorien 6 und 7:");
            using (var ctx = new NorthwindEntities())
            {
                var cats = ctx.Categories.Where(c => c.CategoryID >= 6 && c.CategoryID <= 7);

                Console.WriteLine("\n" + cats.ToString());     // SQL-Statement ausgeben

                foreach (Category c in cats)
                {

                    // Die zur Kategorie c gehörende Produkteliste wird bei eingeschaltetem
                    // Lazy Loading beim Zugriff auf Produktdaten (über die Navigationseigenschaft
                    // "Products") automatisch nachgeladen.
                    Console.WriteLine($"\n{c.CategoryName} ({c.Products.Count()} Produkte)");

                    foreach (var p in c.Products)
                    {
                    Console.WriteLine($"    {p.ProductName}");
                    }
                }
                Console.WriteLine();
            }



            Console.WriteLine("\nExplizites Laden");
            Console.WriteLine("----------------\n");


            // Navigation Reference

            // Um Lazy Loading für das Projekt auszuschalten geht man in Model5.edmx, rechtsklick
            // auf einen freien Bereich und Properties aufrufen, dann "Lazy Loading Enabled" auf
            // false setzen. 

            Console.WriteLine("Produkte mit ID >= 75:");
            using (var ctx = new NorthwindEntities())
            {
                // Lazy Loading nur für diesen Kontext (nicht das ganze Projekt) abschalten
                // Das automatische Laden von Detaildaten ist damit ausgeschaltet.
                ctx.Configuration.LazyLoadingEnabled = false;

                var prods = ctx.Products.Where(p => p.ProductID >= 75);

                foreach (Product p in prods)
                {
                    Console.WriteLine($"{p.ProductID} - {p.ProductName}  (CategoryID = {p.CategoryID})");

                    // Wenn Lazy Loading ausgeschaltet ist, dann wird beim Zugriff auf die
                    // Navigationseigenschaft "Category" eine NullReferenceException geworfen,
                    // da die zugehörigen Detaildaten nicht geladen sind.
                    // Die Navigationseigenschaft "Category" = null.

                    // Die zur Navigationseigenschaft "Category" gehörenden Detaildaten können
                    // explizit geladen werden. Da ein Produkt zu einer (oder keiner) Kategorie
                    // gehört, werden die Kategoriedaten mit den Methoden Reference() und Load()
                    // geladen.
                    // Vor dem Laden sollte geprüft werden, ob die Daten nicht bereits geladen
                    // wurden.


                    // Wenn die Navigationseigenschaft noch nicht geladen ist, wird sie (nach)geladen
                    //if (!ctx.Entry(p).Reference("Category").IsLoaded)
                    //{
                    //// Durch Entry kann das einzelne Product angesprochen werden
                    //ctx.Entry(p).Reference("Category").Load();
                    //}


                    // ODER mit Lambda-Ausdruck (und IntelliSense-Unterstützung)


                    if (!ctx.Entry(p).Reference(c => c.Category).IsLoaded)
                    {
                        ctx.Entry(p).Reference(c => c.Category).Load();
                    }

                    // Die Navigationseigenschaft "Category" könnte trotz explizitem Laden null
                    // sein, wenn dem Produkt in der DB keine Kategorie zugeordnet ist.
                    // (Das Feld "CategoryID" der Tabelle "Product" erlaubt Nullwerte!)
                    if (p.Category != null)
                    {
                    Console.WriteLine($"    {p.Category.CategoryName}");

                    }
                }
                Console.WriteLine();
            }


            // Navigation Collection

            Console.WriteLine("Kategorien 6 und 7:");
            using (var ctx = new NorthwindEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false;

                var cats = ctx.Categories.Where(c => c.CategoryID >= 6 && c.CategoryID <= 7);

                foreach (Category cat in cats)
                {
                    // Wenn Lazy Loading ausgeschaltet ist, dann werden die zur Navigations-
                    // eigenschaft "Products" gehörigen Detaildaten (= Liste aller zur Kategorie
                    // gehörenden Produkte) nicht geladen und die Liste ist leer.

                    // Die zur Navigationseigenschaft "Products" gehörenden Detaildaten können
                    // explizit geladen werden. Da zu einer Kategorie viele Produkte gehören,
                    // werden die Produktdaten mit den Methoden Collection() und Load() geladen.
                    // Vor dem Laden sollte geprüft werden, ob die Daten nicht bereits geladen
                    // wurden.

                    if (!ctx.Entry(cat).Collection("Products").IsLoaded)
                    {
                        ctx.Entry(cat).Collection(c => c.Products).Load();
                    }

                    Console.WriteLine($"\n{cat.CategoryName} ({cat.Products.Count()} Produkte)");

                    foreach (var p in cat.Products)
                    {
                        Console.WriteLine($"    {p.ProductName}");
                    }
                }
            }


            Console.WriteLine("\nEager Loading");
            Console.WriteLine("---------------");

            // Navigation Reference

            using (var ctx = new NorthwindEntities())
            {
                var prods = ctx.Products
                    .Include("Category")        // Detaildatensätze aus Category werden hier schon mitgeladen
                    .Where(p => p.ProductID >= 75);

                Console.WriteLine("\n" + prods.ToString());     //SQL-Statement ausgeben

                foreach (Product p in prods)
                {
                    Console.WriteLine($"\n{p.ProductID} - {p.ProductName}  (CategoryID = {p.CategoryID})");
                    Console.WriteLine($"    {p.Category.CategoryName}");
                }
                Console.WriteLine();
            }


            // Navigation Collection

            using (var ctx = new NorthwindEntities())
            {
                var cats = ctx.Categories
                    .Include("Products")
                    .Where(c => c.CategoryID >= 6 && c.CategoryID <= 7);

                Console.WriteLine("\n" + cats.ToString());     //SQL-Statement ausgeben

                foreach (Category c in cats)
                {
                    Console.WriteLine($"\n{c.CategoryName} ({c.Products.Count()} Produkte)");

                    foreach (var p in c.Products)
                    {
                        Console.WriteLine($"    {p.ProductName}");
                    }
                }
                Console.WriteLine();
            }

            Console.ReadKey();
        }
    }
}
