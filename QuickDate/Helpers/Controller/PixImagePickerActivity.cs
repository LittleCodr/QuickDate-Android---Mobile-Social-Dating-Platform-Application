using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.Content;
using IO.Ak1.Pix.Helpers;
using IO.Ak1.Pix.Models;
using Newtonsoft.Json;
using QuickDate.Activities.Base;
using QuickDate.Helpers.Utils;
using System;
using System.Collections.Generic;
using Object = Java.Lang.Object;
using Uri = Android.Net.Uri;

namespace QuickDate.Helpers.Controller
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class PixImagePickerActivity : BaseActivity, Kotlin.Jvm.Functions.IFunction1
    {
        public const int RequestCode = 4352;
        private OptionPixImage OptionPixImage;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.PixImagePickerLayout);
                SystemUiHelperKt.SetupScreen(this);

                SupportActionBar?.Hide();

                var options = new Options();
                options.Ratio = Ratio.RatioAuto; //Image/video capture ratio
                Methods.Path.Chack_MyFolder();

                OptionPixImage = JsonConvert.DeserializeObject<OptionPixImage>(Intent?.GetStringExtra("OptionPixImage") ?? "") ?? new OptionPixImage();
                if (OptionPixImage != null)
                {
                    options.Count = OptionPixImage.AllowMultiple ? 10 : //Number of images to restrict selection count 
                        1; //Number of images to restrict selection count

                    switch (OptionPixImage.Mode)
                    {
                        case "All":
                            options.Mode = Mode.All;
                            break;
                        case "Picture":
                            options.Mode = Mode.Picture;
                            break;
                        case "Video":
                            options.Mode = Mode.Video;
                            break;
                        default:
                            options.Mode = Mode.All;
                            break;
                    }

                    options.Path = OptionPixImage.Path; //Custom Path For media Storage
                }
                else
                {
                    options.Count = 1; //Number of images to restrict selection count
                    options.Mode = Mode.All;
                    options.Path = Methods.Path.FolderDiskMyApp; //Custom Path For media Storage
                }

                options.SpanCount = 4; //Number for columns in grid
                options.FrontFacing = false; //Front Facing camera on start
                options.VideoOptions = new VideoOptions()
                {
                    // VideoDurationLimitInSeconds = 30 //Duration for video recording
                };

                options.Flash = Flash.Auto; //Option to select flash type
                options.PreSelectedUrls = new List<Uri>();

                UsabilityHelperKt.AddPixToActivity(this, Resource.Id.container, options, this);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnBackPressed()
        {
            try
            {
                PixBus.Instance.OnBackPressedEvent();
                base.OnBackPressed();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public Object Invoke(Object p0)
        {
            try
            {
                if (p0 is PixEventCallback.Results callback)
                {
                    if (callback.Status == PixEventCallback.Status.Success)
                    {
                        List<string> list = new List<string>();
                        foreach (var uri in callback.Data)
                        {
                            var filepath = Methods.AttachmentFiles.GetActualPathFromFile(this, uri);
                            if (!string.IsNullOrEmpty(filepath))
                            {
                                list.Add(filepath);
                            }
                        }

                        ResultIntentPixImage ResultPixImage = new ResultIntentPixImage()
                        {
                            IsSuccessful = true,
                            List = list
                        };

                        var resultIntent = new Intent();
                        resultIntent.PutExtra("ResultPixImage", JsonConvert.SerializeObject(ResultPixImage));
                        SetResult(Result.Ok, resultIntent);

                        Finish();
                    }
                    else if (callback.Status == PixEventCallback.Status.BackPressed)
                    {
                        SupportFragmentManager.PopBackStack();
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
            return p0;
        }
    }

    public class PixImagePickerUtils
    {
        #region Dialog Gallery

        public static void OpenDialogGallery(Activity activity, bool allowVideo = false, bool allowMultiple = false)
        {
            try
            {
                OptionPixImage optionPixImage = OptionPixImage.GetOptionPixImage(allowVideo, allowMultiple);

                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    Intent intent = new Intent(activity, typeof(PixImagePickerActivity));
                    intent.PutExtra("OptionPixImage", JsonConvert.SerializeObject(optionPixImage));
                    activity.StartActivityForResult(intent, PixImagePickerActivity.RequestCode);
                }
                else
                {
                    if (PermissionsController.CheckPermissionStorage(activity, "file") && ContextCompat.CheckSelfPermission(activity, Manifest.Permission.Camera) == Permission.Granted)
                    {
                        Intent intent = new Intent(activity, typeof(PixImagePickerActivity));
                        intent.PutExtra("OptionPixImage", JsonConvert.SerializeObject(optionPixImage));
                        activity.StartActivityForResult(intent, PixImagePickerActivity.RequestCode);
                    }
                    else
                    {
                        new PermissionsController(activity).RequestPermission(108, "file");
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion
    }

    public class ResultIntentPixImage
    {
        public bool IsSuccessful { set; get; }
        public List<string> List { set; get; }
    }

    public class OptionPixImage
    {
        public string Mode { set; get; }
        public bool AllowMultiple { set; get; }
        public string Path { set; get; }

        public static OptionPixImage GetOptionPixImage(bool allowVideo = false, bool allowMultiple = false)
        {
            try
            {
                OptionPixImage optionPixImage = new OptionPixImage()
                {
                    AllowMultiple = allowMultiple,
                    Mode = allowVideo ? "All" : "Picture",
                    Path = allowVideo ? Methods.Path.FolderDiskMyApp : Methods.Path.FolderDiskImage
                };
                return optionPixImage;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }

}