using SuperSocketRRPCClient.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuperSocketRRPCClient.AttributeEntity
{
    /// <summary>
    /// 请求过滤器
    /// </summary>
    public abstract class  RequestFilterAttribte:Attribute
    {
        /// <summary>
        /// 标签  仅仅用来做注释使用
        /// </summary>
        public string Label { get; set; }


        /// <summary>
        /// 请求前
        /// </summary>
        /// <param name="baseProvideServices">实例化对象</param>
        /// <returns>false 则不执行，true 则通过</returns>
        public abstract bool BeforeExecution(BaseProvideServices baseProvideServices);

        /// <summary>
        /// 请求后
        /// </summary>
        /// <param name="baseProvideServices">实例化对象</param>
        /// <param name="result">值</param>
        /// <returns>false 则不发送结果值 true则通过</returns>
        public abstract bool Afterxecution(BaseProvideServices baseProvideServices,object result);
    }
}
