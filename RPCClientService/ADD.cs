using IRPCClientService;
using System;

namespace RPCClientService
{
    public class ADD : IADD
    {
        void IADD.ADD(int x, int y)
        {
            Console.WriteLine(x+y);
        }
    }
}
