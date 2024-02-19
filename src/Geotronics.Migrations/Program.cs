using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Webinex.Migrations;

var builder = new ConfigurationBuilder()
    .SetBasePath(Path.Combine(AppContext.BaseDirectory))
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.Personal.json", optional: true)
    .AddEnvironmentVariables();

var configuration = builder.Build();

new ServiceCollection().AddStarterKitMigrations(x => x
        .UseAssembly(typeof(Program).Assembly)
        .UseDefaultConnectionString(
            configuration.GetConnectionString("Default"))
        .ConfigureRunner(runner => runner.AddPostgres11_0()))
    .BuildServiceProvider()
    .GetRequiredService<IStarterKitMigrator>()
    .Run(args);