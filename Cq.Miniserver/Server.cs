﻿namespace Cq.Miniserver
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    public class Server
    {
        private TcpListener listener;

        public Server()
        {
            this.listener = new TcpListener(IPAddress.Loopback, 6636);
            this.listener.Start();

            this.ProcessLoop();
        }

        private void ProcessLoop()
        {
            ThreadPool.QueueUserWorkItem(
                delegate
                {
                    while (true)
                    {
                        this.ProcessClient(this.listener.AcceptTcpClient());
                    }
                });
        }

        private const int BufferSize = 1024 * 1024;

        private void ProcessClient(TcpClient client)
        {
            ThreadPool.QueueUserWorkItem(
                delegate
                {
                    var stream = client.GetStream();

                    try
                    {
                        var buffer = new byte[BufferSize];
                        var length = stream.Read(buffer, 0, BufferSize);

                        if (length == BufferSize)
                        {
                            Console.WriteLine("Request too big");
                            // 500? 
                        }

                        if (length > 0)
                        {
                            var request = new Request(buffer, length);
                            var response = new Response(request);
                            var header = response.Header;

                            stream.Write(header, 0, header.Length);

                            if (response.Content != null)
                            {
                                stream.Write(response.Content, 0, response.Content.Length);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // 500
                        Console.WriteLine("Error: {0}\r\n{1}", ex.Message, ex.StackTrace);
                    }

                    stream.Dispose();
                });
        }
    }
}