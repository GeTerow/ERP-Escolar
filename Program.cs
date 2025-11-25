// Builder
using TaskWeb.Repositories;
using TaskWeb.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IUsuarioRepository>(_ => 
    new UsuarioDatabaseRepository(
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
builder.Services.AddTransient<ITurnoRepository>(_ => 
    new TurnoDatabaseRepository(
        builder.Configuration.GetConnectionString("default")));
builder.Services.AddTransient<ISlotAulaRepository>(_ => 
    new SlotAulaDatabaseRepository(
        builder.Configuration.GetConnectionString("default")));
builder.Services.AddTransient<IDisponibilidadeProfessorRepository>(_ => 
    new DisponibilidadeProfessorDatabaseRepository(
        builder.Configuration.GetConnectionString("default")));
builder.Services.AddScoped<GradeValidationService>();
builder.Services.AddScoped<GradeGenerationService>();
builder.Services.AddSession();      
builder.Services.AddControllersWithViews();

// App
var app = builder.Build();

DatabaseSeeder.Seed(app.Services);

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






