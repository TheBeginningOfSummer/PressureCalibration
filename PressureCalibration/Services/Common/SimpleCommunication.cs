using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Calibration.Services
{
    public class SimpleCommunication
    {

    }

    public class TcpComm
    {
        public TcpComm() { }

        public TcpClient client = new();

        int BufferSize = 4096;

        public bool ConnectServer(string host, int port)
        {
            try
            {
                client.ConnectAsync(host, port).Wait(300);
                return client.Connected;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public void Close()
        {
            client.Close();
        }

        public bool IsOnline()
        {
            return !((client.Client.Poll(1000, SelectMode.SelectRead) && (client.Client.Available == 0)) || !client.Client.Connected);
        }

        public void SendMsg(string sendmsg, out string recvmsg)
        {
            recvmsg = "";

            //发送字符串
            byte[] buffer = Encoding.ASCII.GetBytes(sendmsg); //msg为发送的字符串   
            NetworkStream streamToServer = client.GetStream();
            streamToServer.WriteTimeout = 50;
            streamToServer.ReadTimeout = 50;

            try
            {
                lock (streamToServer)
                {
                    streamToServer.Write(buffer, 0, buffer.Length);// 发往服务器
                }
                //接收字符串
                buffer = new byte[BufferSize];

                lock (streamToServer)
                {
                    int bytesRead = streamToServer.Read(buffer, 0, BufferSize);
                    if (bytesRead > 0) recvmsg = Encoding.Default.GetString(buffer);
                }
            }
            catch (Exception e)
            {
                recvmsg = e.Message;
            }
        }

    }

    public class UdpComm
    {
        private UdpClient _udpClient;
        private IPEndPoint _remoteEndPoint;

        public UdpComm(string host, int port)
        {
            _udpClient = new UdpClient();
            _remoteEndPoint = new IPEndPoint(IPAddress.Parse(host), port);

            _udpClient.Client.SendTimeout = 100;
            _udpClient.Client.ReceiveTimeout = 100;
        }

        public void SendMessage(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            _udpClient.Send(buffer, buffer.Length, _remoteEndPoint);
        }

        public string ReceiveMessage()
        {
            byte[] buffer = _udpClient.Receive(ref _remoteEndPoint);
            return Encoding.UTF8.GetString(buffer);
        }

        public void Close()
        {
            _udpClient.Close();
        }
    }

}
