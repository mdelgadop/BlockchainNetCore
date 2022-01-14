using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;

namespace JumaCoin.Business.classes.Sockets
{
    public class AsyncService
    {
        private IPAddress ipAddress;
        public int Port;
        private string host;
        public string Host { get {return ipAddress == null ? host : ipAddress.ToString();} }
        private INode observer;

        public AsyncService(string host, int port, INode observer)
        {
            this.Port = port;
            this.host = host;
            this.observer = observer;
            string hostName = host;//Dns.GetHostName();
            IPHostEntry ipHostInfo = Dns.GetHostEntry(hostName);
            this.ipAddress = null;
            for (int i = 0; i < ipHostInfo.AddressList.Length; ++i) {
                if (ipHostInfo.AddressList[i].AddressFamily ==
                  AddressFamily.InterNetwork)
                {
                  this.ipAddress = ipHostInfo.AddressList[i];
                  break;
                }
            }
            if (this.ipAddress == null)
                throw new Exception("No IPv4 address for server");
        }

        public async void Run()
        {
          TcpListener listener = new TcpListener(this.ipAddress, this.Port);
          listener.Start();
          Console.Write("SERVER running on " + this.ipAddress + ":" + this.Port + ". Hit <enter> to stop service\n");
          while (true) {
            try {
              TcpClient tcpClient = await listener.AcceptTcpClientAsync();
              Task t = Process(tcpClient);
              await t;
            }
            catch (Exception ex) {
              Console.WriteLine(ex.Message);
            }
          }
        }

        private async Task Process(TcpClient tcpClient) {
            string clientEndPoint = tcpClient.Client.RemoteEndPoint.ToString();
            try {
                NetworkStream networkStream = tcpClient.GetStream();
                StreamReader reader = new StreamReader(networkStream);
                StreamWriter writer = new StreamWriter(networkStream);
                writer.AutoFlush = true;
                while (true) {
                string request = await reader.ReadLineAsync();
                if (request != null) {
                    //Console.WriteLine("SERVER - Received service request from " + clientEndPoint + ":" + request);
                    string response = Response(request);
                    //Console.WriteLine("SERVER - Answer: " + response + "\n");
                    await writer.WriteLineAsync(response);
                }
                else
                    break; // Client closed connection
                }
                tcpClient.Close();
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                if (tcpClient.Connected)
                tcpClient.Close();
            }
        }
        
        private string Response(string request)
        {
            return observer.ReceiveMessageFromClient(request);
        }
        
    }
}