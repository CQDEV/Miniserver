namespace Cq.Miniserver
{
    using System.Collections.Generic;
    using System.Linq;

    public class ServiceObjectCollectionContainerCollection : List<ServiceObjectCollectionContainer>
    {
        public ServiceObjectCollectionContainer GetContainer(string name)
        {
            return this.FirstOrDefault(x => x.Name == name);
        }
    }
}
