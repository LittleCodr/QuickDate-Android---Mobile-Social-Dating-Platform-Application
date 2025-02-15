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
using Com.Facebook.Ads;
using Com.Google.Android.Gms.Ads.Admanager;
using QuickDate.Activities.Base;
using QuickDate.Helpers.Ads;
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

namespace QuickDate.Activities.MyProfile
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class FavoriteEditActivity : BaseActivity
    {
        #region Variables Basic

        private TextView MusicIcon, DishIcon, SongIcon, HobbyIcon, CityIcon, SportIcon, BookIcon, MovieIcon, ColorIcon, TvShowIcon;
        private EditText EdtMusic, EdtDish, EdtSong, EdtHobby, EdtCity, EdtSport, EdtBook, EdtMovie, EdtColor, EdtTvShow;
        private AppCompatButton BtnSave;
        private AdManagerAdView AdManagerAdView;
        private RewardedVideoAd RewardedVideoAd;

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
                SetContentView(Resource.Layout.ButtomSheetFavoriteEdit);

                //Get Value And Set Toolbar
                InitComponent();
                InitBackground();
                InitToolbar();

                GetMyInfoData();

                if (AppSettings.ShowFbRewardVideoAds)
                    RewardedVideoAd = AdsFacebook.InitRewardVideo(this);
                else
                    AdsGoogle.Ad_RewardedVideo(this);
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
                RewardedVideoAd?.Destroy();
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
                    Intent resultIntent = new Intent();
                    SetResult(Result.Canceled, resultIntent);
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
                MusicIcon = FindViewById<TextView>(Resource.Id.IconMusic);
                EdtMusic = FindViewById<EditText>(Resource.Id.MusicEditText);

                DishIcon = FindViewById<TextView>(Resource.Id.IconDish);
                EdtDish = FindViewById<EditText>(Resource.Id.DishEditText);

                SongIcon = FindViewById<TextView>(Resource.Id.IconSong);
                EdtSong = FindViewById<EditText>(Resource.Id.SongEditText);

                HobbyIcon = FindViewById<TextView>(Resource.Id.IconHobby);
                EdtHobby = FindViewById<EditText>(Resource.Id.HobbyEditText);

                CityIcon = FindViewById<TextView>(Resource.Id.IconCity);
                EdtCity = FindViewById<EditText>(Resource.Id.CityEditText);

                SportIcon = FindViewById<TextView>(Resource.Id.IconSport);
                EdtSport = FindViewById<EditText>(Resource.Id.SportEditText);

                BookIcon = FindViewById<TextView>(Resource.Id.IconBook);
                EdtBook = FindViewById<EditText>(Resource.Id.BookEditText);

                MovieIcon = FindViewById<TextView>(Resource.Id.IconMovie);
                EdtMovie = FindViewById<EditText>(Resource.Id.MovieEditText);

                ColorIcon = FindViewById<TextView>(Resource.Id.IconColor);
                EdtColor = FindViewById<EditText>(Resource.Id.ColorEditText);

                TvShowIcon = FindViewById<TextView>(Resource.Id.IconTvShow);
                EdtTvShow = FindViewById<EditText>(Resource.Id.TvShowEditText);

                BtnSave = FindViewById<AppCompatButton>(Resource.Id.ApplyButton);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, MusicIcon, FontAwesomeIcon.Music);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, DishIcon, FontAwesomeIcon.Fish);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeBrands, SongIcon, FontAwesomeIcon.Mandalorian);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, HobbyIcon, FontAwesomeIcon.Smile);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, CityIcon, FontAwesomeIcon.City);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, SportIcon, FontAwesomeIcon.FootballBall);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, BookIcon, FontAwesomeIcon.Book);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, MovieIcon, FontAwesomeIcon.Film);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, ColorIcon, FontAwesomeIcon.Palette);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, TvShowIcon, FontAwesomeIcon.Tv);

                Methods.SetColorEditText(EdtMusic, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtDish, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtSong, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtHobby, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtCity, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtSport, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtBook, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtMovie, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtColor, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtTvShow, QuickDateTools.IsTabDark() ? Color.White : Color.Black);

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
                var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    toolbar.Title = GetString(Resource.String.Lbl_FavoriteInfo);
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
                        {"music", EdtMusic.Text},
                        {"dish", EdtDish.Text},
                        {"song", EdtSong.Text},
                        {"hobby",EdtHobby.Text},
                        {"city",EdtCity.Text},
                        {"sport",EdtSport.Text},
                        {"book",EdtBook.Text},
                        {"movie",EdtMovie.Text},
                        {"colour",EdtColor.Text},
                        {"tv",EdtTvShow.Text},
                    };

                    var (apiStatus, respond) = await RequestsAsync.Users.UpdateProfileAsync(dictionary);
                    if (apiStatus == 200)
                    {
                        if (respond is UpdateProfileObject result)
                        {
                            var local = ListUtils.MyUserInfo?.FirstOrDefault();
                            if (local != null)
                            {
                                local.Music = EdtMusic.Text;
                                local.Dish = EdtDish.Text;
                                local.Song = EdtSong.Text;
                                local.Hobby = EdtHobby.Text;
                                local.City = EdtCity.Text;
                                local.Sport = EdtSport.Text;
                                local.Book = EdtBook.Text;
                                local.Movie = EdtMovie.Text;
                                local.Colour = EdtColor.Text;
                                local.Tv = EdtTvShow.Text;

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
                        //Show a Error image with a message
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

        private void BackIconOnClick(object sender, EventArgs e)
        {
            try
            {
                Intent resultIntent = new Intent();
                SetResult(Result.Canceled, resultIntent);
                Finish();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
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
                    EdtMusic.Text = dataUser.Music;
                    EdtDish.Text = dataUser.Dish;
                    EdtSong.Text = dataUser.Song;
                    EdtHobby.Text = dataUser.Hobby;
                    EdtCity.Text = dataUser.City;
                    EdtSport.Text = dataUser.Sport;
                    EdtBook.Text = dataUser.Book;
                    EdtMovie.Text = dataUser.Movie;
                    EdtColor.Text = dataUser.Colour;
                    EdtTvShow.Text = dataUser.Tv;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

    }
}