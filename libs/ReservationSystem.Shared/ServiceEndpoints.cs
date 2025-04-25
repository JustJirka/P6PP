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
        public static string SendBookingConfirmationEmail => $"{BaseUrl}/api/notification/user/sendbookingconfirmationemail";
        public static string SendBookingCancellationEmail => $"{BaseUrl}/api/notification/user/sendbookingcancellationemail";
        public static string GetAllNotifications(int UserId, bool unreadOnly=true) => $"{BaseUrl}/api/notification/logs/getallnotifications/{UserId}";
        public static string SetAllNotificationsAsRead(int UserId) => $"{BaseUrl}/api/notification/logs/setallnotificationsasread/{UserId}";
        public static string SetSomeNotificationsAsRead => $"{BaseUrl}/api/notification/logs/setsomenotificationsasread";
    }
    
}
