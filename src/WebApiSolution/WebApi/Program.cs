using System.Globalization;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddLocalizationOptions(); // Required to initialize languages 

var app = builder.Build();

app.UseRequestLocalization(); // Required to initialize request localization

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

//######## if you want to apply to all api, use this ########
//app.UseMiddleware<LocalizationMiddleware>();

//######## if you want to apply to special (named) api, use this ########
app.UseWhen(context => context.Request.Path.StartsWithSegments("/WeatherForecast"), appBuilder =>
{
    appBuilder.UseMiddleware<LocalizationMiddleware>();
});

app.Run();
