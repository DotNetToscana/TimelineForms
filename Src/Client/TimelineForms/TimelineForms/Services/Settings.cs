using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace TimelineForms.Services
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters.
    /// </summary>
    public static class Settings
    {
        private const string USER_NAME = "UserName";
        private const string PASSWORD = "Password";
        private const string POST_ON_FACEBOOK = "PostOnFacebook";

        private static ISettings AppSettings => CrossSettings.Current;

        public static bool IsLogged => !string.IsNullOrWhiteSpace(UserName);

        public static string UserName
        {
            get { return AppSettings.GetValueOrDefault<string>(USER_NAME, null); }
            private set { AppSettings.AddOrUpdateValue(USER_NAME, value); }
        }

        public static string Password
        {
            get { return AppSettings.GetValueOrDefault<string>(PASSWORD, null); }
            private set { AppSettings.AddOrUpdateValue(PASSWORD, value); }
        }

        public static bool PostOnFacebook
        {
            get { return AppSettings.GetValueOrDefault<bool>(POST_ON_FACEBOOK, true); }
            set { AppSettings.AddOrUpdateValue(POST_ON_FACEBOOK, value); }
        }

        public static void SaveCredential(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public static void ClearCredential()
        {
            UserName = null;
            Password = null;
        }
    }
}