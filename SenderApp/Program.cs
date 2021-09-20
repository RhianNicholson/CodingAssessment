using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NetMQ;
using NetMQ.Sockets;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;
using CodingAssessment.Models;

namespace SenderApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var listOfPeople = new List<People>();

            using (TextFieldParser parser = new TextFieldParser(@"people.csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                int count = 0;
                while (!parser.EndOfData)
                {
                    if (count > 0)
                    {
                        string[] row = parser.ReadFields();
                        listOfPeople.Add(new People
                        {
                            // This assumes that it will always be an int, else id use a tryparse
                            Id = int.Parse(row[0]),
                            FirstName = row[1],
                            LastName = row[2],
                            City = row[3],
                            State = row[4],
                            Country = row[5]
                        });
                    }
                    else
                    {
                        // Just skipping the first line, there are other ways for sure
                        parser.ReadLine();
                    }
                    count += 1;
                }
            }


            using (var client = new RequestSocket())
            {
                client.Connect("tcp://localhost:5556");
                foreach (var item in listOfPeople)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    MemoryStream ms = new MemoryStream();
                    bf.Serialize(ms, item);
                    client.SendFrame(ms.ToArray());
                    client.ReceiveFrameString();
                }
            }
        }
    }
}
