// Builder
using TaskWeb.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IUsuarioRepository>(_ => 
    new UsuarioDatabaseRepository(
        builder.Configuration.GetConnectionString("default")));
builder.Services.AddTransient<IAlunoRepository>(_ => 
    new AlunoDatabaseRepository(
        builder.Configuration.GetConnectionString("default")));
builder.Services.AddTransient<IProfessorRepository>(_ => 
    new ProfessorDatabaseRepository(
        builder.Configuration.GetConnectionString("default")));
builder.Services.AddTransient<ITurmaRepository>(_ => 
    new TurmaDatabaseRepository(
        builder.Configuration.GetConnectionString("default")));
builder.Services.AddTransient<IMateriaRepository>(_ => 
    new MateriaDatabaseRepository(
        builder.Configuration.GetConnectionString("default")));
builder.Services.AddTransient<IGradeRepository>(_ => 
    new GradeDatabaseRepository(
        builder.Configuration.GetConnectionString("default")));

builder.Services.AddSession();      
builder.Services.AddControllersWithViews();

// App
var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "root",
    pattern: "",
    defaults: new { controller = "Usuario", action = "Login" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.Run();
