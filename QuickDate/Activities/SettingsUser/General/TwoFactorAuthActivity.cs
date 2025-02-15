using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.Content.Res;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.Dialog;
using QuickDate.Activities.Base;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Authorization;
using QuickDateClient.Classes.Global;
using QuickDateClient.Classes.Users;
using QuickDateClient.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exception = System.Exception;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace QuickDate.Activities.SettingsUser.General
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class TwoFactorAuthActivity : BaseActivity, IDialogListCallBack, IDialogInputCallBack
    {
        #region Variables Basic

        private TextView IconTwoFactor;
        private EditText TxtTwoFactor;
        private AppCompatButton SaveButton;
        private string TypeTwoFactor, TwoFactorEmailCode, CodeName, TypeDialog;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                SetTheme(QuickDateTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);

                Methods.App.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.TwoFactorAuthLayout);

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
                IconTwoFactor = FindViewById<TextView>(Resource.Id.IconTwoFactor);
                TxtTwoFactor = FindViewById<EditText>(Resource.Id.TwoFactorEditText);
                SaveButton = FindViewById<AppCompatButton>(Resource.Id.SaveButton);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, IconTwoFactor, FontAwesomeIcon.ShieldAlt);

                Methods.SetColorEditText(TxtTwoFactor, QuickDateTools.IsTabDark() ? Color.White : Color.Black);

                Methods.SetFocusable(TxtTwoFactor);

                var twoFactorUSer = ListUtils.MyUserInfo?.FirstOrDefault()?.TwoFactor;
                if (twoFactorUSer == "0")
                {
                    TxtTwoFactor.Text = GetText(Resource.String.Lbl_Disable);
                    TypeTwoFactor = "0";
                }
                else
                {
                    TxtTwoFactor.Text = GetText(Resource.String.Lbl_Enable);
                    TypeTwoFactor = "1";
                }

                AdsGoogle.Ad_AdMobNative(this);
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
                Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    toolbar.Title = GetString(Resource.String.Lbl_TwoFactor);
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
                    TxtTwoFactor.Touch += TxtTwoFactorOnTouch;
                    SaveButton.Click += SaveButtonOnClick;
                }
                else
                {
                    TxtTwoFactor.Touch -= TxtTwoFactorOnTouch;
                    SaveButton.Click -= SaveButtonOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        // check code email if good or no than update data user 
        private async void SendButtonOnClick()
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    return;
                }

                if (TwoFactorEmailCode != CodeName)
                    return;

                //Show a progress
                AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading) + "...");

                var dictionary = new Dictionary<string, string>
                {
                    {"two_factor_email_code", CodeName},
                    {"two_factor", TypeTwoFactor},
                };

                var (apiStatus, respond) = await RequestsAsync.Users.UpdateProfileAsync(dictionary);
                if (apiStatus == 200)
                {
                    if (respond is UpdateProfileObject result)
                    {
                        Console.WriteLine(result.Message);

                        var local = ListUtils.MyUserInfo?.FirstOrDefault();
                        if (local != null)
                        {
                            local.TwoFactor = "1";

                            var sqLiteDatabase = new SqLiteDatabase();
                            sqLiteDatabase.InsertOrUpdate_DataMyInfo(local);

                        }

                        Toast.MakeText(this, GetText(Resource.String.Lbl_TwoFactorOn), ToastLength.Short)?.Show();
                        AndHUD.Shared.Dismiss();

                        Finish();
                    }
                }
                else
                {
                    if (respond is ErrorObject errorMessage)
                    {
                        var errorText = errorMessage.ErrorData.ErrorText;
                        //Show a Error image with a message
                        AndHUD.Shared.ShowError(this, errorText, MaskType.Clear, TimeSpan.FromSeconds(2));
                    }

                    //Methods.DisplayReportResult(this, respond);
                }
            }
            catch (Exception exception)
            {
                AndHUD.Shared.Dismiss();
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void TxtTwoFactorOnTouch(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(this);

                var dialogList = new MaterialAlertDialogBuilder(this);

                var arrayAdapter = new List<string>
                {
                    GetString(Resource.String.Lbl_Enable), GetString(Resource.String.Lbl_Disable)
                };

                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        // send data and send api and show liner add code email 
        private async void SaveButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    return;
                }

                switch (TypeTwoFactor)
                {
                    case "1":
                        {
                            //Show a progress
                            AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading) + "...");

                            var (apiStatus, respond) = await RequestsAsync.Auth.SendCodeTwoFactorAsync(UserDetails.Email);
                            if (apiStatus == 200)
                            {
                                if (respond is not SendCodeTwoFactorObject result) return;
                                if (result.Data.MessageBody.Contains("Your confirmation code is"))
                                {
                                    TwoFactorEmailCode = result.Data.MessageBody.Split(':').Last().Replace(" ", "");
                                    Toast.MakeText(this, GetText(Resource.String.Lbl_ConfirmationCodeSent), ToastLength.Short)?.Show();

                                    AndHUD.Shared.Dismiss();

                                    TypeDialog = "ConfirmationCode";
                                    var dialog = new MaterialAlertDialogBuilder(this);
                                    dialog.SetTitle(Resource.String.Lbl_ConfirmationEmailSent);

                                    EditText input = new EditText(this);
                                    input.SetHint(Resource.String.Lbl_ConfirmationCode);
                                    input.InputType = InputTypes.ClassNumber;
                                    LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                                    input.LayoutParameters = lp;

                                    dialog.SetView(input);

                                    dialog.SetPositiveButton(GetText(Resource.String.Lbl_Send), new MaterialDialogUtils(input, this));
                                    dialog.SetNegativeButton(GetText(Resource.String.Lbl_Cancel), new MaterialDialogUtils());

                                    dialog.Show();
                                }
                                else
                                {
                                    //Show a Error image with a message
                                    AndHUD.Shared.ShowError(this, result.Message, MaskType.Clear, TimeSpan.FromSeconds(2));
                                }
                            }
                            else Methods.DisplayReportResult(this, respond);

                            break;
                        }
                    case "0":
                        var dictionary = new Dictionary<string, string>
                        {
                            {"two_factor", "0"},
                        };

                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.UpdateProfileAsync(dictionary) });
                        var local = ListUtils.MyUserInfo?.FirstOrDefault();
                        if (local != null)
                        {
                            local.TwoFactor = "0";

                            var sqLiteDatabase = new SqLiteDatabase();
                            sqLiteDatabase.InsertOrUpdate_DataMyInfo(local);

                        }

                        Finish();
                        break;
                }
            }
            catch (Exception exception)
            {
                AndHUD.Shared.Dismiss();
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region MaterialDialog

        public void OnSelection(IDialogInterface dialog, int position, string itemString)
        {
            try
            {
                if (itemString == GetText(Resource.String.Lbl_Enable))
                {
                    TxtTwoFactor.Text = GetText(Resource.String.Lbl_Enable);
                    TypeTwoFactor = "1";
                }
                else if (itemString == GetText(Resource.String.Lbl_Disable))
                {
                    TxtTwoFactor.Text = GetText(Resource.String.Lbl_Disable);
                    TypeTwoFactor = "0";
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
                    CodeName = input;
                    SendButtonOnClick();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

    }
}