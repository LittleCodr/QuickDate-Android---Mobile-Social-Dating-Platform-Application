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
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Global;
using QuickDateClient.Classes.Users;
using QuickDateClient.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace QuickDate.Activities.SettingsUser.General
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class SocialLinksActivity : BaseActivity
    {
        #region Variables Basic

        private TextView FacebookIcon, TwitterIcon, GooglePlusIcon, InstagramIcon, LinkedInIcon, WebsiteIcon;
        private EditText EdtFacebook, EdtTwitter, EdtGooglePlus, EdtInstagram, EdtLinkedIn, EdtWebsite;
        private AppCompatButton BtnSave;
        private Toolbar Toolbar;

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
                SetContentView(Resource.Layout.SocialLinksLayout);

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
                FacebookIcon = FindViewById<TextView>(Resource.Id.IconFacebook);
                EdtFacebook = FindViewById<EditText>(Resource.Id.FacebookEditText);

                TwitterIcon = FindViewById<TextView>(Resource.Id.IconTwitter);
                EdtTwitter = FindViewById<EditText>(Resource.Id.TwitterEditText);

                GooglePlusIcon = FindViewById<TextView>(Resource.Id.IconGooglePlus);
                EdtGooglePlus = FindViewById<EditText>(Resource.Id.GooglePlusEditText);

                InstagramIcon = FindViewById<TextView>(Resource.Id.IconInstagram);
                EdtInstagram = FindViewById<EditText>(Resource.Id.InstagramEditText);

                LinkedInIcon = FindViewById<TextView>(Resource.Id.IconLinkedIn);
                EdtLinkedIn = FindViewById<EditText>(Resource.Id.LinkedInEditText);

                WebsiteIcon = FindViewById<TextView>(Resource.Id.IconWebsite);
                EdtWebsite = FindViewById<EditText>(Resource.Id.WebsiteEditText);

                BtnSave = FindViewById<AppCompatButton>(Resource.Id.ApplyButton);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeBrands, FacebookIcon, FontAwesomeIcon.FacebookF);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeBrands, TwitterIcon, FontAwesomeIcon.Twitter);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeBrands, GooglePlusIcon, FontAwesomeIcon.GooglePlusG);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeBrands, InstagramIcon, FontAwesomeIcon.Instagram);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeBrands, LinkedInIcon, FontAwesomeIcon.LinkedinIn);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeBrands, WebsiteIcon, FontAwesomeIcon.Edge);

                Methods.SetColorEditText(EdtFacebook, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtTwitter, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtGooglePlus, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtInstagram, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtLinkedIn, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtWebsite, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
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
                    Toolbar.Title = GetString(Resource.String.Lbl_SocialLinks);
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
                }
                else
                {
                    BtnSave.Click -= BtnSaveOnClick;
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
                if (Methods.CheckConnectivity())
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                    var dictionary = new Dictionary<string, string>
                    {
                        {"facebook", EdtFacebook.Text},
                        {"twitter", EdtTwitter.Text},
                        {"google", EdtGooglePlus.Text},
                        {"instagram",EdtInstagram.Text},
                        {"linkedin",EdtLinkedIn.Text},
                        {"website",EdtWebsite.Text},
                    };

                    var (apiStatus, respond) = await RequestsAsync.Users.UpdateProfileAsync(dictionary);
                    if (apiStatus == 200)
                    {
                        if (respond is UpdateProfileObject result)
                        {
                            var local = ListUtils.MyUserInfo?.FirstOrDefault();
                            if (local != null)
                            {
                                local.Facebook = EdtFacebook.Text;
                                local.Twitter = EdtTwitter.Text;
                                local.Google = EdtGooglePlus.Text;
                                local.Instagram = EdtInstagram.Text;
                                local.Linkedin = EdtLinkedIn.Text;
                                local.Website = EdtWebsite.Text;

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
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                AndHUD.Shared.Dismiss();
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
                    EdtFacebook.Text = dataUser.Facebook;
                    EdtTwitter.Text = dataUser.Twitter;
                    EdtGooglePlus.Text = dataUser.Google;
                    EdtInstagram.Text = dataUser.Instagram;
                    EdtLinkedIn.Text = dataUser.Linkedin;
                    EdtWebsite.Text = dataUser.Website;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }
}