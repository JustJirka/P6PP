using AdminSettings.Data;
using AdminSettings.Persistence;
using AdminSettings.Persistence.Interface;
using AdminSettings.Persistence.Repository;
using AdminSettings.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

// Configure port here + launchSettings.json ( + later Dockerfile EXPOSE XXXX)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(9090);
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AdminSettings API",
        Version = "v1"
    });
});

builder.Services.AddControllers();

builder.Services.AddDbContext<AdminSettingsDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 25)))); 

builder.Services.AddHttpClient<IUserServiceClient, UserServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://user-service:5189");
});



builder.Services.AddMemoryCache();
builder.Services.AddSingleton<DapperContext>();

builder.Services.AddScoped<SystemSettingsSeeder>();

builder.Services.AddScoped<SystemSettingsService>();
builder.Services.AddScoped<AuditLogService>();
builder.Services.AddScoped<AuditLogRepository>();
builder.Services.AddScoped<DatabaseBackupService>();

builder.Services.AddScoped<DatabaseInitializer>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AdminSettings API v1");
    });
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var databaseInitializer = services.GetRequiredService<DatabaseInitializer>();

    await databaseInitializer.InitializeDatabaseAsync();
    
    var seeder = services.GetRequiredService<SystemSettingsSeeder>();
    await seeder.SeedAsync();
}


app.UseHttpsRedirection();
app.MapControllers();

app.Run();

