using Newtonsoft.Json;
using SuperSocket.ClientEngine;
using SuperSocketRRPCClient;
using SuperSocketRRPCUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SuperSocketRRPCClient
{
   public class SocketClientMain: EasyClient
    {
        /// <summary>
        /// 容器对象
        /// </summary>
        private UnityInIt unityCon { get; set; }

        IPEndPoint RemoteEndpoint { get; set; }

        /// <summary>
        /// 远程任务队列
        /// </summary>
        public RemoteCallQueue RemoteCallQueue { get;private set; }

        public SocketClientMain(string ip, int prot,int second) {
            RemoteCallQueue = new RemoteCallQueue(second);
            unityCon = new UnityInIt();
            Error += onError;
            Closed += onClose;
            RemoteEndpoint = new IPEndPoint(IPAddress.Parse(ip), prot);
            Initialize(new MyReceiveFilter(), onReceived);

            ConnectionInit().Wait();
        }
        /// <summary>
        /// 是否正在尝试重新连接
        /// </summary>
        bool TryreConnect = false;

        /// <summary>
        /// 是否为递归重试
        /// </summary>
        /// <param name="tryreConnect"></param>
        /// <returns></returns>
        private async Task ConnectionInit(bool tryreConnect=false) {
            if (IsConnected||(TryreConnect&& !tryreConnect))
            {
                return;
            }

            await Task.Yield();
            var resl = ConnectAsync(RemoteEndpoint).Result;
            if (resl)
            {
                Console.WriteLine("连接成功");
                TryreConnect = false;
            }
            else
            {
                TryreConnect = true;
                Console.WriteLine("连接服务器失败 5秒后重连");
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(500);
                    Console.Write(".");
                }
                Console.WriteLine("");
                    
                ConnectionInit(true).Wait();
            }
        }

        private void onClose(object sender, EventArgs e) {
            Console.WriteLine("连接关闭 尝试重新连接");
            ConnectionInit();
        }
        private void onError(object o, ErrorEventArgs errorEvent) {
            Console.WriteLine("连接异常："+errorEvent.Exception.Message);
            Console.WriteLine("尝试重新连接");
            ConnectionInit();
        }
        /// <summary>
        /// 收到请求事件
        /// </summary>
        /// <param name="stringPackageInfo"></param>
        private  void onReceived(RequestBaseInfo stringPackageInfo)
        {
            RequestExecutiveInformation info=null;
            try
            {
                info = JsonConvert.DeserializeObject<RequestExecutiveInformation>(stringPackageInfo.bodyMeg);
            }
            catch (Exception e)
            {
                Console.WriteLine("解析失败"+stringPackageInfo.bodyMeg);
                return;
            }
          
            if (info.ReturnValue != null && RemoteCallQueue.GetTaskIDAndSuccess(info.ID,info.ReturnValue))
            {
                //处理完成
            }
            else if (info.ReturnValue!=null)
            {
                Console.WriteLine($"收到一个意外的请求 它有结果但是没有找到该任务的信息 ID:{info.ID} FullName:{info.FullName} Return:{info.ReturnValue} 来自于:{RemoteEndpoint.ToString()}");
            }
            else
            {
                ImplementFunc(info, stringPackageInfo);
            }
        }

        /// <summary>
        /// 执行RPC的调用
        /// </summary>
        /// <param name="session"></param>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
         void ImplementFunc(RequestExecutiveInformation info,RequestBaseInfo requestInfo)
        {
            //接收RPC的请求
            var type = unityCon.GetService(info.FullName, out object executionObj);
            var methodType = type.GetMethod(info.MethodName);
            List<object> paraList = new List<object>();
            for (int i = 0; i < info.Arguments.Count; i++)
            {
                var paras = methodType.GetParameters();
                paraList.Add(JsonConvert.DeserializeObject(info.Arguments[i], paras[i].ParameterType));
            }
            info.ReturnValue = JsonConvert.SerializeObject(methodType.Invoke(executionObj, paraList.ToArray()));
            var msg = JsonConvert.SerializeObject(info);
            SendMessage(msg);
        }

        public void SendMessage(string message)
        {
            var dataBody = Encoding.UTF8.GetBytes(message);

            var dataLen = BitConverter.GetBytes(dataBody.Length);//int类型占4位，根据协议这里也只能4位，否则会出错

            var sendData = new byte[4 + dataBody.Length];//长度为4

            // +-------+-------------------------------+
            // |request|                               |
            // | name  |    request body               |
            // |  (4)  |                               |
            // |       |                               |
            // +-------+-------------------------------+

            Array.ConstrainedCopy(dataLen, 0, sendData, 0, 4);
            Array.ConstrainedCopy(dataBody, 0, sendData, 4, dataBody.Length);

            base.Send(sendData);
        }

        /// <summary>
        /// 为所有启动的服务器注册服务
        /// </summary>
        /// <typeparam name="IT"></typeparam>
        /// <typeparam name="T"></typeparam>
        public void AddServer<IT, T>()
           where IT : class
           where T : IT
        {
            unityCon.AddServer<IT, T>();
        }
    }
}
