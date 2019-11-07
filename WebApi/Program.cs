using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

using RabbitMQ.Client;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build()
//                .StartEventBus()
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
//                .AddRabbitMQEventBus(new ConnectionFactory(){HostName = "localhost", Port = 5672}, 
//                    "Quartz_service_queue", 3)
                .UseStartup<Startup>();
    }
}
