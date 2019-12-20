using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSocketRRPCClient
{
    
    public class MyReceiveFilter : SuperSocket.ProtoBase.FixedHeaderReceiveFilter<RequestBaseInfo>
    {
        public MyReceiveFilter()
       : base(4)
        {

        }

        public override RequestBaseInfo ResolvePackage(IBufferStream bufferStream)
        {
            var last = bufferStream.Buffers.Last();
            var body = last.Array.Skip(last.Offset).Take(last.Count).ToArray();
            return new RequestBaseInfo(body);
        }

        protected override int GetBodyLengthFromHeader(IBufferStream bufferStream, int length)
        {
            //前四位是 长度 后面是内容
            return BitConverter.ToInt32(bufferStream.Buffers.Last().Take(4).ToArray(), 0);
        }
    }
}
