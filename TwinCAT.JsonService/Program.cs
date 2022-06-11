using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();