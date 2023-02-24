// See https://aka.ms/new-console-template for more information
using MyScoketImplement;
using MyScoketSpace;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel.Design;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

Console.WriteLine("Hello, World!");

//获取域名的ip
IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("localhost");
IPAddress ipAddress = ipHostInfo.AddressList[0];

var localIp = Dns.GetHostName();

//socket demo
{
    //其他服务可具有 1,024 到 65,535 范围内的注册端口号
    var ipEndPoint = new LIPEndPoint(ipAddress, 11_000);
    var ipEndPoint1 = new LIPEndPoint(ipAddress, 11_001);

    //var server = new SocketServer(ipEndPoint);
    //server.BindListenAsync();
    //var client = SocketFactory.GetSocketClient();
    //var client = new SocketClient(ipEndPoint);
    //await client.ConnectAsync();
    //await Task.Run(async () =>
    //{
    //    for (int i = 0; i < 10; i++)
    //    {
    //        await client.SendAsync($"msg: <|EOM|>{i}");
    //    }
    //});



    var server = SocketFactory.GetSocketServer<SocketServerOne>(ipEndPoint);
    var server1 = SocketFactory.GetSocketServer<SocketServerOne>(ipEndPoint1);

    for (int i = 0; i < 3; i++)
    {
        Task.Run(async () =>
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("Same IpPort Thread Id " + threadId);
            SocketClient client = SocketFactory.GetSocketClient<SocketClientOne>(ipEndPoint);
            while (client is null)
            {
                client = SocketFactory.GetSocketClient<SocketClientOne>(ipEndPoint);
            }
            await client.ConnectAsync();
            for (int j = 0; j < 100; j++)
            {
                client.SendAsync($"thread{threadId} client{client.GetHashCode()} send msg:{j}");
                Thread.Sleep(1000*threadId*(j+1));
            }
            client.Shutdown();
        });
    }
    TaskFactory taskFactory = new TaskFactory();
    Task.Run(async () =>
    {
        var threadId = Thread.CurrentThread.ManagedThreadId;
        Console.WriteLine("another client Thread Id " + threadId);
        SocketClient client = SocketFactory.GetSocketClient<SocketClientOne>(ipEndPoint1);
        {
            await client.ConnectAsync();
            for (int j = 0; j < 100; j++)
            {
                client.SendAsync($"thread{threadId} another client{client.GetHashCode()} send msg:{j}");
                Thread.Sleep(3000);
            }
            client.Shutdown();
        }
    });
    server.BindListenAsync();
    await server1.BindListenAsync();
}

namespace MyScoketSpace
{
    public class SocketFactory
    {
        private static bool IsLockC = false;
        private static bool IsLockS = false;
        private static readonly object objC = new object();
        private static readonly object objS = new object();
        public static ConcurrentDictionary<LIPEndPoint, SocketClient> IPSocketClientDic = new();
        public static ConcurrentDictionary<LIPEndPoint, SocketServer> IPSocketServerDic = new();
        private static readonly SocketFactory socketFactory = new();
        public static SocketClient? GetSocketClient<T>(LIPEndPoint lsIPEndPoint) where T : SocketClient
        {
            #region 也可以不用，有tryadd
            //SocketClient? instance = default;
            //if (IsLockC == false)
            //{
            //    lock (objC)
            //    {
            //        IsLockC = true;
            //        if (IPSocketClientDic.TryGetValue(lsIPEndPoint, out var client))
            //        {
            //            return client;
            //        }
            //        else
            //        {
            //            IPEndPoint ipend = new IPEndPoint(lsIPEndPoint.Address, lsIPEndPoint.Port);
            //            instance = (SocketClient)Activator.CreateInstance(typeof(T), new object[] { ipend });
            //            if (instance is not null)
            //                IPSocketClientDic.TryAdd(lsIPEndPoint, instance);
            //            else
            //                throw new Exception("can~t create SocketClient");
            //        }
            //        IsLockC = false;
            //    }
            //}
            //return instance;
            #endregion
            SocketClient? instance = default;
            if (IPSocketClientDic.TryGetValue(lsIPEndPoint, out var client))
            {
                return client;
            }
            else
            {
                IPEndPoint ipend = new IPEndPoint(lsIPEndPoint.Address, lsIPEndPoint.Port);
                instance = (SocketClient)Activator.CreateInstance(typeof(T), new object[] { ipend });
                if (instance is not null)
                    IPSocketClientDic.TryAdd(lsIPEndPoint, instance);
                else
                    throw new Exception("can~t create SocketClient");
            }
            return instance;
        }
        public static SocketServer? GetSocketServer<T>(LIPEndPoint lsIPEndPoint) where T : SocketServer
        {
            SocketServer? instance = default;
            if (IsLockS == false)
            {
                lock (objS)
                {
                    IsLockS = true;
                    try
                    {
                        if (IPSocketServerDic.TryGetValue(lsIPEndPoint, out var client))
                        {
                            return client;
                        }
                        else
                        {
                            IPEndPoint ipend = new IPEndPoint(lsIPEndPoint.Address, lsIPEndPoint.Port);
                            instance = (SocketServer)Activator.CreateInstance(typeof(T), new object[] { ipend });
                            if (instance is not null)
                                IPSocketServerDic.TryAdd(lsIPEndPoint, instance);
                            else
                                throw new Exception("can~t create SocketServer");
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        IsLockS = false;
                    }
                }
            }
            return instance;
        }
    }

    public abstract class SocketClient : IDisposable
    {
        private IPEndPoint iPEndPoint;
        private bool disposedValue;
        protected virtual Socket socket { get; set; }
        //protected abstract EventHandler SendEvent { get; set; }
        /// <summary>
        /// 自定义接收消息后是否继续等待接收   res: false(处理后直接退出此次通信)   true(接着等待服务器返回数据继续运行委托)
        /// </summary>
        protected virtual Func<TRecvMessage, bool> RecvedHandle => (TRecvMessage recv) => false;
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

        public virtual async Task ConnectAsync()
        {
            await socket.ConnectAsync(iPEndPoint);
        }
        public virtual async Task<TRecvMessage?> SendAsync(TSendMessage msg)
        {
            if (!socket.Connected)
                await ConnectAsync();
            byte[] buff;
            //using (var ms = new MemoryStream())
            //{
            //    IFormatter formatter = new BinaryFormatter();
            //    formatter.Serialize(ms, msg);
            //    buff = ms.GetBuffer();
            //}
            //or
            var str = JsonConvert.SerializeObject(msg);
            buff = Encoding.UTF8.GetBytes(str);

            _ = await socket.SendAsync(buff, SocketFlags.None);
            Console.WriteLine($"send success {msg}");
            string res = string.Empty;
            TRecvMessage? recvRes;
            //receive
            while (true)
            {
                var buffer = new byte[1_024];
                var received = await socket.ReceiveAsync(buffer, SocketFlags.None);
                res = Encoding.UTF8.GetString(buffer, 0, received);
                recvRes = JsonConvert.DeserializeObject<TRecvMessage>(res);
                if (recvRes is not null)
                {
                    var continueRecv = RecvedHandle?.Invoke(recvRes);
                    Console.WriteLine($"recevied success {res}");
                    if (continueRecv == false)
                    {
                        this.Shutdown();
                        break;
                    }
                }
            }
            return recvRes;
        }
        public virtual async Task SendAsync(string str)
        {
            if (!socket.Connected)
                await ConnectAsync();
            byte[] buff;
            buff = Encoding.UTF8.GetBytes(str);
            _ = await socket.SendAsync(buff, SocketFlags.None);
            Console.WriteLine($"send success {str}");
            string res = string.Empty;
            TRecvMessage? recvRes;
            //receive
            while (false)
            {
                var buffer = new byte[1_024];
                var received = await socket.ReceiveAsync(buffer, SocketFlags.None);
                res = Encoding.UTF8.GetString(buffer, 0, received);
                Console.WriteLine($"recevied success {res}");
                break;
            }
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

        public override bool Equals(object? obj)
        {
            return obj is SocketClient client &&
                   EqualityComparer<IPEndPoint>.Default.Equals(iPEndPoint, client.iPEndPoint);
        }

        public override int GetHashCode()
        {
            //return HashCode.Combine(iPEndPoint);
            return iPEndPoint.Address.GetHashCode()^iPEndPoint.Port.GetHashCode();
        }
    }

    public abstract class SocketServer : IDisposable
    {
        private IPEndPoint ipEndPoint;
        private Socket listener;
        public SocketServer(IPEndPoint iPEndPoint)
        {
            this.ipEndPoint = iPEndPoint;
            listener = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }
        public virtual async Task BindListenAsync()
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

                //var eom = "<|EOM|>";
                if (!string.IsNullOrEmpty(response))//response.IndexOf(eom) > -1 /* is end of message */)
                {
                    Console.WriteLine(
                        $"Socket server received message: \"{response}\"");

                    var ackMessage = "<|ACK|>";
                    var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
                    await handler.SendAsync(echoBytes, 0);
                    Console.WriteLine(
                            $"Socket server sent acknowledgment: \"{ackMessage}\"");
                    //退出的话只执行一次服务监听，不退出一直监听
                    //break;
                }
            }
        }

        public virtual void Dispose()
        {
            listener.Dispose();
        }
    }

    public class LIPEndPoint : IPEndPoint
    {
        public LIPEndPoint(long address, int port) : base(address, port)
        {
        }

        public LIPEndPoint(IPAddress address, int port) : base(address, port)
        {
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            if (!(obj is LIPEndPoint))
                return false;
            var curEP = (LIPEndPoint)obj;
            return this.Address == curEP.Address && this.Port == curEP.Port;
        }

        public override int GetHashCode()
        {
            return this.Address.GetHashCode() ^ this.Port.GetHashCode();
        }
    }
    public abstract class TSendMessage { }
    public abstract class TRecvMessage { }

}


namespace MyScoketImplement
{

    public class SocketClientOne : SocketClient
    {
        protected override Socket socket { get => base.socket; set => base.socket = value; }


        public SocketClientOne(IPEndPoint iPEndPoint) : base(iPEndPoint)
        {
        }

        public SocketClientOne(long ipAddress, int port) : base(ipAddress, port)
        {
        }

        public SocketClientOne(IPAddress ipAddress, int port) : base(ipAddress, port)
        {
        }

        public override Task ConnectAsync()
        {
            return base.ConnectAsync();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }

    public class SocketServerOne : SocketServer
    {
        public SocketServerOne(IPEndPoint iPEndPoint) : base(iPEndPoint)
        {
        }
    }

    public class SendMessageDto : TSendMessage
    {
        public int ret { get; set; }
        public List<int> list { get; set; }
        public string message { get; set; }
    }
    public class RecvMessageDto : TRecvMessage
    {
        public int ret { get; set; }
        public List<string> message { get; set; }
    }

}
