// See https://aka.ms/new-console-template for more information
using MyScoketSpace;
using System;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;

Console.WriteLine("Hello, World!");

//获取域名的ip
IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("localhost");
IPAddress ipAddress = ipHostInfo.AddressList[0];

var localIp = Dns.GetHostName();

//socket demo
{
    //其他服务可具有 1,024 到 65,535 范围内的注册端口号
    var ipEndPoint = new IPEndPoint(ipAddress, 11_000);

    var server = new SocketServer(ipEndPoint);
    server.BindListenAsync();

    var client = new SocketClient(ipEndPoint);
    await client.ConnectAsync();
    await Task.Run(async () =>
    {
        for (int i = 0; i < 10; i++)
        {
            await client.SendAsync($"msg: <|EOM|>{i}");
        }
    });
}

namespace MyScoketSpace
{
    internal class SocketClient:IDisposable
    {
        private IPEndPoint iPEndPoint;
        private bool disposedValue;
        private readonly Socket socket;
        public SocketClient(IPEndPoint iPEndPoint)
        {
            this.iPEndPoint = iPEndPoint;
            socket = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }
        public SocketClient(long ipAddress, int port)
        {
            iPEndPoint = new IPEndPoint(ipAddress, port);
            socket = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }
        public SocketClient(IPAddress ipAddress, int port)
        {
            iPEndPoint = new IPEndPoint(ipAddress, port);
            socket = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public async Task ConnectAsync()
        {
            await socket.ConnectAsync(iPEndPoint);
        }
        public async Task<object> SendAsync(string msg)
        {
            if (!socket.Connected)
                await ConnectAsync();

            var msgBytes = Encoding.UTF8.GetBytes(msg);
            _ = await socket.SendAsync(msgBytes, SocketFlags.None);
            Console.WriteLine($"send success {msg}");
            string res = string.Empty;
            //receive
            while (true)
            {
                var buffer = new byte[1_024];
                var received = await socket.ReceiveAsync(buffer, SocketFlags.None);
                res = Encoding.UTF8.GetString(buffer, 0, received);
                if (res == "<|ACK|>")
                {
                    Console.WriteLine($"recevied success {res}");
                    break;
                }
            }
            return res;
        }
        public void Shutdown()
        {
            //此操作会关闭发送和接收操作
            socket.Shutdown(SocketShutdown.Both);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~SocketClient()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    internal class SocketServer:IDisposable
    {
        private IPEndPoint ipEndPoint;
        private Socket listener;
        public SocketServer(IPEndPoint iPEndPoint)
        {
            this.ipEndPoint = iPEndPoint;
            listener = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }
        public async Task BindListenAsync()
        {
            listener.Bind(ipEndPoint);
            listener.Listen(ipEndPoint.Port);
            var handler = await listener.AcceptAsync();
            while (true)
            {
                // Receive message.
                var buffer = new byte[1_024];
                var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);

                var eom = "<|EOM|>";
                if (response.IndexOf(eom) > -1 /* is end of message */)
                {
                    Console.WriteLine(
                        $"Socket server received message: \"{response.Replace(eom, "")}\"");

                    var ackMessage = "<|ACK|>";
                    var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
                    await handler.SendAsync(echoBytes, 0);
                    Console.WriteLine(
                            $"Socket server sent acknowledgment: \"{ackMessage}\"");
                    //退出的话只执行一次服务监听，不退出一直监听
                    //break;
                }
            }
            Console.WriteLine("server end");
        }

        public void Dispose()
        {
            listener.Dispose();
        }
    }
}