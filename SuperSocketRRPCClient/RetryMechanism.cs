using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SuperSocketRRPCClient
{
    /// <summary>
    /// 重试机制
    /// </summary>
    public class RetryMechanism
    {
        SocketClientMain socket { get; set; }
        /// <summary>
        /// 重试机制
        /// </summary>
        /// <param name="socket"></param>
        public RetryMechanism(SocketClientMain socket)
        {
            this.socket = socket;
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
        public async Task ConnectionInit(bool tryreConnect = false)
        {
            if (socket.IsConnected || (TryreConnect && !tryreConnect))
            {
                return;
            }

            await Task.Yield();
            var resl = socket.ConnectAsync(socket.RemoteEndpoint).Result;
            if (resl)
            {
                socket.Log("连接成功");
                TryreConnect = false;
            }
            else
            {
                TryreConnect = true;
                socket.Log("连接服务器失败 5秒后重连", LoggerType.Error);
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(500);
                    Console.Write(".");
                }
                Console.WriteLine("");

                ConnectionInit(true).Wait();
            }
        }

        /// <summary>
        /// 进行连接 
        /// </summary>
        public void Connection()
        {
            if (socket.IsConnected)
            {
                socket.Log("连接已完成，取消本次请求", LoggerType.Warning);
                return;
            }
            if (TryreConnect)
            {
                socket.Log("已有线程在进行重试", LoggerType.Warning);
                return;
            }
            ConnectionInit();
        }


        /// <summary>
        /// 连接关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void onClose(object sender, EventArgs e)
        {
            socket.Log("连接关闭 尝试重新连接", LoggerType.Warning);
            ConnectionInit();
        }
        
        /// <summary>
        /// 连接异常
        /// </summary>
        /// <param name="o"></param>
        /// <param name="errorEvent"></param>
        public void onError(object o, ErrorEventArgs errorEvent)
        {
            socket.Log("连接异常：" + errorEvent.Exception.Message, LoggerType.Error);
            socket.Log("尝试重新连接", LoggerType.Warning);
            ConnectionInit();
        }

    }
}
