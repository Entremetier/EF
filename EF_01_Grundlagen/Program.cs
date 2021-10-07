using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF_01_Grundlagen
{
    class Program
    {
        static void Main(string[] args)
        {
            // Alle Datensätze der Tabelle "Products" anzeigen
            // -----------------------------------------------

            // Als erstes ein Contextobjekt erstellen (in Model1.Context.cs zu finden)
            // Zur using Anweisung gehört ein Codeblock, darum die {}
            using (NorthwindEntities context = new NorthwindEntities()) 
            { 

            }








            Console.ReadKey();
        }
    }
}
