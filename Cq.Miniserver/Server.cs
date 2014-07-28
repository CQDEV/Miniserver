namespace Cq.Miniserver
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;

    public class Server
    {
        private TcpListener listener;

        private Queue<string> logQueue;

        private DataContractSerializer serializer;

        protected DataContractSerializer Serializer
        {
            get
            {
                return this.serializer ?? (this.serializer = new DataContractSerializer(typeof(ServiceObjectCollection)));
            }
        }

        public Server()
        {
            this.listener = new TcpListener(
                IPAddress.Parse(ConfigurationManager.AppSettings["ip"]),
                int.Parse(ConfigurationManager.AppSettings["port"]));

            this.logQueue = new Queue<string>();

            if (File.Exists("data.xml"))
            {
                using (var stream = File.OpenRead("data.xml"))
                {
                    ServiceCall.Objects = (ServiceObjectCollection)this.Serializer.ReadObject(stream);
                }
            }

            Console.WriteLine("Service started: {0}", this.listener.LocalEndpoint);

            this.listener.Start();

            this.ProcessLoop();
            this.ProcessLog();
            this.ProcessStorage();
        }

        private void ProcessStorage()
        {
            ThreadPool.QueueUserWorkItem(
                delegate
                {
                    while (true)
                    {
                        this.logQueue.Enqueue(string.Format("[{0}] Storing", DateTime.Now.ToLongTimeString()));

                        lock (ServiceObjectCollection.Lock)
                        {
                            using (var stream = new MemoryStream())
                            {
                                this.Serializer.WriteObject(stream, ServiceCall.Objects);
                                stream.Flush();

                                File.WriteAllBytes("data.xml", stream.ToArray());
                            }
                        }

                        Thread.Sleep(10000);
                    }
                });
        }

        private void ProcessLog()
        {
            ThreadPool.QueueUserWorkItem(
                delegate
                {
                    while (true)
                    {
                        if (this.logQueue.Count > 0)
                        {
                            Console.WriteLine(this.logQueue.Dequeue());
                        }

                        Thread.Sleep(100);
                    }
                });
        }

        private void ProcessLoop()
        {
            ThreadPool.QueueUserWorkItem(
                delegate
                {
                    try
                    {
                        while (true)
                        {
                            this.ProcessClient(this.listener.AcceptTcpClient());
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[{0}] {1}", DateTime.Now.ToLongTimeString(), ex.Message);
                    }
                });
        }

        private const int RequestBufferSize = 1024 * 1024;

        private void ProcessClient(TcpClient client)
        {
            ThreadPool.QueueUserWorkItem(
                delegate
                {
                    var stream = client.GetStream();

                    var log = string.Format("[{0}] ", DateTime.Now.ToLongTimeString());

                    try
                    {
                        var requestBuffer = new byte[RequestBufferSize];
                        var length = stream.Read(requestBuffer, 0, RequestBufferSize);

                        if (length == RequestBufferSize)
                        {
                            Console.WriteLine("Request too big");
                            this.SendResponse(stream, Response.GetErrorResponse());
                        }

                        if (length > 0)
                        {
                            var request = new Request(requestBuffer, length);
                            log += string.Format("> {0} {1} ", request.Method, request.Path);

                            var response = new Response(request);

                            log += string.Format("< {0} {1} {2}", response.StatusCode, response.ContentType, response.ContentLength);

                            this.SendResponse(stream, response);
                        }
                        else
                        {
                            log += "Empty request";
                            this.SendResponse(stream, Response.GetErrorResponse());
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            this.SendResponse(stream, Response.GetErrorResponse());
                        }
                        catch { }

                        Console.WriteLine("Error: {0}", ex.Message);
                    }

                    this.logQueue.Enqueue(log);

                    stream.Dispose();
                });
        }

        private void SendResponse(Stream stream, Response response)
        {
            var header = response.Header;

            stream.Write(header, 0, header.Length);

            if (response.Content != null)
            {
                stream.Write(response.Content, 0, response.Content.Length);
            }
        }
    }
}