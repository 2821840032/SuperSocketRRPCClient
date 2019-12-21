using IRPCService;
using RPCService;
using SuperSocketAOPClientContainer;
using SuperSocketRRPCClient;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using SuperSocketRRPCClient.Entity;

namespace ATest
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketClientMain client = new SocketClientMain("127.0.0.1", 2012, 15);
            AOPContainer Container = new AOPContainer();
            //client.AddServer<IADD, ADD>();

            while ("q"!= Console.ReadLine())
            {
                try
                {
                    Console.WriteLine(Container.GetServices<IADD>(client).GetRequestInfo());
                }
                catch (Exception)
                {
                    
                }
              
            }
            Console.WriteLine("Over");
            
        }
    }
}
