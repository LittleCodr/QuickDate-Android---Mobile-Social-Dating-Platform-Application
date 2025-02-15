using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.Slider;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using System;
using System.Linq;
using Exception = System.Exception;
using Object = Java.Lang.Object;

namespace QuickDate.Activities.SearchFilter.Fragment
{
    public class BasicFragment : AndroidX.Fragment.App.Fragment, IBaseOnChangeListener
    {
        #region  Variables Basic

        private SearchFilterTabbedActivity GlobalContext;

        private TextView AgeNumberTextView, OnlineTextView, TxtDistanceCount;
        private AppCompatButton ButtonApply;
        private TextView ResetTextView;
        private RelativeLayout MainLayout;
        private AppCompatButton ButtonMan, ButtonGirls, ButtonBoth;
        private Switch OnlineSwitch;
        private Slider DistanceBar;

        public int DistanceCount;
        public long AgeMin = UserDetails.FilterOptionAgeMin, AgeMax = UserDetails.FilterOptionAgeMax;
        public string Gender = UserDetails.FilterOptionGender;
        public bool SwitchState;
        public RangeSlider AgeSeekBar;

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
                View view = inflater.Inflate(Resource.Layout.ButtomSheetSearchFilter, container, false);
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
                MainLayout = view.FindViewById<RelativeLayout>(Resource.Id.mainLayout);
                AgeNumberTextView = view.FindViewById<TextView>(Resource.Id.Agenumber);
                OnlineTextView = view.FindViewById<TextView>(Resource.Id.OnlineTextView);
                ButtonMan = view.FindViewById<AppCompatButton>(Resource.Id.ManButton);
                ButtonGirls = view.FindViewById<AppCompatButton>(Resource.Id.GirlsButton);
                ButtonBoth = view.FindViewById<AppCompatButton>(Resource.Id.BothButton);

                ResetTextView = view.FindViewById<TextView>(Resource.Id.Resetbutton);
                ResetTextView.Visibility = AppSettings.ShowResetFilterForAllPages ? ViewStates.Visible : ViewStates.Gone;
                ButtonApply = view.FindViewById<AppCompatButton>(Resource.Id.ApplyButton);

                AgeSeekBar = view.FindViewById<RangeSlider>(Resource.Id.AgeSeekBar);
                DistanceBar = view.FindViewById<Slider>(Resource.Id.distanceSlider);
                OnlineSwitch = view.FindViewById<Switch>(Resource.Id.togglebutton);
                TxtDistanceCount = view.FindViewById<TextView>(Resource.Id.Distancenumber);

                AgeSeekBar.TrackHeight = 30;
                AgeSeekBar.SetThumbStrokeColorResource(Resource.Color.gnt_white);
                AgeSeekBar.ThumbStrokeWidth = 5f;
                AgeSeekBar.ValueFrom = 18;
                AgeSeekBar.ValueTo = 75;

                try
                {
                    var method = Java.Lang.Class.ForName("com.google.android.material.slider.BaseSlider").GetDeclaredMethods().FirstOrDefault(x => x.Name == "addOnChangeListener");
                    method?.Invoke(AgeSeekBar, this); // this is implementing IBaseOnChangeListener
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                DistanceBar.TrackHeight = 30;
                DistanceBar.ThumbStrokeWidth = 5f;
                DistanceBar.SetThumbStrokeColorResource(Resource.Color.gnt_white);
                DistanceBar.ValueFrom = 0;
                DistanceBar.ValueTo = 50;
                DistanceBar.StepSize = 5;
                DistanceBar.Touch += DistanceBarOnTouch;

                MainLayout.Visibility = ViewStates.Gone;


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


        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    ButtonMan.Click += ButtonManOnClick;
                    ButtonGirls.Click += ButtonGirlsOnClick;
                    ButtonBoth.Click += ButtonBothOnClick;
                    OnlineSwitch.CheckedChange += OnlineSwitchCheckedChange;
                    ButtonApply.Click += GlobalContext.ActionButtonOnClick;
                    ResetTextView.Click += ResetTextViewClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        private void ResetTextViewClick(object sender, EventArgs e)
        {
            GlobalContext.ResetAllFilters("BasicTab");
        }

        private void OnlineSwitchCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            SwitchState = OnlineSwitch.Checked;
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

        #region SeekBar

        private void DistanceBarOnTouch(object sender, View.TouchEventArgs e)
        {
            switch (e.Event.Action)
            {
                case MotionEventActions.Move:
                case MotionEventActions.Up:
                    OnValueChange(DistanceBar, DistanceBar.Value, true);  //(Slider slider, float value, boolean fromUser)
                    break;
            }
            e.Handled = false;
        }

        private void OnValueChange(Slider sender, float value, bool fromUser)
        {
            try
            {
                TxtDistanceCount.Text = value + " " + GetText(Resource.String.Lbl_km);
                DistanceCount = (int)value;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        public void SetLocalData()
        {
            try
            {
                AgeMin = UserDetails.AgeMin;
                AgeMax = UserDetails.AgeMax;
                Gender = UserDetails.Gender;
                SwitchState = UserDetails.SwitchState;
                DistanceCount = int.TryParse(UserDetails.Located, out int distance) ? distance : 0;

                AgeSeekBar.ValueFrom = UserDetails.AgeMin;
                AgeSeekBar.ValueTo = UserDetails.AgeMax;

                DistanceBar.Value = DistanceCount;
                OnlineSwitch.Checked = UserDetails.SwitchState;
                AgeNumberTextView.Text = AgeMin + " - " + AgeMax;

                if (UserDetails.Gender == UserDetails.FilterOptionGender)
                {
                    ButtonBoth.SetBackgroundResource(Resource.Drawable.round_button_pressed);
                    ButtonBoth.SetTextColor(Color.ParseColor("#ffffff"));

                    ButtonGirls.SetBackgroundResource(Resource.Drawable.round_button_outline_withoutstock);
                    ButtonGirls.SetTextColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                    ButtonMan.SetBackgroundResource(Resource.Drawable.round_button_outline_withoutstock);
                    ButtonMan.SetTextColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));
                }
                else if (UserDetails.Gender == UserDetails.FilterOptionGenderFemale)
                {
                    ButtonGirls.SetBackgroundResource(Resource.Drawable.round_button_pressed);
                    ButtonGirls.SetTextColor(Color.ParseColor("#ffffff"));

                    ButtonBoth.SetBackgroundResource(Resource.Drawable.round_button_outline_withoutstock);
                    ButtonBoth.SetTextColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                    ButtonMan.SetBackgroundResource(Resource.Drawable.round_button_outline_withoutstock);
                    ButtonMan.SetTextColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));
                }
                else if (UserDetails.Gender == UserDetails.FilterOptionGenderMale)
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

        //Select Age SeekBar >> Right #Max and >> Left #Min 
        public void OnValueChange(Object p0, float p1, bool p2)
        {
            try
            {
                if (p0 is RangeSlider rangeSlider)
                {
                    var sliderValues = rangeSlider.Values;

                    AgeMin = sliderValues.FirstOrDefault()?.IntValue() ?? 0;
                    AgeMax = sliderValues.LastOrDefault()?.IntValue() ?? 0;

                    AgeNumberTextView.Text = AgeMin + " - " + AgeMax;
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }

        }
    }
}