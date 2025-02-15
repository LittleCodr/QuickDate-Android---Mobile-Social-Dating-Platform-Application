using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.Dialog;
using Newtonsoft.Json;
using QuickDate.Activities.Call.Tools;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Call;
using QuickDateClient.Classes.Chat;
using QuickDateClient.Requests;
using System;
using System.Collections.Generic;
using Exception = System.Exception;

namespace QuickDate.Activities.Call
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", Exported = true, LaunchMode = LaunchMode.SingleTask, AutoRemoveFromRecents = true, ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.UiMode)]
    public class VideoAudioComingCallActivity : AppCompatActivity, IDialogListCallBack, IDialogInputCallBack//, ValueAnimator.IAnimatorUpdateListener
    {
        private string CallType = "0";

        private DataCallObject CallUserObject;

        private ImageView UserImageView;
        private TextView UserNameTextView, TypeCallTextView;
        public static VideoAudioComingCallActivity CallActivity;

        private FrameLayout MainCallView;
        private LinearLayout DeclineButtonLayout, AnswerCallButtonLayout, MessageButtonLayout;

        private string PermissionsType;
        private HomeActivity GlobalContext;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                SetTheme(QuickDateTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);
                SetContentView(Resource.Layout.InComingCallLayout);
                Window.AddFlags(WindowManagerFlags.KeepScreenOn);

                CallActivity = this;
                GlobalContext = HomeActivity.GetInstance();

                CallType = Intent?.GetStringExtra("type") ?? "";

                if (!string.IsNullOrEmpty(Intent?.GetStringExtra("callUserObject")))
                    CallUserObject = JsonConvert.DeserializeObject<DataCallObject>(Intent?.GetStringExtra("callUserObject") ?? "");

                MainCallView = FindViewById<FrameLayout>(Resource.Id.activity_call_chat_view);

                UserNameTextView = FindViewById<TextView>(Resource.Id.UsernameTextView);
                TypeCallTextView = FindViewById<TextView>(Resource.Id.TypecallTextView);
                UserImageView = FindViewById<ImageView>(Resource.Id.UserImageView);

                DeclineButtonLayout = FindViewById<LinearLayout>(Resource.Id.DeclineButtonLayout);
                AnswerCallButtonLayout = FindViewById<LinearLayout>(Resource.Id.AnswerCallButtonLayout);
                MessageButtonLayout = FindViewById<LinearLayout>(Resource.Id.MessageButtonLayout);

                DeclineButtonLayout.Click += DeclineButtonLayoutOnClick;
                AnswerCallButtonLayout.Click += AnswerCallButtonLayoutOnClick;
                MessageButtonLayout.Click += MessageCallButton_Click;

                if (CallUserObject != null)
                {
                    if (!string.IsNullOrEmpty(CallUserObject.Fullname)) UserNameTextView.Text = CallUserObject.Fullname;

                    if (!string.IsNullOrEmpty(CallUserObject.Avater))
                        GlideImageLoader.LoadImage(this, CallUserObject.Avater, UserImageView, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                }

                if (CallType == "Twilio_video_call" || CallType == "Agora_video_call_recieve")
                    TypeCallTextView.Text = GetText(Resource.String.Lbl_Video_call);
                else
                    TypeCallTextView.Text = GetText(Resource.String.Lbl_Voice_call);
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
                //StartAnimation();
                base.OnResume();
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
                //StopAnimation();

                base.OnPause();
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
                //StopAnimation();
                GlobalContext?.OffWakeLock();
                base.OnDestroy();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void MessageCallButton_Click(object sender, EventArgs e)
        {
            try
            {

                if (Methods.CheckConnectivity())
                {
                    var arrayAdapter = new List<string>();
                    var dialogList = new MaterialAlertDialogBuilder(this);

                    arrayAdapter.Add(GetString(Resource.String.Lbl_MessageCall1));
                    arrayAdapter.Add(GetString(Resource.String.Lbl_MessageCall2));
                    arrayAdapter.Add(GetString(Resource.String.Lbl_MessageCall3));
                    arrayAdapter.Add(GetString(Resource.String.Lbl_MessageCall4));
                    arrayAdapter.Add(GetString(Resource.String.Lbl_MessageCall5));

                    dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                    dialogList.SetPositiveButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                    dialogList.Show();
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void AnswerCallButtonLayoutOnClick(object sender, EventArgs e)
        {
            OnAnswered();
        }

        private void DeclineButtonLayoutOnClick(object sender, EventArgs e)
        {
            OnDeclined();
        }

        #region MaterialDialog

        public void OnSelection(IDialogInterface dialog, int position, string itemString)
        {
            try
            {
                string text = itemString;

                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
                else
                {
                    if (text == GetString(Resource.String.Lbl_MessageCall5))
                    {
                        var dialogBuilder = new MaterialAlertDialogBuilder(this);


                        EditText input = new EditText(this);
                        input.SetHint(Resource.String.Lbl_Write_your_message);
                        input.InputType = InputTypes.TextFlagImeMultiLine;
                        LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                        input.LayoutParameters = lp;

                        dialogBuilder.SetView(input);

                        dialogBuilder.SetPositiveButton(GetText(Resource.String.Lbl_Send), new MaterialDialogUtils(input, this));
                        dialogBuilder.SetNegativeButton(GetText(Resource.String.Lbl_Cancel), new MaterialDialogUtils());
                        dialogBuilder.Show();

                    }
                    else
                    {
                        SendMess(text);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnInput(IDialogInterface dialog, string input)
        {
            try
            {
                if (input.Length > 0)
                {
                    SendMess(input);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        private async void SendMess(string text)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
                else
                {
                    var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    var time2 = unixTimestamp.ToString();

                    //Here on This function will send Selected audio file to the user 
                    var (apiStatus, respond) = await RequestsAsync.Chat.SendMessageAsync(CallUserObject.ToId, text, "", "", time2);
                    if (apiStatus == 200)
                    {
                        if (respond is SendMessageObject result)
                        {
                            Console.WriteLine(result.Message);

                            Intent intentCall = new Intent(this, typeof(CallingService));
                            intentCall.PutExtra("callUserObject", JsonConvert.SerializeObject(CallUserObject));
                            intentCall.PutExtra("type", CallType);

                            intentCall.SetAction(CallingService.ActionDecline);
                            StartService(intentCall);

                            FinishVideoAudio();
                        }
                    }
                    else Methods.DisplayReportResult(this, respond);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void FinishVideoAudio()
        {
            try
            {
                Finish();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnAnswered()
        {
            try
            {
                switch (CallType)
                {
                    case "Agora_video_call_recieve":
                    case "Twilio_video_call":
                        {
                            PermissionsType = "VideoCall";
                            if ((int)Build.VERSION.SdkInt >= 23)
                            {
                                if (CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted && CheckSelfPermission(Manifest.Permission.RecordAudio) == Permission.Granted && CheckSelfPermission(Manifest.Permission.ModifyAudioSettings) == Permission.Granted)
                                {
                                    StartCall(TypeCall.Video);
                                }
                                else
                                {
                                    new PermissionsController(this).RequestPermission(103);
                                }
                            }
                            else
                            {
                                StartCall(TypeCall.Video);
                            }
                            break;
                        }
                    case "Agora_audio_call_recieve":
                    case "Twilio_audio_call":
                        {
                            PermissionsType = "AudioCall";
                            if ((int)Build.VERSION.SdkInt >= 23)
                            {
                                if (CheckSelfPermission(Manifest.Permission.RecordAudio) == Permission.Granted && CheckSelfPermission(Manifest.Permission.ModifyAudioSettings) == Permission.Granted)
                                {
                                    StartCall(TypeCall.Audio);
                                }
                                else
                                {
                                    new PermissionsController(this).RequestPermission(102);
                                }
                            }
                            else
                            {
                                StartCall(TypeCall.Audio);
                            }
                            break;
                        }
                }

                FinishVideoAudio();
            }
            catch (Exception exception)
            {
                FinishVideoAudio();
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void OnDeclined()
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    return;
                }

                Intent intentCall = new Intent(this, typeof(CallingService));
                intentCall.PutExtra("callUserObject", JsonConvert.SerializeObject(CallUserObject));
                intentCall.PutExtra("type", CallType);

                intentCall.SetAction(CallingService.ActionDecline);
                StartService(intentCall);

                FinishVideoAudio();
            }
            catch (Exception exception)
            {
                CallConstant.CallActive = false;
                FinishVideoAudio();
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #region Permissions

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (grantResults.Length <= 0 || grantResults[0] != Permission.Granted)
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long)?.Show();
                    return;
                }

                if (requestCode == 102)
                {
                    if (PermissionsType == "AudioCall")
                    {
                        StartCall(TypeCall.Audio);
                    }
                }
                else if (requestCode == 103)
                {
                    if (PermissionsType == "VideoCall")
                    {
                        StartCall(TypeCall.Video);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Call

        private void StartCall(TypeCall type)
        {
            try
            {
                var dataSettings = ListUtils.SettingsSiteList;
                Intent intentCall = new Intent(this, typeof(CallingService));
                if (type == TypeCall.Audio)
                {
                    if (dataSettings?.AgoraChatCall == "1")
                    {
                        intentCall.PutExtra("type", "Agora_audio_call_recieve");
                    }
                    else if (dataSettings?.TwilioChatCall == "1")
                    {
                        intentCall.PutExtra("type", "Twilio_audio_call");
                    }
                }
                else if (type == TypeCall.Video)
                {
                    if (dataSettings?.AgoraChatCall == "1")
                    {
                        intentCall.PutExtra("type", "Agora_video_call_recieve");
                    }
                    else if (dataSettings?.TwilioChatCall == "1")
                    {
                        intentCall.PutExtra("type", "Twilio_video_call");
                    }
                }

                if (CallUserObject != null)
                {
                    intentCall?.PutExtra("callUserObject", JsonConvert.SerializeObject(CallUserObject));
                }

                intentCall.SetAction(CallingService.ActionAnswer);
                StartService(intentCall);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        //private void StartAnimation()
        //{ 
        //    try
        //    {
        //        ValueAnimator animator = ValueAnimator.OfFloat(0.0f, 1.0f);
        //        animator.SetDuration(500);
        //        animator.RepeatCount = ValueAnimator.Infinite;
        //        animator.RepeatMode = ValueAnimatorRepeatMode.Reverse;
        //        animator.AddUpdateListener(this);

        //        animator.Start();
        //    }
        //    catch (Exception e)
        //    {
        //        Methods.DisplayReportResultTrack(e);
        //    }
        //}

        //private void StopAnimation()
        //{ 
        //    try
        //    {
        //        ObjectAnimator.OfFloat(FindViewById(Resource.Id.activity_call_chat_view), "alpha", 0f)?.SetDuration(125)?.Start();
        //    }
        //    catch (Exception e)
        //    {
        //        Methods.DisplayReportResultTrack(e);
        //    }
        //}

        //public void OnAnimationUpdate(ValueAnimator animation)
        //{
        //    try
        //    {
        //        int start = Color.ParseColor("#424242");
        //        int mid = Color.ParseColor("#B71C1C");
        //        int end = Color.ParseColor("#424242");

        //        ArgbEvaluator evaluator = new ArgbEvaluator();

        //        GradientDrawable gradient = (GradientDrawable) MainCallView.Background;

        //        float fraction = animation.AnimatedFraction;
        //        int newStrat = (int) evaluator.Evaluate(fraction, start, end);
        //        int newMid = (int) evaluator.Evaluate(fraction, mid, start);
        //        int newEnd = (int) evaluator.Evaluate(fraction, end, mid);
        //        int[] newArray = {newStrat, newMid, newEnd};
        //        gradient.SetColors(newArray);
        //    }
        //    catch (Exception e)
        //    {
        //        Methods.DisplayReportResultTrack(e);
        //    } 
        //}
    }
}