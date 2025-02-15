using Android.App;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using QuickDate.Activities.Live.Utils;
using QuickDate.Activities.SettingsUser;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Utils;
using System;

namespace QuickDate.Activities.Live.Page
{
    public class UgcPrivacyDialog
    {
        private readonly AppCompatActivity ActivityContext;
        private Dialog PrivacyDialogWindow;
        private WebView HybridView;
        public UgcPrivacyDialog(AppCompatActivity context)
        {
            try
            {
                ActivityContext = context;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void DisplayPrivacyDialog()
        {
            try
            {
                PrivacyDialogWindow = new Dialog(ActivityContext, QuickDateTools.IsTabDark() ? Resource.Style.MyDialogThemeDark : Resource.Style.MyDialogTheme);
                PrivacyDialogWindow.SetContentView(Resource.Layout.PaymentWebViewLayout);

                var title = (TextView)PrivacyDialogWindow.FindViewById(Resource.Id.toolbar_title);
                if (title != null)
                    title.Text = " ";

                var closeButton = (TextView)PrivacyDialogWindow.FindViewById(Resource.Id.toolbar_close);
                if (closeButton != null)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, closeButton, IonIconsFonts.Close);

                    closeButton.SetTextSize(ComplexUnitType.Sp, 20f);
                    closeButton.Click += CloseButtonOnClick;
                }

                var acceptButton = (AppCompatButton)PrivacyDialogWindow.FindViewById(Resource.Id.AcceptButton);
                if (acceptButton != null)
                {
                    acceptButton.Visibility = ViewStates.Visible;
                    acceptButton.Click += AcceptButtonOnClick;
                }
                HybridView = PrivacyDialogWindow.FindViewById<WebView>(Resource.Id.LocalWebView);

                //Set WebView
                if (HybridView != null)
                {
                    HybridView.SetWebViewClient(new WebViewClient());
                    //Load url to be rendered on WebView
                    HybridView.LoadUrl("file:///android_asset/Ugc.html");   // now it will not fail here

                    int px10 = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 10, Application.Context.Resources.DisplayMetrics);
                    int px50 = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 50, Application.Context.Resources.DisplayMetrics);

                    Methods.SetMargin(HybridView, px10, 0, px10, px50);
                }

                PrivacyDialogWindow.Show();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void AcceptButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                MainSettings.UgcPrivacy?.Edit()?.PutBoolean("UgcPrivacy_key", true)?.Commit();
                PrivacyDialogWindow.Hide();
                PrivacyDialogWindow.Dismiss();

                new LiveUtil(ActivityContext).OpenLive();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void CloseButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (PrivacyDialogWindow != null)
                {
                    PrivacyDialogWindow.Hide();
                    PrivacyDialogWindow.Dismiss();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void StopDialog()
        {
            try
            {
                if (PrivacyDialogWindow != null)
                {
                    PrivacyDialogWindow.Hide();
                    PrivacyDialogWindow.Dismiss();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }
}