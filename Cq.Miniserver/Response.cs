namespace Cq.Miniserver
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Text;

    public class Response
    {
        private bool? isWindows;
        protected bool IsWindows
        {
            get
            {
                if (!this.isWindows.HasValue)
                {
                    var platform = Environment.OSVersion.Platform;

                    this.isWindows = platform != PlatformID.MacOSX && platform != PlatformID.Unix;
                }

                return this.isWindows.Value;
            }
        }

        public static Dictionary<string, byte[]> Cache = new Dictionary<string, byte[]>();

        public const string Version = "HTTP/1.1";
        public string StatusCode { get; set; }
        public string Status { get; set; }

        public string ContentType { get; set; }
        public byte[] Content { get; set; }

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

        public int CacheTime { get; set; }

        public byte[] Header
        {
            get
            {
                var builder = new StringBuilder();

                builder.AppendLine(string.Format("{0} {1} {2}", Version, this.StatusCode, this.Status));
                builder.AppendFormat("content-type: {0}\r\n", this.ContentType);
                builder.AppendFormat("content-length: {0}\r\n", this.ContentLength);
                builder.AppendFormat("cache-control: max-age={0}\r\n", this.CacheTime);
                builder.Append("server: Cq.Miniserver/1.0\r\n\r\n");

                return Encoding.ASCII.GetBytes(builder.ToString());
            }
        }

        private Response()
        {
        }

        private Response(string statusCode, string status)
            : this()
        {
            this.StatusCode = statusCode;
            this.Status = status;
        }

        public Response(Request request)
            : this("200", "OK")
        {
            var path = request.Path;

            if (path.ToLower().StartsWith("/$"))
            {
                ServiceCall.Process(request, this);

                return;
            }

            this.CacheTime = 86400;

            if (path == "/")
            {
                path = "/index.html";
            }

            var filePath = string.Format("{0}{1}", ConfigurationManager.AppSettings["root"], path).ToLower();

            if (this.IsWindows)
            {
                filePath.Replace('/', '\\');
            }

            if (!File.Exists(filePath))
            {
                this.ErrorNotFound();
                return;
            }

            var extension = filePath.Substring(filePath.LastIndexOf('.') + 1);

            if (!Mime.ContentTypeMapping.ContainsKey(extension))
            {
                this.ErrorApplication();
                return;
            }

            this.ContentType = Mime.ContentTypeMapping[extension];

            this.Content = File.ReadAllBytes(filePath);
        }

        private void ErrorNotFound()
        {
            this.StatusCode = "404";
            this.Status = "Not found.";
            this.Content = null;
            this.ContentType = null;
        }

        public void ErrorApplication()
        {
            this.StatusCode = "500";
            this.Status = "Application error.";
            this.Content = null;
            this.ContentType = null;
        }

        public static Response GetErrorResponse()
        {
            var response = new Response();
            response.ErrorApplication();

            return response;
        }
    }
}
