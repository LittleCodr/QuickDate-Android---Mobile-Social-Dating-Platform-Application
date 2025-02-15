using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using AndroidX.Preference;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickDate.Activities.SettingsUser.Custom
{
    public class CustomSwitchPreference : SwitchPreferenceCompat
    {

        protected CustomSwitchPreference(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public CustomSwitchPreference(Context context) : base(context)
        {
            try
            {
                LayoutResource = Resource.Layout.SettingCustomSwitch;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public CustomSwitchPreference(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            try
            {
                LayoutResource = Resource.Layout.SettingCustomSwitch;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public CustomSwitchPreference(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            try
            {
                LayoutResource = Resource.Layout.SettingCustomSwitch;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public CustomSwitchPreference(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            try
            {
                LayoutResource = Resource.Layout.SettingCustomSwitch;
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

                holder.ItemView.SetBackgroundColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#444444") : Color.ParseColor("#ffffff"));

                var title = holder.ItemView.FindViewById<TextView>(Resource.Id.title);
                title.SetTextColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));
                title.Text = Title;

                var summary = holder.ItemView.FindViewById<TextView>(Resource.Id.summary);
                summary.SetTextColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#ffffff") : Color.ParseColor("#CECECE"));
                summary.Text = Summary;

                var Switch = holder.ItemView.FindViewById<Switch>(Resource.Id.togglebutton);
                Switch.Checked = Checked;
                Switch.CheckedChange += SwitchOnCheckedChange;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private string ChatOnline, PSearchEngines, PRandomUsers, PFindMatchPage, PConfirmFollowers;
        private void SwitchOnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            try
            {
                Checked = e.IsChecked;
                Console.WriteLine(Key);

                var dataUser = ListUtils.MyUserInfo?.FirstOrDefault();
                if (Key == "chatOnline_key")
                {
                    if (e.IsChecked)
                    {
                        dataUser.Online = "1";
                        ChatOnline = "1";
                    }
                    else
                    {
                        dataUser.Online = "0";
                        ChatOnline = "0";
                    }

                    dataUser.Online = ChatOnline;

                    SqLiteDatabase database = new SqLiteDatabase();
                    database.InsertOrUpdate_DataMyInfo(dataUser);

                    if (Methods.CheckConnectivity())
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Chat.SwitchOnlineAsync(ChatOnline) });
                }
                else if (Key == "searchEngines_key")
                {
                    if (e.IsChecked)
                    {
                        dataUser.PrivacyShowProfileOnGoogle = "1";
                        PSearchEngines = "1";
                    }
                    else
                    {
                        dataUser.PrivacyShowProfileOnGoogle = "0";
                        PSearchEngines = "0";
                    }

                    dataUser.PrivacyShowProfileOnGoogle = PSearchEngines;

                    SqLiteDatabase database = new SqLiteDatabase();
                    database.InsertOrUpdate_DataMyInfo(dataUser);


                    var dataPrivacy = new Dictionary<string, string>
                    {
                        {"privacy_show_profile_on_google", PSearchEngines}
                    };

                    if (Methods.CheckConnectivity())
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.UpdateProfileAsync(dataPrivacy) });
                }
                else if (Key == "randomUsers_key")
                {
                    if (e.IsChecked)
                    {
                        dataUser.PrivacyShowProfileRandomUsers = "1";
                        PRandomUsers = "1";
                    }
                    else
                    {
                        dataUser.PrivacyShowProfileRandomUsers = "0";
                        PRandomUsers = "0";
                    }

                    dataUser.PrivacyShowProfileRandomUsers = PRandomUsers;

                    SqLiteDatabase database = new SqLiteDatabase();
                    database.InsertOrUpdate_DataMyInfo(dataUser);


                    var dataPrivacy = new Dictionary<string, string>
                    {
                        {"privacy_show_profile_random_users", PRandomUsers}
                    };
                    if (Methods.CheckConnectivity())
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.UpdateProfileAsync(dataPrivacy) });
                }
                else if (Key == "findMatchPage_key")
                {
                    if (e.IsChecked)
                    {
                        dataUser.PrivacyShowProfileMatchProfiles = "1";
                        PFindMatchPage = "1";
                    }
                    else
                    {
                        dataUser.PrivacyShowProfileMatchProfiles = "0";
                        PFindMatchPage = "0";
                    }

                    dataUser.PrivacyShowProfileMatchProfiles = PFindMatchPage;

                    SqLiteDatabase database = new SqLiteDatabase();
                    database.InsertOrUpdate_DataMyInfo(dataUser);


                    var dataPrivacy = new Dictionary<string, string>
                    {
                        {"privacy_show_profile_match_profiles", PFindMatchPage}
                    };
                    if (Methods.CheckConnectivity())
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.UpdateProfileAsync(dataPrivacy) });
                }
                else if (Key == "confirmFollowers_key")
                {
                    if (e.IsChecked)
                    {
                        dataUser.ConfirmFollowers = "1";
                        PConfirmFollowers = "1";
                    }
                    else
                    {
                        dataUser.ConfirmFollowers = "0";
                        PConfirmFollowers = "0";
                    }

                    dataUser.ConfirmFollowers = PConfirmFollowers;

                    SqLiteDatabase database = new SqLiteDatabase();
                    database.InsertOrUpdate_DataMyInfo(dataUser);

                    var dataPrivacy = new Dictionary<string, string>
                    {
                        {"confirm_followers", PConfirmFollowers}
                    };
                    if (Methods.CheckConnectivity())
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.UpdateProfileAsync(dataPrivacy) });
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}