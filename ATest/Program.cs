using IRPCService;
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
                //unity.RegisterSingleton<IADD, ADD>();
            },15,0);
            AOPContainer Container = new AOPContainer();
            //client.AddServer<IADD, ADD>();

            string ll = Console.ReadLine();
            while ("q"!= ll)
            {
                Container.GetServices<IRPCService.IADD>(client).ADDV(100,50);
                
                //Parallel.For(0, 20, (A) =>
                //{
                //    var result = Container.GetServices<IADD>(client).AsyncMM(50);
                //    Console.WriteLine(A + "/1000");
                //});
                ll = Console.ReadLine();
            }
            Console.WriteLine("Over");
            
        }
    }
}
