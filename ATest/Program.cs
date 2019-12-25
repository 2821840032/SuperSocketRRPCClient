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
            },15);
            AOPContainer Container = new AOPContainer();
            client.AddServer<IADD, ADD>();

            while ("q"!= Console.ReadLine())
            {
                try
                {
                    Guid IID = Guid.NewGuid();
                    Parallel.For(0, 1, (id) =>
                    {
                        var result = Container.GetServices<IADD>(client).GetRequestInfo(IID);
                        if (result != IID)
                        {
                            Console.WriteLine("错误！！！");
                        }
                        else
                        {
                            Console.WriteLine(IID);
                        }

                    });

                }
                catch (Exception)
                {
                    
                }
              
            }
            Console.WriteLine("Over");
            
        }
    }
}
