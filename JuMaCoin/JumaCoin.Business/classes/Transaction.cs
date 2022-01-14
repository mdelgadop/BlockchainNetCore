using System;
using System.Text;
using JumaCoin.Business.classes.Helpers;

namespace JumaCoin.Business.classes
{
    public class Transaction
    {
        public long Id { get; set; }
        
        public DateTime Timestamp { get; set; }

        public string Sender { get; set; }

        public string Receiver { get; set; }

        public decimal Amount { get; set; }

        public string Hash { get {return HashHelper.Calculate(string.Format("{0}{1}{2}{3}", Sender, Receiver, Amount, Timestamp.ToString()));} }

        public string PublicKey { get; set; }

        private byte[] Signature { get; set; }

        public Transaction(string sender, string receiver, decimal amount)
        {
            Sender = sender;
            Receiver = receiver;
            Amount = amount;
            Timestamp = System.DateTime.Now;
        }
    
        public void Sign(string publicKey, string privateKey)
        {
            PublicKey = publicKey;
            Signature = RSAHelper.Sign(Hash, privateKey);
        }

        public bool IsValid()
        {
            return RSAHelper.Verify(Hash, Signature, PublicKey);
        }

        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize<Transaction>(this);
        }

   }
}