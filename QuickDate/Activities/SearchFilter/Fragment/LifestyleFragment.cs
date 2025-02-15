using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
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
    public class LifestyleFragment : AndroidX.Fragment.App.Fragment, IDialogListCallBack
    {
        #region  Variables Basic

        private SearchFilterTabbedActivity GlobalContext;

        private EditText EdtRelationship, EdtSmoke, EdtDrink;
        private AppCompatButton ButtonApply;
        private TextView ResetTextView;
        private string TypeDialog;
        public int IdRelationShip, IdSmoke, IdDrink;

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
                View view = inflater.Inflate(Resource.Layout.FilterLifestyleLayout, container, false);
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
                AddOrRemoveEvent(true);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);

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

        #endregion

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                EdtRelationship = view.FindViewById<EditText>(Resource.Id.RelationshipEditText);
                EdtSmoke = view.FindViewById<EditText>(Resource.Id.SmokeEditText);
                EdtDrink = view.FindViewById<EditText>(Resource.Id.DrinkEditText);

                ResetTextView = view.FindViewById<TextView>(Resource.Id.Resetbutton);
                ResetTextView.Visibility = AppSettings.ShowResetFilterForAllPages ? ViewStates.Visible : ViewStates.Gone;
                ButtonApply = view.FindViewById<AppCompatButton>(Resource.Id.ApplyButton);

                Methods.SetColorEditText(EdtRelationship, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtSmoke, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtDrink, QuickDateTools.IsTabDark() ? Color.White : Color.Black);

                Methods.SetFocusable(EdtRelationship);
                Methods.SetFocusable(EdtSmoke);
                Methods.SetFocusable(EdtDrink);

                var template = view.FindViewById<TemplateView>(Resource.Id.my_template);
                AdsGoogle.Ad_AdMobNative(GlobalContext, template);
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
                var relationship = ListUtils.SettingsSiteList?.Relationship?.FirstOrDefault(a => a.ContainsKey(UserDetails.RelationShip))?.Values.FirstOrDefault();
                IdRelationShip = string.IsNullOrWhiteSpace(UserDetails.RelationShip) ? 0 : int.Parse(UserDetails.RelationShip);
                EdtRelationship.Text = relationship;

                var smoke = ListUtils.SettingsSiteList?.Smoke?.FirstOrDefault(a => a.ContainsKey(UserDetails.Smoke))?.Values.FirstOrDefault();
                IdSmoke = string.IsNullOrWhiteSpace(UserDetails.Smoke) ? 0 : int.Parse(UserDetails.Smoke);
                EdtSmoke.Text = smoke;

                var drink = ListUtils.SettingsSiteList?.Drink?.FirstOrDefault(a => a.ContainsKey(UserDetails.Drink))?.Values.FirstOrDefault();
                IdDrink = string.IsNullOrWhiteSpace(UserDetails.Drink) ? 0 : int.Parse(UserDetails.Drink);
                EdtDrink.Text = drink;
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
                    EdtRelationship.Touch += EdtRelationshipOnClick;
                    EdtSmoke.Touch += EdtSmokeOnClick;
                    EdtDrink.Touch += EdtDrinkOnClick;
                    ButtonApply.Click += GlobalContext.ActionButtonOnClick;
                    ResetTextView.Click += ResetTextViewOnClick;
                }
                else
                {
                    EdtRelationship.Touch -= EdtRelationshipOnClick;
                    EdtSmoke.Touch -= EdtSmokeOnClick;
                    EdtDrink.Touch -= EdtDrinkOnClick;
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
            GlobalContext.ResetAllFilters("LifestyleTab");
        }

        //RelationShip
        private void EdtRelationshipOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(Activity);

                TypeDialog = "Relationship";
                //string[] relationshipArray = Application.Context.Resources.GetStringArray(Resource.Array.RelationShipArray);
                var relationshipArray = ListUtils.SettingsSiteList?.Relationship;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(Context);

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

        //Drink
        private void EdtDrinkOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(Activity);

                TypeDialog = "Drink";
                //string[] drinkArray = Application.Context.Resources.GetStringArray(Resource.Array.DrinkArray);
                var drinkArray = ListUtils.SettingsSiteList?.Drink;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(Context);

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

                Methods.HideKeyboard(Activity);

                TypeDialog = "Smoke";
                //string[] smokeArray = Application.Context.Resources.GetStringArray(Resource.Array.SmokeArray);
                var smokeArray = ListUtils.SettingsSiteList?.Smoke;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(Context);

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


        #endregion

        #region MaterialDialog
        public void OnSelection(IDialogInterface dialog, int position, string itemString)
        {
            try
            {
                switch (TypeDialog)
                {
                    case "Relationship":
                        {
                            var relationshipArray = ListUtils.SettingsSiteList?.Relationship?.FirstOrDefault(a => a.ContainsValue(itemString))?.Keys.FirstOrDefault();
                            IdRelationShip = int.Parse(relationshipArray ?? "1");
                            EdtRelationship.Text = itemString;
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