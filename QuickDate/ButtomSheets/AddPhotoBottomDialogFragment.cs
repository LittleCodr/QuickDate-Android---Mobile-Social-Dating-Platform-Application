using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.BottomSheet;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Utils;
using System;

namespace QuickDate.ButtomSheets
{
    public class AddPhotoBottomDialogFragment : BottomSheetDialogFragment
    {
        #region Variables Basic

        public ImageView UserAvatar;
        public TextView Headline, SkipTextView, Seconderytext, Icon, Icon2;
        public AppCompatButton AddPhoto;
        public HomeActivity GlobalContext;

        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            GlobalContext = HomeActivity.GetInstance();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                var contextThemeWrapper = QuickDateTools.IsTabDark() ? new ContextThemeWrapper(Activity, Resource.Style.MyTheme_Dark) : new ContextThemeWrapper(Activity, Resource.Style.MyTheme);

                // clone the inflater using the ContextThemeWrapper

                LayoutInflater localInflater = inflater.CloneInContext(contextThemeWrapper);

                View view = localInflater?.Inflate(Resource.Layout.ButtomSheetAddPhoto, container, false);

                InitComponent(view);

                AddPhoto.Click += AddPhotoOnClick;
                SkipTextView.Click += SkipTextViewOnClick;

                return view;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        #endregion

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                UserAvatar = view.FindViewById<ImageView>(Resource.Id.useravatar);
                Headline = view.FindViewById<TextView>(Resource.Id.headline);
                Seconderytext = view.FindViewById<TextView>(Resource.Id.seconderytext);
                Icon = view.FindViewById<TextView>(Resource.Id.Icon);
                Icon2 = view.FindViewById<TextView>(Resource.Id.Icon2);
                AddPhoto = view.FindViewById<AppCompatButton>(Resource.Id.addButton);
                SkipTextView = view.FindViewById<TextView>(Resource.Id.skipbutton);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, Icon, IonIconsFonts.Camera);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, Icon2, IonIconsFonts.Camera);

                GlideImageLoader.LoadImage(Activity, "no_profile_image", UserAvatar, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Event

        private void AddPhotoOnClick(object sender, EventArgs e)
        {
            try
            {
                GlobalContext.TypeAvatar = "Avatar";
                PixImagePickerUtils.OpenDialogGallery(GlobalContext);
                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void SkipTextViewOnClick(object sender, EventArgs e)
        {
            try
            {
                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

    }
}