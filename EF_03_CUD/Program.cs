using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* EF erzeugt für jede Entität bei Bedarf automatisch eine SQL-Anweisung für die
 * Operationen Create, Update und Delete (CUD) -> INSERT-, UPDATE-, DELETE-Anweisung.
 * 
 * Man kann aber auch SPs (Stored Procedure) für diese Operationen verwenden. 
 * Dazu müssen die SPs mit dem
 * EDM-Assistenten in das EDM eingebunden werden. Beim Import der SPs wird die Option
 * "Ausgewählte gespeicherte Prozeduren und Funktionen in das Entitätsmodell
 * importieren" deaktiviert, weil diese SPs der Entität direkt zugeordnet werden.
 * 
 * Nach dem Import kann man im Fenster "Zuordnungsdetails" über das 2. Icon auf der
 * linken Seite (-> "Entität zu Funktionen zuordnen") festlegen, welche SP (Stored Procedure)
 * für die Insert-, Update- und Delete-Operation verwendet werden soll. 
 * Danach müssen noch den Parametern der SP Eigenschaften der Entität zugewiesen werden. 
 * 
 * Spalten, wie zum Beispiel fortlaufende IDs oder Zeitstempel, die im Zuge einer
 * Insert- bzw. Update-Operation von der Datenbank gesetzt und in die Entität
 * übernommen werden sollen, sind in der SP mittels einer SELECT-Anweisung zurück-
 * zuliefern. Diesen Spalten müssen im Bereich "Bindungen für Ergebnisspalten"
 * ebenfalls Eigenschaften der Entität zugewiesen werden.
 * 
 * Achtung: Wenn man für eine CUD-Operation eine SP (Stored Procedure) verwendet, 
 * dann muss man für alle 3 CUD-Operationen SPs verwenden.
 */

namespace EF_03_CUD
{
    class Program
    {
        static void Main(string[] args)
        {
            // Alle Shipper ausgeben
            Console.WriteLine("Alle Shipper:");
            ShippersAusgeben();


            // Neuen Shipper hinzufügen (CREATE)
            Console.WriteLine("Neuen Shipper hinzufügen");
            Console.WriteLine("------------------------");

            using (var ctx = new NorthwindEntities())
            {
                // Ein neues Shippersobjekt erstellen
                Shipper newShipper = new Shipper()
                {
                    CompanyName = "DHL",
                    Phone = "(555)-147-2583"
                };

                // Neuen Shipper dem Change Tracker des Kontexts hinzufügen (State = "added")
                ctx.Shippers.Add(newShipper);

                // Die Änderungen am Kontext in der Datenbank speichern, erst hier passiert
                // etwas in der Datenbank
                ctx.SaveChanges();

                Console.WriteLine($"\nNeue ShipperID: {newShipper.ShipperID}");
                Console.WriteLine("\nAlle Shipper nach dem Hinzufügen eines neuen Shippers:");
                ShippersAusgeben();
            }


            // Existierenden Shipper ändern (UPDATE)
            Console.WriteLine("Existierenden Shipper ändern");
            Console.WriteLine("----------------------------");

            using (var ctx = new NorthwindEntities())
            {
                // Mittels Primärschlüssel eine Entität im Kontext finden
                var shipper = ctx.Shippers.Find(4);

                // Der Entität andere Eigenschaften zuweisen
                shipper.CompanyName = "UPS";
                shipper.Phone = "(123) 456-7890";

                ctx.SaveChanges();  // Änderungen in der DB speichern

                Console.WriteLine("\nAlle Shipper nach dem Ändern des Shippers mit der ID 4:");
                ShippersAusgeben();
            }



            // Einen existierenden Shipper entfernen (DELETE)
            Console.WriteLine("Einen Shipper löschen");
            Console.WriteLine("---------------------");

            using (var ctx = new NorthwindEntities())
            {
                // Den zuletzt hinzugefügten Shipper als Entität zurückliefern

                //var shipper = ctx.Shippers.LastOrDefault(); // LINQ2Entity-Query (NotSupportedException
                // weil LastOrDefault() von LINQ2Entitys
                // nicht unterstützt werden, 
                // Siehe: https://docs.microsoft.com/de-de/dotnet/framework/data/adonet/ef/language-reference/supported-and-unsupported-linq-methods-linq-to-entities


                // Auch die nächste Anweisung führt zu einer NotSupportedException,
                // weil die Sequenz vor dem Aufruf von Skip() unbedingt sortiert sein muss.
                //var shipper = ctx.Shippers
                //    .Skip(ctx.Shippers.Count() - 1)
                //    .FirstOrDefault();


                // Möglich wäre die folgende Anweisung, die aber u.U. nicht performant ist
                // (bei sehr großen Tabellen, sortieren kann lange dauern).
                //var shipper = ctx.Shippers
                //    .OrderByDescending(s => s.ShipperID)
                //    .FirstOrDefault();


                //Lösung: SqlQuery() verwenden
                var shipper = ctx.Shippers.SqlQuery("SELECT * FROM Shippers").LastOrDefault();

                if (shipper != null)
                {
                    ctx.Shippers.Remove(shipper);   // Markiert die Entität als gelöscht

                    ctx.SaveChanges();              // Änderungen in der DB speichern

                    Console.WriteLine("\nAlle Shipper nach dem löschen des letzten Shippers:");
                    ShippersAusgeben();
                }


            }



            Console.ReadKey();
        }

        public static void ShippersAusgeben()
        {
            using (var context = new NorthwindEntities())
            {
                foreach (var s in context.Shippers)
                {
                    Console.WriteLine($"{s.ShipperID, -5} {s.CompanyName, -40} {s.Phone}");
                }
                Console.WriteLine();
            }
        }
    }
}
