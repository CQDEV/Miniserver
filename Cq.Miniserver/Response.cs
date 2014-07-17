﻿namespace Cq.Miniserver
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Text;

    public class Response
    {
        public static Dictionary<string, string> ContentTypeMapping = new Dictionary<string, string> 
        { 
            { "html", "text/html" },
            { "js", "text/javascript" },
            { "css", "text/css" },
        };

        public static Dictionary<string, byte[]> Cache = new Dictionary<string, byte[]>();

        public const string Version = "HTTP/1.1";
        public string StatusCode { get; private set; }
        public string Status { get; private set; }

        public string ContentType { get; private set; }

        public byte[] Content { get; private set; }

        public string ContentLength
        {
            get
            {
                if (this.Content == null)
                {
                    return "0";
                }

                return this.Content.Length.ToString();
            }
        }

        public byte[] Header
        {
            get
            {
                var builder = new StringBuilder();

                builder.AppendLine(string.Format("{0} {1} {2}", Version, this.StatusCode, this.Status));
                builder.AppendFormat("content-type: {0}\r\n", this.ContentType);
                builder.AppendFormat("content-length: {0}\r\n\r\n", this.ContentLength);

                return Encoding.ASCII.GetBytes(builder.ToString());
            }
        }

        public Response(Request request)
        {
            var path = request.Path;

            this.StatusCode = "200";
            this.Status = "OK";
            this.ContentType = "application/octet-stream";

            if (path.ToLower().StartsWith("/$"))
            {
                // process service here & return
                return;
            }

            if (path == "/")
            {
                path = "/index.html";
            }

            var filePath = string.Format(
                "{0}{1}",
                ConfigurationManager.AppSettings["root"],
                path.Replace('/', '\\')).ToLower();

            if (!File.Exists(filePath))
            {
                this.StatusCode = "404";
                this.Status = "Not found.";
                return;
            }

            var extension = filePath.Substring(filePath.LastIndexOf('.') + 1);

            if (ContentTypeMapping.ContainsKey(extension))
            {
                this.ContentType = ContentTypeMapping[extension];
            }

            this.Content = File.ReadAllBytes(filePath);
        }
    }
}