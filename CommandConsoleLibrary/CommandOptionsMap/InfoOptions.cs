using CommandLine;
using SuperSocketRRPCClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Concurrent;
using SuperSocketRRPCClient.Entity;
using System.Data;

namespace CommandConsoleLibrary.CommandOptionsMap
{
    [Verb("info", HelpText = "信息")]
   public class InfoOptions
    {
        public int Run(DateTime dateTime,List<Type> commandExecutionRPC, SocketClientMain client)
        {
            Console.WriteLine("开始时间："+dateTime);
            Console.WriteLine("远程地址："+client.RemoteEndpoint.ToString());
            Console.WriteLine("命令可调用对象数量："+commandExecutionRPC.Count);
            Console.WriteLine("任务数量："+client.RemoteCallQueue.MethodCallQueues.Count);
            Console.WriteLine("任务异常任务数量："+client.RemoteCallQueue.MethodCallQueues.Values.Where(d=>d.State== ReceiveMessageState.Error).Count());
            Console.WriteLine("运行异常数量："+ LoggerAssembly.LoggerList.Where(d=>d.LoggerType== LoggerType.Error).Count());
            return 0;
        }
    }
}
