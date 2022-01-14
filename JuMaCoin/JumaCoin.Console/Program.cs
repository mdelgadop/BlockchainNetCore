using System;
using System.Security.Cryptography;
using System.Text;
using JumaCoin.Business.classes;
using JumaCoin.Business.classes.Helpers;
using JumaCoin.Business.classes.Helpers.SerializeHelpers;

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
            

            /*string a = "{asdfg},{qwerty},{pepepepe},{caca}";
            string[] array = a.Split(",");
            foreach(string v in array)
            {
                System.Console.WriteLine(v);
            }*/

            //string tagini = "<amount>";
            //string tagfin = "</amount>";

            //string amount = a.Substring(a.IndexOf(tagini) + tagini.Length, a.IndexOf(tagfin, a.IndexOf(tagini)) - (a.IndexOf(tagini) + tagini.Length));
            


            Blockchain nodo1 = new Blockchain("127.0.0.1", 10001, 15001);
            /*Blockchain nodo2 = new Blockchain("127.0.0.1", 10002, 15002);

            nodo1.RegisterNode(nodo2.Host, nodo2.PortServer);
            nodo2.RegisterNode(nodo1.Host, nodo1.PortServer);*/
            
            nodo1.NewTransaction("Mario", "Laura", 5);
            nodo1.NewTransaction("Laura", "Mario", 3);
            nodo1.NewBlock();

            nodo1.NewTransaction("Mario", "Laura", 2);
            nodo1.NewTransaction("Laura", "Mario", 1);
            nodo1.NewBlock();

            //System.Console.WriteLine(nodo1.ToString());

            //Blockchain caca = System.Text.Json.JsonSerializer.Deserialize<Blockchain>(nodo1.ToString());
            
            System.Console.WriteLine("---------------------------------------------------------");
            string base64Result = System.Text.Json.JsonSerializer.Serialize<Blockchain>(nodo1);
            System.Console.WriteLine(base64Result);
            System.Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            Blockchain b2 = (new BlockchainSerializeHelper()).Deserialize(base64Result);

            System.Console.WriteLine("---------------------------------------------------------");
            //string b2str = System.Text.Json.JsonSerializer.Serialize<Blockchain>(b2);
            System.Console.WriteLine(b2.Blocks.Count);
            //System.Console.WriteLine("---------------------------------------------------------");
        }

    }
}
