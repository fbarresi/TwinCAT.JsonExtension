using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using TwinCAT.JsonService.Interfaces;
using TwinCAT.JsonService.Services;
using TwinCAT.JsonService.Settings;

var builder = WebApplication.CreateBuilder(args);

var loggingDirectory = builder.Configuration.GetValue(typeof(string), "LoggingDirectory") as string ?? "";

builder.Host
    .UseSerilog((ctx, lc) => 
        lc
            .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(Path.Combine(loggingDirectory,"logs","TwincatJsonService-.log"), 
                rollingInterval: RollingInterval.Day, 
                retainedFileTimeLimit: TimeSpan.FromDays(30), 
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    );

//Configs
builder.Services.AddOptions<BeckhoffClientSettings>()
    .BindConfiguration("BeckhoffClientSettings") // 👈 Bind the section
    .ValidateDataAnnotations() // 👈 Enable validation
    .ValidateOnStart(); // 👈 Validate on app start
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<BeckhoffClientSettings>>().Value); //use scoped for auto reload


// Add services to the container.
builder.Services.AddSingleton<ClientService>();
builder.Services.AddSingleton<IHostedService, ClientService>(
    serviceProvider => serviceProvider.GetService<ClientService>());
builder.Services.AddSingleton<IClientService, ClientService>(
    serviceProvider => serviceProvider.GetService<ClientService>());


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "TwinCAT Json Service",
        Description = "A Web API for with json format for read/write operations with TwinCAT",
        TermsOfService = new Uri("https://github.com/fbarresi/TwinCAT.JsonExtension"),
        Contact = new OpenApiContact
        {
            Name = "Federico Barresi",
            Url = new Uri("https://github.com/fbarresi")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://github.com/fbarresi/TwinCAT.JsonExtension/blob/master/LICENSE")
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();