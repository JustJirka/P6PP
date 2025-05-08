using PaymentService.API.Data;
using PaymentService.API.Extensions;
using PaymentService.API.Features;
using PaymentService.API.Features.Payments;
using PaymentService.API.Persistence;
using PaymentService.API.Persistence;
// Ensure the correct namespace is used for the DatabaseInitializer and DatabaseSeeder

var builder = WebApplication.CreateBuilder(args);
var corsSettingsSection = builder.Configuration.GetSection("Cors");
builder.Services.Configure<CorsSettings>(corsSettingsSection);
var corsSettings = corsSettingsSection.Get<CorsSettings>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDevClient", policy =>
    {
        policy.WithOrigins(corsSettings.AllowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5185);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<DapperContext>();


builder.Services.RegisterServices(builder.Configuration);

var Services = builder.Services;
var configuration = builder.Configuration;

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var databaseInitializer = services.GetRequiredService<DatabaseInitializer>();
    await databaseInitializer.InitializeDatabaseAsync();

    //// Seed the database
    var dbSeeder = services.GetRequiredService<DatabaseSeeder>();
    await dbSeeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAngularDevClient");
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    // USER ENDPOINTS
    
    CreatePaymentEndpoint.Register(endpoints);
    GetPaymentByIdEndpoint.Register(endpoints);
    UpdatePaymentEndpoint.Register(endpoints);
    GetBalanceByIdEndpoint.Register(endpoints);
    CreateBalanceEndpoint.Register(endpoints);
    CreateBillEndpoint.Register(endpoints);

});

app.Run();
