using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace HotelSystem
{
    class OrderClass
    {
        /* 
         * senderId: the identity of the sender, you can use thread name or thread id;
         * Intended Hotel Supplier ID
         * cardNo: an integer that represents a credit card number;
         * receiverID: the identity of the receiver, you can use thread name or a unique name that defined for a hotel supplier;
         * amount: an integer that represents the number of roomto order;
         * ================================================================
         * Customized:
         * price: the price for the order.
         * finalPrice: the price after calculating the total in the orderProcessing class.
         * startTime: The time stamp of the order's creation time.
         * processTime: The time stamp of the order's processed time.
         * 
         */

        public String senderId { get; set; }
        public String receiverId { get; set; }
        public int supplierId { get; set; }
        public int cardNo {get; set;}
        public int amount { get; set; }
        public int price { get; set; }
        public double finalPrice { get; set; }
        public DateTime startTime { get; set; }
        //Used only in HotelSuppliers.cs
        public DateTime processTime { get; set; }

        public OrderClass()
        {
            startTime = DateTime.Now;
        }

        public override string ToString()
        {
            var response = String.Format("Travel Agency: {0}\n" +
                "Supplier ID: {1}\n" +
                "Price/room: ${2}\n" +
                "Room(s) amount: {3}\n" +
                "Total Price: ${4}",
                senderId,
                supplierId,
                price.ToString(),
                amount.ToString(),
                finalPrice.ToString());

            return response;
        }
    }
}
