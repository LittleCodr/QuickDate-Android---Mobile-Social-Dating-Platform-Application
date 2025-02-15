using Android;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using QuickDate.Activities.Live.Page;
using QuickDate.Activities.SettingsUser;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using System;

namespace QuickDate.Activities.Live.Utils
{
    public class LiveUtil
    {
        private readonly AppCompatActivity Activity;
        public LiveUtil(AppCompatActivity activity)
        {
            try
            {
                Activity = activity;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #region Live

        //Go Live
        public void GoLiveOnClick()
        {
            try
            {
                switch ((int)Build.VERSION.SdkInt)
                {
                    // Check if we're running on Android 5.0 or higher
                    case < 23:
                        OpenDialogLive();
                        break;
                    default:
                        {
                            if (PermissionsController.CheckPermissionStorage(Activity) &&
                                ContextCompat.CheckSelfPermission(Activity, Manifest.Permission.Camera) == Permission.Granted &&
                                ContextCompat.CheckSelfPermission(Activity, Manifest.Permission.RecordAudio) == Permission.Granted &&
                                ContextCompat.CheckSelfPermission(Activity, Manifest.Permission.ModifyAudioSettings) == Permission.Granted)
                            {
                                OpenDialogLive();
                            }
                            else
                            {
                                new PermissionsController(Activity).RequestPermission(111);
                            }

                            break;
                        }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void OpenDialogLive()
        {
            try
            {
                var ugcPrivacyPerf = MainSettings.UgcPrivacy?.GetBoolean("UgcPrivacy_key", false) ?? false;
                if (ugcPrivacyPerf)
                {
                    OpenLive();
                }
                else
                {
                    new UgcPrivacyDialog(Activity).DisplayPrivacyDialog();
                }

                //var dialog = new MaterialAlertDialogBuilder(this).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);
                //dialog.SetTitle(GetText(Resource.String.Lbl_CreateLiveVideo));
                //dialog.Input(Resource.String.Lbl_AddLiveVideoContext, 0, false, (materialDialog, s) =>
                //{
                //    try
                //    {

                //    }
                //    catch (Exception e)
                //    {
                //        Methods.DisplayReportResultTrack(e);
                //    }
                //});
                //dialog.InputType(InputTypes.TextFlagImeMultiLine);
                //dialog.SetPositiveButton(GetText(Resource.String.Lbl_Go_Live), new AppTools.MyMaterialDialog());
                //dialog.SetNegativeButton(GetText(Resource.String.Lbl_Cancel), new AppTools.MyMaterialDialog());
                //dialog;
                //dialog.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void OpenLive()
        {
            try
            {
                var streamName = "stream_" + UserDetails.UserId + "_" + Methods.Time.CurrentTimeMillis();
                if (string.IsNullOrEmpty(streamName) || string.IsNullOrWhiteSpace(streamName))
                {
                    Toast.MakeText(Activity, Activity.GetText(Resource.String.Lbl_PleaseEnterLiveStreamName), ToastLength.Short)?.Show();
                    return;
                }
                //Owner >> ClientRoleBroadcaster , Users >> ClientRoleAudience
                Intent intent = new Intent(Activity, typeof(LiveStreamingActivity));
                intent.PutExtra(LiveConstants.KeyClientRole, IO.Agora.Rtc2.Constants.ClientRoleBroadcaster);
                intent.PutExtra("StreamName", streamName);
                Activity.StartActivity(intent);

                //var dialog = new MaterialAlertDialogBuilder(this).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);
                //dialog.SetTitle(GetText(Resource.String.Lbl_CreateLiveVideo));
                //dialog.Input(Resource.String.Lbl_AddLiveVideoContext, 0, false, (materialDialog, s) =>
                //{
                //    try
                //    {

                //    }
                //    catch (Exception e)
                //    {
                //        Methods.DisplayReportResultTrack(e);
                //    }
                //});
                //dialog.InputType(InputTypes.TextFlagImeMultiLine);
                //dialog.SetPositiveButton(GetText(Resource.String.Lbl_Go_Live), new AppTools.MyMaterialDialog());
                //dialog.SetNegativeButton(GetText(Resource.String.Lbl_Cancel), new AppTools.MyMaterialDialog());
                //dialog;
                //dialog.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion
    }
}