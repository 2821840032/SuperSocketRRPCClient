using Castle.DynamicProxy;
using SuperSocketClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSocketAOPClientContainer
{
    public class AOPRPCInterceptor : StandardInterceptor
    {
        Func<IInvocation, object> Implement { get; set; }
        public AOPRPCInterceptor(Func<IInvocation, object> implement) {
            this.Implement = implement;
        }
        protected override void PreProceed(IInvocation invocation)
        {
            Console.WriteLine(invocation.Method.Name + "执行前,入参：" + string.Join(",", invocation.Arguments));
        }

        protected override void PerformProceed(IInvocation invocation)
        {
            Console.WriteLine(invocation.Method.Name + "执行中");
            try
            {
                invocation.ReturnValue=Implement.Invoke(invocation);
            }
            catch (Exception ex)
            {
                HandleException(ex);
              
            }
        }

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
