using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.Content.Res;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Util;
using Com.Google.Android.Gms.Ads;
using Google.Android.Material.Dialog;
using Google.Android.Material.FloatingActionButton;
using Java.IO;
using Newtonsoft.Json;
using QuickDate.Activities.Base;
using QuickDate.Activities.MyProfile.Adapters;
using QuickDate.Activities.Viewer;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.Library.Anjo.IntegrationRecyclerView;
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
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;
using Uri = Android.Net.Uri;

namespace QuickDate.Activities.MyProfile
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class AllMediaActivity : BaseActivity
    {
        #region Variables Basic

        private AllMediaAdapter MAdapter;
        private SwipeRefreshLayout SwipeRefreshLayout;
        private RecyclerView MRecycler;
        private GridLayoutManager LayoutManager;
        private ViewStub EmptyStateLayout;
        private View Inflated;
        private AdView MAdView;
        private FloatingActionButton AddButton;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);
                SetTheme(QuickDateTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);

                // Create your application here
                SetContentView(Resource.Layout.RecyclerDefaultLayout);

                //Get Value And Set Toolbar
                InitComponent();
                InitBackground();
                InitToolbar();
                SetRecyclerViewAdapters();
                InitBackPressed();

                GetMyInfoData();
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
                AdsGoogle.LifecycleAdView(MAdView, "Resume");
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
                AdsGoogle.LifecycleAdView(MAdView, "Pause");
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

        protected override void OnDestroy()
        {
            try
            {
                AdsGoogle.LifecycleAdView(MAdView, "Destroy");
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Menu 

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    BackPressed();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public void BackPressed()
        {
            try
            {
                SetResult(Result.Ok);
                Finish();
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
                MRecycler = (RecyclerView)FindViewById(Resource.Id.recyler);
                EmptyStateLayout = FindViewById<ViewStub>(Resource.Id.viewStub);

                SwipeRefreshLayout = (SwipeRefreshLayout)FindViewById(Resource.Id.swipeRefreshLayout);
                SwipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
                SwipeRefreshLayout.Refreshing = true;
                SwipeRefreshLayout.Enabled = true;
                SwipeRefreshLayout.SetProgressBackgroundColorSchemeColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#424242") : Color.ParseColor("#f7f7f7"));
                SwipeRefreshLayout.SetPadding(5, 0, 0, 0);

                AddButton = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButtonView);
                AddButton.Visibility = ViewStates.Visible;

                MAdView = FindViewById<AdView>(Resource.Id.adView);
                AdsGoogle.InitAdView(MAdView, MRecycler);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void InitToolbar()
        {
            try
            {
                var toolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolBar != null)
                {
                    toolBar.Title = GetText(Resource.String.Lbl_MediaFiles);

                    toolBar.SetTitleTextColor(QuickDateTools.IsTabDark() ? AppSettings.TitleTextColorDark : AppSettings.TitleTextColor);
                    SetSupportActionBar(toolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);

                    var icon = AppCompatResources.GetDrawable(this, AppSettings.FlowDirectionRightToLeft ? Resource.Drawable.icon_back_arrow_right : Resource.Drawable.icon_back_arrow_left);
                    icon?.SetTint(QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                    SupportActionBar.SetHomeAsUpIndicator(icon);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void SetRecyclerViewAdapters()
        {
            try
            {
                MAdapter = new AllMediaAdapter(this)
                {
                    MediaList = new ObservableCollection<MediaFile>()
                };

                LayoutManager = new GridLayoutManager(this, 3);
                LayoutManager.SetSpanSizeLookup(new MySpanSizeLookup(8, 1, 2));
                MRecycler.AddItemDecoration(new GridSpacingItemDecoration(1, 1, true));
                MRecycler.SetLayoutManager(LayoutManager);
                MRecycler.HasFixedSize = true;
                MRecycler.SetItemViewCacheSize(10);
                MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;
                var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                var preLoader = new RecyclerViewPreloader<MediaFile>(this, MAdapter, sizeProvider, 8);
                MRecycler.AddOnScrollListener(preLoader);
                MRecycler.SetAdapter(MAdapter);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void InitBackPressed()
        {
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                {
                    OnBackInvokedDispatcher.RegisterOnBackInvokedCallback(0, new BackCallAppBase2(this, "AllMediaActivity"));
                }
                else
                {
                    OnBackPressedDispatcher.AddCallback(new BackCallAppBase1(this, "AllMediaActivity", true));
                }
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
                    MAdapter.OnItemClick += MAdapterOnItemClick;
                    MAdapter.OnRemoveItemClick += MAdapterOnOnRemoveItemClick;
                    SwipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh;
                    AddButton.Click += AddButtonOnClick;
                }
                else
                {
                    MAdapter.OnItemClick -= MAdapterOnItemClick;
                    MAdapter.OnRemoveItemClick -= MAdapterOnOnRemoveItemClick;
                    SwipeRefreshLayout.Refresh -= SwipeRefreshLayoutOnRefresh;
                    AddButton.Click -= AddButtonOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        //Add Media File 
        private void AddButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
                    return;
                }

                var naxPhotoPerUser = ListUtils.SettingsSiteList?.MaxPhotoPerUser;
                var isPro = ListUtils.MyUserInfo?.FirstOrDefault()?.IsPro ?? "0";
                if (isPro == "0" && MAdapter.ItemCount == Convert.ToInt32(naxPhotoPerUser))
                {
                    //You have exceeded the maximum of likes or swipes per this day
                    Toast.MakeText(this, GetString(Resource.String.Lbl_ErrorLimitOfMediaUploads), ToastLength.Short)?.Show();
                    return;
                }

                PixImagePickerUtils.OpenDialogGallery(this, true);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Refresh
        private void SwipeRefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            try
            {
                MAdapter.MediaList.Clear();
                MAdapter.NotifyDataSetChanged();

                StartApiService();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Open 
        private async void MAdapterOnItemClick(object sender, AllMediaAdapterClickEventArgs e)
        {
            try
            {
                var position = e.Position;
                if (position > -1)
                {
                    var item = MAdapter.GetItem(position);
                    if (item != null)
                    {
                        if (string.IsNullOrEmpty(item.UrlFile))
                            item.UrlFile = item.Full;

                        var fileName = item.UrlFile.Split('/').Last();
                        if (item.IsVideo == "1" && !string.IsNullOrEmpty(item.VideoFile))
                        {
                            item.UrlFile = await QuickDateTools.GetFile(UserDetails.UserId.ToString(), Methods.Path.FolderDiskVideo, fileName, item.VideoFile);

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
                            item.UrlFile = await QuickDateTools.GetFile(UserDetails.UserId.ToString(), Methods.Path.FolderDiskImage, fileName, item.Full);

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
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Remove 
        private void MAdapterOnOnRemoveItemClick(object sender, AllMediaAdapterClickEventArgs e)
        {
            try
            {
                var position = e.Position;
                if (position > -1)
                {
                    var item = MAdapter.GetItem(position);
                    if (item != null)
                    {
                        if (!Methods.CheckConnectivity())
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                            return;
                        }

                        if (!string.IsNullOrEmpty(item.Id))
                        {
                            var dialog = new MaterialAlertDialogBuilder(this);
                            dialog.SetTitle(GetText(Resource.String.Lbl_Warning));
                            dialog.SetMessage(GetText(Resource.String.Lbl_AskDeleteFile));
                            dialog.SetPositiveButton(GetText(Resource.String.Lbl_Yes), (materialDialog, action) =>
                            {
                                try
                                {
                                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.DeleteMediaFileUserAsync(item.Id) });

                                    MAdapter.Remove(item);

                                    var dataUser = ListUtils.MyUserInfo?.FirstOrDefault();
                                    var dataImage = dataUser?.Mediafiles?.FirstOrDefault(file => file.Id == item.Id);
                                    if (dataImage != null)
                                    {
                                        dataUser.Mediafiles?.Remove(dataImage);

                                        SqLiteDatabase dbDatabase = new SqLiteDatabase();
                                        dbDatabase.InsertOrUpdate_DataMyInfo(dataUser);

                                    }
                                }
                                catch (Exception exception)
                                {
                                    Methods.DisplayReportResultTrack(exception);
                                }
                            });
                            dialog.SetNegativeButton(GetText(Resource.String.Lbl_No), new MaterialDialogUtils());
                            dialog.Show();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Permissions && Result

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                if (requestCode == PixImagePickerActivity.RequestCode && resultCode == Result.Ok)
                {
                    var listPath = JsonConvert.DeserializeObject<ResultIntentPixImage>(data.GetStringExtra("ResultPixImage") ?? "");
                    if (listPath?.List?.Count > 0)
                    {
                        var filepath = listPath.List.FirstOrDefault();
                        if (!string.IsNullOrEmpty(filepath))
                        {
                            //Do something with your Uri 
                            PickiTonCompleteListener(filepath);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                if (requestCode == 108)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        PixImagePickerUtils.OpenDialogGallery(this);
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

        #region Load Media Files 

        private void StartApiService()
        {
            if (!Methods.CheckConnectivity())
                Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
            else
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { LoadMediaFilesAsync });
        }

        private async Task LoadMediaFilesAsync()
        {
            if (Methods.CheckConnectivity())
            {
                int countList = MAdapter.MediaList.Count;
                var (apiStatus, respond) = await RequestsAsync.Users.ProfileAsync(UserDetails.UserId.ToString(), "data,media");
                if (apiStatus != 200 || respond is not ProfileObject result || result.Data == null)
                {
                    Methods.DisplayReportResult(this, respond);
                }
                else
                {
                    var respondList = result.Data.Mediafiles.Count;
                    if (respondList > 0)
                    {
                        foreach (var item in from item in result.Data.Mediafiles let check = MAdapter.MediaList.FirstOrDefault(a => a.Id == item.Id) where check == null select item)
                        {
                            MAdapter.MediaList.Add(item);

                            await Task.Factory.StartNew(async () =>
                            {
                                try
                                {
                                    if (item.IsVideo == "1")
                                    {
                                        var fileName = item.VideoFile.Split('/').Last();
                                        item.UrlFile = await QuickDateTools.GetFile(UserDetails.UserId.ToString(), Methods.Path.FolderDiskVideo, fileName, item.VideoFile);
                                    }
                                    else
                                    {
                                        var fileName = item.Full.Split('/').Last();
                                        item.UrlFile = await QuickDateTools.GetFile(UserDetails.UserId.ToString(), Methods.Path.FolderDiskImage, fileName, item.Full);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Methods.DisplayReportResultTrack(e);
                                }
                            }).ConfigureAwait(false);
                        }

                        if (countList > 0)
                        {
                            RunOnUiThread(() => { MAdapter.NotifyItemRangeInserted(countList, MAdapter.MediaList.Count - countList); });
                        }
                        else
                        {
                            RunOnUiThread(() => { MAdapter.NotifyDataSetChanged(); });
                        }
                    }
                }

                RunOnUiThread(ShowEmptyPage);
            }
            else
            {
                Inflated = EmptyStateLayout.Inflate();
                EmptyStateInflater x = new EmptyStateInflater();
                x.InflateLayout(Inflated, EmptyStateInflater.Type.NoConnection);
                if (!x.EmptyStateButton.HasOnClickListeners)
                {
                    x.EmptyStateButton.Click += null;
                    x.EmptyStateButton.Click += EmptyStateButtonOnClick;
                }

                Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
            }
        }

        private void ShowEmptyPage()
        {
            try
            {
                SwipeRefreshLayout.Refreshing = false;

                if (MAdapter.MediaList.Count > 0)
                {
                    MRecycler.Visibility = ViewStates.Visible;
                    EmptyStateLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    MRecycler.Visibility = ViewStates.Gone;

                    if (Inflated == null)
                        Inflated = EmptyStateLayout.Inflate();

                    EmptyStateInflater x = new EmptyStateInflater();
                    x.InflateLayout(Inflated, EmptyStateInflater.Type.NoMedia);
                    if (!x.EmptyStateButton.HasOnClickListeners)
                    {
                        x.EmptyStateButton.Click += null;
                    }
                    EmptyStateLayout.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception e)
            {
                SwipeRefreshLayout.Refreshing = false;
                Methods.DisplayReportResultTrack(e);
            }
        }

        //No Internet Connection 
        private void EmptyStateButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                StartApiService();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        private void PickiTonCompleteListener(string filepath)
        {
            try
            {
                var type = Methods.AttachmentFiles.Check_FileExtension(filepath);
                if (type == "Image")
                {
                    SendFile(filepath, filepath, "Image");
                }
                else if (type == "Video")
                {
                    var fileName = filepath.Split('/').Last();
                    var fileNameWithoutExtension = fileName.Split('.').First();

                    var path = Methods.Path.FolderDcimVideo + "/" + fileNameWithoutExtension + ".png";

                    var videoPlaceHolderImage = Methods.MultiMedia.GetMediaFrom_Gallery(Methods.Path.FolderDcimVideo, fileNameWithoutExtension + ".png");
                    if (videoPlaceHolderImage == "File Dont Exists")
                    {
                        var bitmapImage = Methods.MultiMedia.Retrieve_VideoFrame_AsBitmap(this, filepath);
                        if (bitmapImage != null)
                            Methods.MultiMedia.Export_Bitmap_As_Image(bitmapImage, fileNameWithoutExtension, Methods.Path.FolderDcimVideo);
                        else
                        {
                            File file2 = new File(filepath);
                            var photoUri = FileProvider.GetUriForFile(this, PackageName + ".fileprovider", file2);

                            Glide.With(this)
                                .AsBitmap()
                                .Load(photoUri) // or URI/path
                                .Into(new MySimpleTarget(filepath)); //image view to set thumbnail to 
                        }
                    }

                    SendFile(filepath, path, "Video");
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private async void SendFile(string path, string thumb, string type)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
                    return;
                }

                //Show a progress
                AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                var time = Methods.GetTimestamp(DateTime.Now);
                MAdapter.MediaList.Insert(0, new MediaFile
                {
                    Id = time,
                    Full = type == "Image" ? path : thumb,
                    Avater = thumb,
                    UrlFile = path,
                });
                MAdapter.NotifyDataSetChanged();

                switch (type)
                {
                    case "Image":
                        {
                            //sent api 
                            var (apiStatus, respond) = await RequestsAsync.Users.UploadMediaFileUserAsync(path);
                            if (apiStatus == 200)
                            {
                                if (respond is UploadVideoFileObject resultImage)
                                {
                                    var dataUser = ListUtils.MyUserInfo?.FirstOrDefault();
                                    dataUser?.Mediafiles.Insert(0, new MediaFile
                                    {
                                        Avater = path,
                                        Full = path,
                                        Id = resultImage.Data.Id
                                    });

                                    var item = MAdapter.MediaList.FirstOrDefault(a => a.Id == time);
                                    if (item != null)
                                    {
                                        item.Id = resultImage.Data.Id;
                                    }

                                    var reviewMediaFiles = ListUtils.SettingsSiteList?.ReviewMediaFiles;
                                    if (reviewMediaFiles == "1") //Uploaded successfully, file will be reviewed
                                        Toast.MakeText(this, GetText(Resource.String.Lbl_UploadedSuccessfullyWithReviewed), ToastLength.Long)?.Show();
                                    else
                                        Toast.MakeText(this, GetText(Resource.String.Lbl_UploadedSuccessfully), ToastLength.Long)?.Show();

                                    AndHUD.Shared.Dismiss();
                                }
                            }
                            else
                            {
                                //Show a Error image with a message
                                if (respond is ErrorObject resultImage)
                                {
                                    if (resultImage.ErrorData.ErrorId == "233")
                                    {
                                        AndHUD.Shared.ShowError(this, GetText(Resource.String.Lbl_ErrorLimitOfMediaUploads), MaskType.Clear, TimeSpan.FromSeconds(2));
                                    }
                                    else
                                    {
                                        Methods.DisplayAndHudErrorResult(this, respond);
                                    }
                                }
                            }
                            break;
                        }
                    case "Video":
                        {
                            //sent api 
                            var (apiStatus, respond) = await RequestsAsync.Users.UploadVideoFileUserAsync(path, thumb);
                            if (apiStatus == 200)
                            {
                                if (respond is UploadVideoFileObject result)
                                {
                                    var newPath = result.Data.VideoFile;
                                    if (!newPath.Contains(InitializeQuickDate.WebsiteUrl))
                                    {
                                        newPath = InitializeQuickDate.WebsiteUrl + result.Data.VideoFile;
                                    }

                                    var newPathAvatar = result.Data.File;
                                    if (!newPathAvatar.Contains(InitializeQuickDate.WebsiteUrl))
                                    {
                                        newPathAvatar = InitializeQuickDate.WebsiteUrl + result.Data.File;
                                    }

                                    var dataUser = ListUtils.MyUserInfo?.FirstOrDefault();
                                    dataUser?.Mediafiles.Insert(0, new MediaFile
                                    {
                                        Full = newPathAvatar,
                                        VideoFile = newPath,
                                        Id = result.Data.Id,
                                        IsVideo = "1"
                                    });

                                    var item = MAdapter.MediaList.FirstOrDefault(a => a.Id == time);
                                    if (item != null)
                                    {
                                        item.Full = newPathAvatar;
                                        item.VideoFile = newPath;
                                        item.Id = result.Data.Id;

                                        var index = MAdapter.MediaList.IndexOf(MAdapter.MediaList.FirstOrDefault(a => a.Id == time));
                                        if (index > -1)
                                            MAdapter.NotifyItemChanged(index);
                                    }

                                    var reviewMediaFiles = ListUtils.SettingsSiteList?.ReviewMediaFiles;
                                    if (reviewMediaFiles == "1") //Uploaded successfully, file will be reviewed
                                        Toast.MakeText(this, GetText(Resource.String.Lbl_UploadedSuccessfullyWithReviewed), ToastLength.Long)?.Show();
                                    else
                                        Toast.MakeText(this, GetText(Resource.String.Lbl_UploadedSuccessfully), ToastLength.Long)?.Show();

                                    AndHUD.Shared.Dismiss();
                                }
                            }
                            else
                            {
                                if (respond is ErrorObject resultImage)
                                {
                                    if (resultImage.ErrorData.ErrorId == "233")
                                    {
                                        AndHUD.Shared.ShowError(this, GetText(Resource.String.Lbl_ErrorLimitOfMediaUploads), MaskType.Clear, TimeSpan.FromSeconds(2));
                                    }
                                    else
                                    {
                                        Methods.DisplayAndHudErrorResult(this, respond);
                                    }
                                }
                            }
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                AndHUD.Shared.Dismiss();
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void GetMyInfoData()
        {
            try
            {
                if (ListUtils.MyUserInfo.Count == 0)
                {
                    var sqlEntity = new SqLiteDatabase();
                    sqlEntity.GetDataMyInfo();

                }

                var dataUser = ListUtils.MyUserInfo?.FirstOrDefault();
                if (dataUser != null)
                {
                    if (dataUser.Mediafiles?.Count > 0)
                    {
                        MAdapter.MediaList = new ObservableCollection<MediaFile>(dataUser.Mediafiles);
                        MAdapter.NotifyDataSetChanged();
                    }
                }

                StartApiService();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

    }
}