using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using HowMuchLeft.ConsoleUI.Jobs;
using System.Globalization;
using HowMuchLeft.ConsoleUI.Configuration;
using System.Reflection;

using IHost host = CreateHostbuilder(args).Build();
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

static IHostBuilder CreateHostbuilder(String[] args)
{
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

    return Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            var appAssembly = Assembly.Load(new AssemblyName(hostingContext.HostingEnvironment.ApplicationName));
            var appPath = Path.GetDirectoryName(appAssembly.Location);

            config.SetBasePath(appPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddCommandLine(args);
        })
        .ConfigureServices((hostingContext, services) =>
        {
            services.AddSingleton<CommandLineOptions>(CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(args).Value);
            services.AddSingleton<WorkTimeJob>();            
        });
}