using Microsoft.VisualStudio.TestTools.UnitTesting;
using JumaCoin.Business.classes;
using System.Collections.Generic;
using System.Linq;
using JumaCoin.Business.classes.Helpers;

namespace JuMaCoin.Tests
{
    //dotnet test "C:/Users/Mario Delgado/Desktop/MyDocumentation/Software/JuMaCoin/JuMaCoin.Tests/JuMaCoin.Tests.csproj"

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMyTest()
        {
            IList<string> data = new List<string>();
            data.Add("a");
            data.Add("b");
            Assert.AreEqual(data.Aggregate((i, j) => i + j), "ab");
        }

        [TestMethod]
        public void TestMyTest2()
        {
            string hash = "0123";
            string ceros = string.Empty.PadLeft(0, '0');
            Assert.IsTrue(hash.StartsWith(ceros));
            ceros = string.Empty.PadLeft(1, '0');
            Assert.IsTrue(hash.StartsWith(ceros));
            ceros = string.Empty.PadLeft(2, '0');
            Assert.IsFalse(hash.StartsWith(ceros));
        }

        [TestMethod]
        public void TestNewBlock()
        {
            Blockchain blockchain = new Blockchain();
            Assert.IsNotNull( blockchain);
            blockchain.NewBlock();
            Assert.AreEqual(blockchain.Blocks.Count, 1);
            Assert.AreEqual(blockchain.Blocks[0].Id, 0);
            Assert.AreEqual(blockchain.Blocks[0].Data.Length, 0);
            Assert.IsTrue(string.IsNullOrEmpty(blockchain.Blocks[0].PreviousHash));
        }
        
        [TestMethod]
        public void TestAddBlock()
        {
            Blockchain blockchain = new Blockchain();
            Block block1 = blockchain.NewBlock();
            Block block2 = blockchain.NewBlock();
            Block block3 = blockchain.NewBlock();
            
            Assert.AreEqual(blockchain.Blocks.Count, 3);
            
            Assert.AreEqual(blockchain.Blocks[0].Hash, block1.Hash);
            Assert.AreEqual(blockchain.Blocks[1].Hash, block2.Hash);
            Assert.AreEqual(blockchain.Blocks[2].Hash, block3.Hash);

            Assert.IsTrue(block1.Hash.StartsWith("0000"));
            Assert.IsTrue(block2.Hash.StartsWith("0000"));
            Assert.IsTrue(block3.Hash.StartsWith("0000"));

            Assert.IsTrue(string.IsNullOrEmpty(block1.PreviousHash));
            Assert.AreEqual(block2.PreviousHash, block1.Hash);
            Assert.AreEqual(block3.PreviousHash, block2.Hash);
        }
    
        [TestMethod]
        public void TestNewTransaction() //dotnet test "C:/Users/Mario Delgado/Desktop/MyDocumentation/Software/JuMaCoin/JuMaCoin.Tests/JuMaCoin.Tests.csproj" --filter Name~TestNewTransaction
        {
            Blockchain blockchain = new Blockchain();
            blockchain.NewTransaction("Mario", "Laura", 5);
            Assert.AreEqual(blockchain.CurrentTransactions.Count, 1);
            Assert.AreEqual(blockchain.CurrentTransactions[0].Sender, "Mario");
            Assert.AreEqual(blockchain.CurrentTransactions[0].Receiver, "Laura");
            Assert.AreEqual(blockchain.CurrentTransactions[0].Amount, 5);
            
            blockchain.NewBlock();
            Assert.AreEqual(blockchain.CurrentTransactions.Count, 0);
            Assert.AreEqual(blockchain.Blocks.Count, 1);

            System.Console.WriteLine(blockchain.ToString());
        }
    
        [TestMethod]
        public void TestSerialization()
        {
            #region Create node for testing
            Blockchain nodo1 = new Blockchain("127.0.0.1", 10003, 15003);

            nodo1.NewTransaction("Mario", "Laura", 5);
            nodo1.NewTransaction("Laura", "Mario", 3);
            nodo1.NewBlock();

            nodo1.NewTransaction("Mario", "Laura", 2);
            nodo1.NewTransaction("Laura", "Mario", 1);
            nodo1.NewBlock();
            #endregion Create node for testing

            SerializeHelper<Blockchain> serializer = new SerializeHelper<Blockchain>();

            string nodo1Str = serializer.Serialize(nodo1);

            Blockchain nodo1Copy = serializer.Deserialize(nodo1Str);

            Assert.AreEqual(nodo1.Host, nodo1Copy.Host);
            Assert.AreEqual(nodo1.PortClient, nodo1Copy.PortClient);
            Assert.AreEqual(nodo1.PortServer, nodo1Copy.PortServer);
            Assert.AreEqual(nodo1.Blocks.Count, nodo1Copy.Blocks.Count);
            Assert.AreEqual(nodo1.Blocks[0].Hash, nodo1Copy.Blocks[0].Hash);
            Assert.AreEqual(nodo1.Blocks[0].Proof, nodo1Copy.Blocks[0].Proof);
            Assert.AreEqual(nodo1.Blocks[0].PreviousHash, nodo1Copy.Blocks[0].PreviousHash);
            Assert.AreEqual(nodo1.Blocks[0].Data.Length, nodo1Copy.Blocks[0].Data.Length);
            Assert.AreEqual(nodo1.Blocks[0].Data[0].Hash, nodo1Copy.Blocks[0].Data[0].Hash);
            Assert.AreEqual(nodo1.Blocks[0].Data[0].Amount, nodo1Copy.Blocks[0].Data[0].Amount);
            Assert.AreEqual(nodo1.Blocks[0].Data[0].Sender, nodo1Copy.Blocks[0].Data[0].Sender);
            Assert.AreEqual(nodo1.Blocks[0].Data[0].Receiver, nodo1Copy.Blocks[0].Data[0].Receiver);

        }

            
        [TestMethod]
        public void TestPerformConsensus()
        {
            #region Create node for testing
            Blockchain nodo1 = new Blockchain("127.0.0.1", 10001, 15001);
            Blockchain nodo2 = new Blockchain("127.0.0.1", 10002, 15002);

            nodo1.RegisterNode(nodo2.Host, nodo2.PortServer);
            nodo2.RegisterNode(nodo1.Host, nodo1.PortServer);
            
            nodo1.NewTransaction("Mario", "Laura", 5);
            nodo1.NewTransaction("Laura", "Mario", 3);
            nodo1.NewBlock();

            nodo1.NewTransaction("Mario", "Laura", 2);
            nodo1.NewTransaction("Laura", "Mario", 1);
            nodo1.NewBlock();
            #endregion Create node for testing

            nodo2.PerformConsensus();

            System.Threading.Thread.Sleep(5000);

            Assert.AreEqual(nodo2.Blocks.Count, 2);
            Assert.AreEqual(nodo2.Blocks[0].Data.Length, 2);
            Assert.AreEqual(nodo2.Blocks[1].Data.Length, 2);
            
            Assert.AreEqual(nodo2.Blocks[0].Data[0].Amount, nodo1.Blocks[0].Data[0].Amount);
            Assert.AreEqual(nodo2.Blocks[0].Data[0].Receiver, nodo1.Blocks[0].Data[0].Receiver);
            Assert.AreEqual(nodo2.Blocks[0].Data[0].Sender, nodo1.Blocks[0].Data[0].Sender);

            Assert.AreEqual(nodo2.Blocks[0].Data[1].Amount, nodo1.Blocks[0].Data[1].Amount);
            Assert.AreEqual(nodo2.Blocks[0].Data[1].Receiver, nodo1.Blocks[0].Data[1].Receiver);
            Assert.AreEqual(nodo2.Blocks[0].Data[1].Sender, nodo1.Blocks[0].Data[1].Sender);

            Assert.AreEqual(nodo2.Blocks[1].Data[0].Amount, nodo1.Blocks[1].Data[0].Amount);
            Assert.AreEqual(nodo2.Blocks[1].Data[0].Receiver, nodo1.Blocks[1].Data[0].Receiver);
            Assert.AreEqual(nodo2.Blocks[1].Data[0].Sender, nodo1.Blocks[1].Data[0].Sender);

            Assert.AreEqual(nodo2.Blocks[1].Data[1].Amount, nodo1.Blocks[1].Data[1].Amount);
            Assert.AreEqual(nodo2.Blocks[1].Data[1].Receiver, nodo1.Blocks[1].Data[1].Receiver);
            Assert.AreEqual(nodo2.Blocks[1].Data[1].Sender, nodo1.Blocks[1].Data[1].Sender);

        }
    }
}
