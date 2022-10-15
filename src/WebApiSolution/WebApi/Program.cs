using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

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
