using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace HotelSystem
{
    class TravelAgency
    { 
        private int roomPrice { get; set; }

        private String encodedStr = "";

        private String KEY = "Yiyang_Lu";
        private int receiverId { get; set; }

        private static Random rand = new Random();

        /// <summary>
        /// Start the supplier operation.
        /// </summary>
        public void startAgencyProcess()
        {
            HotelSupplier supplier = new HotelSupplier();
            while(supplier.getRoomPriceNow() != int.MinValue)
            {
                //roomPrice = supplier.getRoomPriceNow();
                Thread.Sleep(1200);

                if (supplier.getRoomPriceNow() != int.MinValue)
                {
                    makeAnOrder();
                }
                
            }
        }

        /// <summary>
        /// Create an order based on random generated cardNumber and room amount.
        /// </summary>
        private void makeAnOrder()
        {
            Program.DebugMessage("Generating a credit card number.");
            int cardNumber = rand.Next(5000, 10000);
            Program.DebugMessage(String.Format("Generation completed, current card No: {0}", cardNumber));

            Program.DebugMessage("Generating a random order amount.");
            int roomAmount = rand.Next(1, 4);
            Program.DebugMessage(String.Format("Generation completed, current amount: {0}", roomAmount));

            
            DateTime now = DateTime.Now;
            Console.WriteLine(String.Format("Order has been created.\n" +
            "From Supplier ID: {0}\n" +
            "Rooms Ordered: {1} Unit(s)\n" +
            "Price/Room: ${2}\n" +
            "Order Created at: {3}\n", Thread.CurrentThread.Name, roomAmount, roomPrice, now));

            OrderClass newOrder = new OrderClass
            {
                senderId = Thread.CurrentThread.Name,
                cardNo = cardNumber,
                receiverId = this.receiverId.ToString(),
                price = Math.Abs(roomPrice),
                startTime = now,
                amount = roomAmount
            };

            Program.DebugMessage("Sending the order to the encoder.");
            Encoder(newOrder);
        }


        /*
         * String senderId, receiverId;
         * int supplierId, cardNo, amount, price;
         * double finalPrice;
         * DateTime startTime;
        */
        /// <summary>
        /// Encode an OrderClass object to string, and put it into the buffer.
        /// </summary>
        /// <param name="order">
        /// OrderClass object contains order information.
        /// </param>
        public void Encoder(OrderClass order)
        {
            lock (this)
            {
                while(encodedStr != "")
                {
                    Monitor.Wait(this);
                }

                var orderPlainText = String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}",
                    order.senderId, order.receiverId,
                    order.supplierId, order.cardNo,
                    order.amount, order.price,
                    //Record the start time stamp.
                    order.startTime.ToString("yyyy/MM/dd HH:mm:ss.fff"));

                //We know that XOR operation has following property:
                //A ^ B = C, C ^ B = A
                //So it can be used to encrypt and decrypt string.
                for (int i = 0; i < orderPlainText.Length; i++)
                {
                    encodedStr += (char)(orderPlainText[i] ^ KEY[i % KEY.Length]);
                }

                Program.DebugMessage("Encoded string: " + encodedStr);

                sendOrderToBuffer(encodedStr);
                encodedStr = "";
            }
        }

        private void sendOrderToBuffer(string encoded)
        {
            Program.buf.setOneCell(encoded);
        }

        //Event handler for PriceCut.
        public void roomInfo(int price)
        {
            Console.WriteLine("Room On sale at ${0} -- From Supplier ID: {1}", price, Thread.CurrentThread.Name);
            roomPrice = price;
        }
    }

    
}
