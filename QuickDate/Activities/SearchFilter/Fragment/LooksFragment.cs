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
    public class LooksFragment : AndroidX.Fragment.App.Fragment, IDialogListCallBack
    {
        #region  Variables Basic

        private SearchFilterTabbedActivity GlobalContext;

        private EditText EdtBody, EdtFromHeight, EdtToHeight;
        private AppCompatButton ButtonApply;
        private TextView ResetTextView;
        private AdManagerAdView AdManagerAdView;
        private string TypeDialog;
        public int IdBody;
        public string FromHeight = UserDetails.FilterOptionFromHeight, ToHeight = UserDetails.FilterOptionToHeight;

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
                View view = inflater.Inflate(Resource.Layout.FilterLooksEditLayout, container, false);
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
                EdtBody = view.FindViewById<EditText>(Resource.Id.BodyEditText);
                EdtFromHeight = view.FindViewById<EditText>(Resource.Id.FromHeightEditText);
                EdtToHeight = view.FindViewById<EditText>(Resource.Id.ToHeightEditText);

                ResetTextView = view.FindViewById<TextView>(Resource.Id.Resetbutton);
                ResetTextView.Visibility = AppSettings.ShowResetFilterForAllPages ? ViewStates.Visible : ViewStates.Gone;
                ButtonApply = view.FindViewById<AppCompatButton>(Resource.Id.ApplyButton);

                Methods.SetColorEditText(EdtBody, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtFromHeight, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(EdtToHeight, QuickDateTools.IsTabDark() ? Color.White : Color.Black);

                Methods.SetFocusable(EdtBody);
                Methods.SetFocusable(EdtFromHeight);
                Methods.SetFocusable(EdtToHeight);

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
                IdBody = string.IsNullOrWhiteSpace(UserDetails.Body) ? 0 : int.Parse(UserDetails.Body);
                FromHeight = UserDetails.FromHeight;
                ToHeight = UserDetails.ToHeight;

                var bodyType = ListUtils.SettingsSiteList?.Body?.FirstOrDefault(a => a.ContainsKey(UserDetails.Body))?.Values.FirstOrDefault();
                EdtBody.Text = bodyType;
                EdtFromHeight.Text = FromHeight;
                EdtToHeight.Text = ToHeight;
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
                    EdtBody.Touch += EdtBodyOnClick;
                    EdtFromHeight.Touch += EdtHeightOnClick;
                    EdtToHeight.Touch += EdtToHeightOnClick;
                    ButtonApply.Click += GlobalContext.ActionButtonOnClick;
                    ResetTextView.Click += ResetTextViewOnClick;
                }
                else
                {
                    EdtBody.Touch -= EdtBodyOnClick;
                    EdtFromHeight.Touch -= EdtHeightOnClick;
                    EdtToHeight.Touch -= EdtToHeightOnClick;
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
            GlobalContext.ResetAllFilters("LooksTab");
        }

        //Body
        private void EdtBodyOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(Activity);

                TypeDialog = "Body";
                //string[] bodyArray = Application.Context.Resources.GetStringArray(Resource.Array.BodyArray);
                var bodyArray = ListUtils.SettingsSiteList?.Body;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(Context);

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

        private void EdtHeightOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(Activity);

                TypeDialog = "FromHeight";
                //string[] heightArray = Application.Context.Resources.GetStringArray(Resource.Array.HeightArray);
                var heightArray = ListUtils.SettingsSiteList?.Height;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(Context);

                if (heightArray != null) arrayAdapter.AddRange(heightArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.SetTitle(GetText(Resource.String.Lbl_FromHeight));
                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void EdtToHeightOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(Activity);

                TypeDialog = "ToHeight";
                //string[] heightArray = Application.Context.Resources.GetStringArray(Resource.Array.HeightArray);
                var heightArray = ListUtils.SettingsSiteList?.Height;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(Context);

                if (heightArray != null) arrayAdapter.AddRange(heightArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.SetTitle(GetText(Resource.String.Lbl_ToHeight));
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
                    case "Body":
                        {
                            var bodyArray = ListUtils.SettingsSiteList?.Body?.FirstOrDefault(a => a.ContainsValue(itemString))?.Keys.FirstOrDefault();
                            IdBody = int.Parse(bodyArray ?? "1");
                            EdtBody.Text = itemString;
                            break;
                        }
                    case "FromHeight":
                        FromHeight = ListUtils.SettingsSiteList?.Height?[position]?.Keys.FirstOrDefault() ?? UserDetails.FilterOptionFromHeight;
                        EdtFromHeight.Text = itemString;
                        break;
                    case "ToHeight":
                        ToHeight = ListUtils.SettingsSiteList?.Height?[position]?.Keys.FirstOrDefault() ?? UserDetails.FilterOptionToHeight;
                        EdtToHeight.Text = itemString;
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