using Laobian.Share.Extension;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Laobian.Blog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureLogging(builder => builder.AddEmail());
                    webBuilder.UseStartup<Startup>();
                });
    }
}
