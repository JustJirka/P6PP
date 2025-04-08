using PaymentService.API.Data;
using Stripe;
using PaymentService.API.Extensions;
using PaymentService.API.Features;
using PaymentService.API.Features.Roles;
// Ensure the correct namespace is used for the DatabaseInitializer and DatabaseSeeder

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5185);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterServices(builder.Configuration);

var Services = builder.Services;
var configuration = builder.Configuration;

StripeConfiguration.ApiKey = configuration.GetSection("Stripe")["SecretKey"];

Services.Configure<StripeSettings>(configuration.GetSection("Stripe")); 

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var databaseInitializer = services.GetRequiredService<DatabaseInitializer>();
    await databaseInitializer.InitializeDatabaseAsync();

    // Seed the database
    var dbSeeder = services.GetRequiredService<DatabaseSeeder>();
    await dbSeeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.MapEndpoints(endpoints =>
{
    // USER ENDPOINTS
    GetUserByIdEndpoint.Register(endpoints);
    GetUsersEndpoint.Register(endpoints);
    DeleteUserEndpoint.Register(endpoints);
    UpdateUserEndpoint.Register(endpoints);
    CreateUserEndpoint.Register(endpoints);
    AssignUserRoleEndpoint.Register(endpoints);

    // ROLE ENDPOINTS
    GetRoleByIdEndpoint.Register(endpoints);
    GetRolesEndpoint.Register(endpoints);
    CreatePaymentEndpoint.Register(endpoints);
});

app.Run();
