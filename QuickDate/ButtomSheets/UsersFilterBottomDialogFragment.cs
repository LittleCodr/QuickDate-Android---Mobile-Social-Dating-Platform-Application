using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.BottomSheet;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickDate.ButtomSheets
{
    public class UsersFilterBottomDialogFragment : BottomSheetDialogFragment
    {
        #region Variables Basic

        private TextView Title, BirthdayTextView, BirthdayValue, GenderTextView, OnlineTextView, ResetTextView;
        private ImageView IconBack;
        private RelativeLayout BirthdayLayout;
        private AppCompatButton ButtonMan, ButtonGirls, ButtonBoth, ButtonApply;
        private Switch OnlineSwitch;

        private string Gender = UserDetails.FilterOptionGenderMale, Birthday = "";
        private bool SwitchState = true;
        private HomeActivity GlobalContext;
        private string TypePage;

        #endregion

        #region General

        public UsersFilterBottomDialogFragment(string typePage)
        {
            TypePage = typePage;
        }

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
                View view = localInflater?.Inflate(Resource.Layout.ButtomSheetUsersFilter, container, false);
                return view;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            try
            {
                base.OnViewCreated(view, savedInstanceState);

                //Get Value And Set Toolbar
                InitComponent(view);

                IconBack.Click += IconBackOnClick;
                BirthdayLayout.Click += BirthdayLayoutOnClick;
                ButtonMan.Click += ButtonManOnClick;
                ButtonGirls.Click += ButtonGirlsOnClick;
                ButtonBoth.Click += ButtonBothOnClick;
                ButtonApply.Click += ButtonApplyOnClick;
                ResetTextView.Click += ResetTextViewOnClick;
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
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                Title = view.FindViewById<TextView>(Resource.Id.titlepage);
                IconBack = view.FindViewById<ImageView>(Resource.Id.IconBack);
                BirthdayTextView = view.FindViewById<TextView>(Resource.Id.BirthdayTextView);
                BirthdayValue = view.FindViewById<TextView>(Resource.Id.BirthdayValue);
                GenderTextView = view.FindViewById<TextView>(Resource.Id.GenderTextView);
                OnlineTextView = view.FindViewById<TextView>(Resource.Id.OnlineTextView);
                ResetTextView = view.FindViewById<TextView>(Resource.Id.Resetbutton);
                BirthdayLayout = view.FindViewById<RelativeLayout>(Resource.Id.LayoutBirthday);
                ButtonMan = view.FindViewById<AppCompatButton>(Resource.Id.ManButton);
                ButtonGirls = view.FindViewById<AppCompatButton>(Resource.Id.GirlsButton);
                ButtonBoth = view.FindViewById<AppCompatButton>(Resource.Id.BothButton);
                ButtonApply = view.FindViewById<AppCompatButton>(Resource.Id.ApplyButton);
                OnlineSwitch = view.FindViewById<Switch>(Resource.Id.togglebutton);

                OnlineSwitch.Checked = false;

                ButtonGirls.SetBackgroundResource(Resource.Drawable.round_button_outline_withoutstock);
                ButtonGirls.SetTextColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                ButtonBoth.SetBackgroundResource(Resource.Drawable.round_button_pressed);
                ButtonBoth.SetTextColor(Color.ParseColor("#ffffff"));

                ButtonMan.SetBackgroundResource(Resource.Drawable.round_button_outline_withoutstock);
                ButtonMan.SetTextColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                SetLocalData();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        //Save and sent data and set new search 
        private void ButtonApplyOnClick(object sender, EventArgs e)
        {
            try
            {
                // check current state of a Switch (true or false).
                SwitchState = OnlineSwitch.Checked;

                if (TypePage == "HotOrNot")
                {
                    UserDetails.HotOrNotFilterGender = Gender;
                    UserDetails.HotOrNotFilterBirthday = Birthday;
                    UserDetails.HotOrNotFilterIsOnline = SwitchState;

                    if (GlobalContext.TrendingFragment != null)
                    {
                        var checkList = GlobalContext.TrendingFragment.MAdapter?.TrendingList?.Where(q => q.Type == ItemType.HotOrNot).ToList();
                        if (checkList?.Count > 0)
                        {
                            GlobalContext.TrendingFragment.MAdapter.TrendingList.RemoveAll(checkList);
                            GlobalContext.TrendingFragment.MAdapter.NotifyDataSetChanged();
                        }

                        var emptyStateChecker = GlobalContext.TrendingFragment.MAdapter?.TrendingList?.FirstOrDefault(a => a.Type == ItemType.EmptyPage);
                        if (emptyStateChecker != null)
                        {
                            GlobalContext.TrendingFragment.MAdapter.TrendingList.Remove(emptyStateChecker);
                            GlobalContext.TrendingFragment.MAdapter.NotifyDataSetChanged();
                        }

                        if (GlobalContext.TrendingFragment.MainScrollEvent != null)
                            GlobalContext.TrendingFragment.MainScrollEvent.IsLoading = false;

                        GlobalContext.TrendingFragment.SwipeRefreshLayout.Refreshing = true;
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => GlobalContext.TrendingFragment.LoadHotOrNotAsync() });


                        GlobalContext.TrendingFragment?.HotOrNotFragment?.ApplyFilter();
                    }
                }
                else if (TypePage == "CardMach")
                {
                    UserDetails.UsersFilterGender = Gender;
                    UserDetails.UsersFilterBirthday = Birthday;
                    UserDetails.UsersFilterIsOnline = SwitchState;

                    GlobalContext.CardFragment?.ApplyFilter();
                }

                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Reset Value
        private void ResetTextViewOnClick(object sender, EventArgs e)
        {
            try
            {
                ButtonGirls.SetBackgroundResource(Resource.Drawable.round_button_outline_withoutstock);
                ButtonGirls.SetTextColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                ButtonBoth.SetBackgroundResource(Resource.Drawable.round_button_pressed);
                ButtonBoth.SetTextColor(Color.ParseColor("#ffffff"));

                ButtonMan.SetBackgroundResource(Resource.Drawable.round_button_outline_withoutstock);
                ButtonMan.SetTextColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));
                Gender = UserDetails.FilterOptionGender;

                BirthdayValue.Text = "";

                SwitchState = false;
                OnlineSwitch.Checked = false;

                Birthday = "";
                if (TypePage == "HotOrNot")
                {
                    UserDetails.HotOrNotFilterGender = Gender;
                    UserDetails.HotOrNotFilterBirthday = Birthday;
                    UserDetails.HotOrNotFilterIsOnline = SwitchState;
                }
                else if (TypePage == "CardMach")
                {
                    UserDetails.UsersFilterGender = Gender;
                    UserDetails.UsersFilterBirthday = Birthday;
                    UserDetails.UsersFilterIsOnline = SwitchState;
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Birthday
        private void BirthdayLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                var frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                {
                    try
                    {
                        BirthdayValue.Text = time.Date.ToString("dd-MM-yyyy");
                        Birthday = time.Date.ToString("dd-MM-yyyy");
                    }
                    catch (Exception exception)
                    {
                        Methods.DisplayReportResultTrack(exception);
                    }
                });
                frag.Show(ChildFragmentManager, DatePickerFragment.Tag);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Back
        private void IconBackOnClick(object sender, EventArgs e)
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

        //Select gender >> Both (0,1)
        private void ButtonBothOnClick(object sender, EventArgs e)
        {
            try
            {
                //round_button_outline >> Un click
                //round_button_pressed >> click
                ButtonBoth.SetBackgroundResource(Resource.Drawable.round_button_pressed);
                ButtonBoth.SetTextColor(Color.ParseColor("#ffffff"));

                ButtonGirls.SetBackgroundResource(Resource.Drawable.round_button_outline_withoutstock);
                ButtonGirls.SetTextColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                ButtonMan.SetBackgroundResource(Resource.Drawable.round_button_outline_withoutstock);
                ButtonMan.SetTextColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                Gender = UserDetails.FilterOptionGender;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Select gender >> Girls (1)
        private void ButtonGirlsOnClick(object sender, EventArgs e)
        {
            try
            {
                //round_button_outline >> Un click
                //round_button_pressed >> click
                ButtonGirls.SetBackgroundResource(Resource.Drawable.round_button_pressed);
                ButtonGirls.SetTextColor(Color.ParseColor("#ffffff"));

                ButtonBoth.SetBackgroundResource(Resource.Drawable.round_button_outline_withoutstock);
                ButtonBoth.SetTextColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                ButtonMan.SetBackgroundResource(Resource.Drawable.round_button_outline_withoutstock);
                ButtonMan.SetTextColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));
                Gender = UserDetails.FilterOptionGenderFemale;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Select gender >> Man (0)
        private void ButtonManOnClick(object sender, EventArgs e)
        {
            try
            {
                //round_button_outline >> Un click
                //round_button_pressed >> click
                ButtonMan.SetBackgroundResource(Resource.Drawable.round_button_pressed);
                ButtonMan.SetTextColor(Color.ParseColor("#ffffff"));

                ButtonBoth.SetBackgroundResource(Resource.Drawable.round_button_outline_withoutstock);
                ButtonBoth.SetTextColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                ButtonGirls.SetBackgroundResource(Resource.Drawable.round_button_outline_withoutstock);
                ButtonGirls.SetTextColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                Gender = UserDetails.FilterOptionGenderMale;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        private void SetLocalData()
        {
            try
            {
                if (TypePage == "HotOrNot")
                {
                    UserDetails.HotOrNotFilterGender = Gender;
                    UserDetails.HotOrNotFilterBirthday = Birthday;
                    UserDetails.HotOrNotFilterIsOnline = SwitchState;
                }
                else if (TypePage == "CardMach")
                {
                    Gender = UserDetails.UsersFilterGender;
                    Birthday = UserDetails.UsersFilterBirthday;
                    SwitchState = UserDetails.UsersFilterIsOnline;
                }

                OnlineSwitch.Checked = SwitchState;

                if (Gender == UserDetails.FilterOptionGender)
                {
                    ButtonBoth.SetBackgroundResource(Resource.Drawable.round_button_pressed);
                    ButtonBoth.SetTextColor(Color.ParseColor("#ffffff"));

                    ButtonGirls.SetBackgroundResource(Resource.Drawable.round_button_outline_withoutstock);
                    ButtonGirls.SetTextColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                    ButtonMan.SetBackgroundResource(Resource.Drawable.round_button_outline_withoutstock);
                    ButtonMan.SetTextColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));
                }
                else if (Gender == UserDetails.FilterOptionGenderFemale)
                {
                    ButtonGirls.SetBackgroundResource(Resource.Drawable.round_button_pressed);
                    ButtonGirls.SetTextColor(Color.ParseColor("#ffffff"));

                    ButtonBoth.SetBackgroundResource(Resource.Drawable.round_button_outline_withoutstock);
                    ButtonBoth.SetTextColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                    ButtonMan.SetBackgroundResource(Resource.Drawable.round_button_outline_withoutstock);
                    ButtonMan.SetTextColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));
                }
                else if (Gender == UserDetails.FilterOptionGenderMale)
                {
                    ButtonMan.SetBackgroundResource(Resource.Drawable.round_button_pressed);
                    ButtonMan.SetTextColor(Color.ParseColor("#ffffff"));

                    ButtonBoth.SetBackgroundResource(Resource.Drawable.round_button_outline_withoutstock);
                    ButtonBoth.SetTextColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                    ButtonGirls.SetBackgroundResource(Resource.Drawable.round_button_outline_withoutstock);
                    ButtonGirls.SetTextColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }
}