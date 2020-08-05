using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace HotelSystem
{
    class HotelSupplier
    {
        private static Random rand = new Random();
        public static event priceCutEvent priceCut;
        public int cutCounter = 0;
        //Secret key for decrypting the encrypted order information later.
        private String KEY = "Yiyang_Lu";
        //Initialized room price.
        public static int roomPrice = 360;
        public String encodedStr, decodedStr;
         
        /// <summary>
        /// Initialize a hotel supplier object
        /// </summary>
        public HotelSupplier()
        {
            encodedStr = "";
            decodedStr = "";
        }

        /// <summary>
        /// Check if the supplies are available for confirmation.
        /// </summary>
        public void checkSupplies()
        {
            //Check if the cut is exceeding 10
            while(cutCounter < 10)
            {
                while (ordersInQueue())
                {
                    if (ordersInQueue())
                    {
                        getOrderFromCellBuffer();
                    }
                    
                }

                //using price model to update the price.
                parsePricingModel();
            }

            //Stop the Thread by making the roomPrice to INT_MIN for further usage.
            if (cutCounter == 10)
            {
                roomPrice = int.MinValue;
            }

            //Just in case, some of the orders were left in the buffer untouched...
            while (ordersInQueue())
            {
                if (ordersInQueue())
                {
                    getOrderFromCellBuffer();

                }
            }
        }

        public int getRoomPriceNow()
        {
            return roomPrice;
        }



        /// <summary>
        /// The Pricing Model Function.
        ///
        /// The logic is that:
        /// New price will be randomly generated everytime when parsing the pricing model
        /// If the newPrice is lower than the current price.
        ///
        /// Let's make a cut, and make the new price as the price now.
        /// </summary>
        private void parsePricingModel()
        {
            lock (this)
            {
                Thread.Sleep(1200);
                int newPrice = rand.Next(150, 310);
                
                if (newPrice < roomPrice && newPrice != int.MinValue)
                {
                    if(priceCut != null)
                    {
                        priceCut(newPrice);
                        cutCounter++;
                    }
                }

                roomPrice = newPrice;
            }

        }

        /// <summary>
        /// Call the buffer to add encoded string.
        /// </summary>
        private void getOrderFromCellBuffer()
        {
            encodedStr = Program.buf.getOneCell();
            Decoder(encodedStr);
        }

        /// <summary>
        /// Decode the string that is parsed into the decoder.
        /// </summary>
        /// <param name="encodedStr">
        /// Encrypted string will be decoded here, 
        /// and an OrderClass object will be distributed for processing.
        /// </param>
        public void Decoder(string encodedStr)
        {
            lock (this)
            {
                while (decodedStr != "")
                {
                    Monitor.Wait(this);
                }

                //Decryption algorithm has the same logic as encryption.
                for (int i = 0; i < encodedStr.Length; i++)
                {
                    decodedStr += (char)(encodedStr[i] ^ KEY[i % KEY.Length]);
                }

                //Split the args to create the order object.
                string[] args = decodedStr.Split("|");
                OrderClass order = new OrderClass
                {
                    senderId = args[0],
                    receiverId = args[1],
                    supplierId = int.Parse(args[2]),
                    cardNo = int.Parse(args[3]),
                    amount = int.Parse(args[4]),
                    price = int.Parse(args[5]),
                    startTime = DateTime.Parse(args[6])
                };

                doProcessingOrder(order);
                decodedStr = "";
            }
        }

        /// <summary>
        /// Check if there any orders in buffer.
        /// </summary>
        /// <returns>true if yes, false otherwise.</returns>
        public Boolean ordersInQueue()
        {
            return Program.buf.element != 0;
        }

        /// <summary>
        /// Process the order by opening a stand-alone thread.
        /// </summary>
        /// <param name="order">
        /// The OrderClass object that is decrypted with the decoder.
        /// </param>
        private void doProcessingOrder(OrderClass order)
        {
            var orderProcessThread = new Thread(() => orderProcessing(order));
            orderProcessThread.Start();
        }

        /// <summary>
        /// Where the order is processed, and details are printed.
        /// </summary>
        /// <param name="order">
        /// An OrderClass object that being processed.
        /// </param>
        private void orderProcessing(OrderClass order)
        {
            if (validCard(order.cardNo))
            {
                //unitPrice * NoOfRooms + Tax + LocationCharge.
                //Let's say that Tax is 10% of the charge of (unitPrice * NoOfRoom)
                //And the location charge is a fixed price of (supplierId / 1000)
                var taxRate = 0.1;
                Program.DebugMessage(String.Format("Credit card number test passed on order senderID: {0}", order.senderId));
                order.finalPrice = (double)(order.amount * order.price * taxRate + order.supplierId / 1000 + order.amount * order.price);
                order.processTime = DateTime.Now;
                Console.WriteLine(String.Format("Order has been confirmed\n" +
                    "Process Time: {0}\n" +
                    "Order first made: {4}\n" +
                    "Room Ordered Amount: {2}\n" +
                    "Room base value: ${3}\n" +
                    "Total Charge(Including Tax + Location Fee): ${1}\n", order.processTime, order.finalPrice,
                                            order.amount, order.price, order.startTime));
            }
            else
            {
                Console.WriteLine("Invalid card number.");
            }
        }

        /// <summary>
        /// Check if the credit card is valid.
        /// </summary>
        /// <param name="cardNo">
        /// Credit card number.
        /// </param>
        /// <returns>
        /// true if valid, false otherwise.
        /// </returns>
        private bool validCard(int cardNo)
        {
            return (cardNo >= 5000 && cardNo <= 10000);
        }
    }
}
