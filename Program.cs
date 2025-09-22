// Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// App
var app = builder.Build();

app.MapControllerRoute("default", "{controller=tag}/{action=index}");

app.Run();
