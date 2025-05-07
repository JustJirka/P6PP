using BookingService.API;
using ReservationSystem.Shared.Clients;
using ReservationSystem.Shared.Cors;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext(builder.Configuration, builder.Environment);
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuth();
builder.Services.AddMediatR();
builder.Services.AddSwagger();
builder.Services.AddControllers()
    .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .AddEndpointValidation();

builder.Services.AddHttpClient<NetworkHttpClient>();
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
app.UseCors("AllowAngularDevClient");
/*// Ng serve Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularNgClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
*/

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.ApplyMigrations();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowAngularDevClient");
app.UseCors("AllowAngularNgClient");

app.MapControllers();

app.Run();
