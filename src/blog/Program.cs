﻿using Laobian.Share.Log;
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

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureLogging(builder => builder.AddQueuedLogger());
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}