using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;
using SuperSocketRRPCClient;
using SuperSocketRRPCClient.Entity;

namespace SuperSocketAOPClientContainer
{

    /// <summary>
    /// AOP 容器对象
    /// </summary>
    public class AOPContainer
    {
         
      
        ProxyGenerator generator { get; set; }
        
        /// <summary>
        /// 容器对象
        /// </summary>
        public AOPContainer()
        {
            generator = new ProxyGenerator();
           
        }

        /// <summary>
        /// 获取远程类
        /// </summary>
        /// <param name="session">需要通讯的对象</param>
        /// <param name="serverType">需要实例化的类型</param>
        /// <returns></returns>
        public object GetServices(SocketClientMain session, Type serverType)
        {
            return generator.CreateInterfaceProxyWithoutTarget(serverType, new AOPRPCInterceptor((invocation) => {
                return ImplementFunc(invocation, session);
            }));
        }

        /// <summary>
        /// 获取远程类
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="session">需要通讯的对象</param>
        /// <returns></returns>
        public T GetServices<T>(SocketClientMain session) where T:class
        {
            return generator.CreateInterfaceProxyWithoutTarget<T>(new AOPRPCInterceptor((invcation)=> {
               return ImplementFunc(invcation, session);
            }));
        }

        /// <summary>
        /// 调用实现函数
        /// </summary>
        /// <param name="invocation">信息</param>
        /// <param name="session">连接对象</param>
        /// <returns></returns>
        public object ImplementFunc(IInvocation invocation, SocketClientMain session)
        {
            RequestExecutiveInformation information = new RequestExecutiveInformation()
            {
                FullName = invocation.Method.DeclaringType.FullName,
                ID = Guid.NewGuid(),
                MethodName = invocation.Method.Name,
                Arguments = invocation.Arguments.Select(d => JsonConvert.SerializeObject(d)).ToList()
            };
            //在这里不能使用task  会极大的阻塞线程的执行 原因未知
            var result = session.RemoteCallQueue.AddTaskQueue(information, session);
            session.RemoteCallQueue.RemoteExecutionFunc(result);


            result.WaitHandle.WaitOne();
            switch (result.State)
            {
                case ReceiveMessageState.Wait:
                    throw new Exception("任务出现错误，目前正在等待状态，却通过了健康检查");
                case ReceiveMessageState.Success:
                    var obj = JsonConvert.DeserializeObject(result.ReturnValue, invocation.Method.ReturnType);
                    return obj;
                case ReceiveMessageState.Overtime:
                    throw new Exception("任务超时：" + result.ReturnValue);
                case ReceiveMessageState.Error:
                    throw new Exception("任务出现了异常：" + result.ReturnValue);
                case ReceiveMessageState.Other:
                    throw new Exception("不存在的任务");
                default:
                    break;
            }
            throw new Exception("任务出现了异常：它没有按规矩走");
        }
    }


}
