using Core.Interfaces;
using Infrastructure.Configurations;
using Infrastructure.Extensions;
using Infrastructure.Services;
using WorkerService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        // Configure FileMonitorOptions
        services.Configure<FileMonitorOptions>(context.Configuration.GetSection("FileMonitorOptions"));

        // Configure DbContext with PostgreSQL using extension method
        services.AddDatabase(context.Configuration);

        // Register services
        services.AddScoped<IFileMonitorService, FileMonitorService>();
        services.AddScoped<IFileParserService, FileParserService>();
        services.AddScoped<IDataService, DataService>();
        services.AddScoped<IValidationService, ValidationService>();

        // Register the worker
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
