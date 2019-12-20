using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSocketClient
{
    /// <summary>
    /// 接受信息最基础的类
    /// </summary>
    public class RequestBaseInfo : IPackageInfo
    {
        public byte[] body { get; set; }
        public RequestBaseInfo(byte[] body)
        {
            this.body = body;
        }
        public string bodyMeg => Encoding.UTF8.GetString(body);
    }
}
