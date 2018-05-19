using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace assignment12
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("UDP or TCP ? ");
                var conaction = Console.ReadLine();

                if (conaction == "UDP")
                {
                    var localEndPoint = new IPEndPoint(IPAddress.Parse("10.255.230.177"), 4000);
                    var clientSocket = new Socket(SocketType.Dgram, ProtocolType.Udp);

                    clientSocket.Bind(localEndPoint);

                    var remuteEndPoint = new IPEndPoint(IPAddress.Parse("10.255.230.177"), 3000);

                    while (true)
                    {
                        Console.WriteLine("operator: first_value: second_value=");
                        string smth = Console.ReadLine();
                        
                        var send = localEndPoint + "@" + smth + "@";
                        Byte[] sendBytes = Encoding.ASCII.GetBytes(send);
                        clientSocket.SendTo(sendBytes, remuteEndPoint);
                        


                        string data = null;
                        byte[] buffer = new byte[2048]; // read in chunks of 2KB
                        int bytesRead;

                        while ((bytesRead = clientSocket.Receive(buffer)) > 0)
                        {

                            string p = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                            data += p;
                            if (p.Contains("@"))
                                break;

                        }
                        
                        Console.WriteLine("result=" + data.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries)[0]);

                    }
                }
                else if (conaction == "TCP")
                {
                    var client = new TcpClient();

                    client.Connect(new IPEndPoint(IPAddress.Parse("10.255.230.177"), 3000));

                    using (NetworkStream stream = client.GetStream())
                    using (StreamReader rd = new StreamReader(stream))
                    using (StreamWriter wr = new StreamWriter(stream))
                    {
                        var localEndPoint = client.Client.LocalEndPoint.ToString();
                        while (true)
                        {
                            Console.WriteLine("operator: first_value: second_value=");
                            string smth = Console.ReadLine();
                            wr.WriteLine(localEndPoint + "@" + smth + "@");
                            wr.Flush();

                            string data = null;

                            byte[] buffer = new byte[2048]; // read in chunks of 2KB
                            int bytesRead;

                            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {

                                string p = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                                data += p;
                                if (p.Contains("@"))
                                    break;

                            }

                            Console.WriteLine("result=" + data.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                        }
                    }
                    break;
                }
                else
                {
                    Console.WriteLine("Wrrong input.");
                }
            }
        }
    }
}
