using SuperSocketRRPCClient.AttributeEntity;
using SuperSocketRRPCClient.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPCClientService
{
    public class TestAtter : RequestFilterAttribte
    {

        public override void Afterxecution(BaseProvideServices baseProvideServices, ref object result)
        {
            Console.WriteLine("请求后");
        }

        public override void BeforeExecution(BaseProvideServices baseProvideServices, ref object result)
        {
            result = "1";
            Console.WriteLine("请求前");
        }
    }
}
