using System;
using System.Collections.Generic;

namespace JumaCoin.Business.classes.Sockets
{
    public struct NodeServer
    {
        public string Host { get; set; }

        public int PortServer { get; set; }

        public NodeServer(string host, int portServer)
        {
            Host = host;
            PortServer = portServer;
        }
    }

    // State object for reading client data asynchronously  
    public abstract class Node : INode
    {
        private string host { get; set; }
        public string Host { get { return Server == null ? host : Server.Host; } }

        public int PortClient { get { return Client == null ? 0 : Client.Port;} }

        public int PortServer { get { return Server == null ? 0 : Server.Port;} }

        public AsyncClient Client { get; set; }

        public AsyncService Server { get; set; }

        public List<NodeServer> NodesServer { get; set; }

        public Node() : this("127.0.0.1", (new System.Random()).Next(10000, 15000), (new System.Random()).Next(15000, 20000))
        {
            
        }

        public Node(string host, int portClient, int portServer) : base()
        {
            this.host = host;
            NodesServer = new List<NodeServer>();

            Client = new AsyncClient(host, portClient, this);
            Server = new AsyncService(host, portServer, this);

            System.Threading.Thread thread1 = new System.Threading.Thread(Server.Run);
            thread1.Start();
        }

       ///Registra un nodo vecino
        public void RegisterNode(string host, int port)
        {
            NodesServer.Add(new NodeServer(host, port));
        }

        public void SendMessageBroadcast(string data)
        {
            if(NodesServer != null)
            {
                foreach(NodeServer nodeServer in NodesServer)
                {
                    System.Console.WriteLine("Llamando a " + nodeServer.Host + ":" + nodeServer.PortServer);
                    Client.SendMessageToServer(nodeServer.Host, nodeServer.PortServer, data);
                }
            }
        }

        public abstract string ReceiveMessageFromClient(string messageReceived);

        public abstract void ReceiveMessageFromServer(string messageReceived);
    }  
}