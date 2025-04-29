using Microsoft.EntityFrameworkCore;
using Analytics.Infrastructure.Data;
using Analytics.Application.Services.Interface;
using Analytics.Domain.Interface;
using Analytics.Application.Services;
using Analytics.Infrastructure.Data.Repositories;
using Quartz;
using Quartz.Impl;
using Analytics.Application.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8006);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDatabaseSyncService, DatabaseSyncService>();  
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IPaymentsRepository, PaymentsRepository>();
builder.Services.AddScoped<IPaymentsService, PaymentsService>();
builder.Services.AddScoped<IUserCreditRepository, UserCreditRepository>();
builder.Services.AddScoped<IUserCreditService, UserCreditService>();

// Register Quartz.NET
builder.Services.AddQuartz(q =>
{
    // Define the job and trigger.
    var jobKey = new JobKey("databaseSyncJob");

    q.AddJob<DatabaseSyncJob>(opts => opts.WithIdentity(jobKey));

    // Create a trigger that fires once a day, e.g., at 3 AM every day.
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("databaseSyncTrigger")
        // Cron expression: At 03:00 AM every day.
        .WithCronSchedule("0 0 3 * * ?"));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<DatabaseInit>();
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddHttpClient();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInit>();
    await initializer.InitializeDatabase();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("MyCorsPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
