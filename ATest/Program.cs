using SuperSocketAOPClientContainer;
using SuperSocketClient;
using System;

namespace ATest
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketClientMain client = new SocketClientMain("127.0.0.1", 2012);
            AOPContainer Container = new AOPContainer();
            //client.AddServer<IADD, ADD>();
            while (true)
            {
                Console.ReadLine();
                //Container.GetServices<IADD>(client).ADD(1, 8);
                //Console.WriteLine(.Result);
            }
        }
    }
}
