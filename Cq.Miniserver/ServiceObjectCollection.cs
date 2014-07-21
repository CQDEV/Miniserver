namespace Cq.Miniserver
{
    using System.Collections.Generic;
    using System.Linq;

    public class ServiceObjectCollection : List<ServiceObject>
    {
        public int Add(string data)
        {
            var id = 1;

            if (this.Count > 0)
            {
                id = this.Max(x => x.Id) + 1;
            }


            this.Add(new ServiceObject { Id = id, Data = data });

            return id;
        }
    }
}
