using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;

namespace JumaCoin.Business.classes.Sockets
{
    public class AsyncClient
    {
        public int Port;
        private string host;
        private INode observer;

        public AsyncClient() : this("127.0.0.1", (new System.Random()).Next(10000, 15000), null) {}

        public AsyncClient(string host, int port, INode observer)
        {
            this.Port = port;
            this.host = host;
            this.observer = observer;
        }

        public async void SendMessageToServer(string server, int port, string data)
        {
            try {
                //System.Console.WriteLine("CLIENT Sending " + data);
                Task<string> tsResponse = SendRequest(server, port, data);
                //System.Console.WriteLine("Sent request, waiting for response");
                await tsResponse;
                if(observer != null)
                    observer.ReceiveMessageFromServer(tsResponse.Result);
            }
            catch (Exception ex) {
                System.Console.WriteLine("ERROR: " + ex.Message);
            }
        }

        private static async Task<string> SendRequest(string server, int port, string data)
        {
            try {
                TcpClient client = new TcpClient();
                await client.ConnectAsync(server, port); // Connect
                NetworkStream networkStream = client.GetStream();
                StreamWriter writer = new StreamWriter(networkStream);
                StreamReader reader = new StreamReader(networkStream);
                writer.AutoFlush = true;
                await writer.WriteLineAsync(data);
                string response = await reader.ReadLineAsync();
                client.Close();
                return response;
            }
            catch (Exception ex) {
                return ex.Message;
            }
        }
    }

}