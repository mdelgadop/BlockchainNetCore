using System;
using System.Collections.Generic;
using JumaCoin.Business.classes.Helpers;
using System.Linq;

namespace JumaCoin.Business.classes
{
    public class Block
    {
        public long Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string Hash { get; set; }

        public string PreviousHash { get; set; }

        public int Proof { get; set; }

        public Transaction[] Data { get; set; }

        public Block(int index, DateTime timestamp, IList<Transaction> transactions, string previousHash)
        {
            Id = index;
            Timestamp = timestamp;
            Data = transactions != null ? transactions.ToArray() : new Transaction[0];
            PreviousHash = previousHash;
        }

        public int MineBlock(int difficulty)
        {
            Proof = 0;
            string data = string.Format("{0}{1}{2}{3}", Id, Timestamp.ToString(), Data.Length == 0 ? string.Empty : Data.Select(t => t.Hash).Aggregate((i, j) => i + j), Proof);
            while (!ValidProof(difficulty, data))
            {
                Proof++;
                data = string.Format("{0}{1}{2}{3}", Id, Timestamp.ToString(), Data.Length == 0 ? string.Empty : Data.Select(t => t.Hash).Aggregate((i, j) => i + j), Proof);
            }

            Hash = HashHelper.Calculate(data);
            
            return Proof;
        }

        private bool ValidProof(int difficulty, string data)
        {
            string hash = HashHelper.Calculate(data);
            string ceros = string.Empty.PadLeft(difficulty, '0');
 
            return hash.StartsWith(ceros);
        }
 
        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize<Block>(this);
            /*
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("{ ");

            sb.Append("\"Id\" : \"" + Id + "\"");
            sb.Append(", ");
            sb.Append("\"Timestamp\" : \"" + Timestamp.ToString() + "\"");
            sb.Append(", ");
            sb.Append("\"Hash\" : \"" + Hash + "\"");
            sb.Append(", ");
            sb.Append("\"PreviousHash\" : \"" + PreviousHash + "\"");
            sb.Append(", ");
            sb.Append("\"Proof\" : \"" + Proof + "\"");
            sb.Append(", ");
            
            sb.Append("\"Data\":[");
            bool firstTransaction = true;
            foreach(Transaction transaction in Data)
            {
                if(!firstTransaction)
                    sb.Append(", ");
                sb.Append(transaction.ToString());
                firstTransaction = false;
            }
            sb.Append("]");

            sb.Append("}");

            return sb.ToString();*/
        }

    }
}