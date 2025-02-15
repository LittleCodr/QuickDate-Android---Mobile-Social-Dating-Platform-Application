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
    public class LooksEditActivity : BaseActivity, IDialogListCallBack
    {
        #region Variables Basic

        private TextView EthnicityIcon, BodyIcon, HeightIcon, HairColorIcon;
        private EditText EdtEthnicity, EdtBody, EdtHeight, EdtHairColor;
        private AppCompatButton BtnSave;
        private string TypeDialog, IdHeight;
        private int IdEthnicity, IdBody, IdHairColor;
        private AdView BannerAd;
        private InterstitialAd InterstitialAd;
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
                SetContentView(Resource.Layout.ButtomSheetLooksEdit);

                //Get Value And Set Toolbar
                InitComponent();
                InitBackground();
                InitToolbar();

                GetMyInfoData();

                if (AppSettings.ShowFbInterstitialAds)
                    InterstitialAd = AdsFacebook.InitInterstitial(this);
                else if (AppSettings.ShowAppLovinInterstitialAds)
                    AdsAppLovin.Ad_Interstitial(this);
                else
                    AdsGoogle.Ad_Interstitial(this);
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
                InterstitialAd?.Destroy();
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
                EthnicityIcon = FindViewById<TextView>(Resource.Id.IconEthnicity);
                EdtEthnicity = FindViewById<EditText>(Resource.Id.EthnicityEditText);

                BodyIcon = FindViewById<TextView>(Resource.Id.IconBody);
                EdtBody = FindViewById<EditText>(Resource.Id.BodyEditText);

                HeightIcon = FindViewById<TextView>(Resource.Id.IconHeight);
                EdtHeight = FindViewById<EditText>(Resource.Id.HeightEditText);

                HairColorIcon = FindViewById<TextView>(Resource.Id.IconHairColor);
                EdtHairColor = FindViewById<EditText>(Resource.Id.HairColorEditText);

                BtnSave = FindViewById<AppCompatButton>(Resource.Id.ApplyButton);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, EthnicityIcon, FontAwesomeIcon.TheaterMasks);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, BodyIcon, FontAwesomeIcon.Male);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, HeightIcon, FontAwesomeIcon.TextHeight);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, HairColorIcon, FontAwesomeIcon.Cannabis);

                Methods.SetColorEditText(EdtEthnicity, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtBody, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtHeight, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtHairColor, QuickDateTools.IsTabDark() ? Color.White : Color.Black);

                Methods.SetFocusable(EdtEthnicity);
                Methods.SetFocusable(EdtBody);
                Methods.SetFocusable(EdtHairColor);
                Methods.SetFocusable(EdtHeight);

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
                    toolbar.Title = GetString(Resource.String.Lbl_EditLooksInfo);
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
                    EdtEthnicity.Touch += EdtEthnicityOnClick;
                    EdtBody.Touch += EdtBodyOnClick;
                    EdtHairColor.Touch += EdtHairColorOnClick;
                    EdtHeight.Touch += EdtHeightOnClick;
                }
                else
                {
                    BtnSave.Click -= BtnSaveOnClick;
                    EdtEthnicity.Touch -= EdtEthnicityOnClick;
                    EdtBody.Touch -= EdtBodyOnClick;
                    EdtHairColor.Touch -= EdtHairColorOnClick;
                    EdtHeight.Touch -= EdtHeightOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        //HairColor
        private void EdtHairColorOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(this);
                TypeDialog = "HairColor";
                //string[] hairColorArray = Application.Context.Resources.GetStringArray(Resource.Array.HairColorArray);
                var hairColorArray = ListUtils.SettingsSiteList?.HairColor;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                if (hairColorArray != null) arrayAdapter.AddRange(hairColorArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault()?.Replace("&prime;", "′").Replace("&Prime;", "″"))));

                dialogList.SetTitle(GetText(Resource.String.Lbl_ChooseHairColor));
                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Body
        private void EdtBodyOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(this);
                TypeDialog = "Body";
                //string[] bodyArray = Application.Context.Resources.GetStringArray(Resource.Array.BodyArray);
                var bodyArray = ListUtils.SettingsSiteList?.Body;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                if (bodyArray != null) arrayAdapter.AddRange(bodyArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.SetTitle(GetText(Resource.String.Lbl_BodyType));
                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Ethnicity
        private void EdtEthnicityOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(this);
                TypeDialog = "Ethnicity";
                //string[] ethnicityArray = Application.Context.Resources.GetStringArray(Resource.Array.EthnicityArray);
                var ethnicityArray = ListUtils.SettingsSiteList?.Ethnicity;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                if (ethnicityArray != null) arrayAdapter.AddRange(ethnicityArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.SetTitle(GetText(Resource.String.Lbl_BodyType));
                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void EdtHeightOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(this);
                TypeDialog = "Height";
                //string[] heightArray = Application.Context.Resources.GetStringArray(Resource.Array.HeightArray);
                var heightArray = ListUtils.SettingsSiteList?.Height;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                if (heightArray != null) arrayAdapter.AddRange(heightArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.SetTitle(GetText(Resource.String.Lbl_Height));
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
                        {"ethnicity", IdEthnicity.ToString()},
                        {"body", IdBody.ToString()},
                        {"height", IdHeight},
                        {"hair_color",IdHairColor.ToString()},
                    };

                    var (apiStatus, respond) = await RequestsAsync.Users.UpdateProfileAsync(dictionary);
                    if (apiStatus == 200)
                    {
                        if (respond is UpdateProfileObject result)
                        {
                            var local = ListUtils.MyUserInfo?.FirstOrDefault();
                            if (local != null)
                            {
                                local.Ethnicity = IdEthnicity.ToString();
                                local.Body = IdBody.ToString();
                                local.Height = EdtHeight.Text.Replace("cm", "").Replace(" ", "");
                                local.HairColor = IdHairColor.ToString();

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
                    string ethnicity = QuickDateTools.GetEthnicity(dataUser.Ethnicity);
                    if (!string.IsNullOrEmpty(ethnicity))
                    {
                        EdtEthnicity.Text = ethnicity;
                        IdEthnicity = Convert.ToInt32(dataUser.Ethnicity);
                    }

                    string body = QuickDateTools.GetBody(dataUser.Body);
                    if (!string.IsNullOrEmpty(body))
                    {
                        EdtBody.Text = body;
                        IdBody = Convert.ToInt32(dataUser.Body);
                    }

                    EdtHeight.Text = dataUser.Height + " " + GetText(Resource.String.Lbl_Cm);

                    string hairColor = QuickDateTools.GetHairColor(dataUser.HairColor);
                    if (!string.IsNullOrEmpty(hairColor))
                    {
                        EdtHairColor.Text = hairColor;
                        IdHairColor = Convert.ToInt32(dataUser.HairColor);
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
                    case "Ethnicity":
                        {
                            var ethnicityArray = ListUtils.SettingsSiteList?.Ethnicity?.FirstOrDefault(a => a.ContainsValue(itemString))?.Keys.FirstOrDefault();
                            IdEthnicity = int.Parse(ethnicityArray ?? "1");
                            EdtEthnicity.Text = itemString;
                            break;
                        }
                    case "Body":
                        {
                            var bodyArray = ListUtils.SettingsSiteList?.Body?.FirstOrDefault(a => a.ContainsValue(itemString))?.Keys.FirstOrDefault();
                            IdBody = int.Parse(bodyArray ?? "1");
                            EdtBody.Text = itemString;
                            break;
                        }
                    case "HairColor":
                        {
                            var hairColorArray = ListUtils.SettingsSiteList?.HairColor?.FirstOrDefault(a => a.ContainsValue(itemString))?.Keys.FirstOrDefault();
                            IdHairColor = int.Parse(hairColorArray ?? "1");
                            EdtHairColor.Text = itemString;
                            break;
                        }
                    case "Height":
                        IdHeight = ListUtils.SettingsSiteList?.Height?[position]?.Keys.FirstOrDefault();
                        EdtHeight.Text = itemString;
                        break;
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