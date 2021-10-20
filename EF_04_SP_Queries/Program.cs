using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* Stored Procedures zum Abrufen von Daten
 * 
 * Mit dem EF kann man nicht nur auf Tabellen zugreifen, sondern es können auch am Datenbank-
 * Server gespeicherte Prozeduren zum Abfragen von Daten in das EDM eingebunden werden.
 * Die SPs werden als Methoden in der Datenbankkontextklasse eingerichtet und können demnach
 * über den Kontext aufgerufen werden.
 * 
 * Dazu müssen die SPs im EDM-Assistenten ausgewählt und die Option "Ausgewählte gespeicherte
 * Prozeduren und Funktionen in das Entitätsmodell importieren" ausgewählt werden. Es wird
 * für jede importierte SP ein sog. komlexer Typ eingerichtet, den das EF aus den Datentypen
 * der Rückgabewerte ermittelt. Liefert die Abfrage eine Entität zurück, dann kann der
 * komplexe Typ nachträglich durch die Entität ersetzt werden.
 * 
 * Hat man beim Import die o.g. Option nicht ausgewählt, dann kann man nachträglich im 
 * Datenbankbereich des Modellbrowsers (...Model.Store) einzelne SPs als Methoden in das EDM
 * einbinden (durch Auswahl der Option "Funktionsimport hinzufügen..." im Kontextmenü der
 * jeweiligen SP).
 */

namespace EF_04_SP_Queries
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*** Mit Stored Procedures Daten abrufen ***\n");

            // Die 10 teuersten Produkte
            // -------------------------

            Console.WriteLine("Die 10 teuersten Produkte:");

            using (var ctx = new NorthwindEntities())
            {
                var top10Products = ctx.Ten_Most_Expensive_Products();

                // Das Resultat ausgeben
                foreach (var item in top10Products)
                {
                    Console.WriteLine($"{item.TenMostExpensiveProducts, -25} {item.UnitPrice, 8:n} Euro");
                }
                Console.WriteLine();
            }


            // Die x teuersten Produkte
            // -------------------------

            using (var ctx = new NorthwindEntities())
            {
                int n = 15;

                Console.WriteLine($"Die {n} teuersten Produkte:");

                foreach (var item in ctx.sp_MostExpensiveProducts(n))
                {
                    Console.WriteLine($"{item.ProductID, -5} {item.ProductName, -35}" +
                        $"{item.UnitPrice,8:n} Euro");
                }
                    
            }



            /* Übung: AlleBestellungenEinesKunden
             *  
             * Schreiben Sie eine Methode AusgebenAlleOrdersVonCustomer(), die eine Liste aller
             * Bestellungen für einen Kunden auf der Konsole ausgibt. An die Methode soll eine
             * CustomerID übergeben werden, die den Kunden angibt, für den die Bestellungen
             * aufzulisten sind. Verwenden Sie in der Methode die (vorhandene) Stored Procedure
             * "CustOrderHist" aus der Datenbank Northwind, um die Daten abzufragen. Rufen Sie
             * die Methode einmal für den Kunden "ALFKI" und einmal für den Kunden "BOLID" auf.
             * 
             * Die Ausgabe sollte in etwa so aussehen:
             * 
             * Bestellungen des Kunden ALFKI:
             *     6 Aniseed Syrup
             *    21 Chartreuse verte
             *    40 Escargots de Bourgogne
             *    20 Flotemysost
             *    16 Grandma's Boysenberry Spread
             *    15 Lakkalikööri
             *     2 Original Frankfurter grüne Soße
             *    15 Raclette Courdavault
             *    17 Rössle Sauerkraut
             *     2 Spegesild
             *    20 Vegie-spread
             * 
             * Bestellungen des Kunden BOLID:
             *    40 Alice Mutton
             *    24 Chef Anton's Cajun Seasoning
             *    40 Filo Mix
             *    16 Ravioli Angelo
             *    50 Rhönbräu Klosterbier
             *    20 Thüringer Rostbratwurst
             * --------------------------------------------------------------------------------
             */


            Console.ReadKey();
        }
    }
}
