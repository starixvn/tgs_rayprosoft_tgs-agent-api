using System.CommandLine;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.DataInitializers;

var builder = Host.CreateDefaultBuilder();

builder
    .ConfigureWebHostDefaults(webHostBuilder => webHostBuilder
        .ConfigureKestrel(serverOptions =>
        {
            serverOptions.ListenUnixSocket("/tgs-seeding.sock");
        }));

var app = builder.Build();

var connectionStringOption = new Option<string>("--connection-string", "連線字串");
connectionStringOption.AddAlias("-c");
connectionStringOption.SetDefaultValueFactory(() =>
{
    var config = app.Services.GetRequiredService<IConfiguration>();

    return config.GetConnectionString("Tgs");
});

var rootCommand = new RootCommand("建立Tgs初始化資料");
rootCommand.AddGlobalOption(connectionStringOption);

rootCommand.SetHandler<string>(
    async connectionString =>
    {
        var dbOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
        dbOptionsBuilder
            .UseNpgsql(connectionString, serverOption =>
            {
                serverOption.UseTrigrams();
            })
            .UseSnakeCaseNamingConvention();

        var dbContext = new TgsRepository(dbOptionsBuilder.Options);

        var seeding = new DataInitializer(dbContext, app.Services.GetRequiredService<IWebHostEnvironment>());
        await seeding.SetInitialDataAsync().ConfigureAwait(false);
    },
    connectionStringOption);

await rootCommand.InvokeAsync(args).ConfigureAwait(false);
