using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Text;
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
    public class MoreFragment : AndroidX.Fragment.App.Fragment, IDialogInputCallBack, IDialogListCallBack
    {
        #region  Variables Basic

        private SearchFilterTabbedActivity GlobalContext;

        private EditText EdtInterest, EdtEducation, EdtPets;
        private AppCompatButton ButtonApply;
        private TextView ResetTextView;
        private AdManagerAdView AdManagerAdView;
        private string TypeDialog;
        public string Interest;
        public int IdEducation, IdPets;

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
                View view = inflater.Inflate(Resource.Layout.FilterMoreLayout, container, false);
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
                EdtInterest = view.FindViewById<EditText>(Resource.Id.InterestEditText);
                EdtEducation = view.FindViewById<EditText>(Resource.Id.EducationEditText);
                EdtPets = view.FindViewById<EditText>(Resource.Id.PetsEditText);

                ResetTextView = view.FindViewById<TextView>(Resource.Id.Resetbutton);
                ResetTextView.Visibility = AppSettings.ShowResetFilterForAllPages ? ViewStates.Visible : ViewStates.Gone;
                ButtonApply = view.FindViewById<AppCompatButton>(Resource.Id.ApplyButton);

                Methods.SetColorEditText(EdtInterest, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtEducation, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtPets, QuickDateTools.IsTabDark() ? Color.White : Color.Black);

                Methods.SetFocusable(EdtInterest);
                Methods.SetFocusable(EdtEducation);
                Methods.SetFocusable(EdtPets);

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
                var education = ListUtils.SettingsSiteList?.Education?.FirstOrDefault(a => a.ContainsKey(UserDetails.Education))?.Values.FirstOrDefault();
                IdEducation = string.IsNullOrWhiteSpace(UserDetails.Education) ? 0 : int.Parse(UserDetails.Education);
                EdtEducation.Text = education;

                var pets = ListUtils.SettingsSiteList?.Pets?.FirstOrDefault(a => a.ContainsKey(UserDetails.Pets))?.Values.FirstOrDefault();
                IdPets = string.IsNullOrWhiteSpace(UserDetails.Pets) ? 0 : int.Parse(UserDetails.Pets);
                EdtPets.Text = pets;
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
                    EdtInterest.Touch += EdtInterestOnClick;
                    EdtEducation.Touch += EdtEducationOnClick;
                    EdtPets.Touch += EdtPetsOnClick;
                    ButtonApply.Click += GlobalContext.ActionButtonOnClick;
                    ResetTextView.Click += ResetTextViewOnClick;
                }
                else
                {
                    EdtInterest.Touch -= EdtInterestOnClick;
                    EdtEducation.Touch -= EdtEducationOnClick;
                    EdtPets.Touch -= EdtPetsOnClick;
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
            GlobalContext.ResetAllFilters("MoreTab");
        }

        //Interest
        private void EdtInterestOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(Activity);

                TypeDialog = "Interest";
                var dialog = new MaterialAlertDialogBuilder(Context);
                dialog.SetTitle(GetString(Resource.String.Lbl_Interest));

                EditText input = new EditText(Context);
                input.SetHint(Resource.String.Lbl_EnterTextInterest);
                input.InputType = InputTypes.TextFlagImeMultiLine;
                LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                input.LayoutParameters = lp;

                dialog.SetView(input);

                dialog.SetPositiveButton(GetText(Resource.String.Lbl_Submit), new MaterialDialogUtils(input, this));
                dialog.SetNegativeButton(GetText(Resource.String.Lbl_Cancel), new MaterialDialogUtils());

                dialog.Show();
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

                Methods.HideKeyboard(Activity);

                TypeDialog = "Education";
                //string[] educationArray = Application.Context.Resources.GetStringArray(Resource.Array.EducationArray);
                var educationArray = ListUtils.SettingsSiteList?.Education;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(Context);

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

        //Pets
        private void EdtPetsOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(Activity);

                TypeDialog = "Pets";
                //string[] petsArray = Application.Context.Resources.GetStringArray(Resource.Array.PetsArray);
                var petsArray = ListUtils.SettingsSiteList?.Pets;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(Context);

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


        #endregion

        #region MaterialDialog
        public void OnSelection(IDialogInterface dialog, int position, string itemString)
        {
            try
            {
                switch (TypeDialog)
                {
                    case "Education":
                        {
                            var educationArray = ListUtils.SettingsSiteList?.Education?.FirstOrDefault(a => a.ContainsValue(itemString))?.Keys.FirstOrDefault();
                            IdEducation = int.Parse(educationArray ?? "1");
                            EdtEducation.Text = itemString;
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

        public void OnInput(IDialogInterface dialog, string input)
        {
            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    if (input.Length <= 0) return;

                    Interest = input;
                    EdtInterest.Text = input;
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