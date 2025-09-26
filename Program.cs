// Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// App
var app = builder.Build();

app.MapControllerRoute("default", "{controller=tag}/{action=index}");

app.Run();
