namespace Cq.Miniserver
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;

    public class ServiceObjectCollection : List<ServiceObject>
    {
        private ServiceObjectCollection()
        {
        }

        private static readonly string DataPath = ConfigurationManager.AppSettings["data"];

        public static object Lock = new object();

        private static DataContractSerializer serializer;

        protected static DataContractSerializer Serializer
        {
            get
            {
                return serializer ?? (serializer = new DataContractSerializer(typeof(ServiceObjectCollection)));
            }
        }

        public static ServiceObjectCollection Get()
        {
            ServiceObjectCollection result = null;

            if (File.Exists(DataPath))
            {
                using (var stream = File.OpenRead(DataPath))
                {
                    result = (ServiceObjectCollection)Serializer.ReadObject(stream);
                }
            }
            else
            {
                result = new ServiceObjectCollection();
            }

            return result;
        }

        public void Save()
        {
            lock (ServiceObjectCollection.Lock)
            {
                using (var stream = new MemoryStream())
                {
                    Serializer.WriteObject(stream, ServiceCall.Objects);
                    stream.Flush();

                    File.WriteAllBytes(DataPath, stream.ToArray());
                }
            }
        }

        private ServiceObject GetItemByContainerAndId(string container, ulong id)
        {
            return this.FirstOrDefault(x => x.Container == container && x.Id == id);
        }

        public ulong Create(string container, string data)
        {
            lock (Lock)
            {
                var items = this.Where(x => x.Container == container);
                var index = 1UL;

                if (items.Count() > 0)
                {
                    index = items.Max(x => x.Id) + 1;
                }

                this.Add(
                    new ServiceObject 
                    {
                        Container = container,
                        Id = index,
                        Data = data
                    });

                return index;
            }
        }

        public IEnumerable<ServiceObject> Query(string container)
        {
            return this.Where(x => x.Container == container);
        }

        public ServiceObject Read(string container, ulong id)
        {
            return this.GetItemByContainerAndId(container, id);
        }

        public void Update(string container, ulong id, string data)
        {
            lock (Lock)
            {
                var item = this.GetItemByContainerAndId(container, id);

                if (item != null)
                {
                    item.Data = data;
                }
            }
        }

        public void Delete(string container, ulong id)
        {
            lock (Lock)
            {
                var item = this.GetItemByContainerAndId(container, id);

                if (item != null)
                {
                    this.Remove(item);
                }
            }
        }
    }
}
