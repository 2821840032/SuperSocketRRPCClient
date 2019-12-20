using System;
using System.Collections.Generic;
using System.Text;

namespace SuperSocketRRPCClient.Entity
{
    /// <summary>
    /// 接收信息的状态
    /// </summary>
    public enum ReceiveMessageState
    {
        Wait=0,
        Success=1,
        Overtime=2,
        Error =3,
        Other=4,
    }
}
