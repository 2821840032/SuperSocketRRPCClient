using System;
using System.Collections.Generic;
using System.Text;

namespace SuperSocketRRPCClient.Entity
{
    /// <summary>
    /// 提供服务的基础类 如果你需要用到其中的信息的话
    /// </summary>
   public class BaseProvideServices
    {
        /// <summary>
        /// 操作连接对象
        /// </summary>
        public SocketClientMain Socket { get; set; }

        /// <summary>
        /// 本次任务的信息
        /// </summary>
        public RequestExecutiveInformation Info { get; set; }

        /// <summary>
        /// 最基础的请求信息
        /// </summary>
        public RequestBaseInfo RequestInfo { get; set; }

    }
}
