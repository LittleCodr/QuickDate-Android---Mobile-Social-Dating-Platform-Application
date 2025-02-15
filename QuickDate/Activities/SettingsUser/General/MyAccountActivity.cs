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
using Com.Google.Android.Gms.Ads.Admanager;
using Google.Android.Material.Dialog;
using QuickDate.Activities.Base;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Global;
using QuickDateClient.Classes.Users;
using QuickDateClient.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using Exception = System.Exception;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace QuickDate.Activities.SettingsUser.General
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MyAccountActivity : BaseActivity, IDialogListCallBack
    {
        #region Variables Basic

        private TextView UsernameIcon, EmailIcon, PhoneNumberIcon, CountryIcon;
        private EditText EdtUsername, EdtEmail, EdtPhoneNumber, EdtCountry;
        private AppCompatButton BtnSave;
        private Toolbar Toolbar;
        private string Country;
        private HomeActivity GlobalContext;
        private AdManagerAdView AdManagerAdView;

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
                SetContentView(Resource.Layout.MyAccountLayout);

                GlobalContext = HomeActivity.GetInstance();

                //Get Value And Set Toolbar
                InitComponent();
                InitBackground();
                InitToolbar();

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
                AdsGoogle.LifecycleAdManagerAdView(AdManagerAdView, "Resume");
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
                AdsGoogle.LifecycleAdManagerAdView(AdManagerAdView, "Pause");
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
                AdsGoogle.LifecycleAdManagerAdView(AdManagerAdView, "Destroy");
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
                UsernameIcon = FindViewById<TextView>(Resource.Id.IconUsername);
                EdtUsername = FindViewById<EditText>(Resource.Id.UsernameEditText);

                EmailIcon = FindViewById<TextView>(Resource.Id.IconEmail);
                EdtEmail = FindViewById<EditText>(Resource.Id.EmailEditText);

                PhoneNumberIcon = FindViewById<TextView>(Resource.Id.IconPhoneNumber);
                EdtPhoneNumber = FindViewById<EditText>(Resource.Id.PhoneNumberEditText);

                CountryIcon = FindViewById<TextView>(Resource.Id.IconCountry);
                EdtCountry = FindViewById<EditText>(Resource.Id.CountryEditText);

                BtnSave = FindViewById<AppCompatButton>(Resource.Id.ApplyButton);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, UsernameIcon, FontAwesomeIcon.Users);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, EmailIcon, FontAwesomeIcon.At);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, PhoneNumberIcon, FontAwesomeIcon.Phone);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, CountryIcon, FontAwesomeIcon.Flag);

                Methods.SetColorEditText(EdtUsername, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtEmail, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtPhoneNumber, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtCountry, QuickDateTools.IsTabDark() ? Color.White : Color.Black);

                Methods.SetFocusable(EdtCountry);

                AdManagerAdView = FindViewById<AdManagerAdView>(Resource.Id.multiple_ad_sizes_view);
                AdsGoogle.InitAdManagerAdView(AdManagerAdView);
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
                Toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (Toolbar != null)
                {
                    Toolbar.Title = GetString(Resource.String.Lbl_MyAccount);
                    Toolbar.SetTitleTextColor(QuickDateTools.IsTabDark() ? AppSettings.TitleTextColorDark : AppSettings.TitleTextColor);
                    SetSupportActionBar(Toolbar);
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
                    BtnSave.Click += BtnSaveOnClick;
                    EdtCountry.Click += EdtCountryOnClick;
                }
                else
                {
                    BtnSave.Click -= BtnSaveOnClick;
                    EdtCountry.Click -= EdtCountryOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }


        #endregion

        #region Events

        //Click save data and sent api
        private async void BtnSaveOnClick(object sender, EventArgs e)
        {
            try
            {
                var correctly = Methods.FunString.IsPhoneNumber(EdtPhoneNumber.Text);
                if (!correctly)
                {
                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_VerificationFailed), GetText(Resource.String.Lbl_PleaseAddPhoneNumberCorrectly), GetText(Resource.String.Lbl_Ok));
                    return;
                }

                if (Methods.CheckConnectivity())
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                    var dictionary = new Dictionary<string, string>
                    {
                        {"username", EdtUsername.Text},
                        {"email", EdtEmail.Text},
                        {"country",Country},
                        {"phone_number", EdtPhoneNumber.Text},
                    };

                    var (apiStatus, respond) = await RequestsAsync.Users.UpdateProfileAsync(dictionary);
                    if (apiStatus == 200)
                    {
                        if (respond is UpdateProfileObject result)
                        {
                            var local = ListUtils.MyUserInfo?.FirstOrDefault();
                            if (local != null)
                            {
                                local.Username = EdtUsername.Text;
                                local.Email = EdtEmail.Text;
                                local.PhoneNumber = EdtPhoneNumber.Text;
                                local.Country = Country;

                                SqLiteDatabase database = new SqLiteDatabase();
                                database.InsertOrUpdate_DataMyInfo(local);

                            }

                            Toast.MakeText(this, GetText(Resource.String.Lbl_SuccessfullyUpdated), ToastLength.Short)?.Show();
                            AndHUD.Shared.Dismiss();

                            Intent resultIntent = new Intent();
                            SetResult(Result.Ok, resultIntent);
                            Finish();
                        }
                    }
                    else
                    {
                        //Methods.DisplayReportResult(this, respond);
                        if (respond is ErrorObject error)
                        {
                            var errorText = error.ErrorData.ErrorText;
                            AndHUD.Shared.ShowError(this, errorText, MaskType.Clear, TimeSpan.FromSeconds(2));
                        }
                    }
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }

                if (!correctly)
                    Toast.MakeText(this, GetText(Resource.String.Lbl_PleaseAddPhoneNumberCorrectly), ToastLength.Short)?.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                AndHUD.Shared.Dismiss();
            }
        }

        //Country
        private void EdtCountryOnClick(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    //string[] countriesArray = Context.Resources.GetStringArray(Resource.Array.countriesArray);
                    var countriesArray = ListUtils.SettingsSiteList?.Countries;

                    var arrayAdapter = new List<string>();
                    var dialogList = new MaterialAlertDialogBuilder(this);

                    if (countriesArray != null) arrayAdapter.AddRange(countriesArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault()?.Name)));

                    dialogList.SetTitle(GetText(Resource.String.Lbl_Location));
                    dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                    dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                    dialogList.Show();
                }
                catch (Exception exception)
                {
                    Methods.DisplayReportResultTrack(exception);
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
                if (requestCode == 125 && resultCode == Result.Ok)
                {
                    var status = data.GetStringExtra("status") ?? "";
                    if (status == "1")
                    {
                        EdtPhoneNumber.Text = ListUtils.MyUserInfo?.FirstOrDefault()?.PhoneNumber ?? "";
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

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
                    EdtUsername.Text = dataUser.Username;
                    EdtEmail.Text = dataUser.Email;
                    EdtPhoneNumber.Text = dataUser.PhoneNumber;
                    EdtCountry.Text = QuickDateTools.GetCountry(dataUser.Country);

                    if (dataUser.PhoneVerified == "0" && dataUser.Verified == "0" && AppSettings.ValidationEnabled)
                    {
                        var window = new PopupController(this);
                        window.DisplayAddPhoneNumber();

                        if (GlobalContext.TracksCounter != null)
                            GlobalContext.TracksCounter.LastCounterEnum = TracksCounter.TracksCounterEnum.AddPhoneNumber;
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #region MaterialDialog

        public void OnSelection(IDialogInterface dialog, int position, string itemString)
        {
            try
            {
                var id = position;
                var text = itemString;

                //string[] countriesArrayId = Context.Resources.GetStringArray(Resource.Array.countriesArray_id);
                var countriesArrayId = ListUtils.SettingsSiteList?.Countries?.FirstOrDefault(a => a.Values?.FirstOrDefault()?.Name == itemString)?.Keys.FirstOrDefault();

                var data = countriesArrayId;
                Country = data;
                EdtCountry.Text = text;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

    }
}