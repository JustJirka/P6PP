namespace ReservationSystem.Shared;

public static class ServiceEndpoints
{
    public static class UserService
    {
        // USERS
        private const string BaseUrl = "http://user-service:5189";

        public static string CreateUser => $"{BaseUrl}/api/user";
        public static string GetUserById(int id) => $"{BaseUrl}/api/user/{id}";
        public static string UpdateUser(int id) => $"{BaseUrl}/api/user/{id}";
        public static string GetUsers => $"{BaseUrl}/api/users";
        public static string DeleteUser(int id) => $"{BaseUrl}/api/user/{id}";

        // ROLES
        public static string CreateRole => $"{BaseUrl}/api/role";
        public static string GetRoleById(int id) => $"{BaseUrl}/api/role/{id}";
        public static string GetRoles => $"{BaseUrl}/api/roles";
        public static string AssignRole => $"{BaseUrl}/api/user/role";
    }

    public static class AuthService
    {
        private const string BaseUrl = "http://auth-service:8005";
        public static string Login => $"{BaseUrl}/api/auth/login";
        public static string Register => $"{BaseUrl}/api/auth/register";
        public static string ResetPassword => $"{BaseUrl}/api/auth/reset-password";

        public static string DeleteUser(int id) => $"{BaseUrl}/api/auth/delete/{id}";
        public static string ValidateToken => $"{BaseUrl}/api/auth/validate";

    }

    public static class NotificationService
    {
        private const string BaseUrl = "http://notification-service:5181";
        public static string SendEmail => $"{BaseUrl}/api/notification/sendemail";
        public static string SendVerificationEmail => $"{BaseUrl}/api/notification/user/sendverificationemail";
        public static string SendPasswordResetEmail => $"{BaseUrl}/api/notification/user/sendpasswordresetemail";
        public static string SendRegistrationEmail(int id) => $"{BaseUrl}/api/notification/user/sendregistrationemail/{id}";
        public static string GetAllTemplates => $"{BaseUrl}/api/notification/templates/getalltemplates";
        public static string EditTemplate => $"{BaseUrl}/api/notification/templates/edittemplate";
        public static string SendBookingConfirmationEmail => $"{BaseUrl}/api/notification/booking/sendbookingconfirmationemail";
        public static string SendBookingCancellationEmail => $"{BaseUrl}/api/notification/booking/sendbookingcancellationemail";
        public static string GetAllNotifications(int UserId, bool unreadOnly = true) => $"{BaseUrl}/api/notification/logs/getallnotifications/{UserId}";
        public static string SetAllNotificationsAsRead(int UserId) => $"{BaseUrl}/api/notification/logs/setallnotificationsasread/{UserId}";
        public static string SetSomeNotificationsAsRead => $"{BaseUrl}/api/notification/logs/setsomenotificationsasread";
    }

    public static class AdminSettingsService
    {
        private const string BaseUrl = "http://admin-settings:9090";
        // SystemSettings
        public static string GetSystemSettings => $"{BaseUrl}/api/system-settings";
        public static string UpdateSystemSettings => $"{BaseUrl}/api/system-settings";
        public static string GetDatabaseBackupSetting => $"{BaseUrl}/api/system-settings/BackupSetting";
        public static string UpdateDatabaseBackupSetting => $"{BaseUrl}/api/system-settings/BackupSetting";
        public static string GetNotificationEnabled => $"{BaseUrl}/api/system-settings/notification-enabled";
        public static string GetEnableAuditLogs => $"{BaseUrl}/api/system-settings/audit-log-enabled";
        public static string SetEnableAuditLogs(bool enabled) => $"{BaseUrl}/api/system-settings/audit-log-enabled/{enabled}";

        // Backup
        public static string RunBackup => $"{BaseUrl}/api/backup/run";

        // AuditLogs
        public static string GetAuditLogs => $"{BaseUrl}/api/audit";
        public static string AddAuditLog => $"{BaseUrl}/api/audit";
        public static string GetUserAuditLogs(string userId) => $"{BaseUrl}/api/audit/user/{userId}";
        public static string GetAuditLogsByAction(string action) => $"{BaseUrl}/api/audit/action/{action}";
        public static string ArchiveAuditLog(int id) => $"{BaseUrl}/api/audit/{id}/archive";
    }



    public static class PaymentService
    {
        private const string BaseUrl = "http://payment-service:5185";
        public static string CreatePayment => $"{BaseUrl}/api/createpayment";
        public static string GetPaymentById(int id) => $"{BaseUrl}/api//{id}";
        public static string UpdatePayment => $"{BaseUrl}/api/updatepayment";
        public static string CreateBalance => $"{BaseUrl}/api/createbalance";
        public static string GetBalanceById(int id) => $"{BaseUrl}/api/getbalance/{id}";
    }

        public static class bookingService
    {
        private const string BaseUrl = "http://booking-service:8080";
        // Services endpoints
        public static string ListServices => $"{BaseUrl}/api/services";
        public static string GetServicesByTrainer(int trainerId) => $"{BaseUrl}/api/services/trainer/{trainerId}";
        public static string GetServiceById(int serviceId) => $"{BaseUrl}/api/services/{serviceId}";
        public static string CreateService => $"{BaseUrl}/api/services";
        public static string UpdateService => $"{BaseUrl}/api/services";
        public static string DeleteService(int serviceId) => $"{BaseUrl}/api/services/{serviceId}";

        // Bookings endpoints
        public static string ListBookings => $"{BaseUrl}/api/bookings";
        public static string GetBookingById(int bookingId) => $"{BaseUrl}/api/bookings/{bookingId}";
        public static string CreateBooking => $"{BaseUrl}/api/bookings";
        public static string DeleteBooking(int bookingId) => $"{BaseUrl}/api/bookings/{bookingId}";

        // Rooms endpoints
        public static string ListRooms => $"{BaseUrl}/api/rooms";
        public static string GetRoomById(int roomId) => $"{BaseUrl}/api/rooms/{roomId}";
        public static string CreateRoom => $"{BaseUrl}/api/rooms";
        public static string UpdateRoom(int roomId) => $"{BaseUrl}/api/rooms/{roomId}";
        public static string DeleteRoom(int roomId) => $"{BaseUrl}/api/rooms/{roomId}";
    }

}
