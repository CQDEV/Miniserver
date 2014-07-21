namespace Cq.Miniserver
{
    using System;
    using System.Text;

    public class ServiceCall
    {
        public static void Process(Request request, Response response)
        {
            // process request
            var pathSplit = request.Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (pathSplit.Length < 2)
            {
                response.ErrorApplication();

                return;
            }

            var collection = pathSplit[1];

            var item = string.Empty;

            if (pathSplit.Length > 2)
            {
                item = pathSplit[2];
            }

            // process response
            response.ContentType = "application/json";

            // read / query
            if (request.Method == "get")
            {
                return;
            }

            // create
            if (request.Method == "post")
            {
                return;
            }

            if (string.IsNullOrEmpty(item))
            {
                response.ErrorApplication();

                return;
            }

            // update
            if (request.Method == "put")
            {
                return;
            }

            // delete
            if (request.Method == "delete")
            {
                return;
            }
        }
    }
}
