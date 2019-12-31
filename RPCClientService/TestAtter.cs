using SuperSocketRRPCClient.AttributeEntity;
using SuperSocketRRPCClient.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPCClientService
{
    public class TestAtter : RequestFilterAttribte
    {
        public override bool Afterxecution(BaseProvideServices baseProvideServices, object result)
        {
            Console.WriteLine("请求后");
         
            return true;
        }

        public override bool BeforeExecution(BaseProvideServices baseProvideServices)
        {
            Console.WriteLine("请求前");
            return true;
        }
    }
}
