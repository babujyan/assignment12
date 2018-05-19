using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net;
using assignment12;

namespace Server
{ 
    class Program
    {
        static void Main(string[] args)
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("10.255.230.177"), 3000);

            var TCPTask = new Task(() =>
            {
                TcpListener listener = new TcpListener(localEndPoint);
                listener.Start();

                while (true)
                {
                    var client = listener.AcceptTcpClient();

                    var task = new Task((c) =>
                    {
                        var cl = (TcpClient)c;

                        using (NetworkStream stream = cl.GetStream())
                        using (StreamReader rd = new StreamReader(stream))
                        using (StreamWriter wr = new StreamWriter(stream))
                        {
                            while (true)
                            {
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

                                var d = data.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries)[1];

                                double result = MathOperetions(d);

                                wr.WriteLine(result + "@");
                                wr.Flush();
                            }


                        }
                    }, client);
                    task.Start();
                }
            });
            
            TCPTask.Start();

            var UDPTask = new Task(() =>
            {
                var clientSocket = new Socket(SocketType.Dgram, ProtocolType.Udp);

                clientSocket.Bind(localEndPoint);
                

                while (true)
                {
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

                    var d = data.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    var endPoint = data.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries)[0];

                    IPEndPoint remutEndPoint = new IPEndPoint(IPAddress.Parse(endPoint.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[0]), int.Parse(endPoint.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1]));

                    double result = MathOperetions(d);

                    
                    string send = result + "@";
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(send);
                    clientSocket.SendTo(sendBytes, remutEndPoint);
                }

            });
            UDPTask.Start();
            Console.ReadLine();
        }

        static double MathOperetions(string data)
        {
            var oper = data.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[0];
            var firstArg = data.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1];
            var secondArg = data.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[2];


            Double.TryParse(firstArg, out double first);
            Double.TryParse(secondArg, out double second);

            MathServer mathServer = new MathServer();

            switch(oper)
            {
                case "+":
                    return mathServer.Add(first,second);      
                    
                case "-":
                    return mathServer.Sub(first, second);
                   
                case "*":
                    return mathServer.Mult(first, second);
                    
                case "/":
                    return mathServer.Div(first, second);

                default:
                    throw new Exception("no such operator listed.");
            }
        }
    }
}
