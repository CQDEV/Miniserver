namespace Cq.Miniserver
{
    using System;
    using System.Text;

    public class ServiceCall
    {
        public static ServiceObjectCollectionContainerCollection Containers = new ServiceObjectCollectionContainerCollection();

        public static void Process(Request request, Response response)
        {
            // process request
            var pathSplit = request.Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (pathSplit.Length < 2)
            {
                response.ErrorApplication();

                return;
            }

            var container = Containers.GetContainer(pathSplit[1]);
            
            if (container == null)
            {
                container = new ServiceObjectCollectionContainer 
                { 
                    Name = pathSplit[1], 
                    Objects = new ServiceObjectCollection() 
                };

                Containers.Add(container);
            }

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
                var id = container.Objects.Add(Encoding.ASCII.GetString(request.Content));

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
