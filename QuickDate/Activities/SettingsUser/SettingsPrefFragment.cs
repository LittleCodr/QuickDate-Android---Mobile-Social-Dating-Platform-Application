using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Preference;
using Bumptech.Glide;
using Google.Android.Material.Dialog;
using Java.Lang;
using QuickDate.Activities.SettingsUser.Custom;
using QuickDate.Activities.SettingsUser.General;
using QuickDate.Activities.SettingsUser.Support;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient;
using QuickDateClient.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exception = System.Exception;

namespace QuickDate.Activities.SettingsUser
{
    public class SettingsPrefFragment : PreferenceFragmentCompat, ISharedPreferencesOnSharedPreferenceChangeListener, IDialogListCallBack
    {
        #region Variables Basic

        private GeneralCustomPreference MyAccountPref, PasswordPref, SocialLinksPref, BlockedUsersPref, StoragePref, HelpPref, AboutPref, DeleteAccountPref, LogoutPref, TwoFactorPref, ManageSessionsPref, WithdrawalsPref, MyAffiliatesPref, TransactionsPref, NightMode, LangPref;
        private CustomSwitchPreference ChatOnlinePref, PSearchEnginesPref, PRandomUsersPref, PFindMatchPagePref, PConfirmFollowersPref;
        private readonly AppCompatActivity ActivityContext;
        private string ChatOnline, PSearchEngines, PRandomUsers, PFindMatchPage, PConfirmFollowers;

        #endregion

        #region General

        public SettingsPrefFragment(AppCompatActivity context)
        {
            try
            {
                ActivityContext = context;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                // create ContextThemeWrapper from the original Activity Context with the custom theme
                Context contextThemeWrapper = QuickDateTools.IsTabDark() ? new ContextThemeWrapper(ActivityContext, Resource.Style.SettingsThemeDark) : new ContextThemeWrapper(ActivityContext, Resource.Style.SettingsTheme);

                // clone the inflater using the ContextThemeWrapper
                LayoutInflater localInflater = inflater.CloneInContext(contextThemeWrapper);

                View view = base.OnCreateView(localInflater, container, savedInstanceState);

                return view;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return null;
            }
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            try
            {
                AddPreferencesFromResource(Resource.Xml.SettingsPrefs);

                InitComponent();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override PreferenceScreen PreferenceScreen
        {
            get => base.PreferenceScreen;
            set
            {
                base.PreferenceScreen = value;
                if (PreferenceScreen != null)
                {
                    var count = PreferenceScreen.PreferenceCount;
                    for (var i = 0; i < count; i++)
                    {
                        PreferenceScreen.GetPreference(i).IconSpaceReserved = false;
                    }
                }
            }
        }

        public override void OnResume()
        {
            try
            {
                base.OnResume();
                PreferenceManager.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);

                //Add OnChange event to Preferences
                AddOrRemoveEvent(true);
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
                PreferenceScreen.SharedPreferences.UnregisterOnSharedPreferenceChangeListener(this);

                //Close OnChange event to Preferences
                AddOrRemoveEvent(false);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                MainSettings.SharedData = PreferenceManager.SharedPreferences;
                PreferenceManager.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);

                MyAccountPref = (GeneralCustomPreference)FindPreference("myAccount_key");
                PasswordPref = (GeneralCustomPreference)FindPreference("editPassword_key");
                SocialLinksPref = (GeneralCustomPreference)FindPreference("socialLinks_key");
                BlockedUsersPref = (GeneralCustomPreference)FindPreference("blocked_key");
                StoragePref = (GeneralCustomPreference)FindPreference("Storage_key");
                HelpPref = (GeneralCustomPreference)FindPreference("help_key");
                AboutPref = (GeneralCustomPreference)FindPreference("about_key");
                DeleteAccountPref = (GeneralCustomPreference)FindPreference("deleteAccount_key");
                LogoutPref = (GeneralCustomPreference)FindPreference("logout_key");
                TwoFactorPref = (GeneralCustomPreference)FindPreference("Twofactor_key");
                ManageSessionsPref = (GeneralCustomPreference)FindPreference("ManageSessions_key");
                WithdrawalsPref = (GeneralCustomPreference)FindPreference("Withdrawals_key");
                MyAffiliatesPref = (GeneralCustomPreference)FindPreference("MyAffiliates_key");
                TransactionsPref = (GeneralCustomPreference)FindPreference("Transactions_key");
                NightMode = (GeneralCustomPreference)FindPreference("Night_Mode_key");
                LangPref = (GeneralCustomPreference)FindPreference("Lang_key");

                ChatOnlinePref = (CustomSwitchPreference)FindPreference("chatOnline_key");
                PSearchEnginesPref = (CustomSwitchPreference)FindPreference("searchEngines_key");
                PRandomUsersPref = (CustomSwitchPreference)FindPreference("randomUsers_key");
                PFindMatchPagePref = (CustomSwitchPreference)FindPreference("findMatchPage_key");
                PConfirmFollowersPref = (CustomSwitchPreference)FindPreference("confirmFollowers_key");
                OnSharedPreferenceChanged(MainSettings.SharedData, "Night_Mode_key");

                //Delete Preference
                //============== SecurityAccount_key ===================
                var mCategoryAccount = (PreferenceCategory)FindPreference("CategoryGeneral_key");
                if (!AppSettings.ShowSettingsAccount)
                    mCategoryAccount.RemovePreference(MyAccountPref);

                if (!AppSettings.ShowSettingsSocialLinks)
                    mCategoryAccount.RemovePreference(SocialLinksPref);

                if (!AppSettings.ShowSettingsBlockedUsers)
                    mCategoryAccount.RemovePreference(BlockedUsersPref);

                if (!AppSettings.ShowSettingsMyAffiliates)
                    mCategoryAccount.RemovePreference(MyAffiliatesPref);

                var mPayment = (PreferenceCategory)FindPreference("Payment_key");
                //============== Payment_key ===================
                if (!AppSettings.ShowSettingsWithdrawals)
                    mPayment.RemovePreference(WithdrawalsPref);

                if (!AppSettings.ShowSettingsTransactions)
                    mPayment.RemovePreference(TransactionsPref);

                //============== SecurityAccount_key ===================
                var mCategorySecurity = (PreferenceCategory)FindPreference("SecurityAccount_key");
                if (!AppSettings.ShowSettingsPassword)
                    mCategorySecurity.RemovePreference(PasswordPref);

                if (!AppSettings.ShowSettingsTwoFactor)
                    mCategorySecurity.RemovePreference(TwoFactorPref);

                if (!AppSettings.ShowSettingsManageSessions)
                    mCategorySecurity.RemovePreference(ManageSessionsPref);

                //============== CategorySupport_key ===================
                var mCategorySupport = (PreferenceCategory)FindPreference("CategorySupport_key");

                if (!AppSettings.ShowSettingsDeleteAccount)
                    mCategorySupport.RemovePreference(DeleteAccountPref);


                //Update Preferences data on Load
                OnSharedPreferenceChanged(MainSettings.SharedData, "chatOnline_key");
                OnSharedPreferenceChanged(MainSettings.SharedData, "searchEngines_key");
                OnSharedPreferenceChanged(MainSettings.SharedData, "randomUsers_key");
                OnSharedPreferenceChanged(MainSettings.SharedData, "findMatchPage_key");
                OnSharedPreferenceChanged(MainSettings.SharedData, "confirmFollowers_key");

                var dataUser = ListUtils.MyUserInfo?.FirstOrDefault();
                if (dataUser != null)
                {
                    ChatOnlinePref.Checked = dataUser.Online == "1";
                    PConfirmFollowersPref.Checked = dataUser.ConfirmFollowers == "1";
                    PFindMatchPagePref.Checked = dataUser.PrivacyShowProfileMatchProfiles == "1";
                    PRandomUsersPref.Checked = dataUser.PrivacyShowProfileRandomUsers == "1";
                    PSearchEnginesPref.Checked = dataUser.PrivacyShowProfileOnGoogle == "1";
                }

                ChatOnlinePref.IconSpaceReserved = false;
                PSearchEnginesPref.IconSpaceReserved = false;
                PRandomUsersPref.IconSpaceReserved = false;
                PFindMatchPagePref.IconSpaceReserved = false;
                PConfirmFollowersPref.IconSpaceReserved = false;
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
                    MyAccountPref.PreferenceClick += MyAccountPrefOnPreferenceClick;
                    PasswordPref.PreferenceClick += PasswordPrefOnPreferenceClick;
                    SocialLinksPref.PreferenceClick += SocialLinksPrefOnPreferenceClick;
                    BlockedUsersPref.PreferenceClick += BlockedUsersPrefOnPreferenceClick;
                    StoragePref.PreferenceClick += StoragePrefOnPreferenceClick;
                    HelpPref.PreferenceClick += HelpPrefOnPreferenceClick;
                    AboutPref.PreferenceClick += AboutPrefOnPreferenceClick;
                    DeleteAccountPref.PreferenceClick += DeleteAccountPrefOnPreferenceClick;
                    LogoutPref.PreferenceClick += LogoutPrefOnPreferenceClick;
                    ChatOnlinePref.PreferenceChange += ChatOnlinePrefOnPreferenceChange;
                    PSearchEnginesPref.PreferenceChange += PSearchEnginesPrefOnPreferenceChange;
                    PRandomUsersPref.PreferenceChange += PRandomUsersPrefOnPreferenceChange;
                    PFindMatchPagePref.PreferenceChange += PFindMatchPagePrefOnPreferenceChange;
                    PConfirmFollowersPref.PreferenceChange += PConfirmFollowersPrefOnPreferenceChange;
                    ManageSessionsPref.PreferenceClick += ManageSessionsPrefOnPreferenceClick;
                    TwoFactorPref.PreferenceClick += TwoFactorPrefOnPreferenceClick;
                    WithdrawalsPref.PreferenceClick += WithdrawalsPrefOnPreferenceClick;
                    MyAffiliatesPref.PreferenceClick += MyAffiliatesPrefOnPreferenceClick;
                    TransactionsPref.PreferenceClick += TransactionsPrefOnPreferenceClick;
                }
                else
                {
                    MyAccountPref.PreferenceClick -= MyAccountPrefOnPreferenceClick;
                    PasswordPref.PreferenceClick -= PasswordPrefOnPreferenceClick;
                    SocialLinksPref.PreferenceClick -= SocialLinksPrefOnPreferenceClick;
                    BlockedUsersPref.PreferenceClick -= BlockedUsersPrefOnPreferenceClick;
                    StoragePref.PreferenceClick -= StoragePrefOnPreferenceClick;
                    HelpPref.PreferenceClick -= HelpPrefOnPreferenceClick;
                    AboutPref.PreferenceClick -= AboutPrefOnPreferenceClick;
                    DeleteAccountPref.PreferenceClick -= DeleteAccountPrefOnPreferenceClick;
                    LogoutPref.PreferenceClick -= LogoutPrefOnPreferenceClick;
                    ChatOnlinePref.PreferenceChange -= ChatOnlinePrefOnPreferenceChange;
                    PSearchEnginesPref.PreferenceChange -= PSearchEnginesPrefOnPreferenceChange;
                    PRandomUsersPref.PreferenceChange -= PRandomUsersPrefOnPreferenceChange;
                    PFindMatchPagePref.PreferenceChange -= PFindMatchPagePrefOnPreferenceChange;
                    PConfirmFollowersPref.PreferenceChange -= PConfirmFollowersPrefOnPreferenceChange;
                    ManageSessionsPref.PreferenceClick -= ManageSessionsPrefOnPreferenceClick;
                    TwoFactorPref.PreferenceClick -= TwoFactorPrefOnPreferenceClick;
                    WithdrawalsPref.PreferenceClick -= WithdrawalsPrefOnPreferenceClick;
                    MyAffiliatesPref.PreferenceClick -= MyAffiliatesPrefOnPreferenceClick;
                    TransactionsPref.PreferenceClick -= TransactionsPrefOnPreferenceClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Event Privacy

        //Privacy >> Confirm request when someone request to be a friend with you
        private void PConfirmFollowersPrefOnPreferenceChange(object sender, Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (!eventArgs.Handled) return;
                    var dataUser = ListUtils.MyUserInfo?.FirstOrDefault();
                    var etp = (CustomSwitchPreference)sender;
                    var value = eventArgs.NewValue.ToString();
                    etp.Checked = bool.Parse(value);
                    if (dataUser == null) return;
                    if (etp.Checked)
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
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.UpdateProfileAsync(dataPrivacy) });
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }


        //Privacy >> Show my profile in find match page
        private void PFindMatchPagePrefOnPreferenceChange(object sender, Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (!eventArgs.Handled) return;
                    var dataUser = ListUtils.MyUserInfo?.FirstOrDefault();
                    var etp = (CustomSwitchPreference)sender;
                    var value = eventArgs.NewValue.ToString();
                    etp.Checked = bool.Parse(value);
                    if (dataUser == null) return;
                    if (etp.Checked)
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
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.UpdateProfileAsync(dataPrivacy) });
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //Privacy >> Show my profile in random users
        private void PRandomUsersPrefOnPreferenceChange(object sender, Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (!eventArgs.Handled) return;
                    var dataUser = ListUtils.MyUserInfo?.FirstOrDefault();
                    var etp = (CustomSwitchPreference)sender;
                    var value = eventArgs.NewValue.ToString();
                    etp.Checked = bool.Parse(value);
                    if (dataUser == null) return;
                    if (etp.Checked)
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
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.UpdateProfileAsync(dataPrivacy) });
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //Privacy >> Show my profile on search engines (google)
        private void PSearchEnginesPrefOnPreferenceChange(object sender, Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (!eventArgs.Handled) return;
                    var dataUser = ListUtils.MyUserInfo?.FirstOrDefault();
                    var etp = (CustomSwitchPreference)sender;
                    var value = eventArgs.NewValue.ToString();
                    etp.Checked = bool.Parse(value);
                    if (dataUser == null) return;
                    if (etp.Checked)
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
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.UpdateProfileAsync(dataPrivacy) });
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Event Support

        //Logout
        private void LogoutPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var dialog = new MaterialAlertDialogBuilder(Activity);
                dialog.SetTitle(Resource.String.Lbl_Warning);
                dialog.SetMessage(GetText(Resource.String.Lbl_Are_you_logout));
                dialog.SetPositiveButton(GetText(Resource.String.Lbl_Ok), (materialDialog, action) =>
                {
                    try
                    {
                        Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_You_will_be_logged), ToastLength.Long)?.Show();
                        ApiRequest.RunLogout = false;
                        ApiRequest.Logout(ActivityContext);
                    }
                    catch (Exception exception)
                    {
                        Methods.DisplayReportResultTrack(exception);
                    }
                });
                dialog.SetNegativeButton(GetText(Resource.String.Lbl_Cancel), new MaterialDialogUtils());

                dialog.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //DeleteAccount
        private void DeleteAccountPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                ActivityContext.StartActivity(new Intent(ActivityContext, typeof(DeleteAccountActivity)));
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //About
        private void AboutPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(AboutAppActivity));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Help
        private void HelpPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(LocalWebViewActivity));
                intent.PutExtra("URL", InitializeQuickDate.WebsiteUrl + "/contact");
                intent.PutExtra("Type", GetText(Resource.String.Lbl_Help));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Event General

        //Transactions
        private void TransactionsPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(TransactionsActivity));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //MyAffiliates
        private void MyAffiliatesPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(MyAffiliatesActivity));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Withdrawals
        private void WithdrawalsPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(WithdrawalsActivity));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Manage Sessions
        private void ManageSessionsPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(ManageSessionsActivity));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Two-Factor Authentication
        private void TwoFactorPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(TwoFactorAuthActivity));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //BlockedUsers
        private void BlockedUsersPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                ActivityContext.StartActivity(new Intent(ActivityContext, typeof(BlockedUsersActivity)));
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //SocialLinks
        private void SocialLinksPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                ActivityContext.StartActivity(new Intent(ActivityContext, typeof(SocialLinksActivity)));
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Change Password
        private void PasswordPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                ActivityContext.StartActivity(new Intent(ActivityContext, typeof(PasswordActivity)));
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //MyAccount
        private void MyAccountPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                ActivityContext.StartActivity(new Intent(ActivityContext, typeof(MyAccountActivity)));
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        //Clear Cache >> Media
        private void StoragePrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var dialog = new MaterialAlertDialogBuilder(ActivityContext);
                dialog.SetTitle(GetText(Resource.String.Lbl_Warning));
                dialog.SetMessage(GetText(Resource.String.Lbl_TheFilesWillBeDeleted));
                dialog.SetPositiveButton(GetText(Resource.String.Lbl_Yes), (materialDialog, action) =>
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_FilesAreNowDeleted), ToastLength.Long)?.Show();

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            var dirPath = ActivityContext.CacheDir;
                            dirPath?.Delete();

                            Glide.Get(ActivityContext)?.ClearMemory();
                            new Thread(() =>
                            {
                                try
                                {
                                    Glide.Get(ActivityContext)?.ClearDiskCache();
                                }
                                catch (Exception exception)
                                {
                                    Methods.DisplayReportResultTrack(exception);
                                }
                            }).Start();

                            Methods.Path.DeleteAll_FolderUser();
                            Methods.Path.Chack_MyFolder();
                        }
                        catch (Exception exception)
                        {
                            Methods.DisplayReportResultTrack(exception);
                        }
                    });
                });
                dialog.SetNegativeButton(GetText(Resource.String.Lbl_No), new MaterialDialogUtils());

                dialog.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //ChatOnline
        private void ChatOnlinePrefOnPreferenceChange(object sender, Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (!eventArgs.Handled) return;
                    var dataUser = ListUtils.MyUserInfo?.FirstOrDefault();
                    var etp = (CustomSwitchPreference)sender;
                    var value = eventArgs.NewValue.ToString();
                    etp.Checked = bool.Parse(value);
                    if (dataUser == null) return;
                    if (etp.Checked)
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


                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Chat.SwitchOnlineAsync(ChatOnline) });
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            try
            {
                switch (key)
                {
                    case "chatOnline_key":
                        {
                            var getValue = MainSettings.SharedData.GetBoolean("chatOnline_key", true);
                            ChatOnlinePref.Checked = getValue;
                            break;
                        }
                    case "searchEngines_key":
                        {
                            var getValue = MainSettings.SharedData.GetBoolean("searchEngines_key", true);
                            PSearchEnginesPref.Checked = getValue;
                            break;
                        }
                    case "randomUsers_key":
                        {
                            var getValue = MainSettings.SharedData.GetBoolean("randomUsers_key", true);
                            PRandomUsersPref.Checked = getValue;
                            break;
                        }
                    case "findMatchPage_key":
                        {
                            var getValue = MainSettings.SharedData.GetBoolean("findMatchPage_key", true);
                            PFindMatchPagePref.Checked = getValue;
                            break;
                        }
                    case "confirmFollowers_key":
                        {
                            var getValue = MainSettings.SharedData.GetBoolean("confirmFollowers_key", true);
                            PConfirmFollowersPref.Checked = getValue;
                            break;
                        }
                    case "Night_Mode_key":
                        {
                            // Set summary to be the user-description for the selected value
                            Preference etp = FindPreference("Night_Mode_key");

                            string getValue = MainSettings.SharedData.GetString("Night_Mode_key", string.Empty);
                            etp.Summary = getValue;
                            break;
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
                string text = itemString;

                string getValue = MainSettings.SharedData.GetString("Night_Mode_key", string.Empty);

                if (text == GetString(Resource.String.Lbl_Light) && getValue != MainSettings.LightMode)
                {
                    //Set Light Mode   
                    NightMode.Summary = ActivityContext.GetString(Resource.String.Lbl_Light);

                    MainSettings.ApplyTheme(MainSettings.LightMode);
                    MainSettings.SharedData?.Edit()?.PutString("Night_Mode_key", MainSettings.LightMode)?.Commit();

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                    {
                        ActivityContext.Window?.ClearFlags(WindowManagerFlags.TranslucentStatus);
                        ActivityContext.Window?.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                    }

                    Intent intent = new Intent(ActivityContext, typeof(HomeActivity));
                    intent.AddCategory(Intent.CategoryHome);
                    intent.SetAction(Intent.ActionMain);
                    intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    intent.AddFlags(ActivityFlags.NoAnimation);
                    ActivityContext.FinishAffinity();
                    ActivityContext.OverridePendingTransition(0, 0);
                    ActivityContext.StartActivity(intent);
                }
                else if (text == GetString(Resource.String.Lbl_Dark) && getValue != MainSettings.DarkMode)
                {
                    NightMode.Summary = ActivityContext.GetString(Resource.String.Lbl_Dark);

                    MainSettings.ApplyTheme(MainSettings.DarkMode);
                    MainSettings.SharedData?.Edit()?.PutString("Night_Mode_key", MainSettings.DarkMode)?.Commit();

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                    {
                        ActivityContext.Window?.ClearFlags(WindowManagerFlags.TranslucentStatus);
                        ActivityContext.Window?.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                    }

                    Intent intent = new Intent(ActivityContext, typeof(HomeActivity));
                    intent.AddCategory(Intent.CategoryHome);
                    intent.SetAction(Intent.ActionMain);
                    intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    intent.AddFlags(ActivityFlags.NoAnimation);
                    ActivityContext.FinishAffinity();
                    ActivityContext.OverridePendingTransition(0, 0);
                    ActivityContext.StartActivity(intent);
                }
                else if (text == GetString(Resource.String.Lbl_SetByBattery) && getValue != MainSettings.DefaultMode)
                {
                    NightMode.Summary = ActivityContext.GetString(Resource.String.Lbl_SetByBattery);

                    MainSettings.SharedData?.Edit()?.PutString("Night_Mode_key", MainSettings.DefaultMode)?.Commit();

                    if ((int)Build.VERSION.SdkInt >= 29)
                    {
                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightFollowSystem;

                        var currentNightMode = Resources?.Configuration?.UiMode & UiMode.NightMask;
                        switch (currentNightMode)
                        {
                            case UiMode.NightNo:
                                // Night mode is not active, we're using the light theme
                                MainSettings.ApplyTheme(MainSettings.LightMode);
                                break;
                            case UiMode.NightYes:
                                // Night mode is active, we're using dark theme
                                MainSettings.ApplyTheme(MainSettings.DarkMode);
                                break;
                        }
                    }
                    else
                    {
                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightAutoBattery;

                        var currentNightMode = Resources?.Configuration?.UiMode & UiMode.NightMask;
                        switch (currentNightMode)
                        {
                            case UiMode.NightNo:
                                // Night mode is not active, we're using the light theme
                                MainSettings.ApplyTheme(MainSettings.LightMode);
                                break;
                            case UiMode.NightYes:
                                // Night mode is active, we're using dark theme
                                MainSettings.ApplyTheme(MainSettings.DarkMode);
                                break;
                        }

                        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                        {
                            ActivityContext.Window?.ClearFlags(WindowManagerFlags.TranslucentStatus);
                            ActivityContext.Window?.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                        }

                        Intent intent = new Intent(ActivityContext, typeof(HomeActivity));
                        intent.AddCategory(Intent.CategoryHome);
                        intent.SetAction(Intent.ActionMain);
                        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                        intent.AddFlags(ActivityFlags.NoAnimation);
                        ActivityContext.FinishAffinity();
                        ActivityContext.OverridePendingTransition(0, 0);
                        ActivityContext.StartActivity(intent);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        public override bool OnPreferenceTreeClick(Preference preference)
        {
            try
            {
                if (preference.Key == "Night_Mode_key")
                {
                    var arrayAdapter = new List<string>();
                    var dialogList = new MaterialAlertDialogBuilder(ActivityContext);

                    dialogList.SetTitle(Resource.String.Lbl_Night_Mode);

                    arrayAdapter.Add(GetText(Resource.String.Lbl_Light));
                    arrayAdapter.Add(GetText(Resource.String.Lbl_Dark));

                    dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                    dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                    dialogList.Show();
                }
                else if (preference.Key == "Lang_key")
                {
                    BottomSheetsLanguage BottomSheetsLanguage = new BottomSheetsLanguage();
                    BottomSheetsLanguage.Show(ActivityContext.SupportFragmentManager, BottomSheetsLanguage.Tag);
                }

                return base.OnPreferenceTreeClick(preference);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return base.OnPreferenceTreeClick(preference);
            }
        }
    }
}