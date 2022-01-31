using System;
using System.Security.Cryptography;
using System.Text;
using JumaCoin.Business.classes;
using JumaCoin.Business.classes.Helpers;

using System.IO;
using System.Xml;
using System.Xml.Serialization;

/*
--SOLUCIÓN
dotnet new sln -o MyApiApp

--PROYECTOS
dotnet new classlib -o JumaCoin.Business
dotnet new console -o MyApiApp.ConsoleApp
dotnet new webapi -o MyApiApp.WebApi 
dotnet new classlib -o MyApiApp.Repository 
dotnet new xunit -o MyApiApp.Tests
dotnet new mstest -o MyApiApp.MSTests
*/
namespace JumaCoin.Console
{
    class Program
    {
        
        static void Main(string[] args)
        {
            //AGREGAR PROYECTO A LA SOLUCIÓN: dotnet sln JuMaCoin.sln add .\JumaCoin.Console\JumaCoin.Console.csproj
            //AGREGAR REFERENCIA: dotnet add JumaCoin.Console/JumaCoin.Console.csproj reference JumaCoin.Business/JumaCoin.Business.csproj
            //dotnet run --project ./JumaCoin.Console/JumaCoin.Console.csproj
            //dotnet run --project "C:/Users/Mario Delgado/Desktop/MyDocumentation/Software/JuMaCoin/JumaCoin.Console/JumaCoin.Console.csproj"
            
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

            nodo2.PerformConsensus();

            System.Threading.Thread.Sleep(5000);
            System.Console.WriteLine(nodo2.Blocks.Count);
            System.Console.WriteLine(nodo2.Blocks[0].Data.Length);
            System.Console.WriteLine(nodo2.Blocks[1].Data.Length);
            
            System.Console.WriteLine(nodo2.Blocks[0].Data[0].Amount);
            System.Console.WriteLine(nodo2.Blocks[0].Data[0].Receiver);
            System.Console.WriteLine(nodo2.Blocks[0].Data[0].Sender);

            System.Console.WriteLine(nodo2.Blocks[0].Data[1].Amount);
            System.Console.WriteLine(nodo2.Blocks[0].Data[1].Receiver);
            System.Console.WriteLine(nodo2.Blocks[0].Data[1].Sender);

            System.Console.WriteLine(nodo2.Blocks[1].Data[0].Amount);
            System.Console.WriteLine(nodo2.Blocks[1].Data[0].Receiver);
            System.Console.WriteLine(nodo2.Blocks[1].Data[0].Sender);

            System.Console.WriteLine(nodo2.Blocks[1].Data[1].Amount);
            System.Console.WriteLine(nodo2.Blocks[1].Data[1].Receiver);
            System.Console.WriteLine(nodo2.Blocks[1].Data[1].Sender);
        }

    }
}
