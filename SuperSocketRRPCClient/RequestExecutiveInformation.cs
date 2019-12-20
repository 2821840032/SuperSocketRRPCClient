﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSocketClient
{
    /// <summary>
    /// 请求信息
    /// </summary>
    public class RequestExecutiveInformation
    {
        /// <summary>
        /// 本次请求的ID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// FullName名称
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 方法名称
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 参数列表
        /// </summary>
        public List<string> Arguments { get; set; }

        /// <summary>
        /// RPC调用结果 json
        /// </summary>
        public string ReturnValue { get; set; }


    }
}