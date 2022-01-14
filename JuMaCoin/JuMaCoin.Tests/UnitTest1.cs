using Microsoft.VisualStudio.TestTools.UnitTesting;
using JumaCoin.Business.classes;
using System.Collections.Generic;
using System.Linq;
using JumaCoin.Business.classes.Helpers.SerializeHelpers;

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
        public void TestSerializationTransaction()
        {
            #region Create node for testing
            Blockchain nodo1 = new Blockchain("127.0.0.1", 10001, 15001);

            nodo1.NewTransaction("Mario", "Laura", 5);
            nodo1.NewTransaction("Laura", "Mario", 3);
            nodo1.NewBlock();

            #endregion Create node for testing

            Transaction transactionOriginal = nodo1.Blocks[0].Data[0];

            TransactionSerializeHelper transactionSerializer = new TransactionSerializeHelper();
            string transactionOriginalStr = transactionSerializer.Serialize(transactionOriginal);
            Assert.IsFalse(string.IsNullOrEmpty(transactionOriginalStr));

            Transaction transactionCopy = transactionSerializer.Deserialize(transactionOriginalStr);
            Assert.IsNotNull(transactionCopy);

            string transactionCopyStr = transactionSerializer.Serialize(transactionCopy);
            Assert.IsFalse(string.IsNullOrEmpty(transactionCopyStr));
            
            Assert.IsTrue(transactionCopyStr.CompareTo(transactionOriginalStr) == 0);
        }

        [TestMethod]
        public void TestSerializationBlock()
        {
            #region Create node for testing
            Blockchain nodo1 = new Blockchain("127.0.0.1", 10002, 15002);

            nodo1.NewTransaction("Mario", "Laura", 5);
            nodo1.NewTransaction("Laura", "Mario", 3);
            nodo1.NewBlock();

            #endregion Create node for testing

            Block blockOriginal = nodo1.Blocks[0];

            BlockSerializeHelper blockSerializer = new BlockSerializeHelper();
            string blockOriginalStr = blockSerializer.Serialize(blockOriginal);
            Assert.IsFalse(string.IsNullOrEmpty(blockOriginalStr));

            Block blockCopy = blockSerializer.Deserialize(blockOriginalStr);
            Assert.IsNotNull(blockCopy);

            string blockCopyStr = blockSerializer.Serialize(blockCopy);
            Assert.IsFalse(string.IsNullOrEmpty(blockCopyStr));
            
            Assert.IsTrue(blockCopyStr.CompareTo(blockOriginalStr) == 0);
        }

        [TestMethod]
        public void TestSerializationBlockchain()
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

            BlockchainSerializeHelper blockchainSerializer = new BlockchainSerializeHelper();
            string blockOriginalStr = blockchainSerializer.Serialize(nodo1);
            Assert.IsFalse(string.IsNullOrEmpty(blockOriginalStr));

            Blockchain nodo1Copy = blockchainSerializer.Deserialize(blockOriginalStr);
            Assert.IsNotNull(nodo1Copy);

            //Number of blocks are the same
            Assert.AreEqual(nodo1.Blocks.Count, nodo1Copy.Blocks.Count);

            //Blocks are the same
            BlockSerializeHelper blockSerializer = new BlockSerializeHelper();
            Assert.AreEqual(blockSerializer.Serialize(nodo1.Blocks[0]), blockSerializer.Serialize(nodo1Copy.Blocks[0]));
            Assert.AreEqual(blockSerializer.Serialize(nodo1.Blocks[1]), blockSerializer.Serialize(nodo1Copy.Blocks[1]));
        }

    }
}
