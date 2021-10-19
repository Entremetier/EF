using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* Entity Framework (EF)
 * 
 * Mit Hilfe des EFs kann man auf einfache Weise aus einer .NET Applikation auf
 * relationale Datenbanken zugreifen.
 * 
 * Das EF überbrückt den Unterschied zwischen den Datenstrukturen in der Programmierung
 * und der persistenten (dauerhaften) Datenspeicherung in relationalen Datenbanken.
 * Objektorientierte Programmiersprachen wie C# arbeiten mit Klassen und Objekten,
 * Datenbanken mit Tabellen.
 * Ein objekt-relationaler Mapper (O/R-Mapper) bildet Daten aus einer relationalen
 * Datenbank auf ein objektorientiertes Datenmodell ab, das bei der Programmierung
 * verwendet wird. Tabellen werden auf Klassen abgebildet, Spalten auf Eigenschaften,
 * Datensätze auf Objekte und Fremdschlüsselbeziehungen auf Objektreferenzen.
 * 
 * Der Programmierer muss sich nicht selbst um die Transformation zwischen Datensätzen
 * und Objekten kümmern. Er kann das EF anweisen, Daten aus der Datenbank zu laden bzw.
 * dort zu speichern. Dabei übernimmt das EF auch das Erstellen der INSERT-, UPDATE-
 * und DELETE-Anweisungen.
 * 
 * Das EF bildet Datenbanken auf objektorientierte Konzepte entsprechend dem sog.
 * Entity Data Model (EDM) ab. Dieses besteht aus den folgenden 3 Schichten:
 *    1) Konzeptionelle Schicht (Conceptual Layer)
 *    2) Logische Schicht (Logical Layer) und
 *    3) Zuordnungsschicht (Mapping Layer)
 * 
 * Die konzeptionelle Schicht beschreibt das zu verwendende Objektmodell, bestehend aus
 * Klassen, den sog. Entitäten oder Entitätsklassen, die miteinander in Beziehung
 * stehen (können). Die logische Schicht beschreibt das Schema der Datenbank.
 * Dazwischen liegt die Zuordnungsschicht, die die Elemente der konzeptionellen Schicht
 * auf die Elemente der logischen Schicht abbildet. Beispielsweise beschreibt sie, dass
 * eine Klasse Person auf eine Tabelle Person abgebildet wird, dass eine Eigenschaft Id
 * einer Spalte PersonId entspricht oder dass ein bestimmter Fremdschlüssel zum
 * Auflösen einer Objektbeziehung heranzuziehen ist.
 * 
 * In Visual Studio hat der Entwickler im Rahmen des EF's 3 Entwurfsmöglichkeiten:
 *    1) Database First
 *    2) Model First und
 *    3) Code First
 * Wir werden ausschließlich Database First anwenden. Bei dieser Entwurfsart wird das
 * EDM automatisch aus den Objekten einer bestehenden Datenbank erstellt.
 * 
 * Neben den einzelnen Entitätsklassen hat der Entwickler auch Zugriff auf den sog.
 * Datenbankkontext oder kurz Kontext. Der Kontext ist eine Klasse, die von DbContext
 * erbt, und das Arbeiten mit der Datenbank managt. Im Kontext kann der Entwickler das
 * Laden von Objekten anstoßen und der Kontext "materialisiert" die Abfrageergebnisse
 * zu Entitäts-Objekten. Der Kontext führt Buch über die Änderungen an diesen Objekten
 * und sorgt dafür, dass die geänderten Objekte in der Datenbank gespeichert werden.
 * 
 * Das EF basiert auf ADO.NET (dem Nachfolger von ActiveX Data Objects), einer Daten-
 * zugriffstechnologie, die einheitlich auf unterschiedliche Datenquellen zugreifen
 * kann, wie z.B. SQL-Datenbanken, XML-Files oder Excel-Tabellen. Verschiedene Daten-
 * provider regeln den Zugriff auf die verschiedenen Datenquellen.
 * 
 * Die in C# integrierte Abfragesprache LINQ (Language Integrated Query) trägt weiter
 * zur Datenbankunabhängigkeit bei. Im EF kommt LINQ to Entities zum Einsatz, das
 * automatisch in das sog. Entity SQL kompiliert wird und das der jeweilige Daten-
 * provider in das native SQL der verwendeten Datenbank umwandelt.
 * 
 * Der Vorteil des Entity Frameworks besteht darin, dass der Entwickler, z.B. im
 * Vergleich mit ADO.NET, viel weniger Datenzugriffscode (selbst) schreiben muss.
 * 
 * Das EF ist nicht geeignet für zeitkritische Anwendungen wie Massenaktualisierungen
 * oder Echtzeit-Anwendungen.
 */

namespace EF_01_Grundlagen
{
    class Program
    {
        static void Main(string[] args)
        {
            // Alle Datensätze der Tabelle "Products" anzeigen
            // -----------------------------------------------

            // Als erstes ein Contextobjekt erstellen (in Model1.Context.cs zu finden)
            // Dazu wird ein using-Statement verwendet, damit offene Ressourcen (Datenbankverbindungen)
            // am Ende des using-Blocks auf jeden Fall (auch im Fehlerfall) geschlossen werden.
            using (NorthwindEntities context = new NorthwindEntities()) 
            {
                // Definition der Abfrage
                // Die Variable products verweist auf die Auflistung Products (=DBSet<Product>) des
                // Datenbankkontextes.
                var products = context.Products;

                Console.WriteLine("Alle Produkte:");

                // Ausführen der Abfrage (erst jetzt fließen die Daten)
                foreach (Product product in products)
                {
                    // Erst hier wird die connection zur Datenbank gemacht ↓↓↓
                    Console.WriteLine($"{product.ProductID} - {product.ProductName,-40} {product.UnitPrice:n}");
                    //Console.WriteLine($"{product.Order_Details.First().OrderID}");
                }
                Console.WriteLine();

            }

            // LINQ to Entities Abfrage in Abfragesyntax
            // -----------------------------------------

            // Context erstellen
            using (var ctx = new NorthwindEntities())
            {
                var productsMore80Euro = from p in ctx.Products
                                         where p.UnitPrice > 80
                                         select p;

                Console.WriteLine("Alle Produkte die mehr als 80 Euro kosten:");
                foreach (var product in productsMore80Euro)
                {
                    Console.WriteLine($"{product.ProductID} - {product.ProductName,-40} {product.UnitPrice:n} Euro");
                }
                Console.WriteLine();
            }

            // LINQ to Entities Abfrage im Methodensyntax
            // ------------------------------------------

            // Context erstellen
            using(var ctx = new NorthwindEntities())
            {
                var orderDetails = ctx.Order_Details
                    .Where(od => od.OrderID == ctx.Orders.Min(o => o.OrderID));

                Console.WriteLine("Order mit der kleinsten OrderID:");
                foreach (var item in orderDetails)
                {
                    //Console.WriteLine($"{item.OrderID, 6} {item.Quantity, 5} {item.ProductID, 5} {item.UnitPrice,8:n} Euro");

                    // ODER Navigationseigenschaft "Product" verwenden, um den Produktnamen auszugeben

                    Console.WriteLine($"{item.OrderID,6} {item.Quantity,5} {item.Product.ProductName, -40} {item.UnitPrice,8:n} Euro");
                }
                Console.WriteLine();
            }






            Console.ReadKey();
        }
    }
}
