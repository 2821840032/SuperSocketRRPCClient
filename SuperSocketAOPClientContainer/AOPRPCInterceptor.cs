using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSocketAOPClientContainer
{
    /// <summary>
    /// 
    /// </summary>
    public class AOPRPCInterceptor : StandardInterceptor
    {
        Func<IInvocation, object> Implement { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="implement"></param>
        public AOPRPCInterceptor(Func<IInvocation, object> implement) {
            this.Implement = implement;
        }
       /// <summary>
       /// 请求前
       /// </summary>
       /// <param name="invocation"></param>
        protected override void PreProceed(IInvocation invocation)
        {
            Console.WriteLine(invocation.Method.Name + "执行前,入参：" + string.Join(",", invocation.Arguments));
        }

        /// <summary>
        /// 请求中
        /// </summary>
        /// <param name="invocation"></param>
        protected override void PerformProceed(IInvocation invocation)
        {
            try
            {
                invocation.ReturnValue=Implement.Invoke(invocation);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw ex;
              
            }
        }

        /// <summary>
        /// 请求后
        /// </summary>
        /// <param name="invocation"></param>
        protected override void PostProceed(IInvocation invocation)
        {
            Console.WriteLine(invocation.Method.Name + "执行后，返回值：" + invocation.ReturnValue);
        }

        private void HandleException(Exception ex)
        {
            Console.WriteLine("发送了错误" + ex);
        }
    }
}
