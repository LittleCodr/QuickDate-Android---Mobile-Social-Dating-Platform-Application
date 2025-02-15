using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Utils;
using QuickDateClient;
using System;

namespace QuickDate.Helpers.Model
{
    public static class UserDetails
    {
        public static string AccessToken = "";
        public static long UserId = 0;
        public static string Username = "";
        public static string FullName = "";
        public static string Password = "";
        public static string Email = "";
        public static string Cookie = "";
        public static string Status = "";
        public static string Avatar = "";
        public static string Cover = "";
        public static string DeviceId = "";
        public static string LangName = "";
        public static string IsPro = "";
        public static string Url = "";

        public static string Lat = "";
        public static string Lng = "";

        public static int FilterOptionAgeMin = 18;
        public static int FilterOptionAgeMax = 75;
        public static string FilterOptionGender = "4525,4526";
        public static string FilterOptionGenderMale = "4525";
        public static string FilterOptionGenderFemale = "4526";
        public static bool FilterOptionIsOnline;
        public static string FilterOptionDistance = "35";
        public static string FilterOptionLanguage = "english";
        public static string FilterOptionFromHeight = "139";
        public static string FilterOptionToHeight = "220";

        public static string HotOrNotFilterGender = "4525,4526";
        public static string HotOrNotFilterBirthday = "";
        public static bool HotOrNotFilterIsOnline;

        public static string UsersFilterGender = "4525,4526";
        public static string UsersFilterBirthday = "";
        public static bool UsersFilterIsOnline;

        public static string Located = FilterOptionDistance; //DistanceCount
        public static long AgeMin = FilterOptionAgeMin, AgeMax = FilterOptionAgeMax;
        public static string Gender = FilterOptionGender, Location = "";
        public static bool SwitchState;

        //new 
        public static string Language = FilterOptionLanguage;
        public static string Ethnicity = "";
        public static string Religion = "";

        public static string RelationShip = "";
        public static string Smoke = "";
        public static string Drink = "";

        public static string FromHeight = FilterOptionFromHeight;
        public static string ToHeight = FilterOptionToHeight;
        public static string Body = "";

        public static string Interest = "";
        public static string Education = "";
        public static string Pets = "";

        public static bool NotificationPopup { get; set; } = true;

        public static int UnixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        public static string Time = UnixTimestamp.ToString();

        //public static string AndroidId = Android.Provider.Settings.Secure.GetString(Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);

        public static void ClearAllValueUserDetails()
        {
            try
            {
                AccessToken = string.Empty;
                UserId = 0;
                Username = string.Empty;
                FullName = string.Empty;
                Password = string.Empty;
                Email = string.Empty;
                Cookie = string.Empty;
                Status = string.Empty;
                Avatar = string.Empty;
                Cover = string.Empty;
                DeviceId = string.Empty;
                LangName = string.Empty;
                IsPro = string.Empty;
                Url = string.Empty;

                Lat = string.Empty;
                Lng = string.Empty;

                FilterOptionAgeMin = 18;
                FilterOptionAgeMax = 75;
                FilterOptionGender = "4525,4526";
                FilterOptionGenderMale = "4525";
                FilterOptionGenderFemale = "4526";
                FilterOptionIsOnline = false;
                FilterOptionDistance = "35";
                FilterOptionLanguage = "english";
                FilterOptionFromHeight = "139";
                FilterOptionToHeight = "220";
                AppSettings.MaxMessageLimitForNonProUser = 3;


                Located = FilterOptionDistance; //DistanceCount
                AgeMin = FilterOptionAgeMin;
                AgeMax = FilterOptionAgeMax;
                Gender = FilterOptionGender = Location = string.Empty;
                Language = FilterOptionLanguage;
                Ethnicity = string.Empty;
                Religion = string.Empty;
                RelationShip = string.Empty;
                Smoke = string.Empty;
                Drink = string.Empty;
                AgeMin = FilterOptionAgeMin;
                FromHeight = FilterOptionFromHeight;
                AgeMax = FilterOptionAgeMax;
                ToHeight = FilterOptionToHeight;
                Body = string.Empty;
                Interest = string.Empty;
                Education = string.Empty;
                Pets = string.Empty;

                HomeActivity.CountNotificationsStatic = 0;
                HomeActivity.CountMessagesStatic = 0;

                Current.AccessToken = string.Empty;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

    }
}
