namespace Cq.Miniserver
{
    using System;
    
    public class Program
    {
        public static void Main(string[] args)
        {
            var server = new Server();

            while (true)
            {
                Console.ReadKey(true);
            }
        }
    }
}
