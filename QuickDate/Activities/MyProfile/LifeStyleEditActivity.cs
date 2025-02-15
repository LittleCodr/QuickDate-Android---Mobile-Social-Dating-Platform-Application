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
using Google.Android.Material.Dialog;
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
using Exception = System.Exception;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace QuickDate.Activities.MyProfile
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class LifeStyleEditActivity : BaseActivity, IDialogListCallBack
    {
        #region Variables Basic

        private TextView LiveWithIcon, CarIcon, ReligionIcon, SmokeIcon, DrinkIcon, TravelIcon;
        private EditText EdtLiveWith, EdtCar, EdtReligion, EdtSmoke, EdtDrink, EdtTravel;
        private AppCompatButton BtnSave;
        private string TypeDialog;
        private int IdLiveWith, IdCar, IdReligion, IdSmoke, IdDrink, IdTravel;
        private AdView BannerAd;
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
                SetContentView(Resource.Layout.ButtomSheetLifeStyleEdit);

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

        protected override void OnDestroy()
        {
            try
            {
                BannerAd?.Destroy();
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
                LiveWithIcon = FindViewById<TextView>(Resource.Id.IconLiveWith);
                EdtLiveWith = FindViewById<EditText>(Resource.Id.LiveWithEditText);

                CarIcon = FindViewById<TextView>(Resource.Id.IconCar);
                EdtCar = FindViewById<EditText>(Resource.Id.CarEditText);

                ReligionIcon = FindViewById<TextView>(Resource.Id.IconReligion);
                EdtReligion = FindViewById<EditText>(Resource.Id.ReligionEditText);

                SmokeIcon = FindViewById<TextView>(Resource.Id.IconSmoke);
                EdtSmoke = FindViewById<EditText>(Resource.Id.SmokeEditText);

                DrinkIcon = FindViewById<TextView>(Resource.Id.IconDrink);
                EdtDrink = FindViewById<EditText>(Resource.Id.DrinkEditText);

                TravelIcon = FindViewById<TextView>(Resource.Id.IconTravel);
                EdtTravel = FindViewById<EditText>(Resource.Id.TravelEditText);

                BtnSave = FindViewById<AppCompatButton>(Resource.Id.ApplyButton);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, LiveWithIcon, FontAwesomeIcon.GlobeAmericas);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, CarIcon, FontAwesomeIcon.Car);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, ReligionIcon, FontAwesomeIcon.Church);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, SmokeIcon, FontAwesomeIcon.Smoking);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, DrinkIcon, FontAwesomeIcon.Beer);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, TravelIcon, FontAwesomeIcon.PlaneDeparture);

                Methods.SetColorEditText(EdtLiveWith, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtCar, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtReligion, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtSmoke, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtDrink, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtTravel, QuickDateTools.IsTabDark() ? Color.White : Color.Black);

                Methods.SetFocusable(EdtLiveWith);
                Methods.SetFocusable(EdtCar);
                Methods.SetFocusable(EdtReligion);
                Methods.SetFocusable(EdtSmoke);
                Methods.SetFocusable(EdtDrink);
                Methods.SetFocusable(EdtTravel);

                LinearLayout adContainer = FindViewById<LinearLayout>(Resource.Id.bannerContainer);
                if (AppSettings.ShowFbBannerAds)
                    BannerAd = AdsFacebook.InitAdView(this, adContainer, null);
                else if (AppSettings.ShowAppLovinBannerAds)
                    AdsAppLovin.InitBannerAd(this, adContainer, null);
                else
                    AdsGoogle.InitBannerAdView(this, adContainer, null);
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
                    toolbar.Title = GetString(Resource.String.Lbl_LifeStyleInfo);
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
                    EdtLiveWith.Touch += EdtLiveWithOnClick;
                    EdtCar.Touch += EdtCarOnClick;
                    EdtReligion.Touch += EdtReligionOnClick;
                    EdtSmoke.Touch += EdtSmokeOnClick;
                    EdtDrink.Touch += EdtDrinkOnClick;
                    EdtTravel.Touch += EdtTravelOnClick;
                }
                else
                {
                    BtnSave.Click -= BtnSaveOnClick;
                    EdtLiveWith.Touch -= EdtLiveWithOnClick;
                    EdtCar.Touch -= EdtCarOnClick;
                    EdtReligion.Touch -= EdtReligionOnClick;
                    EdtSmoke.Touch -= EdtSmokeOnClick;
                    EdtDrink.Touch -= EdtDrinkOnClick;
                    EdtTravel.Touch -= EdtTravelOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        //Travel
        private void EdtTravelOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(this);
                TypeDialog = "Travel";
                //string[] travelArray = Application.Context.Resources.GetStringArray(Resource.Array.TravelArray);
                var travelArray = ListUtils.SettingsSiteList?.Travel;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                if (travelArray != null) arrayAdapter.AddRange(travelArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.SetTitle(GetText(Resource.String.Lbl_Travel));
                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Drink
        private void EdtDrinkOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(this);
                TypeDialog = "Drink";
                //string[] drinkArray = Application.Context.Resources.GetStringArray(Resource.Array.DrinkArray);
                var drinkArray = ListUtils.SettingsSiteList?.Drink;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                if (drinkArray != null) arrayAdapter.AddRange(drinkArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.SetTitle(GetText(Resource.String.Lbl_Drink));
                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Smoke
        private void EdtSmokeOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(this);
                TypeDialog = "Smoke";
                //string[] smokeArray = Application.Context.Resources.GetStringArray(Resource.Array.SmokeArray);
                var smokeArray = ListUtils.SettingsSiteList?.Smoke;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                if (smokeArray != null) arrayAdapter.AddRange(smokeArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.SetTitle(GetText(Resource.String.Lbl_Smoke));
                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Religion
        private void EdtReligionOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(this);
                TypeDialog = "Religion";
                //string[] religionArray = Application.Context.Resources.GetStringArray(Resource.Array.ReligionArray);
                var religionArray = ListUtils.SettingsSiteList?.Religion;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                if (religionArray != null) arrayAdapter.AddRange(religionArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.SetTitle(GetText(Resource.String.Lbl_Religion));
                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Car
        private void EdtCarOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(this);
                TypeDialog = "Car";
                //string[] carArray = Application.Context.Resources.GetStringArray(Resource.Array.CarArray);
                var carArray = ListUtils.SettingsSiteList?.Car;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                if (carArray != null) arrayAdapter.AddRange(carArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.SetTitle(GetString(Resource.String.Lbl_Car));
                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //LiveWith
        private void EdtLiveWithOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(this);
                TypeDialog = "LiveWith";
                //string[] liveWithArray = Application.Context.Resources.GetStringArray(Resource.Array.LiveWithArray);
                var liveWithArray = ListUtils.SettingsSiteList?.LiveWith;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                if (liveWithArray != null) arrayAdapter.AddRange(liveWithArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.SetTitle(GetText(Resource.String.Lbl_LiveWith));
                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

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
                        {"live_with", IdLiveWith.ToString()},
                        {"car", IdCar.ToString()},
                        {"religion", IdReligion.ToString()},
                        {"smoke",IdSmoke.ToString()},
                        {"drink",IdDrink.ToString()},
                        {"travel",IdTravel.ToString()},
                    };

                    var (apiStatus, respond) = await RequestsAsync.Users.UpdateProfileAsync(dictionary);
                    if (apiStatus == 200)
                    {
                        if (respond is UpdateProfileObject result)
                        {
                            var local = ListUtils.MyUserInfo?.FirstOrDefault();
                            if (local != null)
                            {
                                local.LiveWith = IdLiveWith.ToString();
                                local.Car = IdCar.ToString();
                                local.Religion = IdReligion.ToString();
                                local.Smoke = IdSmoke.ToString();
                                local.Drink = IdDrink.ToString();
                                local.Travel = IdTravel.ToString();

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
                    string liveWith = QuickDateTools.GetLiveWith(dataUser.LiveWith);
                    if (!string.IsNullOrEmpty(liveWith))
                    {
                        IdLiveWith = Convert.ToInt32(dataUser.LiveWith);
                        EdtLiveWith.Text = liveWith;
                    }

                    string car = QuickDateTools.GetCar(dataUser.Car);
                    if (!string.IsNullOrEmpty(car))
                    {
                        IdCar = Convert.ToInt32(dataUser.Car);
                        EdtCar.Text = car;
                    }

                    string religion = QuickDateTools.GetReligion(dataUser.Religion);
                    if (!string.IsNullOrEmpty(religion))
                    {
                        IdReligion = Convert.ToInt32(dataUser.Religion);
                        EdtReligion.Text = religion;
                    }

                    string smoke = QuickDateTools.GetSmoke(dataUser.Smoke);
                    if (!string.IsNullOrEmpty(smoke))
                    {
                        IdSmoke = Convert.ToInt32(dataUser.Smoke);
                        EdtSmoke.Text = smoke;
                    }

                    string drink = QuickDateTools.GetDrink(dataUser.Drink);
                    if (!string.IsNullOrEmpty(drink))
                    {
                        IdDrink = Convert.ToInt32(dataUser.Drink);
                        EdtDrink.Text = drink;
                    }

                    string travel = QuickDateTools.GetTravel(dataUser.Travel);
                    if (!string.IsNullOrEmpty(travel))
                    {
                        IdTravel = Convert.ToInt32(dataUser.Travel);
                        EdtTravel.Text = travel;
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
                switch (TypeDialog)
                {
                    case "LiveWith":
                        {
                            var liveWithArray = ListUtils.SettingsSiteList?.LiveWith?.FirstOrDefault(a => a.ContainsValue(itemString))?.Keys.FirstOrDefault();
                            IdLiveWith = int.Parse(liveWithArray ?? "1");
                            EdtLiveWith.Text = itemString;
                            break;
                        }
                    case "Car":
                        {
                            var carArray = ListUtils.SettingsSiteList?.Car?.FirstOrDefault(a => a.ContainsValue(itemString))?.Keys.FirstOrDefault();
                            IdCar = int.Parse(carArray ?? "1");
                            EdtCar.Text = itemString;
                            break;
                        }
                    case "Religion":
                        {
                            var religionArray = ListUtils.SettingsSiteList?.Religion?.FirstOrDefault(a => a.ContainsValue(itemString))?.Keys.FirstOrDefault();
                            IdReligion = int.Parse(religionArray ?? "1");
                            EdtReligion.Text = itemString;
                            break;
                        }
                    case "Smoke":
                        {
                            var smokeArray = ListUtils.SettingsSiteList?.Smoke?.FirstOrDefault(a => a.ContainsValue(itemString))?.Keys.FirstOrDefault();
                            IdSmoke = int.Parse(smokeArray ?? "1");
                            EdtSmoke.Text = itemString;
                            break;
                        }
                    case "Drink":
                        {
                            var drinkArray = ListUtils.SettingsSiteList?.Drink?.FirstOrDefault(a => a.ContainsValue(itemString))?.Keys.FirstOrDefault();
                            IdDrink = int.Parse(drinkArray ?? "1");
                            EdtDrink.Text = itemString;
                            break;
                        }
                    case "Travel":
                        {
                            var travelArray = ListUtils.SettingsSiteList?.Travel?.FirstOrDefault(a => a.ContainsValue(itemString))?.Keys.FirstOrDefault();
                            IdTravel = int.Parse(travelArray ?? "1");
                            EdtTravel.Text = itemString;
                            break;
                        }
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