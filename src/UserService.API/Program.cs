using UserService.API.Abstraction;
using UserService.API.Extensions;
using UserService.API.Features;
using UserService.API.Features.Roles;
using UserService.API.Persistence;
using UserService.API.Persistence.Repositories;
using UserService.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5189);
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterServices(builder.Configuration);
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService.API.Services.UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();

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

app.UseEndpoints(endpoints =>
{
    // USER ENDPOINTS
    GetUserByIdEndpoint.Register(endpoints);
    GetUsersEndpoint.Register(endpoints);
    DeleteUserEndpoint.Register(endpoints);
    UpdateUserEndpoint.Register(endpoints);
    CreateUserEndpoint.Register(endpoints);
    AssignUserRoleEndpoint.Register(endpoints);
    DeactivateUserEndpoint.Register(endpoints);
    ActivateUserEndpoint.Register(endpoints);
    
    
    // ROLE ENDPOINTS
    GetRoleByIdEndpoint.Register(endpoints);
    GetRolesEndpoint.Register(endpoints);
    CreateRoleEndpoint.Register(endpoints);
    
});

app.Run();

