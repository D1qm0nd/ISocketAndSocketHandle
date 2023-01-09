using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ISocketAndSocketHandle
{
    interface ISocketHandle
    {
        /// <summary>
        /// Свойство
        /// Айпи-адрес компьютера-хоста
        /// </summary>
        string IP { get; set; }
        /// <summary>
        /// Свойство
        /// Порт который необходимо открыть
        /// </summary>
        int Port { get; set; }
        /// <summary>
        /// Свойство
        /// Максимальная длина очереди на подключение
        /// </summary>
        int QueueLenght { get; set; }
        AddressFamily addressFamily { get; set; }
        SocketType socketType { get; set; }
        ProtocolType protocolType { get; set; }

        /// <summary>
        /// Точка подключения к сокету
        /// </summary>
        EndPoint SocketEndPoint { get; set; }

        /// <summary>
        /// Сокет по которому осуществляется обмен данными
        /// </summary>
        Socket TaskSocket { get; set; }

        void InitializeSocket(string ip, int port, int queue, AddressFamily addrFam, SocketType sockType, ProtocolType protocol);
    }
}
