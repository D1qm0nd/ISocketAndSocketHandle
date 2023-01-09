using System.Net;
using System.Net.Sockets;


namespace ISocketAndSocketHandle
{
    class SocketHandle : ISocketHandle
    {

        #region features
        public string IP { get; set; }
        public int Port { get; set; }
        public int QueueLenght { get; set; }
        public AddressFamily addressFamily { get; set; }
        public SocketType socketType { get; set; }
        public ProtocolType protocolType { get; set; }

        public Socket TaskSocket { get; set; }
        public EndPoint SocketEndPoint { get; set; }
        #endregion
        #region methods
        public void InitializeSocket(string ip, int port, int queue, AddressFamily addrFam, SocketType sockType, ProtocolType protocol)
        {
            IP = ip;
            Port = port;
            QueueLenght = queue-1;
            addressFamily = addrFam;
            socketType = sockType;
            protocolType = protocol;
            SocketEndPoint = new IPEndPoint(IPAddress.Parse(IP), Port);
            TaskSocket = new Socket(addressFamily, socketType, protocolType);
        }
        #endregion
    }
}
