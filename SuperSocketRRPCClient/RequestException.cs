using System;
using System.Collections.Generic;
using System.Text;

namespace SuperSocketRRPCClient
{
    /// <summary>
    /// 请求异常处理类
    /// </summary>
    public abstract class RequestException
    {
        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="e">异常</param>
        public abstract void HandleException(Exception e);
    }
}
