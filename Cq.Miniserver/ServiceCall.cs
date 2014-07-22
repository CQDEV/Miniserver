﻿namespace Cq.Miniserver
{
    using System;
    using System.Text;

    public class ServiceCall
    {
        public static ServiceObjectCollection Objects = new ServiceObjectCollection();

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

            var item = string.Empty;

            if (pathSplit.Length > 2)
            {
                item = pathSplit[2];
            }

            // process response
            response.ContentType = "application/json";

            // create
            if (request.Method == "post")
            {
                var id = Objects.Create(container, Encoding.ASCII.GetString(request.Content));

                response.Content = Encoding.ASCII.GetBytes("{" + "\"id\":\"" + id + "\"}");

                return;
            }

            // read / query
            if (request.Method == "get")
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
