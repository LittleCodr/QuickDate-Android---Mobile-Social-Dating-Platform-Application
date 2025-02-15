using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Content.Res;
using AndroidX.Core.Content;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Google.Android.Material.Dialog;
using Java.IO;
using QuickDate.Helpers.Utils;
using Sephiroth.ImageZoom;
using System;
using System.Collections.Generic;
using System.Linq;
using Exception = System.Exception;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace QuickDate.Activities.Viewer
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class ImageViewerActivity : AppCompatActivity, IDialogListCallBack
    {
        #region Variables Basic

        private ImageViewTouch Image;
        private ImageView MoreButton;

        private string Id, MediaFile;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.ImageViewerLayout);

                Id = Intent?.GetStringExtra("Id") ?? "";
                MediaFile = Intent?.GetStringExtra("MediaFile") ?? "";

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();

                LoadData();
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
                base.OnTrimMemory(level);
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
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
                base.OnLowMemory();
                GC.Collect(GC.MaxGeneration);
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
                Image = FindViewById<ImageViewTouch>(Resource.Id.imageview);
                MoreButton = FindViewById<ImageView>(Resource.Id.moreButton);
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
                    toolbar.Title = "";
                    toolbar.SetTitleTextColor(Color.White);
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
                    MoreButton.Click += MoreButtonOnClick;
                }
                else
                {
                    MoreButton.Click -= MoreButtonOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        private void MoreButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                arrayAdapter.Add(GetText(Resource.String.Lbl_Share));

                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetPositiveButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Load Data 

        private async void LoadData()
        {
            try
            {
                if (!string.IsNullOrEmpty(MediaFile))
                {
                    var fileName = MediaFile.Split('/').Last();
                    MediaFile = await QuickDateTools.GetFile(Id, Methods.Path.FolderDiskImage, fileName, MediaFile);

                    string imageFile = Methods.MultiMedia.CheckFileIfExits(MediaFile);
                    if (imageFile != "File Dont Exists")
                    {
                        File file2 = new File(MediaFile);
                        var photoUri = FileProvider.GetUriForFile(this, PackageName + ".fileprovider", file2);

                        if (imageFile.Contains(".gif"))
                            Glide.With(this).Load(photoUri).Apply(new RequestOptions().Placeholder(Resource.Drawable.ImagePlacholder).FitCenter()).Into(Image);
                        else
                            Glide.With(this).Load(photoUri).Apply(new RequestOptions()).Into(Image);
                    }
                    else
                    {
                        if (MediaFile.Contains(".gif"))
                            Glide.With(this).Load(MediaFile).Apply(new RequestOptions().Placeholder(Resource.Drawable.ImagePlacholder).FitCenter()).Into(Image);
                        else
                            Glide.With(this).Load(MediaFile).Apply(new RequestOptions()).Into(Image);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region MaterialDialog

        public async void OnSelection(IDialogInterface dialog, int position, string itemString)
        {
            try
            {
                if (itemString == GetText(Resource.String.Lbl_Share))
                {
                    string urlImage = MediaFile;
                    var fileName = urlImage?.Split('/').Last();

                    await ShareFileImplementation.ShareRemoteFile(this, urlImage, urlImage, fileName, GetText(Resource.String.Lbl_Send_to));
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