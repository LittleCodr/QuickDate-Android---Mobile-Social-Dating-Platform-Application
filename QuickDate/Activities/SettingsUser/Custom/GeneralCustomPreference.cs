using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using AndroidX.Preference;
using QuickDate.Helpers.Utils;
using System;

namespace QuickDate.Activities.SettingsUser.Custom
{
    public class GeneralCustomPreference : Preference
    {
        protected GeneralCustomPreference(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public GeneralCustomPreference(Context context) : base(context)
        {
            try
            {
                LayoutResource = Resource.Layout.SettingGeneralPreference;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public GeneralCustomPreference(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            try
            {
                LayoutResource = Resource.Layout.SettingGeneralPreference;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public GeneralCustomPreference(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            try
            {
                LayoutResource = Resource.Layout.SettingGeneralPreference;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public GeneralCustomPreference(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            try
            {
                LayoutResource = Resource.Layout.SettingGeneralPreference;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
        public override void OnBindViewHolder(PreferenceViewHolder holder)
        {
            try
            {
                base.OnBindViewHolder(holder);

                var title = holder.ItemView.FindViewById<TextView>(Resource.Id.title);
                title.Text = Title;

            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }
}