using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Content.Res;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using AT.Markushi.UI;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Load.Resource.Bitmap;
using Bumptech.Glide.Request;
using Google.Android.Material.Dialog;
using Java.IO;
using Newtonsoft.Json;
using QuickDate.Activities.Base;
using QuickDate.Activities.Chat;
using QuickDate.Activities.Gift;
using QuickDate.Activities.Tabbes;
using QuickDate.Activities.UserProfile.Adapters;
using QuickDate.Activities.Viewer;
using QuickDate.Adapters;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.ShimmerUtils;
using QuickDate.Helpers.Utils;
using QuickDate.Library.Anjo;
using QuickDate.Library.Anjo.Share;
using QuickDate.Library.Anjo.Share.Abstractions;
using QuickDate.SQLite;
using QuickDateClient;
using QuickDateClient.Classes.Global;
using QuickDateClient.Classes.Users;
using QuickDateClient.Requests;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Exception = System.Exception;
using String = System.String;
using Uri = Android.Net.Uri;

namespace QuickDate.Activities.UserProfile
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class UserProfileActivity : BaseActivity, IOnMapReadyCallback, IDialogListCallBack
    {
        #region Variables Basic

        private TextView Username, LocationTextView, Description, DescriptionWork, AboutText;
        private ImageView IconBack;
        private CircleButton LikeButton, DesLikeButton;
        private AppCompatButton WebsiteButton, SocialGoogle, SocialFacebook, SocialTwitter, SocialLinkedIn, SocialInstagram;
        private LinearLayout SocialLayout, WorkAndEducationLayout, IntersetLayout, LanguagesLayout, FooterButtonSection;
        private UserInfoObject DataUser;
        private string EventType, DataType, LinkedIn, Twitter, Facebook, Google, Instagram, Website;
        private HomeActivity GlobalContext;
        private ImageView IconMore, UserImage;

        private GoogleMap Map;
        private CircleButton ChatButton, GiftButton, FavoriteButton, FriendRequestsButton;
        private double CurrentLongitude, CurrentLatitude;
        private RecyclerView MainRecyclerView, RecylerInterset, RecylerLanguages;
        private MultiMediaAdapter MAdapter;
        private RequestBuilder FullGlideRequestBuilder;
        private CategoriesAdapter MAdapterInterset, MAdapterLanguages;

        private ViewStub ShimmerPageLayout;
        private View InflatedShimmer;
        private TemplateShimmerInflater ShimmerInflater;

        private StReadMoreOption ReadMoreOption;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);
                SetTheme(QuickDateTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);

                GlobalContext = HomeActivity.GetInstance();

                // Create your application here
                SetContentView(Resource.Layout.UserProfileLayout);

                EventType = Intent?.GetStringExtra("EventPage") ?? "";
                DataType = Intent?.GetStringExtra("DataType") ?? "";

                if (!string.IsNullOrEmpty(Intent?.GetStringExtra("ItemUser")))
                    DataUser = JsonConvert.DeserializeObject<UserInfoObject>(Intent?.GetStringExtra("ItemUser") ?? "");

                //Get Value  
                InitComponent();
                InitBackground();
                InitShimmer();
                SetupRecyclerView();
                GetStickersGiftsLists();

                ReadMoreOption = new StReadMoreOption.Builder()
                    .TextLength(100, StReadMoreOption.TypeCharacter)
                    .MoreLabel(GetText(Resource.String.Lbl_ReadMore))
                    .LessLabel(GetText(Resource.String.Lbl_ReadLess))
                    .MoreLabelColor(Color.ParseColor(AppSettings.MainColor))
                    .LessLabelColor(Color.ParseColor(AppSettings.MainColor))
                    .LabelUnderLine(true)
                    .Build();
                
                if (DataType == "OneSignal")
                {
                    //Run Api 
                    GetUserData(DataUser, "RunApi");
                }
                else if (DataType == "Search")
                {
                    var Username = Intent?.GetStringExtra("Username") ?? "";
                    //Run Api 
                    SearchToUser(Username);
                }
                else
                {
                    GetUserData(DataUser);
                }
                GlobalContext?.TracksCounter?.CheckTracksCounter();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                AddOrRemoveEvent(true);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();
                AddOrRemoveEvent(false);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                var glideRequestOptions = new RequestOptions().SetDiskCacheStrategy(DiskCacheStrategy.All).SetPriority(Priority.High);
                FullGlideRequestBuilder = Glide.With(this).AsBitmap().Apply(glideRequestOptions).Transition(new BitmapTransitionOptions().CrossFade(100));

                FriendRequestsButton = FindViewById<CircleButton>(Resource.Id.FriendRequestsButton);
                FavoriteButton = FindViewById<CircleButton>(Resource.Id.FavoriteButton);
                GiftButton = FindViewById<CircleButton>(Resource.Id.giftButton);

                IconBack = FindViewById<ImageView>(Resource.Id.backIcon);
                UserImage = FindViewById<ImageView>(Resource.Id.UserImage);
                Username = FindViewById<TextView>(Resource.Id.Username);

                IconMore = FindViewById<ImageView>(Resource.Id.iconMore);

                AboutText = FindViewById<TextView>(Resource.Id.AboutText);
                Description = FindViewById<TextView>(Resource.Id.Description);
                LocationTextView = FindViewById<TextView>(Resource.Id.LocationTextView);

                IntersetLayout = FindViewById<LinearLayout>(Resource.Id.intersetLayout);
                LanguagesLayout = FindViewById<LinearLayout>(Resource.Id.languagesLayout);

                WorkAndEducationLayout = FindViewById<LinearLayout>(Resource.Id.workAndEducationLayout);
                DescriptionWork = FindViewById<TextView>(Resource.Id.DescriptionworkSection);

                SocialLayout = FindViewById<LinearLayout>(Resource.Id.socialinfolayout);
                SocialGoogle = FindViewById<AppCompatButton>(Resource.Id.social1);
                SocialFacebook = FindViewById<AppCompatButton>(Resource.Id.social2);
                SocialTwitter = FindViewById<AppCompatButton>(Resource.Id.social3);
                SocialLinkedIn = FindViewById<AppCompatButton>(Resource.Id.social4);
                SocialInstagram = FindViewById<AppCompatButton>(Resource.Id.social5);
                WebsiteButton = FindViewById<AppCompatButton>(Resource.Id.website);

                FooterButtonSection = FindViewById<LinearLayout>(Resource.Id.footerButtonSection);
                LikeButton = FindViewById<CircleButton>(Resource.Id.likebutton2);
                DesLikeButton = FindViewById<CircleButton>(Resource.Id.closebutton1);
                ChatButton = FindViewById<CircleButton>(Resource.Id.Chatbutton1);

                var icon = AppCompatResources.GetDrawable(this, AppSettings.FlowDirectionRightToLeft ? Resource.Drawable.icon_back_arrow_right : Resource.Drawable.icon_back_arrow_left);
                icon?.SetTint(QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                IconBack.SetImageDrawable(icon);

                if (EventType == "HideButton")
                {
                    LikeButton.Visibility = ViewStates.Gone;
                    DesLikeButton.Visibility = ViewStates.Gone;
                    ChatButton.Visibility = ViewStates.Visible;
                }

                if (!AppSettings.EnableFriendSystem)
                    FriendRequestsButton.Visibility = ViewStates.Gone;

                if (!AppSettings.FavoriteEnabled)
                    FavoriteButton.Visibility = ViewStates.Gone;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void InitShimmer()
        {
            try
            {
                ShimmerPageLayout = FindViewById<ViewStub>(Resource.Id.viewStubShimmer);
                InflatedShimmer = ShimmerPageLayout?.Inflate();

                ShimmerInflater = new TemplateShimmerInflater();
                ShimmerInflater.InflateLayout(this, InflatedShimmer, ShimmerTemplateStyle.UserProfileTemplate);
                ShimmerInflater.Show();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void SetupRecyclerView()
        {
            try
            {
                MainRecyclerView = FindViewById<RecyclerView>(Resource.Id.Recyler);
                var multimediaLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
                MainRecyclerView.SetLayoutManager(multimediaLayoutManager);
                MAdapter = new MultiMediaAdapter(this);
                MainRecyclerView.SetAdapter(MAdapter);

                RecylerInterset = FindViewById<RecyclerView>(Resource.Id.RecylerIntersets);
                var intersetLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
                RecylerInterset.SetLayoutManager(intersetLayoutManager);
                MAdapterInterset = new CategoriesAdapter(this) { MCategoriesList = new ObservableCollection<Classes.CategoriesClass>() };
                RecylerInterset.SetAdapter(MAdapterInterset);

                RecylerLanguages = FindViewById<RecyclerView>(Resource.Id.RecylerLanguages);
                var languagesLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
                RecylerLanguages.SetLayoutManager(languagesLayoutManager);
                MAdapterLanguages = new CategoriesAdapter(this) { MCategoriesList = new ObservableCollection<Classes.CategoriesClass>() };
                RecylerLanguages.SetAdapter(MAdapterLanguages);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    IconBack.Click += IconBackOnClick;
                    LikeButton.Click += LikeButtonOnClick;
                    DesLikeButton.Click += DesLikeButtonOnClick;
                    ChatButton.Click += ChatButtonOnClick;
                    SocialGoogle.Click += SocialGoogleOnClick;
                    SocialFacebook.Click += SocialFacebookOnClick;
                    SocialTwitter.Click += SocialTwitterOnClick;
                    SocialLinkedIn.Click += SocialLinkedInOnClick;
                    SocialInstagram.Click += SocialInstagramOnClick;
                    WebsiteButton.Click += WebsiteButtonOnClick;
                    IconMore.Click += IconMoreOnClick;
                    FavoriteButton.Click += IconFavoriteOnClick;
                    FriendRequestsButton.Click += IconFriendRequestsOnClick;
                    GiftButton.Click += GiftButtonOnClick;
                    MAdapter.OnItemClick += MAdapterOnOnItemClick;
                }
                else
                {
                    IconBack.Click -= IconBackOnClick;
                    LikeButton.Click -= LikeButtonOnClick;
                    DesLikeButton.Click -= DesLikeButtonOnClick;
                    ChatButton.Click -= ChatButtonOnClick;
                    SocialGoogle.Click -= SocialGoogleOnClick;
                    SocialFacebook.Click -= SocialFacebookOnClick;
                    SocialTwitter.Click -= SocialTwitterOnClick;
                    SocialLinkedIn.Click -= SocialLinkedInOnClick;
                    SocialInstagram.Click -= SocialInstagramOnClick;
                    WebsiteButton.Click -= WebsiteButtonOnClick;
                    IconMore.Click -= IconMoreOnClick;
                    FavoriteButton.Click -= IconFavoriteOnClick;
                    FriendRequestsButton.Click -= IconFriendRequestsOnClick;
                    GiftButton.Click -= GiftButtonOnClick;
                    MAdapter.OnItemClick -= MAdapterOnOnItemClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        //MultiMedia
        private async void MAdapterOnOnItemClick(object sender, MultiMediaAdapterClickEventArgs e)
        {
            try
            {
                var position = e.Position;
                if (position > -1)
                {
                    var item = MAdapter.GetItem(position);
                    if (item != null)
                    {
                        if (string.IsNullOrEmpty(item.UrlFile) && item.IsPrivate == "1")
                            item.UrlFile = item.PrivateFileFull;
                        else
                            item.UrlFile = item.Full;

                        var fileName = item.UrlFile.Split('/').Last();

                        if (item.IsPrivate == "1")
                        {
                            Toast.MakeText(this, GetString(Resource.String.Lbl_ThisFileIsPrivate), ToastLength.Short)?.Show();

                            item.UrlFile = await QuickDateTools.GetFile(DataUser.Id.ToString(), Methods.Path.FolderDiskImage, fileName, item.PrivateFileFull);

                            if (AppSettings.OpenImageFromApp)
                            {
                                Intent intent = new Intent(this, typeof(ImageViewerActivity));
                                intent.PutExtra("MediaFile", item.UrlFile);
                                StartActivity(intent);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.UrlFile) && (item.UrlFile.Contains("file://") || item.UrlFile.Contains("content://") || item.UrlFile.Contains("storage") || item.UrlFile.Contains("/data/user/0/")))
                                {
                                    File file2 = new File(item.UrlFile);
                                    var photoUri = FileProvider.GetUriForFile(this, PackageName + ".fileprovider", file2);
                                    Intent intent = new Intent();
                                    intent.SetAction(Intent.ActionView);
                                    intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                                    intent.SetDataAndType(photoUri, "image/*");
                                    StartActivity(intent);
                                }
                                else
                                {
                                    Intent intent = new Intent(Intent.ActionView, Uri.Parse(item.UrlFile));
                                    StartActivity(intent);
                                }
                            }
                        }
                        else
                        {
                            if (item.IsVideo == "1" && !string.IsNullOrEmpty(item.VideoFile))
                            {
                                item.UrlFile = await QuickDateTools.GetFile(DataUser.Id.ToString(), Methods.Path.FolderDiskVideo, fileName, item.VideoFile);

                                if (AppSettings.OpenVideoFromApp)
                                {
                                    Intent intent = new Intent(this, typeof(VideoFullScreenActivity));
                                    intent.PutExtra("videoUrl", item.UrlFile);
                                    StartActivity(intent);
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(item.UrlFile) && (item.UrlFile.Contains("file://") || item.UrlFile.Contains("content://") || item.UrlFile.Contains("storage") || item.UrlFile.Contains("/data/user/0/")))
                                    {
                                        File file2 = new File(item.UrlFile);
                                        var mediaUri = FileProvider.GetUriForFile(this, PackageName + ".fileprovider", file2);

                                        Intent intent = new Intent();
                                        intent.SetAction(Intent.ActionView);
                                        intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                                        intent.SetDataAndType(mediaUri, "video/*");
                                        StartActivity(intent);
                                    }
                                    else
                                    {
                                        Intent intent = new Intent(Intent.ActionView, Uri.Parse(item.UrlFile));
                                        StartActivity(intent);
                                    }
                                }
                            }
                            else
                            {
                                item.UrlFile = await QuickDateTools.GetFile(DataUser.Id.ToString(), Methods.Path.FolderDiskImage, fileName, item.Full);

                                if (AppSettings.OpenImageFromApp)
                                {
                                    Intent intent = new Intent(this, typeof(ImageViewerActivity));
                                    intent.PutExtra("MediaFile", item.UrlFile);
                                    StartActivity(intent);
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(item.UrlFile) && (item.UrlFile.Contains("file://") || item.UrlFile.Contains("content://") || item.UrlFile.Contains("storage") || item.UrlFile.Contains("/data/user/0/")))
                                    {
                                        File file2 = new File(item.UrlFile);
                                        var photoUri = FileProvider.GetUriForFile(this, PackageName + ".fileprovider", file2);
                                        Intent intent = new Intent();
                                        intent.SetAction(Intent.ActionView);
                                        intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                                        intent.SetDataAndType(photoUri, "image/*");
                                        StartActivity(intent);
                                    }
                                    else
                                    {
                                        Intent intent = new Intent(Intent.ActionView, Uri.Parse(item.UrlFile));
                                        StartActivity(intent);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //FriendRequests
        private void IconFriendRequestsOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    return;
                }

                if (DataUser != null)
                {
                    switch (FriendRequestsButton?.Tag?.ToString())
                    {
                        case "true":
                            //FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, IconFriendRequests, FontAwesomeIcon.UserPlus); //Lbl_AddFriend
                            FriendRequestsButton.SetImageResource(Resource.Drawable.icon_friendplus_vector);
                            FriendRequestsButton.Tag = "false";
                            DataUser.IsFriend = false;
                            DataUser.IsFriendRequest = false;

                            Toast.MakeText(this, GetString(Resource.String.Lbl_TheFriendshipCanceled), ToastLength.Short)?.Show();

                            // Send Api Remove
                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Friends.AddOrRemoveFriendsAsync(DataUser.Id.ToString()) });
                            break;
                        case "false":
                            //FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, IconFriendRequests, FontAwesomeIcon.UserClock); //Lbl_UnFriend
                            FriendRequestsButton.SetImageResource(Resource.Drawable.icon_friendclock_vector);
                            FriendRequestsButton.Tag = "true";
                            DataUser.IsFriend = true;
                            DataUser.IsFriendRequest = true;

                            Toast.MakeText(this, GetString(Resource.String.Lbl_TheRequestSent), ToastLength.Short)?.Show();

                            // Send Api Add
                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Friends.AddOrRemoveFriendsAsync(DataUser.Id.ToString()) });
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Back
        private void IconBackOnClick(object sender, EventArgs e)
        {
            try
            {
                Finish();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Open chat
        private void ChatButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(this, typeof(MessagesBoxActivity));
                intent.PutExtra("UserId", DataUser.Id.ToString());
                intent.PutExtra("TypeChat", "LastChat");
                intent.PutExtra("UserItem", JsonConvert.SerializeObject(DataUser));

                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    StartActivity(intent);
                }
                else
                {
                    //Check to see if any permission in our group is available, if one, then all are
                    if (PermissionsController.CheckPermissionStorage(this))
                    {
                        StartActivity(intent);
                    }
                    else
                        new PermissionsController(this).RequestPermission(100);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //DesLike
        private void DesLikeButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    return;
                }

                if (DataUser.IsLiked != null && DataUser.IsLiked.Value == false)
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_ThisUserHasBeenDisliked), ToastLength.Short)?.Show();
                    return;
                }

                DataUser.IsLiked = false;

                //sent api 
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.AddLikesAsync("", DataUser.Id.ToString()) });

                ObservableCollection<UserInfoObject> list;
                switch (EventType)
                {
                    case "Close":
                        Finish();
                        break;
                    case "likeAndClose":
                        var checkList = GlobalContext?.TrendingFragment?.MAdapter?.TrendingList?.FirstOrDefault(q => q.Type == ItemType.HotOrNot);
                        if (checkList?.HotOrNotList.Count > 0)
                        {
                            var users1 = checkList.HotOrNotList?.FirstOrDefault(a => a.Id == DataUser.Id);
                            if (users1 != null)
                            {
                                users1.IsLiked = false;
                                GlobalContext?.TrendingFragment?.MAdapter.NotifyDataSetChanged();
                            }
                        }

                        list = GlobalContext?.TrendingFragment?.HotOrNotFragment?.MAdapter?.UsersDateList;
                        var users2 = list?.FirstOrDefault(a => a.Id == DataUser.Id);
                        if (users2 != null)
                        {
                            users2.IsLiked = false;
                            GlobalContext?.TrendingFragment?.HotOrNotFragment?.MAdapter.NotifyDataSetChanged();
                        }

                        Finish();
                        break;
                    case "LikeAndMoveTrending":
                        {
                            var user = GlobalContext?.TrendingFragment?.MAdapter?.TrendingList?.FirstOrDefault(q => q.Id == DataUser.Id);
                            if (user?.UsersData != null)
                            {
                                int index = GlobalContext.TrendingFragment.MAdapter.TrendingList.IndexOf(user);
                                if (index > -1)
                                    index += 1;

                                var nextDataUser = GlobalContext.TrendingFragment.MAdapter.TrendingList[index];
                                if (nextDataUser != null) GetUserData(nextDataUser.UsersData);
                            }
                            break;
                        }
                    case "LikeAndMoveCardMach":
                        {
                            GlobalContext?.CardFragment?.SetDesLikeDirection();
                            list = GlobalContext?.CardFragment?.CardDateAdapter?.UsersDateList;
                            var data = list?.FirstOrDefault(a => a.Id == DataUser.Id);
                            if (data != null)
                            {
                                int index = list.IndexOf(data);
                                if (index > -1)
                                    index += 1;

                                var nextDataUser = list[index];
                                GetUserData(nextDataUser);
                            }

                            break;
                        }
                    default:
                        {
                            Finish();
                            break;
                        }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Like
        private void LikeButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    return;
                }

                if (DataUser.IsLiked != null && DataUser.IsLiked.Value)
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_ThisUserHasBeenLiked), ToastLength.Short)?.Show();
                    return;
                }

                DataUser.IsLiked = true;
                //sent api 
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.AddLikesAsync(DataUser.Id.ToString(), "") });

                ObservableCollection<UserInfoObject> list;
                switch (EventType)
                {
                    case "Close":
                        Finish();
                        break;
                    case "likeAndClose":
                        var checkList = GlobalContext?.TrendingFragment?.MAdapter?.TrendingList?.FirstOrDefault(q => q.Type == ItemType.HotOrNot);
                        if (checkList?.HotOrNotList.Count > 0)
                        {
                            var users1 = checkList.HotOrNotList?.FirstOrDefault(a => a.Id == DataUser.Id);
                            if (users1 != null)
                            {
                                users1.IsLiked = true;
                                GlobalContext?.TrendingFragment?.MAdapter.NotifyDataSetChanged();
                            }
                        }

                        list = GlobalContext?.TrendingFragment?.HotOrNotFragment?.MAdapter?.UsersDateList;
                        var users2 = list?.FirstOrDefault(a => a.Id == DataUser.Id);
                        if (users2 != null)
                        {
                            users2.IsLiked = true;
                            GlobalContext?.TrendingFragment?.HotOrNotFragment?.MAdapter.NotifyDataSetChanged();
                        }

                        Finish();
                        break;
                    case "LikeAndMoveTrending":
                        {
                            var user = GlobalContext?.TrendingFragment?.MAdapter?.TrendingList?.FirstOrDefault(q => q.Id == DataUser.Id);
                            if (user?.UsersData != null)
                            {
                                int index = GlobalContext.TrendingFragment.MAdapter.TrendingList.IndexOf(user);
                                if (index > -1)
                                    index += 1;

                                var nextDataUser = GlobalContext.TrendingFragment.MAdapter.TrendingList[index];
                                if (nextDataUser != null) GetUserData(nextDataUser.UsersData);
                            }
                            break;
                        }
                    case "LikeAndMoveCardMach":
                        {
                            list = GlobalContext?.CardFragment?.CardDateAdapter?.UsersDateList;
                            var data = list?.FirstOrDefault(a => a.Id == DataUser.Id);
                            if (data != null)
                            {
                                int index = list.IndexOf(data);
                                if (index > -1)
                                    index += 1;

                                var nextDataUser = list[index];
                                GetUserData(nextDataUser);
                            }
                            GlobalContext?.CardFragment?.SetLikeDirection();
                            break;
                        }
                    default:
                        {
                            Finish();
                            break;
                        }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Sent Gift
        private void GiftButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var dataUser = ListUtils.MyUserInfo?.FirstOrDefault();
                if (!AppSettings.EnableAppFree && (dataUser?.Balance == "0.00" || dataUser?.Balance == "0.0" || dataUser?.Balance == "0"))
                {
                    var window = new PopupController(this);
                    window.DisplayCreditWindow();
                }
                else
                {
                    Bundle bundle = new Bundle();
                    bundle.PutString("UserId", DataUser.Id.ToString());

                    GiftDialogFragment mGiftFragment = new GiftDialogFragment
                    {
                        Arguments = bundle
                    };

                    mGiftFragment.Show(SupportFragmentManager, mGiftFragment.Tag);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Favorite
        private void IconFavoriteOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    return;
                }

                var favorite = SetFav(FavoriteButton);
                if (favorite)
                {
                    //Insert in DB 
                    if (DataUser != null)
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Favorites.AddFavoritesAsync(DataUser.Id.ToString()) });

                    //User added to favorites
                    Toast.MakeText(this, GetString(Resource.String.Lbl_AddedFavorite), ToastLength.Short)?.Show();
                }
                else
                {
                    // Remove in DB 
                    if (DataUser != null)
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Favorites.DeleteFavoritesAsync(DataUser.Id.ToString()) });

                    //User removed from favorites
                    Toast.MakeText(this, GetString(Resource.String.Lbl_RemovedFavorite), ToastLength.Short)?.Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Click More (Block , Report )
        private void IconMoreOnClick(object sender, EventArgs e)
        {
            try
            {
                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                arrayAdapter.Add(GetText(Resource.String.Lbl_Block));
                arrayAdapter.Add(GetText(Resource.String.Lbl_Report));
                arrayAdapter.Add(GetText(Resource.String.Lbl_Share));

                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Open WebView Website
        private void WebsiteButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    new IntentController(this).OpenBrowserFromApp(Website);
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Open Instagram
        private void SocialInstagramOnClick(object sender, EventArgs e)
        {
            try
            {
                new IntentController(this).OpenInstagramIntent(Instagram);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Open LinkedIn
        private void SocialLinkedInOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    //IMethods.IApp.OpenbrowserUrl(this, "https://www.linkedin.com/in/" + LinkedIn);
                    new IntentController(this).OpenLinkedInIntent(LinkedIn);
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Open Twitter
        private void SocialTwitterOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    //IMethods.IApp.OpenbrowserUrl(this, "https://twitter.com/"+ Twitter);

                    new IntentController(this).OpenTwitterIntent(Twitter);
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Open Facebook
        private void SocialFacebookOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    //IMethods.IApp.OpenbrowserUrl(this, "https://www.facebook.com/"+ Facebook);

                    new IntentController(this).OpenFacebookIntent(Facebook);
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Open Google
        private void SocialGoogleOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    new IntentController(this).OpenBrowserFromApp("https://plus.google.com/u/0/" + Google);
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        private async void SearchToUser(string username)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    return;
                }

                if (Methods.CheckConnectivity())
                {
                    var dictionary = new Dictionary<string, string>
                    {
                        {"username", username},
                        {"limit", "1"},
                        {"offset", "0"},
                    };

                    var (apiStatus, respond) = await RequestsAsync.Users.SearchAsync(dictionary);
                    if (apiStatus != 200 || respond is not ListUsersObject result || result.Data == null)
                    {
                        Methods.DisplayReportResult(this, respond);
                    }
                    else
                    {
                        var respondList = result.Data.Count;
                        if (respondList > 0)
                        {
                            var dataUser = result.Data.FirstOrDefault(a => a.Username == username);
                            if (dataUser != null)
                            {
                                LoadDataUser(dataUser);

                                //Sent Api Visit
                                var data = await ApiRequest.GetInfoData(this, dataUser.Id.ToString());
                                if (data != null)
                                {
                                    DataUser = data.Data;
                                    LoadDataUser(DataUser);
                                }
                                ShimmerInflater?.Hide();
                            }
                        }
                    }
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception e)
            {
                ShimmerInflater?.Hide();
                Methods.DisplayReportResultTrack(e);
            }
        }

        private async void GetUserData(UserInfoObject dataUser, string type = "")
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    return;
                }

                if (dataUser != null)
                {
                    LoadDataUser(dataUser);

                    //Sent Api Visit
                    var data = await ApiRequest.GetInfoData(this, dataUser.Id.ToString());
                    if (data != null)
                    {
                        DataUser = data.Data;
                        LoadDataUser(DataUser); 
                    }
                }
            }
            catch (Exception e)
            {
                ShimmerInflater?.Hide();
                Methods.DisplayReportResultTrack(e);
            }
        }

        private async void LoadDataUser(UserInfoObject dataUser)
        {
            try
            {
                MAdapter.UsersMultiMediaList.Clear();
                MAdapter.NotifyDataSetChanged();

                if (dataUser.Mediafiles?.Count > 0)
                {
                    foreach (var item in dataUser.Mediafiles.Where(item => item.IsVideo == "0" && item.IsApproved == "1" /*&& item.IsPrivate == "0"*/).ToList())
                    {
                        Glide.With(this).Load(item.Avater).Apply(new RequestOptions().SetDiskCacheStrategy(DiskCacheStrategy.All).CenterCrop()).Preload();

                        if (item.IsPrivate == "1")
                        {
                            var fileName = item.PrivateFileFull.Split('/').Last();
                            item.UrlFile = await QuickDateTools.GetFile(dataUser.Id.ToString(), Methods.Path.FolderDiskImage, fileName, item.PrivateFileFull);
                        }
                        else
                        {
                            if (item.IsVideo == "1")
                            {
                                var fileName = item.VideoFile.Split('/').Last();
                                item.UrlFile = await QuickDateTools.GetFile(dataUser.Id.ToString(), Methods.Path.FolderDiskVideo, fileName, item.VideoFile);
                            }
                            else
                            {
                                var fileName = item.Full.Split('/').Last();
                                item.UrlFile = await QuickDateTools.GetFile(dataUser.Id.ToString(), Methods.Path.FolderDiskImage, fileName, item.Full);

                            }
                        }
                    }

                    MAdapter.UsersMultiMediaList = new ObservableCollection<MediaFile>(dataUser.Mediafiles);
                    MAdapter.NotifyDataSetChanged();

                    MainRecyclerView.Visibility = ViewStates.Visible;
                }
                else
                {
                    MainRecyclerView.Visibility = ViewStates.Gone;
                }

                FullGlideRequestBuilder.Load(dataUser.Avater).Into(UserImage);

                Username.Text = QuickDateTools.GetNameFinal(dataUser);
                Username.SetCompoundDrawablesWithIntrinsicBounds(0, 0, dataUser.Verified == "1" ? Resource.Drawable.icon_checkmark_vector : 0, 0);

                if (!string.IsNullOrEmpty(dataUser.About))
                {
                    ReadMoreOption.AddReadMoreTo(AboutText, new Java.Lang.String(Methods.FunString.DecodeString(dataUser.About)));
                    AboutText.Visibility = ViewStates.Visible;
                }
                else
                {
                    AboutText.Visibility = ViewStates.Gone;
                }

                //IconOnline.Visibility = QuickDateTools.GetStatusOnline(dataUser.Lastseen, dataUser.Online) ? ViewStates.Visible : ViewStates.Gone;
                if (!string.IsNullOrEmpty(DataUser.CountryTxt))
                {
                    //var location = await GetAddressAsync(dataUser);
                    LocationTextView.Text = DataUser.CountryTxt;

                    var mapFrag = SupportMapFragment.NewInstance();
                    SupportFragmentManager.BeginTransaction().Add(Resource.Id.map, mapFrag, mapFrag.Tag)?.Commit();
                    mapFrag.GetMapAsync(this);
                }
                else
                {
                    LocationTextView.Text = GetText(Resource.String.Lbl_Unknown);

                    var view = FindViewById<FrameLayout>(Resource.Id.map);
                    if (view != null) view.Visibility = ViewStates.Gone;

                    var mapCardView = FindViewById<FrameLayout>(Resource.Id.mapCardView);
                    if (mapCardView != null) mapCardView.Visibility = ViewStates.Gone;
                }

                if (string.IsNullOrEmpty(dataUser.Interest))
                {
                    IntersetLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    IntersetLayout.Visibility = ViewStates.Visible;

                    var interest = dataUser.Interest.Split(",");
                    MAdapterInterset.MCategoriesList = new ObservableCollection<Classes.CategoriesClass>();
                    foreach (var item in interest)
                    {
                        if (string.IsNullOrEmpty(item) || string.IsNullOrWhiteSpace(item) || item.Contains(","))
                            continue;

                        var color = Methods.FunString.RandomColor();

                        MAdapterInterset.MCategoriesList.Add(new Classes.CategoriesClass()
                        {
                            Name = item,
                            Color1 = color.Item2,
                            Color2 = color.Item1
                        });
                    }
                    MAdapterInterset.NotifyDataSetChanged();
                }

                if (string.IsNullOrEmpty(dataUser.Language))
                {
                    LanguagesLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    LanguagesLayout.Visibility = ViewStates.Visible;

                    if (dataUser.Language.Length > 1)
                    {
                        var language = dataUser.Language.Substring(0, 1).ToUpper() + dataUser.Language?.Substring(1).ToLower();
                        var color = Methods.FunString.RandomColor();
                        MAdapterLanguages.MCategoriesList = new ObservableCollection<Classes.CategoriesClass>()
                        {
                            new Classes.CategoriesClass()
                            {
                                Name = language,
                                Color1 = color.Item2,
                                Color2 = color.Item1
                            }
                        };
                    }

                    MAdapterLanguages.NotifyDataSetChanged();
                }

                if (dataUser.IsFavorite != null && dataUser.IsFavorite.Value)
                {
                    FavoriteButton.SetImageResource(Resource.Drawable.ic_star_vector);
                    FavoriteButton.Tag = "Added";
                }
                else
                {
                    FavoriteButton.SetImageResource(Resource.Drawable.icon_favorites_vector);
                    FavoriteButton.Tag = "Add";
                }

                if (dataUser.IsFriend != null && dataUser.IsFriend.Value)
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, IconFriendRequests, FontAwesomeIcon.UserCheck); //Lbl_UnFriend
                    FriendRequestsButton.SetImageResource(Resource.Drawable.icon_friendcheck_vector);
                    FriendRequestsButton.Tag = "true";
                }
                else if (dataUser.IsFriend != null && !dataUser.IsFriend.Value)
                {
                    if (dataUser.IsFriendRequest != null && !dataUser.IsFriendRequest.Value)
                    {
                        //FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, IconFriendRequests, FontAwesomeIcon.UserPlus); //Lbl_AddFriend
                        FriendRequestsButton.SetImageResource(Resource.Drawable.icon_friendplus_vector);
                        FriendRequestsButton.Tag = "false";
                    }
                    else
                    {
                        //FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, IconFriendRequests, FontAwesomeIcon.UserClock); //Lbl_UnFriend
                        FriendRequestsButton.SetImageResource(Resource.Drawable.icon_friendclock_vector);
                        FriendRequestsButton.Tag = "true";
                    }
                }
                else
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, IconFriendRequests, FontAwesomeIcon.UserPlus); //Lbl_AddFriend
                    FriendRequestsButton.SetImageResource(Resource.Drawable.icon_friendplus_vector);
                    FriendRequestsButton.Tag = "false";
                }

                try
                {
                    string totalTextDescription = String.Empty;

                    string relationship = QuickDateTools.GetRelationship(dataUser.Relationship) ?? "";
                    string workStatus = QuickDateTools.GetWorkStatus(dataUser.WorkStatus) ?? "";
                    //string education = QuickDateTools.GetEducation(dataUser.Education) ?? "";
                    string ethnicity = QuickDateTools.GetEthnicity(dataUser.Ethnicity) ?? "";
                    string body = QuickDateTools.GetBody(dataUser.Body) ?? "";
                    string height = dataUser.Height + " Cm";
                    string hairColor = QuickDateTools.GetHairColor(dataUser.HairColor) ?? "";
                    string character = QuickDateTools.GetCharacter(dataUser.Character) ?? "";
                    string children = QuickDateTools.GetChildren(dataUser.Children) ?? "";
                    string friends = QuickDateTools.GetFriends(dataUser.Friends) ?? "";
                    string pets = QuickDateTools.GetPets(dataUser.Pets) ?? "";
                    string liveWith = QuickDateTools.GetLiveWith(dataUser.LiveWith) ?? "";
                    string car = QuickDateTools.GetCar(dataUser.Car) ?? "";
                    string religion = QuickDateTools.GetReligion(dataUser.Religion) ?? "";
                    string smoke = QuickDateTools.GetSmoke(dataUser.Smoke) ?? "";
                    string drink = QuickDateTools.GetDrink(dataUser.Drink) ?? "";
                    //string travel = QuickDateTools.GetTravel(dataUser.Travel) ?? "";
                    string music = dataUser.Music ?? "";
                    string dish = dataUser.Dish ?? "";
                    string song = dataUser.Song ?? "";
                    string hobby = dataUser.Hobby ?? "";
                    string city = dataUser.City ?? "";
                    string sport = dataUser.Sport ?? "";
                    string book = dataUser.Book ?? "";
                    string movie = dataUser.Movie ?? "";
                    string color = dataUser.Colour ?? "";
                    string tvShow = dataUser.Tv ?? "";

                    if (string.IsNullOrEmpty(workStatus))
                    {
                        WorkAndEducationLayout.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        DescriptionWork.Text = workStatus;
                    }

                    if (!string.IsNullOrEmpty(relationship))
                    {
                        relationship = GetText(Resource.String.Lbl_IAm) + " " + relationship + ", ";
                        totalTextDescription += relationship;
                    }

                    if (!string.IsNullOrEmpty(ethnicity))
                    {
                        ethnicity = ethnicity + " " + GetText(Resource.String.Lbl_person) + ", ";
                        totalTextDescription += ethnicity;
                    }

                    if (!string.IsNullOrEmpty(body))
                    {
                        body = GetText(Resource.String.Lbl_IAm) + " " + body + " ";
                        totalTextDescription += body;
                    }

                    if (!string.IsNullOrEmpty(height) && height != " Cm")
                    {
                        height = !string.IsNullOrEmpty(totalTextDescription)
                            ? GetText(Resource.String.Lbl_And) + " " + height + " " +
                              GetText(Resource.String.Lbl_tall) + ", "
                            : height + " " + GetText(Resource.String.Lbl_tall) + ", ";

                        totalTextDescription += height;
                    }

                    if (!string.IsNullOrEmpty(hairColor))
                    {
                        hairColor = GetText(Resource.String.Lbl_IHave) + " " + hairColor + " " + GetText(Resource.String.Lbl_Hair) + ", ";
                        totalTextDescription += hairColor;
                    }

                    if (!string.IsNullOrEmpty(character))
                    {
                        character = GetText(Resource.String.Lbl_IAm) + " " + character + ", ";
                        totalTextDescription += character;
                    }

                    if (!string.IsNullOrEmpty(friends))
                    {
                        friends = GetText(Resource.String.Lbl_personAnd_IHave) + " " + friends + ", ";
                        totalTextDescription += friends;
                    }

                    if (!string.IsNullOrEmpty(children))
                    {
                        children = GetString(Resource.String.Lbl_My_plans_for_children) + " : " + children + ", ";
                        totalTextDescription += children;
                    }

                    if (!string.IsNullOrEmpty(religion))
                    {
                        religion = GetText(Resource.String.Lbl_IAm_A) + " " + religion + ", ";
                        totalTextDescription += religion;
                    }

                    if (!string.IsNullOrEmpty(liveWith))
                    {
                        if (liveWith == "Alone")
                        {
                            liveWith = GetString(Resource.String.Lbl_ILiveAlone) + ", ";
                        }
                        else
                        {
                            liveWith = GetString(Resource.String.Lbl_ILiveAlone) + " " + liveWith + ", ";
                        }

                        totalTextDescription += liveWith;
                    }

                    if (!string.IsNullOrEmpty(smoke))
                    {
                        if (smoke == "Never")
                        {
                            smoke = GetText(Resource.String.Lbl_IDontSmoke) + ", ";
                        }
                        else
                        {
                            smoke += ", ";
                        }

                        totalTextDescription += smoke;
                    }

                    if (!string.IsNullOrEmpty(drink))
                    {
                        if (drink == "Never")
                        {
                            drink = GetText(Resource.String.Lbl_IDontDrink) + ", ";
                        }
                        else
                        {
                            drink += ", ";
                        }

                        totalTextDescription += drink;
                    }

                    if (!string.IsNullOrEmpty(pets))
                    {
                        pets += ", ";
                        totalTextDescription += pets;
                    }

                    if (!string.IsNullOrEmpty(car))
                    {
                        car += ", ";
                        totalTextDescription += car;
                    }

                    if (!string.IsNullOrEmpty(music))
                    {
                        music = GetText(Resource.String.Lbl_Music) + " : " + music + ", ";
                        totalTextDescription += music;
                    }

                    if (!string.IsNullOrEmpty(dish))
                    {
                        dish = GetText(Resource.String.Lbl_Dish) + " : " + dish + ", ";
                        totalTextDescription += dish;
                    }

                    if (!string.IsNullOrEmpty(song))
                    {
                        song = GetText(Resource.String.Lbl_Song) + " : " + song + ", ";
                        totalTextDescription += song;
                    }

                    if (!string.IsNullOrEmpty(hobby))
                    {
                        hobby = GetText(Resource.String.Lbl_Hobby) + " : " + hobby + ", ";
                        totalTextDescription += hobby;
                    }

                    if (!string.IsNullOrEmpty(city))
                    {
                        city = GetText(Resource.String.Lbl_City) + " : " + city + ", ";
                        totalTextDescription += city;
                    }

                    if (!string.IsNullOrEmpty(sport))
                    {
                        sport = GetText(Resource.String.Lbl_Sport) + " : " + sport + ", ";
                        totalTextDescription += sport;
                    }

                    if (!string.IsNullOrEmpty(book))
                    {
                        book = GetText(Resource.String.Lbl_Book) + " : " + book + ", ";
                        totalTextDescription += book;
                    }

                    if (!string.IsNullOrEmpty(movie))
                    {
                        movie = GetText(Resource.String.Lbl_Movie) + " : " + movie + ", ";
                        totalTextDescription += movie;
                    }

                    if (!string.IsNullOrEmpty(color))
                    {
                        color = GetText(Resource.String.Lbl_Color) + " : " + color + ", ";
                        totalTextDescription += color;
                    }

                    if (!string.IsNullOrEmpty(tvShow))
                    {
                        tvShow = GetText(Resource.String.Lbl_TvShow) + " : " + tvShow + ", ";
                        totalTextDescription += tvShow;
                    }

                    Description.Text = totalTextDescription;
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                    Description.Text = GetText(Resource.String.Lbl_NoDescription);
                    WorkAndEducationLayout.Visibility = ViewStates.Gone;
                }

                if (string.IsNullOrEmpty(dataUser.Google) && string.IsNullOrEmpty(dataUser.Facebook) && string.IsNullOrEmpty(dataUser.Linkedin) && string.IsNullOrEmpty(dataUser.Instagram)
                    && string.IsNullOrEmpty(dataUser.Website) && string.IsNullOrEmpty(dataUser.Twitter))
                {
                    SocialLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    SocialLayout.Visibility = ViewStates.Visible;

                    Typeface font = Typeface.CreateFromAsset(Application.Context.Resources.Assets, "ionicons.ttf");
                    if (!string.IsNullOrEmpty(dataUser.Google))
                    {
                        Google = dataUser.Google;
                        SocialGoogle.SetTypeface(font, TypefaceStyle.Normal);
                        SocialGoogle.Text = IonIconsFonts.LogoGoogle;
                        SocialGoogle.Visibility = ViewStates.Visible;
                    }

                    if (!string.IsNullOrEmpty(dataUser.Facebook))
                    {
                        Facebook = dataUser.Facebook;
                        SocialFacebook.SetTypeface(font, TypefaceStyle.Normal);
                        SocialFacebook.Text = IonIconsFonts.LogoFacebook;
                        SocialFacebook.Visibility = ViewStates.Visible;
                    }

                    if (!string.IsNullOrEmpty(dataUser.Linkedin))
                    {
                        LinkedIn = dataUser.Linkedin;
                        SocialLinkedIn.SetTypeface(font, TypefaceStyle.Normal);
                        SocialLinkedIn.Text = IonIconsFonts.LogoLinkedin;
                        SocialLinkedIn.Visibility = ViewStates.Visible;
                    }

                    if (!string.IsNullOrEmpty(dataUser.Instagram))
                    {
                        Instagram = dataUser.Instagram;
                        SocialInstagram.SetTypeface(font, TypefaceStyle.Normal);
                        SocialInstagram.Text = IonIconsFonts.LogoInstagram;
                        SocialInstagram.Visibility = ViewStates.Visible;
                    }

                    if (!string.IsNullOrEmpty(dataUser.Website))
                    {
                        Website = dataUser.Website;
                        WebsiteButton.SetTypeface(font, TypefaceStyle.Normal);
                        WebsiteButton.Text = IonIconsFonts.Globe;
                        WebsiteButton.Visibility = ViewStates.Visible;
                    }

                    if (!string.IsNullOrEmpty(dataUser.Twitter))
                    {
                        Twitter = dataUser.Twitter;
                        SocialTwitter.SetTypeface(font, TypefaceStyle.Normal);
                        SocialTwitter.Text = IonIconsFonts.LogoTwitter;
                        SocialTwitter.Visibility = ViewStates.Visible;
                    }
                }

                ShimmerInflater?.Hide();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private async Task<string> GetAddressAsync(UserInfoObject userInfo)
        {
            try
            {
#pragma warning disable 618
                var locale = (int)Build.VERSION.SdkInt < 25 ? Resources?.Configuration?.Locale : Resources?.Configuration?.Locales.Get(0) ?? Resources?.Configuration?.Locale;
#pragma warning restore 618
                Geocoder coder = new Geocoder(this, locale);

                var latLng = await GetLocationFromAddress(userInfo.Location);

                var addresses = await coder.GetFromLocationAsync(latLng.Latitude, latLng.Longitude, 1);
                var address = addresses?.FirstOrDefault();
                if (address != null)
                {
                    string nearByAddress = string.Empty;
                    var hasAdminArea = !string.IsNullOrWhiteSpace(address.AdminArea);
                    var hasSubAdminArea = !string.IsNullOrWhiteSpace(address.SubAdminArea);
                    var hasCountry = !string.IsNullOrWhiteSpace(address.CountryName);

                    if (hasSubAdminArea)
                    {
                        nearByAddress += address.SubAdminArea + (hasAdminArea ? ", " : string.Empty);
                    }

                    if (hasAdminArea)
                    {
                        nearByAddress += address.AdminArea + (hasCountry ? ", " : string.Empty);
                    }

                    if (hasCountry)
                    {
                        nearByAddress += address.CountryName;
                    }

                    return nearByAddress;
                }
                return null;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        private bool SetFav(CircleButton favButton)
        {
            try
            {
                if (favButton?.Tag?.ToString() == "Added")
                {
                    favButton.SetImageResource(Resource.Drawable.icon_favorites_vector);
                    favButton.Tag = "Add";
                    return false;
                }
                else
                {
                    favButton.SetImageResource(Resource.Drawable.ic_star_vector);
                    favButton.Tag = "Added";
                    return true;
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return false;
            }
        }

        private void GetStickersGiftsLists()
        {
            try
            {
                var sqlEntity = new SqLiteDatabase();

                var listGifts = sqlEntity.GetGiftsList();
                var listStickers = sqlEntity.GetStickersList();

                if (ListUtils.StickersList.Count == 0 && listStickers?.Count > 0)
                    ListUtils.StickersList = listStickers;

                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetStickers(this) });

                if (ListUtils.GiftsList.Count == 0 && listGifts?.Count > 0)
                    ListUtils.GiftsList = listGifts;

                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetGifts(this) });
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #region Permissions

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 100)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        Intent intent = new Intent(this, typeof(MessagesBoxActivity));
                        intent.PutExtra("UserId", DataUser.Id.ToString());
                        intent.PutExtra("UserItem", JsonConvert.SerializeObject(DataUser));
                        StartActivity(intent);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long)?.Show();
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region MaterialDialog

        public async void OnSelection(IDialogInterface dialog, int position, string itemString)
        {
            try
            {
                if (itemString == GetText(Resource.String.Lbl_Block))
                {
                    if (Methods.CheckConnectivity())
                    {
                        var list = GlobalContext?.ProfileFragment?.FavoriteFragment?.MAdapter?.UserList;
                        if (list?.Count > 0)
                        {
                            var dataFav = list.FirstOrDefault(a => a.Id == DataUser?.Id);
                            if (dataFav != null)
                            {
                                list.Remove(dataFav);
                                GlobalContext?.ProfileFragment?.FavoriteFragment?.MAdapter.NotifyDataSetChanged();
                            }
                        }

                        Toast.MakeText(this, GetText(Resource.String.Lbl_Blocked_successfully), ToastLength.Long)?.Show();

                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.BlockAsync(DataUser?.Id.ToString()) });

                        Finish();
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    }
                }
                else if (itemString == GetText(Resource.String.Lbl_Report))
                {
                    if (Methods.CheckConnectivity())
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Text_report), ToastLength.Short)?.Show();
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.ReportAsync(DataUser?.Id.ToString()) });
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    }
                }
                else if (itemString == GetText(Resource.String.Lbl_Share))
                {
                    //Share Plugin same as video
                    if (!CrossShare.IsSupported) return;

                    await CrossShare.Current.Share(new ShareMessage
                    {
                        Title = InitializeQuickDate.WebsiteUrl + "/@" + DataUser.Username,
                        Text = GetText(Resource.String.Lbl_Check) + " " + QuickDateTools.GetNameFinal(DataUser) + " " + GetText(Resource.String.Lbl_ProfileOn) + " " + AppSettings.ApplicationName + "!",
                        Url = InitializeQuickDate.WebsiteUrl + "/@" + DataUser.Username
                    });
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Location

        public async void OnMapReady(GoogleMap googleMap)
        {
            try
            {
                if (!string.IsNullOrEmpty(DataUser.Location))
                {
                    var latLng = await GetLocationFromAddress(DataUser.Location);
                    if (latLng != null)
                    {
                        CurrentLatitude = latLng.Latitude;
                        CurrentLongitude = latLng.Longitude;
                    }

                    Map = googleMap;

                    //Optional
                    googleMap.UiSettings.ZoomControlsEnabled = false;
                    googleMap.UiSettings.CompassEnabled = false;

                    googleMap.MoveCamera(CameraUpdateFactory.ZoomIn());

                    var makerOptions = new MarkerOptions();
                    makerOptions.SetPosition(new LatLng(CurrentLatitude, CurrentLongitude));
                    makerOptions.SetTitle(GetText(Resource.String.Lbl_Location));

                    Map.AddMarker(makerOptions);
                    Map.MapType = GoogleMap.MapTypeNormal;

                    if (QuickDateTools.IsTabDark())
                    {
                        MapStyleOptions style = MapStyleOptions.LoadRawResourceStyle(this, Resource.Raw.map_dark);
                        Map.SetMapStyle(style);
                    }

                    CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                    builder.Target(new LatLng(CurrentLatitude, CurrentLongitude));
                    builder.Zoom(10);
                    builder.Bearing(155);
                    builder.Tilt(65);

                    CameraPosition cameraPosition = builder.Build();

                    CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                    googleMap.MoveCamera(cameraUpdate);

                    Map.MapClick += MapOnMapClick;

                    var view = FindViewById<FrameLayout>(Resource.Id.map);
                    if (view != null) view.Visibility = ViewStates.Visible;

                    var mapCardView = FindViewById<FrameLayout>(Resource.Id.mapCardView);
                    if (mapCardView != null) mapCardView.Visibility = ViewStates.Visible;
                }
                else
                {
                    var view = FindViewById<FrameLayout>(Resource.Id.map);
                    if (view != null) view.Visibility = ViewStates.Gone;

                    var mapCardView = FindViewById<FrameLayout>(Resource.Id.mapCardView);
                    if (mapCardView != null) mapCardView.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void MapOnMapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            try
            {
                // Create a Uri from an intent string. Use the result to create an Intent?. 
                var uri = Uri.Parse("geo:" + CurrentLatitude + "," + CurrentLongitude);
                var intent = new Intent(Intent.ActionView, uri);
                intent.SetPackage("com.google.android.apps.maps");
                intent.AddFlags(ActivityFlags.NewTask);
                StartActivity(intent);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private async Task<LatLng> GetLocationFromAddress(string strAddress)
        {
#pragma warning disable 618
            var locale = (int)Build.VERSION.SdkInt < 25 ? Resources?.Configuration?.Locale : Resources?.Configuration?.Locales.Get(0) ?? Resources?.Configuration?.Locale;
#pragma warning restore 618
            Geocoder coder = new Geocoder(this, locale);

            try
            {
                var address = await coder.GetFromLocationNameAsync(strAddress, 2);
                if (address == null)
                    return null;

                Address location = address[0];
                var lat = location.Latitude;
                var lng = location.Longitude;

                LatLng p1 = new LatLng(lat, lng);
                return p1;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        #endregion


    }
}