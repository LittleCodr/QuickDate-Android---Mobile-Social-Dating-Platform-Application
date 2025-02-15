using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient;
using QuickDateClient.Classes.Authorization;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using System;
using System.Collections.Generic;
using Task = System.Threading.Tasks.Task;

namespace QuickDate.Activities.Default
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class LoginActivity : SocialLoginBaseActivity
    {
        #region Variables Basic

        private EditText EmailEditText, PasswordEditText;
        private ProgressBar ProgressBar;
        private AppCompatButton BtnSignIn;
        private TextView ForgotPassTextView;
        private ImageView EyesIcon;
        private LinearLayout SignLayout;
        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                // Create your application here
                SetContentView(Resource.Layout.LoginLayout);

                //Get Value And Set Toolbar
                InitComponent();
                InitBackground();
                InitToolbar();
                InitSocialLogins();

                if (AppSettings.EnableSmartLockForPasswords)
                    BuildClients();
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
                var TxtAppName = FindViewById<TextView>(Resource.Id.appName);
                TxtAppName.Text = AppSettings.ApplicationName;

                EmailEditText = FindViewById<EditText>(Resource.Id.edt_email);
                PasswordEditText = FindViewById<EditText>(Resource.Id.edt_password);
                ProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
                BtnSignIn = FindViewById<AppCompatButton>(Resource.Id.SignInButton);
                ForgotPassTextView = FindViewById<TextView>(Resource.Id.txt_forgot_pass);

                SignLayout = FindViewById<LinearLayout>(Resource.Id.SignLayout);

                ProgressBar.Visibility = ViewStates.Invisible;

                EyesIcon = FindViewById<ImageView>(Resource.Id.eyesicon);
                EyesIcon.Tag = "hide";
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
                    SignLayout.Click += SignLayoutOnClick;
                    BtnSignIn.Click += BtnSignInOnClick;
                    ForgotPassTextView.Click += ForgotPassTextViewOnClick;
                    EyesIcon.Click += EyesIconOnClick;
                }
                else
                {
                    SignLayout.Click -= SignLayoutOnClick;
                    BtnSignIn.Click -= BtnSignInOnClick;
                    ForgotPassTextView.Click -= ForgotPassTextViewOnClick;
                    EyesIcon.Click -= EyesIconOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        //Login QuickDate
        private async void BtnSignInOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (!string.IsNullOrEmpty(EmailEditText.Text.Replace(" ", "")) || !string.IsNullOrEmpty(PasswordEditText.Text))
                    { 
                        Methods.HideKeyboard(this);
                        ToggleVisibility(true);

                        var (apiStatus, respond) = await RequestsAsync.Auth.LoginAsync(EmailEditText.Text.Replace(" ", ""), PasswordEditText.Text, UserDetails.DeviceId);
                        switch (apiStatus)
                        {
                            case 200 when respond is LoginObject auth:

                                SetDataLogin(auth);
                                StartActivity(new Intent(this, typeof(HomeActivity)));

                                ToggleVisibility(false);

                                FinishAffinity();
                                break;
                            case 200:
                                {
                                    if (respond is LoginTwoFactorObject auth2)
                                    {
                                        ToggleVisibility(false);

                                        UserDetails.UserId = Convert.ToInt32(auth2.UserId);
                                        Intent intent = new Intent(this, typeof(VerificationAccountActivity));
                                        intent.PutExtra("Type", "TwoFactor");
                                        StartActivity(intent);
                                        Finish();
                                    }

                                    break;
                                }
                            case 400:
                                {
                                    if (respond is ErrorObject error)
                                    {
                                        string errorText = error.Message;
                                        long errorId = error.Code;
                                        switch (errorId)
                                        {
                                            case 1:
                                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_1), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 2:
                                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_2), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 3:
                                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_3), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 4:
                                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_4), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 5:
                                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_5), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            default:
                                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), errorText, GetText(Resource.String.Lbl_Ok));
                                                break;
                                        }
                                    }
                                    ToggleVisibility(false);
                                    break;
                                }
                            case 404:
                                ToggleVisibility(false);
                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), respond.ToString(), GetText(Resource.String.Lbl_Ok));
                                break;
                        }

                    }
                    else
                    {
                        ToggleVisibility(false);
                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_Please_enter_your_data), GetText(Resource.String.Lbl_Ok));
                    }
                }
                else
                {
                    ToggleVisibility(false);
                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_CheckYourInternetConnection), GetText(Resource.String.Lbl_Ok));
                }
            }
            catch (Exception ex)
            {
                ToggleVisibility(false);
                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), ex.Message, GetText(Resource.String.Lbl_Ok));
                Methods.DisplayReportResultTrack(ex);
            }
        }

        //Open Register
        private void SignLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(this, typeof(RegisterActivity)));
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Open Terms of use 
        private void TermsTextViewOnClick(object sender, EventArgs e)
        {
            try
            {
                var url = InitializeQuickDate.WebsiteUrl + "/terms-of-use";
                new IntentController(this).OpenBrowserFromApp(url);
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }

        //Open Forgot Password
        private void ForgotPassTextViewOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(this, typeof(ForgotPasswordActivity)));
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void EyesIconOnClick(object sender, EventArgs e)
        {
            try
            {
                if (EyesIcon.Tag?.ToString() == "hide")
                {
                    EyesIcon.SetImageResource(Resource.Drawable.visibility_on);
                    EyesIcon.Tag = "show";
                    PasswordEditText.InputType = Android.Text.InputTypes.TextVariationNormal | Android.Text.InputTypes.ClassText;
                    PasswordEditText.SetSelection(PasswordEditText.Text.Length);
                }
                else
                {
                    EyesIcon.SetImageResource(Resource.Drawable.visibility_off);
                    EyesIcon.Tag = "hide";
                    PasswordEditText.InputType = Android.Text.InputTypes.TextVariationPassword | Android.Text.InputTypes.ClassText;
                    PasswordEditText.SetSelection(PasswordEditText.Text.Length);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void SetDataLogin(LoginObject auth)
        {
            try
            {
                UserDetails.Username = EmailEditText.Text;
                UserDetails.FullName = EmailEditText.Text;
                UserDetails.Password = PasswordEditText.Text;
                UserDetails.AccessToken = auth.Data.AccessToken;
                UserDetails.UserId = auth.Data.UserId;
                UserDetails.Status = "Pending";
                UserDetails.Cookie = auth.Data.AccessToken;
                UserDetails.Email = EmailEditText.Text;

                Current.AccessToken = auth.Data.AccessToken;

                //Insert user data to database
                var user = new DataTables.LoginTb
                {
                    UserId = UserDetails.UserId.ToString(),
                    AccessToken = UserDetails.AccessToken,
                    Cookie = UserDetails.Cookie,
                    Username = EmailEditText.Text,
                    Password = PasswordEditText.Text,
                    Status = "Pending",
                    Lang = "",
                    DeviceId = UserDetails.DeviceId,
                };
                ListUtils.DataUserLoginList.Add(user);

                var dbDatabase = new SqLiteDatabase();
                dbDatabase.InsertOrUpdateLogin_Credentials(user);

                if (auth.Data.UserInfo != null)
                {
                    dbDatabase.InsertOrUpdate_DataMyInfo(auth.Data.UserInfo);

                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetInfoData(this, UserDetails.UserId.ToString()) });
                }


            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        public override void ToggleVisibility(bool isLoginProgress)
        {
            ProgressBar.Visibility = isLoginProgress ? ViewStates.Visible : ViewStates.Gone;
            BtnSignIn.Visibility = isLoginProgress ? ViewStates.Gone : ViewStates.Visible;
        }

    }
}