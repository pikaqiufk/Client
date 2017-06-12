
using System.Net;

namespace ScorpionNetLib
{
    public class SocketClientSettings
    {
        public SocketClientSettings(EndPoint endPoint)
        {
            BufferSize = 16384;
            NumberOfSaeaForRecSend = 2;
            ServerEndPoint = endPoint;
        }
        public int BufferSize { get; set; }

        public int NumberOfSaeaForRecSend { get; set; }

        public EndPoint ServerEndPoint { get; set; }
    }
}
