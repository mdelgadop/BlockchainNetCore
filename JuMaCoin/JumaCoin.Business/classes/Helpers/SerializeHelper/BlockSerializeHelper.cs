using System;

namespace JumaCoin.Business.classes.Helpers.SerializeHelpers
{
    public class BlockSerializeHelper : JumaCoin.Business.classes.Helpers.SerializeHelper<Block>
    {
        private TransactionSerializeHelper TransactionSerializer { get; set; }

        public BlockSerializeHelper()
        {
            TransactionSerializer = new TransactionSerializeHelper();
        }

        public override Block Deserialize(string data)
        {
            return BlockToConsole(data);
        }

        private Block BlockToConsole(string json)
        {
            int id = 0;
            DateTime timestamp = DateTime.MinValue;
            string hash = string.Empty;
            string previousHash = string.Empty;
            int proof = 0;
            System.Collections.Generic.IList<Transaction> transactionList = new System.Collections.Generic.List<Transaction>();

            System.Collections.Stack symbols = new System.Collections.Stack();
            string currentObject = string.Empty;
            foreach(char c in json)
            {
                if(c == '{' || c == '[')
                {
                    symbols.Push(c);
                    if(symbols.Count > 1)
                    {
                        currentObject = currentObject + c;
                    }
                }
                else if(c == '}' || c == ']')
                {
                    symbols.Pop();
                    if(symbols.Count > 1 || c == ']')
                    {
                        currentObject = currentObject + c;
                    }
                }
                else if(symbols.Count == 1 && c == ',')
                {
                    ExtraerCampo(ref id, ref timestamp, ref hash, ref previousHash, ref proof, ref transactionList, currentObject);
                    currentObject = string.Empty;
                }
                else
                {
                    currentObject = currentObject + c;
                }

                if(symbols.Count == 0)
                {
                    ExtraerCampo(ref id, ref timestamp, ref hash, ref previousHash, ref proof, ref transactionList, currentObject);
                    currentObject = string.Empty;
                }
            }

            Block b = new Block(id, timestamp, transactionList, previousHash);
            b.Hash = hash;
            b.Proof = proof;
            return b;
        }

        private void ExtraerCampo(ref int id, ref DateTime timestamp, ref string hash, ref string previousHash, ref int proof, ref System.Collections.Generic.IList<Transaction> transactionList, string currentObject)
        {
            string[] valores = currentObject.Split(':');
            if (valores[0] == "\"Id\"")
                id = int.Parse(valores[1]);
            else if (valores[0] == "\"Proof\"")
                proof = int.Parse(valores[1]);
            else if (valores[0] == "\"Timestamp\"")
            {
                int i = 1;
                string str = valores[i++];
                while(i < valores.Length)
                    str = str + ":" + valores[i++];

                timestamp = DateTime.Parse(str.Substring(1, str.Length - 2));
            }
            else if (valores[0] == "\"PreviousHash\"")
                previousHash = valores[1].Substring(1, valores[1].Length - 2);
            else if (valores[0] == "\"Hash\"")
                hash = valores[1].Substring(1, valores[1].Length - 2);
            else if (valores[0] == "\"Data\"")
            {
                int i = 1;
                string str = valores[i++];
                while(i < valores.Length)
                    str = str + ":" + valores[i++];

                i = 1;
                while(i < str.Length)
                {
                    while(i < str.Length && str[i] != '{') {i++;}
                    int j = i;
                    while(j < str.Length && str[j] != '}') {j++;}
                    if(i < str.Length)
                    {
                        Transaction t = TransactionSerializer.Deserialize(str.Substring(i, j - i + 1));
                        transactionList.Add(t);
                    }
                    i = j;
                }
            }
        }

    }
}