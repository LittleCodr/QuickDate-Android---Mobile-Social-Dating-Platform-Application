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
using AdView = Com.Facebook.Ads.AdView;
using Exception = System.Exception;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace QuickDate.Activities.MyProfile
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class PersonalityInfoEditActivity : BaseActivity, IDialogListCallBack
    {
        #region Variables Basic

        private TextView CharacterIcon, ChildrenIcon, FriendsIcon, PetsIcon;
        private EditText EdtCharacter, EdtChildren, EdtFriends, EdtPets;
        private AppCompatButton BtnSave;
        private string TypeDialog;
        private int IdCharacter, IdChildren, IdFriends, IdPets;
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
                SetContentView(Resource.Layout.ButtomSheetPersonalityInfoEdit);

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
                CharacterIcon = FindViewById<TextView>(Resource.Id.IconCharacter);
                EdtCharacter = FindViewById<EditText>(Resource.Id.CharacterEditText);

                ChildrenIcon = FindViewById<TextView>(Resource.Id.IconChildren);
                EdtChildren = FindViewById<EditText>(Resource.Id.ChildrenEditText);

                FriendsIcon = FindViewById<TextView>(Resource.Id.IconFriends);
                EdtFriends = FindViewById<EditText>(Resource.Id.FriendsEditText);

                PetsIcon = FindViewById<TextView>(Resource.Id.IconPets);
                EdtPets = FindViewById<EditText>(Resource.Id.PetsEditText);

                BtnSave = FindViewById<AppCompatButton>(Resource.Id.ApplyButton);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, CharacterIcon, FontAwesomeIcon.YinYang);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, ChildrenIcon, FontAwesomeIcon.Baby);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, FriendsIcon, FontAwesomeIcon.UserFriends);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, PetsIcon, FontAwesomeIcon.Cat);

                Methods.SetColorEditText(EdtCharacter, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtChildren, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtFriends, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtPets, QuickDateTools.IsTabDark() ? Color.White : Color.Black);

                Methods.SetFocusable(EdtCharacter);
                Methods.SetFocusable(EdtChildren);
                Methods.SetFocusable(EdtFriends);
                Methods.SetFocusable(EdtPets);

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
                    toolbar.Title = GetString(Resource.String.Lbl_PersonalityInfo);
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
                    EdtCharacter.Touch += EdtCharacterOnClick;
                    EdtChildren.Touch += EdtChildrenOnClick;
                    EdtFriends.Touch += EdtFriendsOnClick;
                    EdtPets.Touch += EdtPetsOnClick;
                }
                else
                {
                    BtnSave.Click -= BtnSaveOnClick;
                    EdtCharacter.Touch -= EdtCharacterOnClick;
                    EdtChildren.Touch -= EdtChildrenOnClick;
                    EdtFriends.Touch -= EdtFriendsOnClick;
                    EdtPets.Touch -= EdtPetsOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        //Pets
        private void EdtPetsOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(this);
                TypeDialog = "Pets";
                //string[] petsArray = Application.Context.Resources.GetStringArray(Resource.Array.PetsArray);
                var petsArray = ListUtils.SettingsSiteList?.Pets;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                if (petsArray != null) arrayAdapter.AddRange(petsArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.SetTitle(GetText(Resource.String.Lbl_Pets));
                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Friends
        private void EdtFriendsOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(this);
                TypeDialog = "Friends";
                //string[] friendsArray = Application.Context.Resources.GetStringArray(Resource.Array.FriendsArray);
                var friendsArray = ListUtils.SettingsSiteList?.Friends;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                if (friendsArray != null) arrayAdapter.AddRange(friendsArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.SetTitle(GetText(Resource.String.Lbl_Friends));
                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Children
        private void EdtChildrenOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(this);
                TypeDialog = "Children";
                //string[] childrenArray = Application.Context.Resources.GetStringArray(Resource.Array.ChildrenArray);
                var childrenArray = ListUtils.SettingsSiteList?.Children;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                if (childrenArray != null) arrayAdapter.AddRange(childrenArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.SetTitle(GetText(Resource.String.Lbl_Children));
                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Character
        private void EdtCharacterOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(this);
                TypeDialog = "Character";
                //string[] characterArray = Application.Context.Resources.GetStringArray(Resource.Array.CharacterArray);
                var characterArray = ListUtils.SettingsSiteList?.Character;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                if (characterArray != null) arrayAdapter.AddRange(characterArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.SetTitle(GetText(Resource.String.Lbl_Character));
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
                        {"character", IdCharacter.ToString()},
                        {"children", IdChildren.ToString()},
                        {"friends", IdFriends.ToString()},
                        {"pets",IdPets.ToString()},
                    };

                    var (apiStatus, respond) = await RequestsAsync.Users.UpdateProfileAsync(dictionary);
                    if (apiStatus == 200)
                    {
                        if (respond is UpdateProfileObject result)
                        {
                            var local = ListUtils.MyUserInfo?.FirstOrDefault();
                            if (local != null)
                            {
                                local.Character = IdCharacter.ToString();
                                local.Children = IdChildren.ToString();
                                local.Friends = IdFriends.ToString();
                                local.Pets = IdPets.ToString();

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
                        if (respond is ErrorObject error)
                        {
                            //Methods.DisplayReportResult(this, respond);
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
                    string character = QuickDateTools.GetCharacter(dataUser.Character);
                    if (!string.IsNullOrEmpty(character))
                    {
                        IdCharacter = Convert.ToInt32(dataUser.Character);
                        EdtCharacter.Text = character;
                    }

                    string children = QuickDateTools.GetChildren(dataUser.Children);
                    if (!string.IsNullOrEmpty(children))
                    {
                        IdChildren = Convert.ToInt32(dataUser.Children);
                        EdtChildren.Text = children;
                    }

                    string friends = QuickDateTools.GetFriends(dataUser.Friends);
                    if (!string.IsNullOrEmpty(friends))
                    {
                        IdFriends = Convert.ToInt32(dataUser.Friends);
                        EdtFriends.Text = friends;
                    }

                    string pets = QuickDateTools.GetPets(dataUser.Pets);
                    if (!string.IsNullOrEmpty(pets))
                    {
                        IdPets = Convert.ToInt32(dataUser.Pets);
                        EdtPets.Text = pets;
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
                    case "Character":
                        {
                            var characterArray = ListUtils.SettingsSiteList?.Character?.FirstOrDefault(a => a.ContainsValue(itemString))?.Keys.FirstOrDefault();
                            IdCharacter = int.Parse(characterArray ?? "1");
                            EdtCharacter.Text = itemString;
                            break;
                        }
                    case "Children":
                        {
                            var childrenArray = ListUtils.SettingsSiteList?.Children?.FirstOrDefault(a => a.ContainsValue(itemString))?.Keys.FirstOrDefault();
                            IdChildren = int.Parse(childrenArray ?? "1");
                            EdtChildren.Text = itemString;
                            break;
                        }
                    case "Friends":
                        {
                            var friendsArray = ListUtils.SettingsSiteList?.Friends?.FirstOrDefault(a => a.ContainsValue(itemString))?.Keys.FirstOrDefault();
                            IdFriends = int.Parse(friendsArray ?? "1");
                            EdtFriends.Text = itemString;
                            break;
                        }
                    case "Pets":
                        {
                            var petsArray = ListUtils.SettingsSiteList?.Pets?.FirstOrDefault(a => a.ContainsValue(itemString))?.Keys.FirstOrDefault();
                            IdPets = int.Parse(petsArray ?? "1");
                            EdtPets.Text = itemString;
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