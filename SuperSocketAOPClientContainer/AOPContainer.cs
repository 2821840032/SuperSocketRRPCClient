using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;
using SuperSocketClient;

namespace SuperSocketAOPClientContainer
{

    public class AOPContainer
    {
         
        /// <summary>
        /// 侦听事件 问题
        /// </summary>
        private EventWaitHandle _waitHandle;
        ProxyGenerator generator { get; set; }
        public AOPContainer()
        {
            generator = new ProxyGenerator();
            _waitHandle = new AutoResetEvent(false);
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
        object ImplementFunc(IInvocation invocation, SocketClientMain session) {
            RequestExecutiveInformation information = new RequestExecutiveInformation()
            {
                FullName=invocation.Method.DeclaringType.FullName,
                ID=Guid.NewGuid(),
                MethodName=invocation.Method.Name,
                Arguments=invocation.Arguments.Select(d=>JsonConvert.SerializeObject(d)).ToList()
            };
            string returnValueString=string.Empty;
            session.MethodIDs.Add(information.ID, (msgValue)=> {
                    returnValueString = msgValue;
                    _waitHandle.Set();
                });
            var msg = JsonConvert.SerializeObject(information);
            session.SendMessage(msg);
            _waitHandle.WaitOne();
            session.MethodIDs.Remove(information.ID);

            var obj = JsonConvert.DeserializeObject(returnValueString, invocation.Method.ReturnType);
            return obj;
        }
    }
   
}
