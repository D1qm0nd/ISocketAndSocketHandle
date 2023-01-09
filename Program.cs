using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Text;


namespace ISocketAndSocketHandle
{
    class Program
    {
        private const string BYE = ".Bye";
        private const string STOP = ".Stop Server";
        public static OwnSocket socket = new OwnSocket();
        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 4 )
                {
                    throw new Exception("Incorrect count of arguments\nip - (string), port - (int), queue - (int), type - (server/client)");
                }

                socket.InitializeSocket(args[0], Convert.ToInt32(args[1]), Convert.ToInt32(args[2]), AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                Task task;
                switch (args[3])
                {
                    case "server":
                        socket.func += ServerInstructions;
                        task = socket.asyncStart(100);
                        task.Wait();
                        break;
                    case "client":
                        socket.func += ClientInstructions;
                        task = socket.asyncStart(100);
                        task.Wait();
                        break;
                    default:
                        throw new Exception("Incorrect type");
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        //Инструкции в случае реализации сервера
        static bool ServerInstructions(int timeOut) 
        {
            bool isWorking = true;

            try
            {
                //Привязка
                socket.TaskSocket.Bind(socket.SocketEndPoint);

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"Hosting server at: {socket.TaskSocket.LocalEndPoint}\nMax queue: {socket.QueueLenght+1}");

                //Прослушивание 
                socket.TaskSocket.Listen(socket.QueueLenght);

                StringBuilder message = new StringBuilder();
                Socket Listener;
                try
                {
                    while (isWorking) //TODO: Выход из цикла обработки сокета
                    {
                        Listener = socket.TaskSocket.Accept();

                        if (Listener.Connected)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"[Connected: {Listener.RemoteEndPoint} {Listener.ProtocolType} {Listener.SocketType}]");
                            Task SendingTask = asyncSendHandle();
                        }
                        Console.ForegroundColor = ConsoleColor.Green;

                        while (Listener.Connected && isWorking)
                        {
                            message.Clear();
                            byte[] buffer = new byte[1024];
                            int recvsize = 0;
                            
                            while (Listener.Available > 0)
                            {
                                Listener.Receive(buffer);
                                message.Append(Encoding.UTF8.GetString(buffer));
                            }
                            recvsize = message.ToString().Trim('\0').Length;

                            if (recvsize > 0)
                            {
                                Console.WriteLine($"{Listener.RemoteEndPoint} " + message.ToString().Trim('\0'));
                            }

                            if (message.ToString().CompareTo(BYE) == 0)
                            {
                                Listener.Disconnect(false);
                            }

                            if (message.ToString().CompareTo(STOP) == 0)
                            {
                                isWorking = false;
                            }
                            Thread.Sleep(timeOut);
                        }
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"[Disconnected: {Listener.RemoteEndPoint}]");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                    #region internal functions
                    void SendHandle()
                    {
                        
                        while (isWorking)
                        {
                            try
                            {
                                //KeyDown
                                if (Console.KeyAvailable)
                                {
                                    string serv_message = Console.ReadLine();
                                    if (Listener.Connected)
                                    { 
                                        Listener.Send(Encoding.UTF8.GetBytes(serv_message));
                                    }

                                    if (serv_message.CompareTo(STOP) == 0)
                                    {
                                        isWorking = false;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(e.Message);
                            }
                        }
                        
                    }
                    async Task asyncSendHandle()
                    {
                        await Task.Run(() => SendHandle());
                    }
                    #endregion
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine($"[Server at {socket.TaskSocket.LocalEndPoint} Closed]");
                socket.TaskSocket.Close();
                Console.ReadLine();
            }
            return true;
        }

        //Инструкции в случае реализации клиента
        static bool ClientInstructions(int timeOut)
        {
            bool isConnected = true;

            try
            {

                socket.TaskSocket.Connect(socket.SocketEndPoint);

                if (socket.TaskSocket.Connected)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"[Connected: {socket.TaskSocket.LocalEndPoint} {socket.TaskSocket.ProtocolType} {socket.TaskSocket.SocketType}]");
                    Console.ForegroundColor = ConsoleColor.Blue;

                    Task RecieveTask = asyncRecieveHandle();
                }

                while (socket.TaskSocket.Connected && isConnected)
                {
                    if (Console.KeyAvailable)
                    {
                        byte[] buffer = new byte[1024];
                        string message = "";
                        do
                            message = Console.ReadLine();
                        while (message == "");
                        buffer = Encoding.UTF8.GetBytes(message);
                        socket.TaskSocket.Send(buffer);
                        if ((message.CompareTo(BYE) == 0 || message.CompareTo(STOP) == 0) || !isConnected)
                        {
                            socket.TaskSocket.Disconnect(false);
                        }
                    }
                    Thread.Sleep(timeOut);
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[Disconnected from {socket.TaskSocket.RemoteEndPoint}]");

                #region internal functions
                void RecieveHandle()
                {
                    while (socket.TaskSocket.Connected)
                    {
                        if (socket.TaskSocket.Available > 0)
                        {
                            int recvsize = 0;
                            byte[] serv_message = new byte[1024];
                            while (socket.TaskSocket.Available > 0)
                            {
                                socket.TaskSocket.Receive(serv_message);
                            }
                            string message = Encoding.UTF8.GetString(serv_message);
                            message = message.Trim('\0');
                            recvsize = message.Length;
                            Console.WriteLine($"{socket.TaskSocket.RemoteEndPoint} "+message);
                            if (message.CompareTo(BYE) == 0)
                            {
                                isConnected = false;
                            }
                        }
                    }
                }
                async Task asyncRecieveHandle()
                {
                    await Task.Run(() => RecieveHandle());
                }
                #endregion
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
            }
            finally
            {

                socket.TaskSocket.Disconnect(false);
                socket.TaskSocket.Shutdown(SocketShutdown.Both);
                socket.TaskSocket.Close();
                Console.ReadLine();
            }
            return true;
        }
    }
}
