using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using AndroidX.AppCompat.Content.Res;
using Google.Android.Material.Dialog;
using Newtonsoft.Json;
using QuickDate.Activities.Base;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Utils;
using QuickDate.Library.Anjo.Share;
using QuickDate.Library.Anjo.Share.Abstractions;
using QuickDateClient.Classes.Blogs;
using System;
using System.Collections.Generic;
using Exception = System.Exception;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace QuickDate.Activities.Blogs
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class ArticlesViewActivity : BaseActivity, IDialogListCallBack
    {
        #region Variables Basic

        private ImageView ImageBlog;
        private TextView TxtTitle, TxtDescription, TxtViews;
        private WebView TxtHtml;
        private ImageButton BtnMore;
        private ArticleDataObject ArticleData;

        public string DataWebHtml { get; private set; }

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
                SetContentView(Resource.Layout.ArticlesViewLayout);

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();

                GetDataArticles();
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

                case Resource.Id.action_share:
                    ShareEvent();
                    break;

                case Resource.Id.action_copy:
                    CopyLinkEvent();
                    break;

            }

            return base.OnOptionsItemSelected(item);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MenuArticleShare, menu);
            ChangeMenuIconColor(menu, QuickDateTools.IsTabDark() ? Color.White : Color.Black);

            return base.OnCreateOptionsMenu(menu);
        }

        private void ChangeMenuIconColor(IMenu menu, Color color)
        {
            for (int i = 0; i < menu?.Size(); i++)
            {
                var drawable = menu.GetItem(i)?.Icon;
                if (drawable == null) continue;
                drawable.Mutate();
                drawable.SetColorFilter(new PorterDuffColorFilter(color, PorterDuff.Mode.SrcAtop));
            }
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                ImageBlog = FindViewById<ImageView>(Resource.Id.imageBlog);
                TxtTitle = FindViewById<TextView>(Resource.Id.title);
                TxtDescription = FindViewById<TextView>(Resource.Id.description);
                TxtHtml = FindViewById<WebView>(Resource.Id.LocalWebView);
                TxtViews = FindViewById<TextView>(Resource.Id.views);
                BtnMore = FindViewById<ImageButton>(Resource.Id.more);
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
                    toolbar.Title = "";
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
                    BtnMore.Click += BtnMoreOnClick;
                }
                else
                {
                    BtnMore.Click -= BtnMoreOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        private void BtnMoreOnClick(object sender, EventArgs e)
        {
            try
            {
                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                arrayAdapter.Add(GetString(Resource.String.Lbl_CopyLink));
                arrayAdapter.Add(GetString(Resource.String.Lbl_Share));

                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Event Menu >> Copy Link
        private void CopyLinkEvent()
        {
            try
            {
                Methods.CopyToClipboard(this, ArticleData.Url);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //Event Menu >> Share
        private async void ShareEvent()
        {
            try
            {
                //Share Plugin same as video
                if (!CrossShare.IsSupported) return;

                await CrossShare.Current.Share(new ShareMessage
                {
                    Title = ArticleData.Title,
                    Text = " ",
                    Url = ArticleData.Url
                });
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region MaterialDialog

        public void OnSelection(IDialogInterface dialog, int position, string itemString)
        {
            try
            {
                string text = itemString;
                if (text == GetString(Resource.String.Lbl_CopyLink))
                {
                    CopyLinkEvent();
                }
                else if (text == GetString(Resource.String.Lbl_Share))
                {
                    ShareEvent();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        private void GetDataArticles()
        {
            try
            {
                ArticleData = JsonConvert.DeserializeObject<ArticleDataObject>(Intent?.GetStringExtra("ArticleObject") ?? "");
                if (ArticleData != null)
                {
                    GlideImageLoader.LoadImage(this, ArticleData.Thumbnail, ImageBlog, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                    SupportActionBar.Title = Methods.FunString.DecodeString(ArticleData.Title);

                    TxtTitle.Text = Methods.FunString.DecodeString(ArticleData.Title);
                    TxtDescription.Text = Methods.FunString.DecodeString(ArticleData.Description);

                    TxtViews.Text = ArticleData.View + " " + GetText(Resource.String.Lbl_Views);

                    string style = QuickDateTools.IsTabDark() ? "<style type='text/css'>body{color: #fff; background-color: #444;}</style>" : "<style type='text/css'>body{color: #444; background-color: #fff;}</style>";
                    string imageFullWidthStyle = "<style>img{display: inline;height: auto;max-width: 100%;}</style>";
                    string content;
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                    {
                        content = Html.FromHtml(ArticleData.Content, FromHtmlOptions.ModeCompact)?.ToString();
                    }
                    else
                    {
                        // This method is deprecated but need to use for old os devices
#pragma warning disable CS0618 // Type or member is obsolete
                        content = Html.FromHtml(ArticleData.Content)?.ToString();
#pragma warning restore CS0618 // Type or member is obsolete
                    }

                    DataWebHtml = "<!DOCTYPE html>";
                    DataWebHtml += $"<head><title></title>{style}{imageFullWidthStyle}</head>";
                    DataWebHtml += "<body>" + content + "</body>";
                    DataWebHtml += "</html>";
                    TxtHtml.SetWebViewClient(new MyWebViewClient(this));
                    TxtHtml.Settings.LoadsImagesAutomatically = true;
                    TxtHtml.Settings.JavaScriptEnabled = true;
                    TxtHtml.Settings.JavaScriptCanOpenWindowsAutomatically = true;
                    TxtHtml.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.TextAutosizing);
                    TxtHtml.Settings.DomStorageEnabled = true;
                    TxtHtml.Settings.AllowFileAccess = true;
                    TxtHtml.Settings.DefaultTextEncodingName = "utf-8";
                    TxtHtml.Settings.UseWideViewPort = true;
                    TxtHtml.Settings.LoadWithOverviewMode = true;
                    TxtHtml.Settings.SetSupportZoom(false);
                    TxtHtml.Settings.BuiltInZoomControls = false;
                    TxtHtml.Settings.DisplayZoomControls = false;

                    //int fontSize = (int)TypedValue.ApplyDimension(ComplexUnitType.Sp, 18, Resources.DisplayMetrics);
                    //TxtHtml.Settings.DefaultFontSize = fontSize;

                    TxtHtml.LoadDataWithBaseURL(null, DataWebHtml, "text/html", "UTF-8", null);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private class MyWebViewClient : WebViewClient
        {
            private readonly ArticlesViewActivity Activity;
            public MyWebViewClient(ArticlesViewActivity mActivity)
            {
                Activity = mActivity;
            }
            public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
            {
                new IntentController(Activity).OpenBrowserFromApp(request.Url.ToString());
                view.LoadDataWithBaseURL(null, Activity.DataWebHtml, "text/html", "UTF-8", null);
                return true;
            }
        }
    }
}