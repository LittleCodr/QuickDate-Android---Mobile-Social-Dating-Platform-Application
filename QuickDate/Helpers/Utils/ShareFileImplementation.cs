using Android.App;
using Android.Content;
using AndroidHUD;
using AndroidX.Core.Content;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Console = System.Console;
using File = System.IO.File;
using Path = System.IO.Path;
using Uri = Android.Net.Uri;

namespace QuickDate.Helpers.Utils
{
    public static class ShareFileImplementation
    {
        /// <summary>
        /// Simply share a local file on compatible services
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="localFilePath">path to local file</param>
        /// <param name="textImage"></param>
        /// <param name="title">Title of popup on share (not included in message)</param>
        /// <param name="activity"></param>
        /// <returns>awaitable Task</returns>
        public static void ShareLocalFile(Activity activity, string postUrl, Uri localFilePath, string textImage, string title)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(localFilePath.Path))
                {
                    Console.WriteLine("ShareFile: ShareLocalFile Warning: localFilePath null or empty");
                    return;
                }

                var intent = new Intent();
                intent.SetFlags(ActivityFlags.ClearTop);
                intent.SetFlags(ActivityFlags.NewTask);
                intent.SetAction(Intent.ActionSend);
                intent.SetType("*/*");
                intent.PutExtra(Intent.ExtraStream, localFilePath);
                intent.PutExtra(Intent.ExtraText, postUrl);
                intent.AddFlags(ActivityFlags.GrantReadUriPermission);

                var chooserIntent = Intent.CreateChooser(intent, title);
                chooserIntent?.SetFlags(ActivityFlags.ClearTop);
                chooserIntent?.SetFlags(ActivityFlags.NewTask);
                activity.StartActivity(chooserIntent);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in ShareFile: ShareLocalFile Exception: {0}", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="text"></param>
        /// <param name="title"></param>
        public static void ShareText(Activity activity, string text, string title = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    Console.WriteLine("ShareFile: ShareLocalFile Warning: localFilePath null or empty");
                    return;
                }

                var intent = new Intent();
                intent.SetFlags(ActivityFlags.ClearTop);
                intent.SetFlags(ActivityFlags.NewTask);
                intent.SetAction(Intent.ActionSend);
                intent.SetType("text/plain");
                intent.PutExtra(Intent.ExtraText, text);

                var chooserIntent = Intent.CreateChooser(intent, title);
                chooserIntent?.SetFlags(ActivityFlags.ClearTop);
                chooserIntent?.SetFlags(ActivityFlags.NewTask);
                activity.StartActivity(chooserIntent);
            }
            catch (Exception ex)
            {
                switch (string.IsNullOrWhiteSpace(ex.Message))
                {
                    case false:
                        Console.WriteLine("Exception in ShareFile: ShareLocalFile Exception: {0}", ex);
                        break;
                }
            }
        }

        /// <summary>
        /// Simply share a file from a remote resource on compatible services
        /// </summary>
        /// <param name="fileUri">uri to external file</param>
        /// <param name="fileName">name of the file</param>
        /// <param name="title">Title of popup on share (not included in message)</param>
        /// <returns>awaitable bool</returns>
        public static async Task ShareRemoteFile(Activity Activity, string postUrl, string fileUri, string fileName, string title)
        {
            try
            {
                Download(Activity, postUrl, fileUri, fileName, title);
                await Task.Delay(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in ShareFile: ShareRemoteFile Exception: {0}", ex.Message);
            }
        }

        public static void Download(Activity Activity, string postUrl, string imageUrl, string fileName, string title)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl) || Activity == null)
                    return;

                Uri photoUri;

                Activity?.RunOnUiThread(async () =>
                {
                    try
                    {
                        var getImage = Methods.MultiMedia.GetMediaFrom_Gallery(Methods.Path.FolderDcimImage, fileName);
                        if (getImage != "File Dont Exists")
                        {
                            Java.IO.File file2 = new Java.IO.File(getImage);
                            photoUri = FileProvider.GetUriForFile(Activity, Activity.PackageName + ".fileprovider", file2);
                            ShareLocalFile(Activity, postUrl, photoUri, imageUrl, title);
                        }
                        else
                        {
                            string filePath = Path.Combine(Methods.Path.FolderDcimImage);
                            string mediaFile = filePath + "/" + fileName;

                            if (!Directory.Exists(filePath))
                                Directory.CreateDirectory(filePath);

                            if (!File.Exists(mediaFile))
                            {
                                AndHUD.Shared.Show(Activity, Activity.GetText(Resource.String.Lbl_Loading));

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
                                var s = await client.GetStreamAsync(new System.Uri(imageUrl));
                                if (s.CanRead)
                                {
                                    await using FileStream fs = new FileStream(mediaFile, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
                                    await s.CopyToAsync(fs);

                                    var getImagePath = Methods.MultiMedia.GetMediaFrom_Gallery(Methods.Path.FolderDcimImage, fileName);
                                    if (getImagePath != "File Dont Exists")
                                    {
                                        Java.IO.File file2 = new Java.IO.File(getImagePath);

                                        photoUri = FileProvider.GetUriForFile(Activity, Activity.PackageName + ".fileprovider", file2);
                                        ShareLocalFile(Activity, postUrl, photoUri, imageUrl, title);
                                    }
                                }

                                AndHUD.Shared.Dismiss();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Methods.DisplayReportResultTrack(e);
                    }
                });

            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }


        //public static void SaveMedia(Context context, int mediaType, string filePath, string directoryName)
        //{
        //    Java.IO.File originalFile = new Java.IO.File(filePath);
        //    if (!originalFile.Exists())
        //    {
        //        ////Log.e("Unity", "Original media file is missing or inaccessible!");
        //        return;
        //    }

        //    int pathSeparator = filePath.LastIndexOf('/');
        //    int extensionSeparator = filePath.LastIndexOf('.');
        //    string filename = pathSeparator >= 0 ? filePath.Substring(pathSeparator + 1) : filePath;
        //    string extension = extensionSeparator >= 0 ? filePath.Substring(extensionSeparator + 1) : "";

        //    // Credit: https://stackoverflow.com/a/31691791/2373034
        //    string mimeType = extension.Length > 0 ? MimeTypeMap.GetMimeType(extension.ToLower()) : null;

        //    ContentValues values = new ContentValues();
        //    values.Put(MediaStore.MediaColumns.Title, filename);
        //    values.Put(MediaStore.MediaColumns.DisplayName, filename);
        //    values.Put(MediaStore.MediaColumns.DateAdded, DateTime.Now.ToLongDateString());

        //    if (!string.IsNullOrEmpty(mimeType))
        //        values.Put(MediaStore.MediaColumns.MimeType, mimeType);

        //    Uri externalContentUri;
        //    if (mediaType == 0)
        //        externalContentUri = MediaStore.Images.Media.ExternalContentUri;
        //    else if (mediaType == 1)
        //        externalContentUri = MediaStore.Video.Media.ExternalContentUri;
        //    else
        //        externalContentUri = MediaStore.Audio.Media.ExternalContentUri;

        //    // Android 10 restricts our access to the raw filesystem, use MediaStore to save media in that case
        //    if (Build.VERSION.SdkInt >= (BuildVersionCodes)29)
        //    {
        //        values.Put(MediaStore.MediaColumns.RelativePath, "DCIM/" + directoryName);
        //        values.Put(MediaStore.MediaColumns.DateTaken, DateTime.Now.ToLongDateString());
        //        values.Put(MediaStore.MediaColumns.IsPending, true);

        //        Uri uri = context.ContentResolver.Insert(externalContentUri, values);
        //        if (uri != null)
        //        {
        //            try
        //            {

        //                if (WriteFileToStream(originalFile, context.ContentResolver.OpenOutputStream(uri)))
        //                {
        //                    values.Put(MediaStore.MediaColumns.IsPending, false);
        //                    context.ContentResolver.Update(uri, values, null, null);
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                //Log.e("Unity", "Exception:", e);
        //                context.ContentResolver.Delete(uri, null, null);
        //            }
        //        }
        //    }
        //}
    }
}