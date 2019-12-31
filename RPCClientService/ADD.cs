using IRPCClientService;
using System;
using SuperSocketRRPCClient.Entity;
using SuperSocketAOPClientContainer;
namespace RPCClientService
{
    [TestAtter]
    public class ADD : BaseProvideServices,IADD
    {
        public void Ma()
        {
            Console.WriteLine("转发请求来自"+this.RequestClientSession);
            AOPContainer aop = new AOPContainer();
            aop.GetServices<IADD>(Socket).Too("too");
        }

        public void Too(string message)
        {
            Console.WriteLine("转发请求来自"+this.RequestClientSession.ToString()+" Message "+message);
        }

        void IADD.ADD(int x, int y)
        {
            Console.WriteLine(x+y);
        }
    }
}
