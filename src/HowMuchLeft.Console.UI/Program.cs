using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using HowMuchLeft.ConsoleUI.Jobs;
using System.Globalization;

using IHost host = CreateHostbuilder().Build();
using var scope = host.Services.CreateScope(); ;

var services = scope.ServiceProvider;

try
{
    services.GetRequiredService<WorkTimeJob>().Run();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

static IHostBuilder CreateHostbuilder()
{
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

    return Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        })
        .ConfigureServices((_, services) =>
        {
            services.AddSingleton<WorkTimeJob>();
        });
}