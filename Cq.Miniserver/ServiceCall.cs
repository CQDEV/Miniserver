namespace Cq.Miniserver
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class ServiceCall
    {
        public static ServiceObjectCollection Objects = new ServiceObjectCollection();

        public static string GetJson(IEnumerable<ServiceObject> data)
        {
            var builder = new StringBuilder();
            builder.Append("[");

            foreach (var d in data)
            {
                builder.Append(d.Json + ",");
            }

            var result = builder.ToString();
            result = result.Substring(0, result.Length - 1);
            result += "]";

            return result;
        }

        public static byte[] GetBytes(string content)
        {
            return Encoding.ASCII.GetBytes(content);
        }

        public static byte[] GetBytes(IEnumerable<ServiceObject> data)
        {
            return GetBytes(GetJson(data));
        }

        public static void Process(Request request, Response response)
        {
            // process request
            var pathSplit = request.Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (pathSplit.Length < 2)
            {
                response.ErrorApplication();

                return;
            }

            var container = pathSplit[1].ToLower();

            var id = string.Empty;

            if (pathSplit.Length > 2)
            {
                id = pathSplit[2];
            }

            // process response
            response.ContentType = "application/json";

            // create
            if (request.Method == "post")
            {
                var newId = Objects.Create(container, Encoding.ASCII.GetString(request.Content));

                response.Content = GetBytes("{" + "\"id\":\"" + newId + "\"}");

                return;
            }

            // read / query
            if (request.Method == "get")
            {
                if (string.IsNullOrEmpty(id))
                {
                    // it's a query
                    response.Content = GetBytes(Objects.Query(container));
                }
                else
                {
                    // it's a read
                    response.Content = GetBytes(Objects.Read(container, ulong.Parse(id)).Json);
                }

                return;
            }

            if (string.IsNullOrEmpty(id))
            {
                response.ErrorApplication();

                return;
            }

            // update
            if (request.Method == "put")
            {
                Objects.Update(container, ulong.Parse(id), Encoding.ASCII.GetString(request.Content));

                response.Content = GetBytes("{}");
                return;
            }

            // delete
            if (request.Method == "delete")
            {
                Objects.Delete(container, ulong.Parse(id));

                response.Content = GetBytes("{}");
                return;
            }
        }
    }
}
