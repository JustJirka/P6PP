namespace NotificationService.API.Services
{
    public class TimerAppService : IHostedService
    {
        private System.Timers.Timer _timer;
        private readonly IServiceProvider _serviceProvider;

        public TimerAppService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new System.Timers.Timer(3600000);
            _timer.Elapsed += async (sender, e) => await OnTimedEvent(sender, e);
            _timer.AutoReset = true;
            _timer.Start();

            return Task.CompletedTask;
        }

        private async Task OnTimedEvent(object? sender, System.Timers.ElapsedEventArgs e)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var bookingAppService = scope.ServiceProvider.GetRequiredService<BookingAppService>();
                await bookingAppService.SendReminder24HourBefore();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Stop();
            _timer.Dispose();
            return Task.CompletedTask;
        }
    }
}