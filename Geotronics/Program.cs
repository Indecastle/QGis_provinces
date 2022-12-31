using FluentMigrator.Runner;
using Geotronics.DataAccess;
using Geotronics.Migrations;
using Geotronics.Services.Common;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb =>
        rb.AddPostgres11_0()
            .WithGlobalConnectionString("DefaultConnection")
            .ScanIn(typeof(AddTables).Assembly).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole());

builder.Services.AddEntityFrameworkNpgsql().AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{

});
builder.Services.AddServices();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = false;
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.WriteIndented = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(x =>
    {
        //x.RouteTemplate = "mycoolapi/swagger/{documentname}/swagger.json";
    });
    app.UseSwaggerUI(options =>
    {
        //options.SwaggerEndpoint("v1/swagger.json", "v1");
        //options.RoutePrefix = "mycoolapi/swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();