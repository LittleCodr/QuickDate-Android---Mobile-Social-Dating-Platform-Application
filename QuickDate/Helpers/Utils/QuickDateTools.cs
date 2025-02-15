using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Locations;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using Newtonsoft.Json;
using QuickDate.Activities.Tabbes;
using QuickDate.Activities.UserProfile;
using QuickDate.Helpers.Model;
using QuickDateClient.Classes.Common;
using QuickDateClient.Classes.Global;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace QuickDate.Helpers.Utils
{
    public static class QuickDateTools
    {
        //private static readonly string[] RelationshipLocal = Application.Context.Resources.GetStringArray(Resource.Array.RelationShipArray);
        //private static readonly string[] WorkStatusLocal = Application.Context.Resources.GetStringArray(Resource.Array.WorkStatusArray);
        //private static readonly string[] EducationLocal = Application.Context.Resources.GetStringArray(Resource.Array.EducationArray);
        //private static readonly string[] HairColorLocal = Application.Context.Resources.GetStringArray(Resource.Array.HairColorArray);
        //private static readonly string[] BodyLocal = Application.Context.Resources.GetStringArray(Resource.Array.BodyArray);
        //private static readonly string[] EthnicityLocal = Application.Context.Resources.GetStringArray(Resource.Array.EthnicityArray);
        //private static readonly string[] PetsLocal = Application.Context.Resources.GetStringArray(Resource.Array.PetsArray);
        //private static readonly string[] FriendsLocal = Application.Context.Resources.GetStringArray(Resource.Array.FriendsArray);
        //private static readonly string[] ChildrenLocal = Application.Context.Resources.GetStringArray(Resource.Array.ChildrenArray);
        //private static readonly string[] CharacterLocal = Application.Context.Resources.GetStringArray(Resource.Array.CharacterArray);
        //private static readonly string[] TravelLocal = Application.Context.Resources.GetStringArray(Resource.Array.TravelArray);
        //private static readonly string[] DrinkLocal = Application.Context.Resources.GetStringArray(Resource.Array.DrinkArray);
        //private static readonly string[] SmokeLocal = Application.Context.Resources.GetStringArray(Resource.Array.SmokeArray);
        //private static readonly string[] ReligionLocal = Application.Context.Resources.GetStringArray(Resource.Array.ReligionArray);
        //private static readonly string[] CarLocal = Application.Context.Resources.GetStringArray(Resource.Array.CarArray);
        //private static readonly string[] LiveWithLocal = Application.Context.Resources.GetStringArray(Resource.Array.LiveWithArray);
        //private static string[] HeightLocal = Application.Context.Resources.GetStringArray(Resource.Array.HeightArray);
        //private static readonly string[] CountriesArray = Application.Context.Resources.GetStringArray(Resource.Array.countriesArray);
        //private static readonly string[] CountriesArrayId = Application.Context.Resources.GetStringArray(Resource.Array.countriesArray_id);

        //private static readonly List<Dictionary<string, string>> RelationshipLocal = ListUtils.SettingsSiteList?.Relationship;
        //private static readonly List<Dictionary<string, string>> WorkStatusLocal = ListUtils.SettingsSiteList?.WorkStatus;
        //private static readonly List<Dictionary<string, string>> EducationLocal = ListUtils.SettingsSiteList?.Education;
        //private static readonly List<Dictionary<string, string>> HairColorLocal = ListUtils.SettingsSiteList?.HairColor;
        //private static readonly List<Dictionary<string, string>> BodyLocal = ListUtils.SettingsSiteList?.Body;
        //private static readonly List<Dictionary<string, string>> EthnicityLocal = ListUtils.SettingsSiteList?.Ethnicity;
        //private static readonly List<Dictionary<string, string>> PetsLocal = ListUtils.SettingsSiteList?.Pets;
        //private static readonly List<Dictionary<string, string>> FriendsLocal = ListUtils.SettingsSiteList?.Friends;
        //private static readonly List<Dictionary<string, string>> ChildrenLocal = ListUtils.SettingsSiteList?.Children;
        //private static readonly List<Dictionary<string, string>> CharacterLocal = ListUtils.SettingsSiteList?.Character;
        //private static readonly List<Dictionary<string, string>> TravelLocal = ListUtils.SettingsSiteList?.Travel;
        //private static readonly List<Dictionary<string, string>> DrinkLocal = ListUtils.SettingsSiteList?.Drink;
        //private static readonly List<Dictionary<string, string>> SmokeLocal = ListUtils.SettingsSiteList?.Smoke;
        //private static readonly List<Dictionary<string, string>> ReligionLocal = ListUtils.SettingsSiteList?.Religion;
        //private static readonly List<Dictionary<string, string>> CarLocal = ListUtils.SettingsSiteList?.Car;
        //private static readonly List<Dictionary<string, string>> LiveWithLocal = ListUtils.SettingsSiteList?.LiveWith;
        //private static List<Dictionary<string, string>> HeightLocal = ListUtils.SettingsSiteList?.Height;
        //private static readonly List<Dictionary<string, string>> BlogCategoriesLocal = ListUtils.SettingsSiteList?.BlogCategories;
        //private static readonly List<Dictionary<string, GetOptionsObject.Country>> CountriesArray = ListUtils.SettingsSiteList?.Countries;

        public static bool GetStatusOnline(string lastSeen, string isShowOnline)
        {
            try
            {
                string time = Methods.Time.TimeAgo(Convert.ToInt32(lastSeen), false);
                bool status = isShowOnline == "1" && time == Methods.Time.LblJustNow ? true : false;
                return status;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return false;
            }
        }

        public static string GetNameFinal(UserInfoObject dataUser)
        {
            try
            {
                if (!string.IsNullOrEmpty(dataUser.FullName))
                    return Methods.FunString.DecodeString(dataUser.FullName);

                string name = !string.IsNullOrEmpty(dataUser.FirstName) && !string.IsNullOrEmpty(dataUser.LastName) ? dataUser.FirstName + " " + dataUser.LastName : dataUser.Username;
                return Methods.FunString.DecodeString(name);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return Methods.FunString.DecodeString(dataUser?.Username);
            }
        }

        public static string GetWorkStatus(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return "";

                string name = Methods.FunString.DecodeString(ListUtils.SettingsSiteList?.WorkStatus?.FirstOrDefault(a => a.ContainsKey(id))?.Values.FirstOrDefault());
                return name;

                //int index = id - 1;
                //if (index > -1)
                //{
                //    string name = WorkStatusLocal[index];
                //    return name;
                //}
                // return ""; 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }

        public static string GetRelationship(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return "";

                string name = Methods.FunString.DecodeString(ListUtils.SettingsSiteList?.Relationship?.FirstOrDefault(a => a.ContainsKey(id))?.Values.FirstOrDefault());
                return name ?? "";
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }

        public static string GetEducation(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return "";

                string name = Methods.FunString.DecodeString(ListUtils.SettingsSiteList?.Education?.FirstOrDefault(a => a.ContainsKey(id))?.Values.FirstOrDefault());
                return name ?? "";
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }

        public static string GetEthnicity(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return "";

                string name = Methods.FunString.DecodeString(ListUtils.SettingsSiteList?.Ethnicity?.FirstOrDefault(a => a.ContainsKey(id))?.Values.FirstOrDefault());
                return name ?? "";
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }

        public static string GetBody(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return "";

                string name = Methods.FunString.DecodeString(ListUtils.SettingsSiteList?.Body?.FirstOrDefault(a => a.ContainsKey(id))?.Values.FirstOrDefault());
                return name ?? "";
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }

        public static string GetHairColor(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return "";

                string name = Methods.FunString.DecodeString(ListUtils.SettingsSiteList?.HairColor?.FirstOrDefault(a => a.ContainsKey(id))?.Values.FirstOrDefault());
                return name ?? "";
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }

        public static string GetCharacter(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return "";

                string name = Methods.FunString.DecodeString(ListUtils.SettingsSiteList?.Character?.FirstOrDefault(a => a.ContainsKey(id))?.Values.FirstOrDefault());
                return name ?? "";
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }

        public static string GetChildren(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return "";

                string name = Methods.FunString.DecodeString(ListUtils.SettingsSiteList?.Children?.FirstOrDefault(a => a.ContainsKey(id))?.Values.FirstOrDefault());
                return name ?? "";
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }

        public static string GetFriends(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return "";

                string name = Methods.FunString.DecodeString(ListUtils.SettingsSiteList?.Friends?.FirstOrDefault(a => a.ContainsKey(id))?.Values.FirstOrDefault());
                return name ?? "";
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }

        public static string GetBlogCategories(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return "";

                string name = Methods.FunString.DecodeString(ListUtils.SettingsSiteList?.BlogCategories?.FirstOrDefault(a => a.ContainsKey(id))?.Values.FirstOrDefault());
                return name ?? "";
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }

        public static string GetLiveWith(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return "";

                string name = Methods.FunString.DecodeString(ListUtils.SettingsSiteList?.LiveWith?.FirstOrDefault(a => a.ContainsKey(id))?.Values.FirstOrDefault());
                return name ?? "";
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }

        public static string GetCar(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return "";

                string name = Methods.FunString.DecodeString(ListUtils.SettingsSiteList?.Car?.FirstOrDefault(a => a.ContainsKey(id))?.Values.FirstOrDefault());
                return name ?? "";
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }

        public static string GetReligion(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return "";

                string name = Methods.FunString.DecodeString(ListUtils.SettingsSiteList?.Religion?.FirstOrDefault(a => a.ContainsKey(id))?.Values.FirstOrDefault());
                return name ?? "";
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }

        public static string GetSmoke(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return "";

                string name = Methods.FunString.DecodeString(ListUtils.SettingsSiteList?.Smoke?.FirstOrDefault(a => a.ContainsKey(id))?.Values.FirstOrDefault());
                return name ?? "";
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }

        public static string GetTravel(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return "";

                string name = Methods.FunString.DecodeString(ListUtils.SettingsSiteList?.Travel?.FirstOrDefault(a => a.ContainsKey(id))?.Values.FirstOrDefault());
                return name ?? "";
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }

        public static string GetNotification(GetNotificationsObject.Datum item)
        {
            try
            {
                string text;
                GetNotificationsObject.Datum check;

                switch (item.Type)
                {
                    case "visit":
                        text = Application.Context.GetText(Resource.String.Lbl_VisitYou);
                        check = ListUtils.VisitsList.FirstOrDefault(a => a.Notifier?.Id == item.Notifier?.Id);
                        if (check == null)
                            ListUtils.VisitsList.Add(item);
                        break;
                    case "like":
                        text = Application.Context.GetText(Resource.String.Lbl_LikeYou);
                        check = ListUtils.LikesList.FirstOrDefault(a => a.Notifier?.Id == item.Notifier?.Id);
                        if (check == null)
                            ListUtils.LikesList.Add(item);
                        break;
                    case "dislike":
                        text = Application.Context.GetText(Resource.String.Lbl_DislikeYou);
                        check = ListUtils.LikesList.FirstOrDefault(a => a.Notifier?.Id == item.Notifier?.Id);
                        if (check == null)
                            ListUtils.LikesList.Add(item);
                        break;
                    case "send_gift":
                        text = Application.Context.GetText(Resource.String.Lbl_SendGiftToYou);
                        check = ListUtils.NotificationList.FirstOrDefault(a => a.Notifier?.Id == item.Notifier?.Id);
                        if (check == null)
                            ListUtils.NotificationList.Add(item);
                        break;
                    case "got_new_match":
                        text = Application.Context.GetText(Resource.String.Lbl_YouGotMatch);
                        check = ListUtils.MatchList.FirstOrDefault(a => a.Notifier?.Id == item.Notifier?.Id);
                        if (check == null)
                            ListUtils.MatchList.Add(item);
                        break;
                    case "friend_request_accepted":
                        text = Application.Context.GetText(Resource.String.Lbl_FriendRequestAccepted);
                        check = ListUtils.RequestsList.FirstOrDefault(a => a.Notifier?.Id == item.Notifier?.Id);
                        if (check == null)
                            ListUtils.RequestsList.Add(item);
                        break;
                    case "friend_request":
                        text = Application.Context.GetText(Resource.String.Lbl_FriendRequestYou);
                        check = ListUtils.RequestsList.FirstOrDefault(a => a.Notifier?.Id == item.Notifier?.Id);
                        if (check == null)
                            ListUtils.RequestsList.Add(item);
                        break;
                    default:
                        text = Methods.FunString.DecodeString(item.Text);
                        check = ListUtils.NotificationList.FirstOrDefault(a => a.Notifier?.Id == item.Notifier?.Id);
                        if (check == null)
                            ListUtils.NotificationList.Add(item);
                        break;
                }

                return text;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }

        public static string GetGender(int id)
        {
            try
            {
                string text;
                string name = Methods.FunString.DecodeString(ListUtils.SettingsSiteList?.Gender?.FirstOrDefault(a => a.ContainsKey(id.ToString()))?.Values.FirstOrDefault());
                text = id switch
                {
                    4525 => Application.Context.GetText(Resource.String.Lbl_Male),
                    4526 => Application.Context.GetText(Resource.String.Lbl_Female),
                    _ => name
                };
                return text;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }

        public static string GetPets(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return "";

                string name = Methods.FunString.DecodeString(ListUtils.SettingsSiteList?.Pets?.FirstOrDefault(a => a.ContainsKey(id))?.Values.FirstOrDefault());
                return name ?? "";
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }

        public static string GetDrink(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return "";

                string name = Methods.FunString.DecodeString(ListUtils.SettingsSiteList?.Drink?.FirstOrDefault(a => a.ContainsKey(id))?.Values.FirstOrDefault());
                return name ?? "";
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }

        public static string GetCountry(string codeCountry)
        {
            try
            {
                if (string.IsNullOrEmpty(codeCountry))
                    return "";

                var name = ListUtils.SettingsSiteList?.Countries?.FirstOrDefault(a => a.ContainsKey(codeCountry))?.Values.FirstOrDefault()?.Name;
                return Methods.FunString.DecodeString(name);

                // var list = CountriesArrayId.ToList();
                // int index = 0;

                // var data = list.FirstOrDefault(a => a.Contains(codeCountry));
                //if (data != null)
                //{
                //    index = list.IndexOf(data);
                //}

                // if (index > -1)
                // {
                //     string name = CountriesArray[index];
                //     return name;
                // }
                // return "";
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }

        public static string GetNotificationsText(GetNotificationsObject.Datum item)
        {
            try
            {
                string text = item.Type switch
                {
                    "visit" => Application.Context.GetText(Resource.String.Lbl_VisitYou),
                    "like" => Application.Context.GetText(Resource.String.Lbl_LikeYou),
                    "dislike" => Application.Context.GetText(Resource.String.Lbl_DislikeYou),
                    "send_gift" => Application.Context.GetText(Resource.String.Lbl_SendGiftToYou),
                    "got_new_match" => Application.Context.GetText(Resource.String.Lbl_YouGotMatch),
                    "message" => Application.Context.GetText(Resource.String.Lbl_MessageNotifications),
                    "approve_receipt" => Application.Context.GetText(Resource.String.Lbl_ApproveReceipt),
                    "disapprove_receipt" => Application.Context.GetText(Resource.String.Lbl_DisapproveReceipt),
                    "accept_chat_request" => Application.Context.GetText(Resource.String.Lbl_AcceptChatRequest),
                    "accept_chdecline_chat_requestat_request" => Application.Context.GetText(Resource.String.Lbl_DeclineChatRequest),
                    "friend_request_accepted" => Application.Context.GetText(Resource.String.Lbl_FriendRequestAccepted),
                    "friend_request" => Application.Context.GetText(Resource.String.Lbl_FriendRequestYou),
                    _ => Methods.FunString.DecodeString(item.Text)
                };

                return text;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }

        public static void OpenProfile(Activity activity, string eventPage, UserInfoObject item, ImageView image)
        {
            try
            {
                if (item.Id != UserDetails.UserId)
                {
                    var intent = new Intent(activity, typeof(UserProfileActivity));
                    intent.PutExtra("EventPage", eventPage); // Close , Move , likeAndClose , HideButton
                    intent.PutExtra("ItemUser", JsonConvert.SerializeObject(item));
                    if (AppSettings.EnableAddAnimationImageUser)
                    {
                        if (image != null)
                        {
                            ActivityOptionsCompat options = ActivityOptionsCompat.MakeSceneTransitionAnimation((Activity)activity, image, "profileimage");
                            activity.StartActivity(intent, options.ToBundle());
                        }
                        else
                        {
                            activity.StartActivity(intent);
                        }
                    }
                    else
                    {
                        activity.StartActivity(intent);
                    }
                }
                else
                {
                    HomeActivity.GetInstance()?.FragmentBottomNavigator.ShowFragment4();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        // Functions Save Images
        private static async void SaveFile(string id, string folder, string fileName, string url)
        {
            try
            {
                if (url.Contains("http"))
                {
                    string folderDestination = folder + id + "/";

                    string filePath = Path.Combine(folderDestination);
                    string mediaFile = filePath + "/" + fileName;

                    if (File.Exists(mediaFile)) return;

                    HttpClient client;
                    if (AppSettings.TurnSecurityProtocolType3072On)
                    {
                        HttpClientHandler clientHandler = new HttpClientHandler();
                        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                        //clientHandler.SslProtocols = SslProtocols.Tls | SslProtocols.Ssl2 | SslProtocols.Ssl3 | SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13 | SslProtocols.Default;

                        // Pass the handler to httpClient(from you are calling api)
                        client = new HttpClient(clientHandler);
                    }
                    else
                    {
                        client = new HttpClient();
                    }

                    var s = await client.GetStreamAsync(new Uri(url));
                    if (s.CanRead)
                    {
                        if (File.Exists(mediaFile)) return;
                        await using FileStream fs = new FileStream(mediaFile, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
                        await s.CopyToAsync(fs);
                    }

                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        // Functions file from folder
        public static async Task<string> GetFile(string id, string folder, string filename, string url)
        {
            try
            {
                string folderDestination = folder + id + "/";

                if (!Directory.Exists(folderDestination))
                {
                    Directory.CreateDirectory(folderDestination);
                }

                string imageFile = Methods.MultiMedia.GetMediaFrom_Gallery(folderDestination, filename);
                if (imageFile == "File Dont Exists")
                {
                    //This code runs on a new thread, control is returned to the caller on the UI thread.
                    await Task.Factory.StartNew(() => { SaveFile(id, folder, filename, url); });
                    return url;
                }
                else
                {
                    return imageFile;
                }
            }
            catch
            {
                // Methods.DisplayReportResultTrack(e);
                return url;
            }
        }

        public static string GetAgeUser(UserInfoObject item)
        {
            try
            {
                if (item.Age != null)
                {
                    return item.Age;
                }
                else if (!string.IsNullOrEmpty(item.Birthday) && item.Birthday != "0000-00-00" && item.Birthday != "0")
                {
                    //1997-05-28 
                    var units = item.Birthday.Split('-');

                    var year = Convert.ToInt32(units[0]);
                    if (units[1][0] == '0')
                        units[1] = units[1][1].ToString();

                    var month = Convert.ToInt32(units[1]);

                    if (units[2][0] == '0')
                        units[2] = units[2][1].ToString();

                    var day = Convert.ToInt32(units[2]);

                    DateTime now = DateTime.Now;
                    DateTime birthday = new DateTime(year, month, day);
                    int age = now.Year - birthday.Year;
                    if (now < birthday.AddYears(age)) age--;

                    return age.ToString();
                }
                return "";
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }

        public static float DistanceBetween(LatLng latLng1, LatLng latLng2)
        {
            try
            {
                Location loc1 = new Location(LocationManager.GpsProvider);
                Location loc2 = new Location(LocationManager.GpsProvider);
                loc1.Latitude = latLng1.Latitude;
                loc1.Longitude = latLng1.Longitude;
                loc2.Latitude = latLng2.Latitude;
                loc2.Longitude = latLng2.Longitude;
                return loc1.DistanceTo(loc2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        public static string ToSi(double d)
        {
            try
            {
                string[] incPrefixes = new[] { "km", "M", "G", "T", "P", "E", "Z", "Y" };
                string[] decPrefixes = new[] { "m", "\u03bc", "n", "p", "f", "a", "z", "y" };

                int degree = (int)Math.Floor(Math.Log10(Math.Abs(d)) / 3);
                double scaled = d * Math.Pow(1000, -degree);

                string prefix = "Km";
                switch (Math.Sign(degree))
                {
                    case 1: prefix = incPrefixes[degree - 1]; break;
                    case -1: prefix = decPrefixes[-degree - 1]; break;
                }

                return scaled.ToString("####") + prefix;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static bool GetStatusAds()
        {
            try
            {
                switch (AppSettings.ShowAds)
                {
                    case ShowAds.AllUsers:
                        return true;
                    case ShowAds.UnProfessional:
                        {
                            var isPro = ListUtils.MyUserInfo?.FirstOrDefault()?.IsPro ?? "0";
                            if (isPro == "0")
                                return true;
                            else
                                return false;
                        }
                    default:
                        return false;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return false;
            }
        }

        public static bool IsTabDark()
        {
            try
            {
                return AppSettings.SetTabDarkTheme is TabTheme.Dark;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return false;
            }
        }

        public static void SetBackgroundTheme(View v)
        {
            try
            {
                if (v == null)
                    return;

                if (AppSettings.SetBackgroundTheme == BackgroundTheme.Color || IsTabDark())
                {
                    v.SetBackgroundResource(0);
                    v.BackgroundTintList = null;
                    v.SetBackgroundColor(IsTabDark() ? Color.ParseColor("#121212") : Color.ParseColor("#ffffff"));
                }
                else
                {
                    v.SetBackgroundResource(Resource.Drawable.firstbackgroundeffects);
                    v.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor(AppSettings.MainColor));
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }
}