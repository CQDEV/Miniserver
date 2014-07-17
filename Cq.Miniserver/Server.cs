namespace Cq.Miniserver
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    public class Server
    {
        private TcpListener listener;

        public Server()
        {
            this.listener = new TcpListener(IPAddress.Any, 6636);
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
                    catch
                    {
                        // 500
                    }

                    stream.Dispose();
                });
        }
    }
}