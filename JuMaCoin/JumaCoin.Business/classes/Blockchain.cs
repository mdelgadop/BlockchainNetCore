using System.Collections.Generic;
using JumaCoin.Business.classes.Helpers;
using JumaCoin.Business.classes.Sockets;

namespace JumaCoin.Business.classes
{
    public class Blockchain : Node
    {
        public List<Block> Blocks { get; set; }

        public List<Transaction> CurrentTransactions { get; set; }

        private const int Difficulty  = 4; //se exigen 4 ceros al inicio del hash para validad la prueba de trabajo

        private string PublicKey { get; set; }

        private string PrivateKey { get; set; }

        public Blockchain() : base()
        {
            InitializeBlockchain();
        }

        public Blockchain(string host, int portClient, int portServer) : base(host, portClient, portServer)
        {
            InitializeBlockchain();
        }

        private void InitializeBlockchain()
        {
            Blocks = new List<Block>();
            CurrentTransactions = new List<Transaction>();
            GenerateKeys();
        }

        public Block NewBlock()
        {
            string previousHash = string.Empty;
            if(Blocks.Count > 0)
                previousHash = Blocks[Blocks.Count - 1].Hash;
            
            Block newBlock = new Block(Blocks.Count, System.DateTime.Now, CurrentTransactions, previousHash);
            newBlock.MineBlock(Difficulty);
            Blocks.Add(newBlock);
            CurrentTransactions = new List<Transaction>();

            return newBlock;
        }

        public void NewTransaction(string sender, string receiver, decimal amount)
        {
            Transaction newTransaction = new Transaction(sender, receiver, amount);
            newTransaction.Id = CurrentTransactions.Count;
            newTransaction.Sign(PublicKey, PrivateKey);
            if(newTransaction.IsValid())
                CurrentTransactions.Add(newTransaction);
        }

        private void GenerateKeys()
        {
            string[] keys = RSAHelper.GenerateKeys();

            PublicKey = keys[0];
            PrivateKey = keys[1];
        }

        #region Nodes management

        //Pregunta a los nodos vecinos por sus cadenas para compararla con la local
        public void PerformConsensus()
        {
            string message = "Hola " + this.Host + ":" + this.PortClient + ". ¿Podrias pasarme tu cadena, por favor?";
            this.SendMessageBroadcast(message);
        }

        //Este nodo recibe la petición de un cliente para que le pase su cadena, y el nodo responde con la cadena
        public override string ReceiveMessageFromClient(string messageReceived)
        {
            return this.ToString();
        }

        //Tras habérsela pedido, este nodo recibe la cadena. Hay que compararla con la actual.
        public override void ReceiveMessageFromServer(string messageReceived)
        {
            //System.Console.WriteLine("Bien! Cadena recibida: " + messageReceived);

            Blockchain blockchain = FromString(messageReceived);

            //si el tamaño de la caden es mayor, reemplazamos nuestra cadena por la copia recibida
            if(blockchain.Blocks.Count > Blocks.Count)
            {
                string prevHash = string.Empty;
                Blocks = blockchain.Blocks;
                foreach(Block block in Blocks)
                {
                    foreach(Transaction transaction in block.Data)
                    {
                        transaction.Sign(PublicKey, PrivateKey);
                    }
                    block.MineBlock(Difficulty);
                    block.PreviousHash = prevHash;
                    prevHash = block.Hash;
                }

            }
        }

        #endregion Nodes management

        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize<Blockchain>(this);
        }

        private Blockchain FromString(string blockchainJson)
        {
            //System.Console.WriteLine("RECIBIDO: " + blockchainJson);
            Blockchain blockchain = System.Text.Json.JsonSerializer.Deserialize<Blockchain>(blockchainJson);
            //System.Console.WriteLine("DESERIALIZADO: " + blockchain.ToString());
            return blockchain;
        }   
    }
}