using Android;
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
using AndroidX.Core.Content;
using Com.Facebook.Ads;
using Com.Google.Android.Gms.Ads.Admanager;
using Google.Android.Material.Dialog;
using QuickDate.Activities.Base;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Model;
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
    public class ProfileInfoEditActivity : BaseActivity, IDialogListCallBack
    {
        #region Variables Basic

        private LinearLayout LayoutGender;
        private TextView NameIcon, GenderIcon, BirthdayIcon, LocationIcon, LanguageIcon, RelationshipIcon, WorkStatusIcon, EducationIcon;
        private EditText EdtFirstName, EdtLastName, EdtGender, EdtBirthday, EdtLocation, EdtLanguage, EdtRelationship, EdtWorkStatus, EdtEducation;
        private AppCompatButton BtnSave;
        private string TypeDialog = "";
        private int IdGender, IdRelationShip, IdWorkStatus, IdEducation;
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
                SetContentView(Resource.Layout.ButtomSheetProfileInfoEdit);

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
                NameIcon = FindViewById<TextView>(Resource.Id.IconName);
                EdtFirstName = FindViewById<EditText>(Resource.Id.FirstNameEditText);
                EdtLastName = FindViewById<EditText>(Resource.Id.LastNameEditText);

                LayoutGender = FindViewById<LinearLayout>(Resource.Id.LayoutGender);
                GenderIcon = FindViewById<TextView>(Resource.Id.IconGender);
                EdtGender = FindViewById<EditText>(Resource.Id.GenderEditText);

                BirthdayIcon = FindViewById<TextView>(Resource.Id.IconBirthday);
                EdtBirthday = FindViewById<EditText>(Resource.Id.BirthdayEditText);

                LocationIcon = FindViewById<TextView>(Resource.Id.IconLocation);
                EdtLocation = FindViewById<EditText>(Resource.Id.LocationEditText);

                LanguageIcon = FindViewById<TextView>(Resource.Id.IconLanguage);
                EdtLanguage = FindViewById<EditText>(Resource.Id.LanguageEditText);

                RelationshipIcon = FindViewById<TextView>(Resource.Id.IconRelationship);
                EdtRelationship = FindViewById<EditText>(Resource.Id.RelationshipEditText);

                WorkStatusIcon = FindViewById<TextView>(Resource.Id.IconWorkStatus);
                EdtWorkStatus = FindViewById<EditText>(Resource.Id.WorkStatusEditText);

                EducationIcon = FindViewById<TextView>(Resource.Id.IconEducation);
                EdtEducation = FindViewById<EditText>(Resource.Id.EducationEditText);

                BtnSave = FindViewById<AppCompatButton>(Resource.Id.ApplyButton);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, NameIcon, FontAwesomeIcon.User);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, GenderIcon, FontAwesomeIcon.Transgender);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, BirthdayIcon, FontAwesomeIcon.BirthdayCake);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, LocationIcon, FontAwesomeIcon.MapMarkerAlt);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, LanguageIcon, FontAwesomeIcon.Language);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, RelationshipIcon, FontAwesomeIcon.Heart);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, WorkStatusIcon, FontAwesomeIcon.Briefcase);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, EducationIcon, FontAwesomeIcon.GraduationCap);

                Methods.SetColorEditText(EdtFirstName, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtLastName, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtGender, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtBirthday, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtLocation, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtLanguage, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtRelationship, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtWorkStatus, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtEducation, QuickDateTools.IsTabDark() ? Color.White : Color.Black);

                Methods.SetFocusable(EdtGender);
                Methods.SetFocusable(EdtBirthday);
                Methods.SetFocusable(EdtLanguage);
                Methods.SetFocusable(EdtRelationship);
                Methods.SetFocusable(EdtWorkStatus);
                Methods.SetFocusable(EdtEducation);

                AdManagerAdView = FindViewById<AdManagerAdView>(Resource.Id.multiple_ad_sizes_view);
                AdsGoogle.InitAdManagerAdView(AdManagerAdView);

                if (AppSettings.UpdateGenderSystem == UpdateGenderSystem.JustWhenRegister)
                    LayoutGender.Visibility = ViewStates.Gone;

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
                    toolbar.Title = GetString(Resource.String.Lbl_EditProfileInfo);
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
                    EdtGender.Touch += EdtGenderOnTouch;
                    EdtBirthday.Touch += EdtBirthdayOnClick;
                    EdtLocation.FocusChange += EdtLocationOnFocusChange;
                    EdtLanguage.Touch += EdtLanguageOnClick;
                    EdtRelationship.Touch += EdtRelationshipOnClick;
                    EdtWorkStatus.Touch += EdtWorkStatusOnClick;
                    EdtEducation.Touch += EdtEducationOnClick;
                }
                else
                {
                    BtnSave.Click -= BtnSaveOnClick;
                    EdtGender.Touch -= EdtGenderOnTouch;
                    EdtBirthday.Touch -= EdtBirthdayOnClick;
                    EdtLocation.FocusChange -= EdtLocationOnFocusChange;
                    EdtLanguage.Touch -= EdtLanguageOnClick;
                    EdtRelationship.Touch -= EdtRelationshipOnClick;
                    EdtWorkStatus.Touch -= EdtWorkStatusOnClick;
                    EdtEducation.Touch -= EdtEducationOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }


        #endregion

        #region Events

        private void EdtGenderOnTouch(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(this);
                TypeDialog = "Gender";
                var genderArray = ListUtils.SettingsSiteList?.Gender;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                if (genderArray != null) arrayAdapter.AddRange(genderArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.SetTitle(GetText(Resource.String.Lbl_Gender));
                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Education
        private void EdtEducationOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(this);
                TypeDialog = "Education";
                //string[] educationArray = Application.Context.Resources.GetStringArray(Resource.Array.EducationArray);
                var educationArray = ListUtils.SettingsSiteList?.Education;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                if (educationArray != null) arrayAdapter.AddRange(educationArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.SetTitle(GetString(Resource.String.Lbl_EducationLevel));
                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //WorkStatus
        private void EdtWorkStatusOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(this);
                TypeDialog = "WorkStatus";
                //string[] workStatusArray = Application.Context.Resources.GetStringArray(Resource.Array.WorkStatusArray);
                var workStatusArray = ListUtils.SettingsSiteList?.WorkStatus;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                if (workStatusArray != null) arrayAdapter.AddRange(workStatusArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.SetTitle(GetText(Resource.String.Lbl_ChooseWorkStatus));
                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Open DatePicker And Get Short Date 
        private void EdtBirthdayOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(this);
                var frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                {
                    try
                    {
                        if (AppSettings.IsUserYearsOld) // 18
                        {
                            if (!Methods.Time.HasAgeRequirement(time.Date)) // over 18 years
                            {
                                EdtBirthday.Text = time.Date.ToString("dd-MM-yyyy");
                            }
                            else
                            {
                                Toast.MakeText(this, GetText(Resource.String.Lbl_Error_IsUserYearsOld), ToastLength.Short)?.Show();
                            }
                        }
                        else //All
                        {
                            EdtBirthday.Text = time.Date.ToString("dd-MM-yyyy");
                        }
                    }
                    catch (Exception exception)
                    {
                        Methods.DisplayReportResultTrack(exception);
                    }
                }, "Birthday");
                frag.Show(SupportFragmentManager, DatePickerFragment.Tag);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //RelationShip
        private void EdtRelationshipOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(this);
                TypeDialog = "Relationship";
                //string[] relationshipArray = Application.Context.Resources.GetStringArray(Resource.Array.RelationShipArray);
                var relationshipArray = ListUtils.SettingsSiteList?.Relationship;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                if (relationshipArray != null) arrayAdapter.AddRange(relationshipArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.SetTitle(GetText(Resource.String.Lbl_ChooseRelationshipStatus));
                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Language
        private void EdtLanguageOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(this);
                TypeDialog = "Language";
                //string[] languageArray = Application.Context.Resources.GetStringArray(Resource.Array.LanguageArray); 
                var languageArray = ListUtils.SettingsSiteList?.Language;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                if (languageArray != null) arrayAdapter.AddRange(languageArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.SetTitle(GetText(Resource.String.Lbl_ChooseLanguage));
                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Get Location
        private void EdtLocationOnFocusChange(object sender, View.FocusChangeEventArgs e)
        {
            try
            {
                if (e.HasFocus)
                {
                    // Check if we're running on Android 5.0 or higher
                    if ((int)Build.VERSION.SdkInt < 23)
                    {
                        // result Code 502
                        new IntentController(this).OpenIntentLocation();
                    }
                    else
                    {
                        if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted && ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) == Permission.Granted)
                            new IntentController(this).OpenIntentLocation();
                        else
                            new PermissionsController(this).RequestPermission(105);
                    }
                }
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
                        {"first_name", EdtFirstName.Text},
                        {"last_name", EdtLastName.Text},
                        {"birthday", EdtBirthday.Text},
                        {"location", EdtLocation.Text},
                        {"language", EdtLanguage.Text?.ToLower()},
                        {"relationship", IdRelationShip.ToString()},
                        {"work_status", IdWorkStatus.ToString()},
                        {"education", IdEducation.ToString()},
                    };

                    if (AppSettings.UpdateGenderSystem == UpdateGenderSystem.All)
                    {
                        dictionary.Add("gender", IdGender.ToString());
                    }

                    var (apiStatus, respond) = await RequestsAsync.Users.UpdateProfileAsync(dictionary);
                    if (apiStatus == 200)
                    {
                        if (respond is UpdateProfileObject result)
                        {
                            Console.WriteLine(result.Message);
                            var local = ListUtils.MyUserInfo?.FirstOrDefault();
                            if (local != null)
                            {
                                local.FirstName = EdtFirstName.Text;
                                local.LastName = EdtLastName.Text;
                                local.Gender = IdGender.ToString();
                                local.Birthday = EdtBirthday.Text;
                                local.Address = EdtLocation.Text;
                                local.Language = EdtLanguage.Text;
                                local.Relationship = IdRelationShip.ToString();
                                local.WorkStatus = IdWorkStatus.ToString();
                                local.Education = IdEducation.ToString();

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
                            AndHUD.Shared.ShowError(this, error.ErrorData.ErrorText, MaskType.Clear, TimeSpan.FromSeconds(2));
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

        //Back
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
                    EdtFirstName.Text = dataUser.FirstName;
                    EdtLastName.Text = dataUser.LastName;

                    if (AppSettings.UpdateGenderSystem == UpdateGenderSystem.All)
                    {
                        EdtGender.Text = ListUtils.SettingsSiteList?.Gender?.FirstOrDefault(a => a.ContainsKey(dataUser.Gender))?.Values.FirstOrDefault() ?? dataUser.GenderTxt;
                        IdGender = Convert.ToInt32(dataUser.Gender);
                    }

                    EdtBirthday.Text = dataUser.Birthday;

                    if (!string.IsNullOrEmpty(dataUser.Location))
                    {
                        EdtLocation.Text = dataUser.Location;
                    }

                    if (!string.IsNullOrEmpty(dataUser.Language))
                    {
                        EdtLanguage.Text = dataUser.Language;
                    }

                    string relationship = QuickDateTools.GetRelationship(dataUser.Relationship);
                    if (!string.IsNullOrEmpty(relationship))
                    {
                        EdtRelationship.Text = relationship;
                        IdRelationShip = Convert.ToInt32(dataUser.Relationship);
                    }

                    string work = QuickDateTools.GetWorkStatus(dataUser.WorkStatus);
                    if (!string.IsNullOrEmpty(work))
                    {
                        EdtWorkStatus.Text = work;
                        IdWorkStatus = Convert.ToInt32(dataUser.WorkStatus);
                    }

                    string education = QuickDateTools.GetEducation(dataUser.Education);
                    if (!string.IsNullOrEmpty(education))
                    {
                        EdtEducation.Text = education;
                        IdEducation = Convert.ToInt32(dataUser.Education);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #region Permissions && Result

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                if (requestCode == 502 && resultCode == Result.Ok) // Location
                {
                    var placeAddress = data.GetStringExtra("Address") ?? "";
                    var placeLatLng = data.GetStringExtra("latLng") ?? "";
                    if (!string.IsNullOrEmpty(placeAddress))
                        EdtLocation.Text = placeAddress;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 105)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        new IntentController(this).OpenIntentLocation();
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long)?.Show();
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

        public void OnSelection(IDialogInterface dialog, int position, string itemString)
        {
            try
            {
                switch (TypeDialog)
                {
                    case "Language":
                        EdtLanguage.Text = itemString;
                        break;
                    case "Relationship":
                        {
                            var relationshipArray = ListUtils.SettingsSiteList?.Relationship?.FirstOrDefault(a => a.ContainsValue(itemString))?.Keys.FirstOrDefault();
                            IdRelationShip = int.Parse(relationshipArray ?? "1");
                            EdtRelationship.Text = itemString;
                            break;
                        }
                    case "WorkStatus":
                        {
                            var workStatusArray = ListUtils.SettingsSiteList?.WorkStatus?.FirstOrDefault(a => a.ContainsValue(itemString))?.Keys.FirstOrDefault();
                            IdWorkStatus = int.Parse(workStatusArray ?? "1");
                            EdtWorkStatus.Text = itemString;
                            break;
                        }
                    case "Education":
                        {
                            var educationArray = ListUtils.SettingsSiteList?.Education?.FirstOrDefault(a => a.ContainsValue(itemString))?.Keys.FirstOrDefault();
                            IdEducation = int.Parse(educationArray ?? "1");
                            EdtEducation.Text = itemString;
                            break;
                        }
                    case "Gender":
                        {
                            var genderArray = ListUtils.SettingsSiteList?.Gender?.FirstOrDefault(a => a.ContainsValue(itemString))?.Keys.FirstOrDefault();
                            IdGender = int.Parse(genderArray ?? UserDetails.FilterOptionGenderMale);
                            EdtGender.Text = itemString;
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