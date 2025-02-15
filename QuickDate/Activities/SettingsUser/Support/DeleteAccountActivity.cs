using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Content.Res;
using AndroidX.AppCompat.Widget;
using Com.Google.Android.Gms.Ads;
using QuickDate.Activities.Base;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using System;
using System.Linq;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace QuickDate.Activities.SettingsUser.Support
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class DeleteAccountActivity : BaseActivity
    {
        #region Variables Basic

        public Toolbar Toolbar;
        public EditText PasswordEditText;
        public CheckBox DeleteCheckBox;
        public AppCompatButton DeleteButton;
        public AdView MAdView;

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
                SetContentView(Resource.Layout.DeleteAccountLayout);

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
                PasswordEditText = FindViewById<EditText>(Resource.Id.passwordEdit);
                DeleteCheckBox = FindViewById<CheckBox>(Resource.Id.DeleteCheckBox);
                DeleteButton = FindViewById<AppCompatButton>(Resource.Id.DeleteButton);

                DeleteCheckBox.Text = GetText(Resource.String.Lbl_IWantToDelete1) + " " + UserDetails.Username + " " +
                                      GetText(Resource.String.Lbl_IWantToDelete2) + " " + AppSettings.ApplicationName +
                                      " " + GetText(Resource.String.Lbl_IWantToDelete3);

                Methods.SetColorEditText(PasswordEditText, QuickDateTools.IsTabDark() ? Color.White : Color.Black);

                MAdView = FindViewById<AdView>(Resource.Id.adView);
                AdsGoogle.InitAdView(MAdView, null);
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
                    toolbar.Title = GetString(Resource.String.Lbl_DeleteAccount);
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
                    DeleteButton.Click += DeleteButtonOnClick;
                }
                else
                {
                    DeleteButton.Click -= DeleteButtonOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }



        #endregion

        #region Events

        private void DeleteButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (DeleteCheckBox.Checked)
                {
                    if (!Methods.CheckConnectivity())
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    }
                    else
                    {
                        var localData = ListUtils.DataUserLoginList.FirstOrDefault();
                        if (localData != null)
                        {
                            if (PasswordEditText.Text == localData.Password)
                            {
                                ApiRequest.RunLogout = false;
                                ApiRequest.Delete(this);
                                Toast.MakeText(this, GetText(Resource.String.Lbl_Your_account_was_successfully_deleted), ToastLength.Long)?.Show();
                            }
                            else
                            {
                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Warning), GetText(Resource.String.Lbl_Please_confirm_your_password), GetText(Resource.String.Lbl_Ok));
                            }
                        }
                    }
                }
                else
                {
                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Warning), GetText(Resource.String.Lbl_Error_Terms), GetText(Resource.String.Lbl_Ok));
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }


        #endregion

    }
}