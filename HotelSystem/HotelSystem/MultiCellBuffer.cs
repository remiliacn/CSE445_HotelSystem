using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace HotelSystem
{

    //This class is inspired by the examples provided by the professor.
    class MultiCellBuffer
    {
        private const int BUFFER_SIZE = 3;
        public int head, tail, element = 0;
        String[] rwBuffer = new string[BUFFER_SIZE];

        //Keep a semaphore tracker to do some handy work in the future.
        Semaphore readWriteTracker = new Semaphore(BUFFER_SIZE, BUFFER_SIZE);

        public void setOneCell(String encodedStr)
        {
            readWriteTracker.WaitOne();
            lock (this)
            {
                while (element == BUFFER_SIZE)
                {
                    Monitor.Wait(this);
                }

                rwBuffer[tail] = encodedStr;

                Program.DebugMessage(String.Format("Buffer occupied in tail: {0}", tail.ToString()));
                tail = (tail + 1) % BUFFER_SIZE;  //Just make sure that tail is up-to-date, and also does not go over the bound. It will rewrite the existing cell if full.

                Program.DebugMessage(String.Format("New tail generated: {0}", tail.ToString()));
                element++;
                readWriteTracker.Release();
                Monitor.Pulse(this);
            }

        }

        public string getOneCell()
        {
            readWriteTracker.WaitOne();
            lock (this)
            {
                //Empty, wait for input.
                while(element == 0)
                {
                    Monitor.Wait(this);
                }

                var item = rwBuffer[head];
                head = (head + 1) % BUFFER_SIZE;
                element--;
                readWriteTracker.Release();
                Monitor.Pulse(this);

                return item;

            }
        }
    }
}
