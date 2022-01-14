using System;
using System.Collections.Generic;

namespace JumaCoin.Business.classes.Sockets
{
    // State object for reading client data asynchronously  
    public interface INode
    {
        string ReceiveMessageFromClient(string messageReceived);

        void ReceiveMessageFromServer(string messageReceived);
    }  
}