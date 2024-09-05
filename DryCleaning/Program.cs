using DryCleaning.Domain;
using DryCleaning.Util;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(new Scheduler());

builder.Services.AddControllers(o =>
{
    // Creamos un provider para nuestros custom binder
    o.ModelBinderProviders.Insert(0, new ListBinderProvider());
});

builder.Services.AddEndpointsApiExplorer();

// Los dos primeros mapeos son para arreglar el bug https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/2771
// Los tres últimos, para hacer que swagger muestre una caja de texto donde se pueden poner valores separados por comas. Luego entrarán en juego los dos custom binders
builder.Services.AddSwaggerGen(options => {
    options.MapType<DateOnly>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "date"
    });
    options.MapType<TimeOnly>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "time"
    });
    options.MapType<DayOfWeek>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "string"
    });
    options.MapType<List<DayOfWeek>>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "string"
    });
    options.MapType<List<DateOnly>>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "string"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
