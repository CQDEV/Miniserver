namespace Cq.Miniserver
{
    public class ServiceObject
    {
        public string Container { get; set; }
        public ulong Id { get; set; }
        public string Data { get; set; }

        public string Json
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.Data))
                {
                    this.Data = "{}";
                }

                var result = string.Format("\"id\":\"{0}\",\"data\":{1}", this.Id, this.Data);

                return "{" + result + "}";
            }
        }
    }
}