using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Locations;
using Android.OS;
using Android.Provider;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Browser.CustomTabs;
using AndroidX.Core.Content;
using Java.IO;
using Java.Text;
using Java.Util;
using QuickDate.Helpers.Utils;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;

namespace QuickDate.Helpers.Controller
{
    public class IntentController
    {
        //############################# DON'T MODIFY HERE ##########################

        private readonly AppCompatActivity Context;
        public static string CurrentPhotoPath;
        public static string CurrentVideoPath;

        public IntentController(AppCompatActivity context)
        {
            try
            {
                Context = context;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //############################# Special for application ##########################

        //################################# General #################################
        /// <summary>
        /// Open intent Image Gallery when the request code of result is 500
        /// </summary>
        /// <param name="title"></param>
        /// <param name="allowMultiple"></param>
        /// <param name="imageCropping"></param>
        public void OpenIntentImageGallery(string title, bool allowMultiple, bool imageCropping = true)
        {
            try
            {
                Methods.Path.Chack_MyFolder();

                Intent intent;
                if ((int)Build.VERSION.SdkInt <= 25)
                    intent = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
                else
                    intent = Environment.GetExternalStorageState(null)!.Equals(Environment.MediaMounted) ? new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri) : new Intent(Intent.ActionPick, MediaStore.Images.Media.InternalContentUri);

                intent.SetType("image/*");
                intent.PutExtra("return-data", true); //added snippet

                if (Build.VERSION.SdkInt > BuildVersionCodes.Q)
                    intent.SetAction(Intent.ActionGetContent);

                if (imageCropping && Build.Manufacturer!.ToLower() == "samsung")
                {
                    intent.PutExtra("crop", "true");
                    var myUri = Uri.FromFile(new File(Methods.Path.FolderDcimImage, Methods.GetTimestamp(DateTime.Now) + ".jpg"));
                    intent.PutExtra(MediaStore.ExtraOutput, myUri);
                    //intent.PutExtra("outputFormat", Bitmap.CompressFormat.Jpeg.ToString());
                }

                if (allowMultiple)
                {
                    intent = new Intent(Intent.ActionPick);
                    intent.SetType("image/*");
                    intent.PutExtra(Intent.ExtraAllowMultiple, true);
                    if (Build.VERSION.SdkInt > BuildVersionCodes.Q)
                        intent.SetAction(Intent.ActionGetContent);
                    intent.PutExtra("return-data", true); //added snippet 
                }

                if (intent.ResolveActivity(Context.PackageManager) != null)
                    Context.StartActivityForResult(Intent.CreateChooser(intent, title), 500);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        /// <summary>
        /// Open intent video Gallery when the request code of result is 501
        /// </summary>
        public void OpenIntentVideoGallery(string title = "video")
        {
            try
            {
                Methods.Path.Chack_MyFolder();

                //var intent = new Intent(Intent.ActionPick, MediaStore.Video.Media.ExternalContentUri);
                ////intent.SetAction(Intent.ActionGetContent);
                //intent.SetType("video/*");
                //intent.PutExtra("return-data", true); //added snippet
                //Context.StartActivityForResult(Intent.CreateChooser(intent, title), 501);
                Intent intent;
                if ((int)Build.VERSION.SdkInt <= 25)
                    intent = new Intent(Intent.ActionPick, MediaStore.Video.Media.ExternalContentUri);
                else
                    intent = Environment.GetExternalStorageState(null)!.Equals(Environment.MediaMounted) ? new Intent(Intent.ActionPick, MediaStore.Video.Media.ExternalContentUri) : new Intent(Intent.ActionPick, MediaStore.Video.Media.InternalContentUri);

                //  In this example we will set the type to video
                intent.SetType("video/*");
                intent.PutExtra("return-data", true); //added snippet

                switch (Build.VERSION.SdkInt)
                {
                    case > BuildVersionCodes.Q:
                        intent.SetAction(Intent.ActionGetContent);
                        break;
                }

                intent.AddFlags(ActivityFlags.GrantReadUriPermission);

                if (intent.ResolveActivity(Context.PackageManager) != null)
                    Context.StartActivityForResult(Intent.CreateChooser(intent, title), 501);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        /// <summary>
        /// Open intent Location when the request code of result is 502
        /// </summary>
        public void OpenIntentLocation()
        {
            try
            {
                var locationManager = (LocationManager)Context.GetSystemService(Android.Content.Context.LocationService);
                if (!locationManager.IsProviderEnabled(LocationManager.GpsProvider))
                {
                    //Open intent Gps
                    OpenIntentGps(locationManager);
                    return;
                }

                //Intent intent = new Intent(Context, typeof(LocationActivity));
                //Context.StartActivityForResult(intent, 502);
            }
            catch (GooglePlayServicesRepairableException e)
            {
                Methods.DisplayReportResultTrack(e);
                Toast.MakeText(Context, "Google Play Services is not available.", ToastLength.Short)?.Show();
            }
            catch (GooglePlayServicesNotAvailableException e)
            {
                Methods.DisplayReportResultTrack(e);
                Toast.MakeText(Context, "Google Play Services is not available", ToastLength.Short)?.Show();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                Toast.MakeText(Context, "Google Play Services e", ToastLength.Short)?.Show();
            }
        }

        private File CreateImageFile()
        {
            // Create an image file name
            string timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").Format(new Date());
            string imageFileName = "Img_" + timeStamp + "_";
            string storageDir = Methods.Path.FolderDcimImage;

            try
            {
                File image = File.CreateTempFile(imageFileName, ".jpg", new File(storageDir));

                // Save a file: path for use with ACTION_VIEW intents
                CurrentPhotoPath = image.AbsolutePath;
                return image;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                File image = new File(storageDir + "/" + imageFileName, ".jpg");
                // Save a file: path for use with ACTION_VIEW intents
                CurrentPhotoPath = image.AbsolutePath;
                return image;
            }
        }

        /// <summary>
        /// Open intent Camera when the request code of result is 503
        /// </summary>
        public void OpenIntentCamera()
        {
            try
            {
                Methods.Path.Chack_MyFolder();

                if (Methods.MultiMedia.IsCameraAvailable())
                {
                    Intent takePictureIntent = new Intent(MediaStore.ActionImageCapture);
                    // Ensure that there's a camera activity to handle the intent
                    var packageManager = takePictureIntent.ResolveActivity(Context.PackageManager);
                    if (packageManager != null)
                    {
                        // Create the File where the photo should go
                        File photoFile = CreateImageFile();

                        // Continue only if the File was successfully created
                        if (photoFile != null)
                        {
                            var photoUri = FileProvider.GetUriForFile(Context, Context.PackageName + ".fileprovider", photoFile);
                            takePictureIntent.PutExtra(MediaStore.ExtraOutput, photoUri);
                        }
                    }

                    Context.StartActivityForResult(takePictureIntent, 503);
                }
                else
                {
                    Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_Camera_Not_Available), ToastLength.Short)?.Show();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private File CreateVideoFile()
        {
            // Create an image file name
            string timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").Format(new Date());
            string videoFileName = "Video_" + timeStamp + "_";
            string storageDir = Methods.Path.FolderDcimMyApp + "/Video";

            try
            {
                File video = File.CreateTempFile(videoFileName, ".mp4", new File(storageDir));

                // Save a file: path for use with ACTION_VIEW intents
                CurrentVideoPath = video.AbsolutePath;
                return video;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                File video = new File(storageDir + "/" + videoFileName, ".mp4");
                // Save a file: path for use with ACTION_VIEW intents
                CurrentVideoPath = video.AbsolutePath;
                return video;
            }
        }

        /// <summary>
        /// Open intent Video Camera when the request code of result is 513
        /// </summary>
        public void OpenIntentVideoCamera()
        {
            try
            {
                Methods.Path.Chack_MyFolder();

                if (Methods.MultiMedia.IsCameraAvailable())
                {
                    Intent intent = new Intent(MediaStore.ActionVideoCapture);

                    var packageManager = intent.ResolveActivity(Context.PackageManager);
                    if (packageManager != null)
                    {
                        // Create the File where the Video should go
                        File videoFile = CreateVideoFile();

                        // Continue only if the File was successfully created
                        if (videoFile != null)
                        {
                            var videoFileUri = FileProvider.GetUriForFile(Context, Context.PackageName + ".fileprovider", videoFile);
                            intent.PutExtra(MediaStore.ExtraOutput, videoFileUri);
                        }
                    }

                    Context.StartActivityForResult(intent, 513);
                }
                else
                {
                    Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_Camera_Not_Available), ToastLength.Short)?.Show();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        /// <summary>
        /// Open intent File when the request code of result is 504
        /// </summary>
        /// <param name="title"></param>
        /// <summary>
        /// Open intent File when the request code of result is 504
        /// </summary>
        public void OpenIntentFile(string title)
        {
            try
            {
                Methods.Path.Chack_MyFolder();

                Intent intent;
                switch (Build.Manufacturer!.ToLower())
                {
                    case "samsung":
                        intent = new Intent("com.sec.android.app.myfiles.PICK_DATA");
                        intent.PutExtra("CONTENT_TYPE", "*/*");
                        intent.AddCategory(Intent.CategoryDefault);
                        break;
                    default:
                        {
                            string[] mimeTypes =
                            {"application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", // .doc & .docx
                            "application/vnd.ms-powerpoint", "application/vnd.openxmlformats-officedocument.presentationml.presentation", // .ppt & .pptx
                            "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", // .xls & .xlsx
                            "text/plain",
                            "application/pdf",
                            "application/zip", "application/vnd.android.package-archive"};

                            intent = new Intent(Intent.ActionOpenDocument); // or ACTION_OPEN_DOCUMENT ActionGetContent
                            intent.SetType("*/*");
                            //intent.PutExtra(Intent.ExtraMimeTypes, mimeTypes);
                            intent.AddCategory(Intent.CategoryOpenable);

                            if ((int)Build.VERSION.SdkInt >= 26)
                            {
                                // intent.SetAction(Intent.ActionCreateDocument);
                                intent.SetFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission | ActivityFlags.GrantPersistableUriPermission);
                            }

                            break;
                        }
                }

                Context.StartActivityForResult(Intent.CreateChooser(intent, title), 504);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);

                var fileIntent = new Intent(Intent.ActionPick);
                fileIntent.SetAction(Intent.ActionGetContent);
                fileIntent.SetType("*/*");
                Context.StartActivityForResult(Intent.CreateChooser(fileIntent, title), 504);
            }
        }

        /// <summary>
        /// Open intent Audio when the request code of result is 505
        /// </summary>
        public void OpenIntentAudio()
        {
            try
            {
                Methods.Path.Chack_MyFolder();

                //Intent intent = (int)Build.VERSION.SdkInt switch
                //{
                //    <= 25 => new Intent(Intent.ActionPick, MediaStore.Audio.Media.ExternalContentUri),
                //    _ => Environment.GetExternalStorageState(null)!.Equals(Environment
                //        .MediaMounted)
                //        ? new Intent(Intent.ActionPick, MediaStore.Audio.Media.ExternalContentUri)
                //        : new Intent(Intent.ActionPick, MediaStore.Audio.Media.InternalContentUri)
                //};
                //intent.SetType("audio/*");

                Intent intent = new Intent(Intent.ActionOpenDocument); // or ACTION_OPEN_DOCUMENT ActionGetContent
                intent.SetType("audio/*");
                //intent.PutExtra(Intent.ExtraMimeTypes, mimeTypes);
                intent.AddCategory(Intent.CategoryOpenable);

                if ((int)Build.VERSION.SdkInt >= 26)
                {
                    // intent.SetAction(Intent.ActionCreateDocument);
                    intent.SetFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission | ActivityFlags.GrantPersistableUriPermission);
                }

                if (intent.ResolveActivity(Context.PackageManager) != null)
                    Context.StartActivityForResult(intent, 505);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        /// <summary>
        /// Open intent Get Contact Number Phone when the request code of result is 506
        /// </summary>
        public void OpenIntentGetContactNumberPhone()
        {
            try
            {
                Intent pickcontact = new Intent(Intent.ActionPick, ContactsContract.Contacts.ContentUri);
                pickcontact.SetType(ContactsContract.CommonDataKinds.Phone.ContentType);
                Context.StartActivityForResult(pickcontact, 506);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        /// <summary>
        /// Open intent Gps when the request code of result is 1050
        /// </summary>
        /// <param name="locationManager"></param>
        public void OpenIntentGps(LocationManager locationManager)
        {
            try
            {
                if (!locationManager.IsProviderEnabled(LocationManager.GpsProvider) && !locationManager.IsProviderEnabled(LocationManager.NetworkProvider))
                {
                    Context.StartActivity(new Intent(Settings.ActionLocationSourceSettings));
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        /// <summary>
        /// Open intent Send Sms
        /// </summary>
        /// <param name="phoneNumber">any number </param>
        /// <param name="textMessages">Example : Hello Xamarin This is My Test SMS</param>
        /// <param name="openIntent">true or false >> If it is false the message will be sent in a hidden manner .. don't open intent </param>
        public void OpenIntentSendSms(string phoneNumber, string textMessages, bool openIntent = true)
        {
            try
            {
                var smsUri = Uri.Parse("smsto:" + phoneNumber);
                var intent = new Intent(Intent.ActionSendto, smsUri);
                intent.PutExtra("sms_body", textMessages);
                intent.AddFlags(ActivityFlags.NewTask);
                Context.StartActivity(intent);

                //Or use this code
                //SmsManager.Default?.SendTextMessage(phoneNumber, null, textMessages, null, null);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        /// <summary>
        /// Open intent Save Contact Number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="detailedInformation">true or false </param>
        public void OpenIntentSaveContacts(string phoneNumber, string name, string email, bool detailedInformation = false)
        {
            try
            {
                switch (detailedInformation)
                {
                    case true:
                        {
                            Intent intent = new Intent(ContactsContract.Intents.Insert.Action);
                            intent.SetType(ContactsContract.RawContacts.ContentType);
                            intent.PutExtra(ContactsContract.Intents.Insert.Phone, phoneNumber);
                            intent.PutExtra(ContactsContract.Intents.Insert.Name, name);
                            intent.PutExtra(ContactsContract.Intents.Insert.Email, email);
                            Context.StartActivity(intent);
                            break;
                        }
                    default:
                        {
                            var contactUri = Uri.Parse("tel:" + phoneNumber);
                            Intent intent = new Intent(ContactsContract.Intents.ShowOrCreateContact, contactUri);
                            intent.PutExtra(ContactsContract.Intents.ExtraRecipientContactName, true);
                            Context.StartActivity(intent);
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="text"></param>
        public void OpenIntentSendEmail(string email, string subject = "", string text = "")
        {
            try
            {
                string mailto = "mailto:" + email + "?cc=" + email + "&subject=" + subject + "&body=" + text;
                Intent emailIntent = new Intent(Intent.ActionSendto);
                emailIntent.SetData(Uri.Parse(mailto));
                Context.StartActivity(Intent.CreateChooser(emailIntent, "Send Email"));
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="phoneNumber"></param>
        public void OpenIntentSendPhoneCall(string phoneNumber)
        {
            try
            {
                var urlNumber = Uri.Parse("tel:" + phoneNumber);
                var intent = new Intent(Intent.ActionCall);
                intent.SetData(urlNumber);
                intent.AddFlags(ActivityFlags.NewTask);

                Context.StartActivity(intent);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        /// <summary>
        /// Open intent Browser From Phone using Url
        /// </summary>
        /// <param name="website"></param>
        public void OpenBrowserFromPhone(string website)
        {
            try
            {
                var uri = Uri.Parse(website);
                var intent = new Intent(Intent.ActionView, uri);
                intent.AddFlags(ActivityFlags.NewTask);
                Context.StartActivity(intent);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        /// <summary>
        /// Open intent Browser From App using Url
        /// </summary>
        /// <param name="url"></param>
        public void OpenBrowserFromApp(string url)
        {
            try
            {
                CustomTabsIntent.Builder builder = new CustomTabsIntent.Builder();
                CustomTabsIntent customTabsIntent = builder.Build();
                customTabsIntent.Intent.SetAction(Intent.ActionView);
                customTabsIntent.Intent.AddFlags(ActivityFlags.NewTask);
                //builder.SetToolbarColor(Color.ParseColor(AppSettings.MainColor));
                builder.SetStartAnimations(Context, Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);
                builder.SetExitAnimations(Context, Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);
                customTabsIntent.LaunchUrl(Context, Uri.Parse(url));
            }
            catch (Exception e)
            {
                OpenBrowserFromPhone(url);
                Methods.DisplayReportResultTrack(e);
            }
        }

        /// <summary>
        /// Open app PackageName by Google play
        /// </summary>
        /// <param name="appPackageName">from Context or Activity object</param>
        public void OpenAppOnGooglePlay(string appPackageName)
        {
            try
            {
                Intent intent;
                try
                {
                    intent = new Intent(Intent.ActionView, Uri.Parse("market://details?id=" + appPackageName));
                    if (intent.ResolveActivity(Context.PackageManager) != null)
                        Context.StartActivity(intent);
                    else
                    {
                        intent = new Intent(Intent.ActionView, Uri.Parse("https://play.google.com/store/apps/details?id=" + appPackageName));
                        Context.StartActivity(intent);
                    }
                }
                catch (ActivityNotFoundException exception)
                {
                    intent = new Intent(Intent.ActionView, Uri.Parse("https://play.google.com/store/apps/details?id=" + appPackageName));
                    Context.StartActivity(intent);
                    Methods.DisplayReportResultTrack(exception);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OpenFacebookIntent(string name)
        {
            try
            {
                Intent intent = new Intent(Intent.ActionView, Uri.Parse("fb://facewebmodal/f?href=https://www.facebook.com/" + name));
                if (intent.ResolveActivity(Context.PackageManager) != null)
                    Context.StartActivity(intent);
                else
                {
                    OpenBrowserFromApp("https://www.facebook.com/" + name);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OpenTwitterIntent(string name)
        {
            try
            {
                Intent intent = new Intent(Intent.ActionView, Uri.Parse("twitter://user?screen_name=" + name));
                if (intent.ResolveActivity(Context.PackageManager) != null)
                    Context.StartActivity(intent);
                else
                {
                    OpenBrowserFromApp("https://twitter.com/#!/" + name);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                OpenBrowserFromApp("https://twitter.com/#!/" + name);
                //Context.StartActivity(new Intent(Intent.ActionView, Uri.Parse("https://twitter.com/#!/" + name)));
            }
        }

        public void OpenLinkedInIntent(string name)
        {
            try
            {
                string url = "https://www.linkedin.com/in/" + name;
                Intent linkedInAppIntent = new Intent(Intent.ActionView, Uri.Parse(url));
                linkedInAppIntent.AddFlags(ActivityFlags.ClearWhenTaskReset);
                Context.StartActivity(linkedInAppIntent);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OpenInstagramIntent(string name)
        {
            try
            {
                Intent likeIng = new Intent(Intent.ActionView, Uri.Parse("http://instagram.com/_u/" + name));
                likeIng.SetPackage("com.instagram.android");

                try
                {
                    if (likeIng.ResolveActivity(Context.PackageManager) != null)
                        Context.StartActivity(likeIng);
                    else
                    {
                        var intent = new Intent(Intent.ActionView, Uri.Parse("http://instagram.com/" + name));
                        Context.StartActivity(intent);
                    }
                }
                catch (ActivityNotFoundException e)
                {
                    Methods.DisplayReportResultTrack(e);
                    var intent = new Intent(Intent.ActionView, Uri.Parse("http://instagram.com/" + name));
                    Context.StartActivity(intent);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OpenYoutubeIntent(string channelId)
        {
            try
            {
                Intent likeIng = new Intent(Intent.ActionView, Uri.Parse("vnd.youtube://user/channel/" + channelId));

                try
                {
                    if (likeIng.ResolveActivity(Context.PackageManager) != null)
                        Context.StartActivity(likeIng);
                    else
                    {
                        var intent = new Intent(Intent.ActionView, Uri.Parse("http://www.youtube.com/" + channelId));
                        Context.StartActivity(intent);
                    }
                }
                catch (ActivityNotFoundException e)
                {
                    Methods.DisplayReportResultTrack(e);
                    var intent = new Intent(Intent.ActionView, Uri.Parse("http://www.youtube.com/" + channelId));
                    Context.StartActivity(intent);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OpenVkontakteIntent(string friendId)
        {
            try
            {
                Intent likeIng = new Intent(Intent.ActionView, Uri.Parse("vkontakte://profile/%d" + friendId));

                try
                {
                    if (likeIng.ResolveActivity(Context.PackageManager) != null)
                        Context.StartActivity(likeIng);
                    else
                    {
                        var intent = new Intent(Intent.ActionView, Uri.Parse("http://vk.com/" + friendId));
                        Context.StartActivity(intent);
                    }
                }
                catch (ActivityNotFoundException e)
                {
                    Methods.DisplayReportResultTrack(e);
                    var intent = new Intent(Intent.ActionView, Uri.Parse("http://vk.com/" + friendId));
                    Context.StartActivity(intent);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OpenTelegramIntent(string friendId)
        {
            try
            {
                Intent likeIng = new Intent(Intent.ActionView, Uri.Parse("tg://resolve?domain=" + friendId));

                try
                {
                    if (likeIng.ResolveActivity(Context.PackageManager) != null)
                        Context.StartActivity(likeIng);
                    else
                    {
                        var intent = new Intent(Intent.ActionView, Uri.Parse("https://telegram.me/" + friendId));
                        Context.StartActivity(intent);
                    }
                }
                catch (ActivityNotFoundException e)
                {
                    Methods.DisplayReportResultTrack(e);
                    var intent = new Intent(Intent.ActionView, Uri.Parse("https://telegram.me/" + friendId));
                    Context.StartActivity(intent);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OpenConversationMailListIntent()
        {
            try
            {
                var emailIntent = new Intent(Intent.ActionView, Uri.Parse("mailto:"));
                IList<ResolveInfo> resInfo = Context.PackageManager.QueryIntentActivities(emailIntent, 0);
                if (resInfo.Count > 0)
                {
                    // First create an intent with only the package name of the first registered email app
                    // and build a picker based on it
                    var intentChooser = Context.PackageManager.GetLaunchIntentForPackage(resInfo.FirstOrDefault()?.ActivityInfo?.PackageName ?? "");
                    var openInChooser = Intent.CreateChooser(intentChooser, "Open E-mail");
                    // Then create a list of LabeledIntent for the rest of the registered email apps
                    var packageManager = Context.PackageManager;

                    ArrayList array = new ArrayList();
                    foreach (var info in resInfo)
                    {
                        var packageName = info.ActivityInfo.PackageName;
                        var intent = packageManager.GetLaunchIntentForPackage(packageName);
                        array.Add(new LabeledIntent(intent, packageName, info.LoadLabel(packageManager), info.Icon));
                    }

                    // Add the rest of the email apps to the picker selection
                    openInChooser.PutExtra(Intent.ExtraInitialIntents, array);
                    openInChooser.AddFlags(ActivityFlags.NewTask);
                    Context.StartActivity(openInChooser);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }
}