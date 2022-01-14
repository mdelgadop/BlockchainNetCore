using System;

namespace JumaCoin.Business.classes.Helpers.SerializeHelpers
{
    public class BlockchainSerializeHelper : JumaCoin.Business.classes.Helpers.SerializeHelper<Blockchain>
    {
        private BlockSerializeHelper BlockSerialize { get; set; }

        public BlockchainSerializeHelper()
        {
            BlockSerialize = new BlockSerializeHelper();
        }

        public override Blockchain Deserialize(string data)
        {
            return BlockchainToConsole(data);
        }

        private Blockchain BlockchainToConsole(string json)
        {
            System.Collections.Generic.IList<Block> blockList = new System.Collections.Generic.List<Block>();
            int ini = json.IndexOf("\"Blocks\":") + "\"Blocks\":".Length;

            System.Collections.Stack symbols = new System.Collections.Stack();
            symbols.Push(json[ini++]);
            string currentObject = string.Empty;

            while(symbols.Count > 0)
            {
                char c = json[ini];
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
                    char k = (char)symbols.Pop();
                    if(symbols.Count > 1 || c == '}' || c == ']')
                    {
                        currentObject = currentObject + c;
                    }

                    if(symbols.Count == 1 && !string.IsNullOrEmpty(currentObject))
                    {
                        Block b = BlockSerialize.Deserialize(currentObject);
                        currentObject = string.Empty;
                        blockList.Add(b);
                    }
                }
                else
                {
                    currentObject = currentObject + c;
                }

                ini++;
            }

            Blockchain bc = new Blockchain();
            bc.Blocks = blockList;
            return bc;
        }

    }
}