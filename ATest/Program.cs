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
using Unity;
namespace ATest
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketClientMain client = new SocketClientMain("127.0.0.1",2012,(unity)=> {
                unity.RegisterSingleton<IADD, ADD>();
            },15,0);
            AOPContainer Container = new AOPContainer();
            client.AddServer<IADD, ADD>();

            string ll = Console.ReadLine();
            while ("q"!= ll)
            {
                try
                {

                    var result = Container.GetServices<IADD>(client).AsyncMM();
                    //var result = Container.GetServices<IADD>(client,Guid.Parse(ll)).GetRequestInfo(Guid.NewGuid());
                    ll = Console.ReadLine();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ll = Console.ReadLine();
                }
              
            }
            Console.WriteLine("Over");
            
        }
    }
}
