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
    /// <summary>
    /// Client SOcket
    /// </summary>
   public class SocketClientMain: EasyClient
    {
      
        /// <summary>
        /// 接收请求处理类
        /// </summary>
        private MonitorReceived monitorReceived { get; set; }
        /// <summary>
        /// 远程地址
        /// </summary>
        public IPEndPoint RemoteEndpoint { get;private set; }

        /// <summary>
        /// 远程任务队列
        /// </summary>
        public RemoteCallQueue RemoteCallQueue { get;private set; }

        RetryMechanism retryMechanism { get; set; }

        /// <summary>
        ///  创建一个连接对象
        /// </summary>
        /// <param name="ip">远程地址</param>
        /// <param name="prot">端口</param>
        /// <param name="maxRetryCount">最大超时后重试次数</param>
        /// <param name="second">超时时间</param>
        /// <param name="immediateConnection">是否为立即连接</param>
        public SocketClientMain(string ip, int prot,int second=10,int maxRetryCount=3, bool immediateConnection=true) {
            RemoteCallQueue = new RemoteCallQueue(second, maxRetryCount);

            monitorReceived = new MonitorReceived(this);

            retryMechanism = new RetryMechanism(this);

            Error += retryMechanism.onError;
            Closed += retryMechanism.onClose;
            RemoteEndpoint = new IPEndPoint(IPAddress.Parse(ip), prot);
            Initialize(new MyReceiveFilter(), monitorReceived.onReceived);

            retryMechanism.ConnectionInit().Wait();
        }


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(string message)
        {
            if (IsConnected)
            {
                Console.WriteLine("发送请求失败 当前状态未连接");
            }
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
            monitorReceived.AddServer<IT, T>();
        }
    }
}
