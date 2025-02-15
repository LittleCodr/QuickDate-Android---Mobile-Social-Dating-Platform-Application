using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Com.Google.Android.Gms.Ads.Admanager;
using Google.Android.Material.Dialog;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Exception = System.Exception;

namespace QuickDate.Activities.SearchFilter.Fragment
{
    public class FilterBackgroundFragment : AndroidX.Fragment.App.Fragment, IDialogListCallBack
    {
        #region  Variables Basic

        private SearchFilterTabbedActivity GlobalContext;

        private EditText EdtLanguage, EdtEthnicity, EdtReligion;
        private AppCompatButton ButtonApply;
        private TextView ResetTextView;
        private AdManagerAdView AdManagerAdView;
        private string TypeDialog;
        public int IdEthnicity, IdReligion;
        public string Language;

        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
            GlobalContext = SearchFilterTabbedActivity.GetInstance();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View view = inflater.Inflate(Resource.Layout.FilterBackgroundLayout, container, false);
                return view;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return null;
            }
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            try
            {
                base.OnViewCreated(view, savedInstanceState);

                InitComponent(view);
                SetLocalData();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);

            }
        }

        public override void OnResume()
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

        public override void OnPause()
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

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public override void OnDestroy()
        {
            try
            {
                AdsGoogle.LifecycleAdManagerAdView(AdManagerAdView, "Destroy");
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                EdtLanguage = view.FindViewById<EditText>(Resource.Id.LanguageEditText);
                EdtEthnicity = view.FindViewById<EditText>(Resource.Id.EthnicityEditText);
                EdtReligion = view.FindViewById<EditText>(Resource.Id.ReligionEditText);

                ResetTextView = view.FindViewById<TextView>(Resource.Id.Resetbutton);
                ResetTextView.Visibility = AppSettings.ShowResetFilterForAllPages ? ViewStates.Visible : ViewStates.Gone;
                ButtonApply = view.FindViewById<AppCompatButton>(Resource.Id.ApplyButton);

                Methods.SetColorEditText(EdtLanguage, QuickDateTools.IsTabDark() ? Color.White : Color.ParseColor("#131313"));
                Methods.SetColorEditText(EdtEthnicity, QuickDateTools.IsTabDark() ? Color.White : Color.ParseColor("#131313"));
                Methods.SetColorEditText(EdtReligion, QuickDateTools.IsTabDark() ? Color.White : Color.ParseColor("#131313"));

                Methods.SetFocusable(EdtLanguage);
                Methods.SetFocusable(EdtEthnicity);
                Methods.SetFocusable(EdtReligion);

                AdManagerAdView = view.FindViewById<AdManagerAdView>(Resource.Id.multiple_ad_sizes_view);
                AdsGoogle.InitAdManagerAdView(AdManagerAdView);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetLocalData()
        {
            try
            {
                Language = UserDetails.Language.ToLower();
                EdtLanguage.Text = Language;

                var ethnicity = ListUtils.SettingsSiteList?.Ethnicity?.FirstOrDefault(a => a.ContainsKey(UserDetails.Ethnicity))?.Values.FirstOrDefault();

                IdEthnicity = string.IsNullOrWhiteSpace(UserDetails.Ethnicity) ? 0 : int.Parse(UserDetails.Ethnicity);

                EdtEthnicity.Text = ethnicity;

                var religion = ListUtils.SettingsSiteList?.Religion?.FirstOrDefault(a => a.ContainsKey(UserDetails.Religion))?.Values.FirstOrDefault();

                IdReligion = string.IsNullOrWhiteSpace(UserDetails.Religion) ? 0 : int.Parse(UserDetails.Religion);

                EdtReligion.Text = religion;
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
                    EdtLanguage.Touch += EdtLanguageOnClick;
                    EdtEthnicity.Touch += EdtEthnicityOnClick;
                    EdtReligion.Touch += EdtReligionOnClick;
                    ButtonApply.Click += GlobalContext.ActionButtonOnClick;
                    ResetTextView.Click += ResetTextViewOnClick;
                }
                else
                {
                    EdtLanguage.Touch -= EdtLanguageOnClick;
                    EdtEthnicity.Touch -= EdtEthnicityOnClick;
                    EdtReligion.Touch -= EdtReligionOnClick;
                    ButtonApply.Click -= GlobalContext.ActionButtonOnClick;
                    ResetTextView.Click -= ResetTextViewOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        private void ResetTextViewOnClick(object sender, EventArgs e)
        {
            GlobalContext.ResetAllFilters("BackgroundTab");
        }

        //Language
        private void EdtLanguageOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(Activity);

                TypeDialog = "Language";
                //string[] languageArray = Application.Context.Resources.GetStringArray(Resource.Array.LanguageArray); 
                var languageArray = ListUtils.SettingsSiteList?.Language;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(Context);

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

        //Ethnicity
        private void EdtEthnicityOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(Activity);

                TypeDialog = "Ethnicity";
                //string[] ethnicityArray = Application.Context.Resources.GetStringArray(Resource.Array.EthnicityArray);
                var ethnicityArray = ListUtils.SettingsSiteList?.Ethnicity;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(Context);

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

        //Religion
        private void EdtReligionOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(Activity);

                TypeDialog = "Religion";
                //string[] religionArray = Application.Context.Resources.GetStringArray(Resource.Array.ReligionArray);
                var religionArray = ListUtils.SettingsSiteList?.Religion;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(Context);

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

        #endregion

        #region MaterialDialog

        public void OnSelection(IDialogInterface dialog, int position, string itemString)
        {
            try
            {
                switch (TypeDialog)
                {
                    case "Language":
                        Language = itemString.ToLower();
                        EdtLanguage.Text = itemString;
                        break;
                    case "Ethnicity":
                        {
                            var ethnicityArray = ListUtils.SettingsSiteList?.Ethnicity?.FirstOrDefault(a => a.ContainsValue(itemString))?.Keys.FirstOrDefault();
                            IdEthnicity = int.Parse(ethnicityArray ?? "1");
                            EdtEthnicity.Text = itemString;
                            break;
                        }
                    case "Religion":
                        {
                            var religionArray = ListUtils.SettingsSiteList?.Religion?.FirstOrDefault(a => a.ContainsValue(itemString))?.Keys.FirstOrDefault();
                            IdReligion = int.Parse(religionArray ?? "1");
                            EdtReligion.Text = itemString;
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