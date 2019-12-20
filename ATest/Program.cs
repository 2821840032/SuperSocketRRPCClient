﻿using IRPCService;
using RPCService;
using SuperSocketAOPClientContainer;
using SuperSocketRRPCClient;
using System;
using System.Threading.Tasks;

namespace ATest
{
    class Program
    {
        static DateTime runA;
        static DateTime runB;
        static void Main(string[] args)
        {
            SocketClientMain client = new SocketClientMain("127.0.0.1", 2012,15);
            AOPContainer Container = new AOPContainer();
            client.AddServer<IADD, ADD>();
            Console.ReadLine();
            runA = DateTime.Now;
            ADDTest(client, Container);
            while (true)
            {
                Console.ReadLine();
                Console.WriteLine("错误数里："+A);

                TimeSpan span = (TimeSpan)( runB- runA);
                Console.WriteLine("运行时间"+span.Milliseconds);
            }
        }
       async static Task ADDTest(SocketClientMain client, AOPContainer Container) {
            await Task.Yield();
            Parallel.For(0, 5000, (id) =>
            {
                ActionAdd(client, Container).Wait();
                Console.WriteLine($"执行程度{id}/50000");
            });
            Console.WriteLine("完成");
            runB = DateTime.Now;
        }
        static int A = 0;
        async static Task ActionAdd(SocketClientMain client, AOPContainer Container)
        {
            await Task.Yield();
            var Ra1 = new Random().Next(10000);
            var Ra2 = new Random().Next(10000);
            try
            {
                var result = Container.GetServices<IADD>(client).ADD(Ra1, Ra2);
                if (result!=(Ra1+Ra2))
                {
                    Console.WriteLine("出现了一个异常的");
                    A++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
