namespace Cq.Miniserver
{
    using System;
    using System.Text;
    
    public class Request
    {
        public string Method { get; private set; }
        public string Path { get; private set; }
        public string Version { get; private set; }

        public byte[] Content { get; private set; }

        public Request(byte[] buffer, int length)
        {
            var i = 1;
            var x = false;

            for (i = 3; i < length && !x; i++)
            {
                var one = buffer[i - 3];
                var two = buffer[i - 2];
                var thr = buffer[i - 1];
                var fou = buffer[i];

                if (one == (byte)13 && two == (byte)10 && thr == (byte)13 && fou == (byte)10)
                {
                    x = true;
                }
            }

            var headers = Encoding.ASCII.GetString(buffer, 0, i).Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var requestLine = headers[0].Split(' ');

            this.Method = requestLine[0];
            this.Path = requestLine[1];
            this.Version = requestLine[2];

            this.Content = new byte[length - i];

            Array.Copy(buffer, i, this.Content, 0, length - i);
        }
    }
}
