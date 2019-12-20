using Newtonsoft.Json;
using SuperSocket.ClientEngine;
using SuperSocketRRPCUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SuperSocketClient
{
   public class SocketClientMain: EasyClient
    {
        /// <summary>
        /// 容器对象
        /// </summary>
        private UnityInIt unityCon { get; set; }

        IPEndPoint RemoteEndpoint { get; set; }
        /// <summary>
        /// 执行事件中的方法
        /// </summary>
        public Dictionary<Guid, Action<string>> MethodIDs { get; set; }
        public SocketClientMain(string ip, int prot) {
            MethodIDs = new Dictionary<Guid, Action<string>>();
            unityCon = new UnityInIt();
            Error += onError;
            Closed += onClose;
            RemoteEndpoint = new IPEndPoint(IPAddress.Parse(ip), prot);
            Initialize(new MyReceiveFilter(), onReceived);

            

            ConnectionInit().Wait();


        }
        private async Task ConnectionInit() {
            await Task.Yield();
            var resl = ConnectAsync(RemoteEndpoint).Result;
            if (resl)
            {
                Console.WriteLine("连接成功");
            }
            else
            {
                while (true)
                {
                    Console.WriteLine("连接失败 5秒后重连");
                    await Task.Delay(50000);
                    ConnectionInit().Wait();
                }
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
            var info = JsonConvert.DeserializeObject<RequestExecutiveInformation>(stringPackageInfo.bodyMeg);
            if (info.ReturnValue != null && MethodIDs.TryGetValue(info.ID, out var action))
            {
                action.Invoke(info.ReturnValue);
                //得到执行结果
            }
            else
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

            //session.MethodIDs
            Console.WriteLine("Server:" + stringPackageInfo.bodyMeg);
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
