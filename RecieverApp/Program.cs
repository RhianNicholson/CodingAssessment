using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NetMQ;
using NetMQ.Sockets;
using CodingAssessment.Models;

namespace RecieverApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Just keep open and listen, kinda brute forcing it so would want a cleaner way in prod code.
            using (var server = new ResponseSocket())
            {
                server.Bind("tcp://*:5556");
                while (true)
                {
                    var msg = server.ReceiveFrameBytes();

                    MemoryStream ms = new MemoryStream();
                    BinaryFormatter bf = new BinaryFormatter();
                    ms.Write(msg, 0, msg.Length);
                    ms.Seek(0, SeekOrigin.Begin);
                    People person = (People)bf.Deserialize(ms);

                    Console.WriteLine($"Id: {person.Id} FirstName: {person.FirstName} LastName: {person.LastName} City: {person.City} State: {person.State} Country: {person.Country}");

                    server.SendFrame("Ok");
                }
            }
        }
    }
}
