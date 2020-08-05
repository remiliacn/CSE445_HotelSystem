using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

public delegate void priceCutEvent(int priceNow);

namespace HotelSystem
{
    class Program
    {

        //I make all following variables to be static for global access.
        //Declaring 2(two) hotel suppliers with randomly generated supplierId:
        private const int AGENCY_SIZE = 5;

        public static Random rand = new Random(DateTime.Now.GetHashCode());
        public static int supplierID1 = rand.Next(1000, 4000);
        public static int supplierID2 = rand.Next(5000, 9000);

        public static HotelSupplier supplier1 = new HotelSupplier();
        public static HotelSupplier supplier2 = new HotelSupplier();

        //Declaring a global MultiCellBuffer
        public static MultiCellBuffer buf = new MultiCellBuffer();
        static void Main()
        {
            Thread supply1 = new Thread(new ThreadStart(supplier1.checkSupplies));
            //Thread name will be later used as a supply ID param for creating an order.
            //Where in my code, the thread name is actually the supplier ID randomly generated before.
            supply1.Name = supplierID1.ToString();
            supply1.Start();

            Thread supply2 = new Thread(new ThreadStart(supplier2.checkSupplies));
            supply2.Name = supplierID2.ToString();
            supply2.Start();
         
            TravelAgency agency = new TravelAgency();
            //Queue the event.
            HotelSupplier.priceCut += new priceCutEvent(agency.roomInfo);

            Thread[] agencyThread = new Thread[AGENCY_SIZE];

            //Fire up 5 agencies.
            for(int i = 0; i < AGENCY_SIZE; i++)
            {
                var agencyId = rand.Next(1000, 9999);
                agencyThread[i] = new Thread(new ThreadStart(agency.startAgencyProcess));
                agencyThread[i].Name = agencyId.ToString();

                agencyThread[i].Start();
            }

            supply1.Join();
            supply2.Join();

            Console.WriteLine("Press any key to exit the program...");
            Console.ReadKey();
        }

        //For debugging purpose.
        public static void DebugMessage(String message)
        {
            Debug.WriteLine(String.Format("[{0}] [DEBUG]: {1}\n", DateTime.Now.ToString(), message));
        }

    }
}
