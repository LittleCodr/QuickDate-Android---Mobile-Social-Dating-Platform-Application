using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.Content.Res;
using AndroidX.AppCompat.Widget;
using QuickDate.Activities.Base;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Authorization;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using System;
using Exception = System.Exception;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace QuickDate.Activities.Default
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class ReplacePasswordActivity : BaseActivity
    {
        #region Variables Basic

        private EditText TxtNumber1, TxtPassword;
        private AppCompatButton BtnVerify, BtnReSent;
        private string EmailCode, Email;
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
                SetContentView(Resource.Layout.ReplacePasswordUserLayout);

                EmailCode = Intent?.GetStringExtra("EmailCode") ?? "";
                Email = Intent?.GetStringExtra("Email") ?? "";

                //Get Value And Set Toolbar
                InitComponent();
                InitBackground();
                InitToolbar();
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

        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                TxtNumber1 = FindViewById<EditText>(Resource.Id.TextNumber1);
                TxtPassword = FindViewById<EditText>(Resource.Id.edt_password);
                BtnVerify = FindViewById<AppCompatButton>(Resource.Id.verifyButton);
                BtnReSent = FindViewById<AppCompatButton>(Resource.Id.reSentButton);

                Methods.SetColorEditText(TxtNumber1, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtPassword, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
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
                var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    toolbar.Title = " ";
                    toolbar.SetTitleTextColor(QuickDateTools.IsTabDark() ? AppSettings.TitleTextColorDark : AppSettings.TitleTextColor);
                    SetSupportActionBar(toolbar);
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

        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    BtnVerify.Click += BtnVerifyOnClick;
                    BtnReSent.Click += BtnReSentOnClick;
                }
                else
                {
                    BtnVerify.Click -= BtnVerifyOnClick;
                    BtnReSent.Click -= BtnReSentOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        private async void BtnVerifyOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(TxtNumber1.Text) && !string.IsNullOrWhiteSpace(TxtNumber1.Text) && !string.IsNullOrEmpty(TxtPassword.Text) && !string.IsNullOrWhiteSpace(TxtPassword.Text))
                {
                    if (EmailCode == TxtNumber1.Text)
                    {
                        if (Methods.CheckConnectivity())
                        {
                            //Show a progress
                            AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                            var (apiStatus, respond) = await RequestsAsync.Auth.ReplacePasswordAsync(Email, TxtNumber1.Text, TxtPassword.Text);
                            if (apiStatus == 200)
                            {
                                if (respond is ReplacePasswordObject auth)
                                {
                                    UserDetails.Password = TxtPassword.Text;

                                    AndHUD.Shared.ShowSuccess(this, GetText(Resource.String.Lbl_SuccessfullyUpdated), MaskType.Clear, TimeSpan.FromSeconds(2));
                                    SetResult(Result.Ok);
                                    Finish();
                                }
                            }
                            else
                            {
                                if (respond is ErrorObject errorMessage)
                                {
                                    var errorId = errorMessage.ErrorData.ErrorId;
                                    if (errorId == "3")
                                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_CodeNotCorrect), GetText(Resource.String.Lbl_Ok));
                                }
                                //Methods.DisplayReportResult(this, respond);
                                AndHUD.Shared.Dismiss();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                        }
                    }
                    else
                    {
                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_CodeNotCorrect), GetText(Resource.String.Lbl_Ok));
                    }
                }
                else
                {
                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_Please_enter_your_data), GetText(Resource.String.Lbl_Ok));
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                AndHUD.Shared.Dismiss();
            }
        }

        private async void BtnReSentOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                    var (apiStatus, respond) = await RequestsAsync.Auth.ResendEmailAsync(Email);
                    if (apiStatus != 200)
                        Methods.DisplayReportResult(this, respond);

                    AndHUD.Shared.Dismiss();
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                AndHUD.Shared.Dismiss();
            }
        }


        #endregion 
    }
}