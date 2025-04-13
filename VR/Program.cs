using VR;
using VR.Configurations;
using VR.Interfaces;
using VR.Services;
using Microsoft.EntityFrameworkCore;
using VR.Data;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<FileMonitorOptions>(context.Configuration.GetSection("FileMonitorOptions"));

        services.AddDbContext<VRDbContext>(options =>
            options.UseNpgsql(context.Configuration.GetConnectionString("PostgreSQLConnectionStrings")));

        services.AddScoped<IFileMonitorService, FileMonitorService>();
        services.AddScoped<IFileParserService, FileParserService>();
        services.AddScoped<IDataService, DataService>();
        services.AddScoped<IValidationService, ValidationService>();

        services.AddHostedService<Worker>();
    })
    .ConfigureLogging((context, logging) =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddDebug();
    })
    .Build();

host.Run();
