// Builder
using TaskWeb.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITagRepository>(new TagMemoryRepository());
builder.Services.AddControllersWithViews();

// App
var app = builder.Build();

app.MapControllerRoute("default", "{controller=tag}/{action=index}/{id?}");

app.Run();
