using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common.Util.Concurrent;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Content.Res;
using AndroidX.Core.Content;
using AndroidX.Credentials;
using Com.Facebook;
using Com.Facebook.Login;
using Com.Facebook.Login.Widget;
using Java.Util.Concurrent;
using Org.Json;
using QuickDate.Activities.Base;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.SocialLogins;
using QuickDate.Helpers.Utils;
using QuickDate.Library.OneSignalNotif;
using QuickDate.SQLite;
using QuickDateClient;
using QuickDateClient.Classes.Authorization;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using System;
using System.Collections.Generic;
using Xamarin.GoogleAndroid.Libraries.Identity.GoogleId;
using Exception = System.Exception;
using Object = Java.Lang.Object;
using Task = System.Threading.Tasks.Task;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace QuickDate.Activities.Default
{
    public abstract class SocialLoginBaseActivity : BaseActivity, IFacebookCallback, GraphRequest.IGraphJSONObjectCallback, ICredentialManagerCallback
    {
        #region Variables Basic

        public LinearLayout WoWonderSignInButton;
        private ICallbackManager MFbCallManager;
        private FbMyProfileTracker ProfileTracker;
        public LinearLayout FbLoginButton;
        public LinearLayout GoogleSignInButton;
        public static ICredentialManager CredentialManager;
        public static SocialLoginBaseActivity Instance;
        private Toolbar Toolbar;

        #endregion

        #region General
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                InitializeQuickDate.Initialize(AppSettings.TripleDesAppServiceProvider, PackageName, AppSettings.TurnSecurityProtocolType3072On, new MyReportModeApp());

                //Set Full screen 
                Methods.App.FullScreenApp(this);

                Instance = this;

                if (string.IsNullOrEmpty(UserDetails.DeviceId))
                    OneSignalNotification.Instance.RegisterNotificationDevice(this);

                if (Methods.CheckConnectivity())
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetSettings_Api(this) });

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                {
                    if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.PostNotifications) == Permission.Granted)
                    {
                        if (string.IsNullOrEmpty(UserDetails.DeviceId))
                            OneSignalNotification.Instance.RegisterNotificationDevice(this);
                    }
                    else
                    {
                        RequestPermissions(new[]
                        {
                            Manifest.Permission.PostNotifications
                        }, 16248);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(UserDetails.DeviceId))
                        OneSignalNotification.Instance.RegisterNotificationDevice(this);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
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
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }


        #endregion

        #region Events

        private void FbLoginButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                LoginManager.Instance.LogInWithReadPermissions(this, new List<string>
                {
                    "email",
                    "public_profile"
                });
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }


        private void MProfileTrackerOnMOnProfileChanged(object sender, OnProfileChangedEventArgs e)
        {
            try
            {
                if (e.MProfile != null)
                {
                    //FbFirstName = e.MProfile.FirstName;
                    //FbLastName = e.MProfile.LastName;
                    //FbName = e.MProfile.Name;
                    //FbProfileId = e.MProfile.Id;

                    var request = GraphRequest.NewMeRequest(AccessToken.CurrentAccessToken, this);
                    var parameters = new Bundle();
                    parameters.PutString("fields", "id,name,age_range,email");
                    request.Parameters = parameters;
                    request.ExecuteAndWait();
                }
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }

        #endregion

        #region Functions

        public void InitToolbar()
        {
            try
            {
                Toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (Toolbar != null)
                {
                    //Toolbar.Title = GetString(Resource.String.Lbl_Register);
                    Toolbar.Title = " ";
                    Toolbar.SetTitleTextColor(QuickDateTools.IsTabDark() ? AppSettings.TitleTextColorDark : AppSettings.TitleTextColor);
                    SetSupportActionBar(Toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(false);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);

                    var icon = AppCompatResources.GetDrawable(this, AppSettings.FlowDirectionRightToLeft ? Resource.Drawable.icon_back_arrow_right : Resource.Drawable.icon_back_arrow_left);
                    icon?.SetTint(QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                    SupportActionBar.SetHomeAsUpIndicator(icon);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void InitSocialLogins()
        {
            try
            {
                //#Facebook
                if (AppSettings.ShowFacebookLogin)
                {
                    //FacebookSdk.SdkInitialize(this);
                    LoginButton loginButton = new LoginButton(this);
                    ProfileTracker = new FbMyProfileTracker();
                    ProfileTracker.StartTracking();

                    FbLoginButton = FindViewById<LinearLayout>(Resource.Id.ll_fblogin);
                    FbLoginButton.Visibility = ViewStates.Visible;
                    FbLoginButton.Click += FbLoginButtonOnClick;

                    ProfileTracker.MOnProfileChanged += MProfileTrackerOnMOnProfileChanged;
                    loginButton.SetPermissions(new string[]
                    {
                        "email",
                        "public_profile"
                    });

                    MFbCallManager = ICallbackManager.Factory.Create();
                    LoginManager.Instance.RegisterCallback(MFbCallManager, this);

                    //FB accessToken
                    var accessToken = AccessToken.CurrentAccessToken;
                    var isLoggedIn = accessToken != null && !accessToken.IsExpired;
                    if (isLoggedIn && Profile.CurrentProfile != null)
                    {
                        LoginManager.Instance.LogOut();
                    }

                    string hash = Methods.App.GetKeyHashesConfigured(this);
                    Console.WriteLine(hash);
                }
                else
                {
                    FbLoginButton = FindViewById<LinearLayout>(Resource.Id.ll_fblogin);
                    FbLoginButton.Visibility = ViewStates.Gone;
                }

                //#Google
                if (AppSettings.ShowGoogleLogin)
                {
                    GoogleSignInButton = FindViewById<LinearLayout>(Resource.Id.ll_Googlelogin);
                    GoogleSignInButton.Click += GoogleSignInButtonOnClick;
                }
                else
                {
                    GoogleSignInButton = FindViewById<LinearLayout>(Resource.Id.ll_Googlelogin);
                    GoogleSignInButton.Visibility = ViewStates.Gone;
                }

                //#WoWonder 
                if (AppSettings.ShowWoWonderLogin)
                {
                    WoWonderSignInButton = FindViewById<LinearLayout>(Resource.Id.ll_WoWonderLogin);
                    WoWonderSignInButton.Click += WoWonderSignInButtonOnClick;

                    // WoWonderSignInButton.Text = GetString(Resource.String.Lbl_LoginWith) + " " + AppSettings.AppNameWoWonder;
                    WoWonderSignInButton.Visibility = ViewStates.Visible;
                }
                else
                {
                    WoWonderSignInButton = FindViewById<LinearLayout>(Resource.Id.ll_WoWonderLogin);
                    WoWonderSignInButton.Visibility = ViewStates.Gone;
                }

            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void SetDataLogin(LoginObject auth)
        {
            try
            {
                UserDetails.Username = auth.Data.UserInfo.Email;
                UserDetails.FullName = auth.Data.UserInfo.FullName;
                UserDetails.Password = auth.Data.UserInfo.Password;
                UserDetails.AccessToken = auth.Data.AccessToken;
                UserDetails.UserId = auth.Data.UserId;
                UserDetails.Status = "Pending";
                UserDetails.Cookie = auth.Data.AccessToken;
                UserDetails.Email = auth.Data.UserInfo.Email;

                Current.AccessToken = auth.Data.AccessToken;
                 
                //Insert user data to database
                var user = new DataTables.LoginTb
                {
                    UserId = UserDetails.UserId.ToString(),
                    AccessToken = UserDetails.AccessToken,
                    Cookie = UserDetails.Cookie,
                    Username = auth.Data.UserInfo.Email,
                    Password = auth.Data.UserInfo.Password,
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

        #region Abstract members
        public abstract void ToggleVisibility(bool isLoginProgress);
        #endregion

        #region Social Logins

        private string FbAccessToken;

        #region Facebook

        public void OnCancel()
        {
            try
            {
                ToggleVisibility(false);

                //SetResult(Result.Canceled);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnError(FacebookException error)
        {
            try
            {

                ToggleVisibility(false);

                // Handle e
                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), error.Message, GetText(Resource.String.Lbl_Ok));

                //SetResult(Result.Canceled);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnSuccess(Object result)
        {
            try
            {
                //var loginResult = result as LoginResult;
                //var id = AccessToken.CurrentAccessToken.UserId;

                ToggleVisibility(true);

                //SetResult(Result.Ok);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public async void OnCompleted(JSONObject json, GraphResponse response)
        {
            try
            {
                ToggleVisibility(true);

                var accessToken = AccessToken.CurrentAccessToken;
                if (accessToken != null)
                {
                    FbAccessToken = accessToken.Token;

                    //Login Api 
                    var (apiStatus, respond) = await RequestsAsync.Auth.SocialLoginAsync(FbAccessToken, "facebook", UserDetails.DeviceId);
                    switch (apiStatus)
                    {
                        case 200:
                            {
                                if (respond is LoginObject auth)
                                {
                                    //if (!string.IsNullOrEmpty(json?.ToString()))
                                    //{
                                    //    var data = json.ToString();
                                    //    var result = JsonConvert.DeserializeObject<FacebookResult>(data);
                                    //}

                                    SetDataLogin(auth);

                                    StartActivity(new Intent(this, typeof(HomeActivity)));
                                    FinishAffinity();
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
            }
            catch (Exception e)
            {
                ToggleVisibility(false);
                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), e.Message, GetText(Resource.String.Lbl_Ok));
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        //======================================================

        #region Google

        //Event Click login using google
        private void GoogleSignInButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                GetGoogleIdOption googleIdOption = new GetGoogleIdOption.Builder()
                    .SetFilterByAuthorizedAccounts(false)
                    .SetServerClientId(AppSettings.ClientId)
                    //.SetAutoSelectEnabled(false) 
                    .Build();

                GetCredentialRequest request = new GetCredentialRequest.Builder()
                    .AddCredentialOption(googleIdOption)
                    .Build();

                CancellationSignal cancellationSignal = new CancellationSignal();
                CredentialManager ??= ICredentialManager.Create(this);
                IExecutor executor = ContextCompat.GetMainExecutor(this);

                CredentialManager.GetCredentialAsync(this, request, cancellationSignal, executor, this);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private async void SetContentGoogle(string gAccessToken)
        {
            try
            {
                //Successful log in hooray!!
                if (!string.IsNullOrEmpty(gAccessToken))
                {
                    ToggleVisibility(true);

                    var (apiStatus, respond) = await RequestsAsync.Auth.SocialLoginAsync(gAccessToken, "google", UserDetails.DeviceId);
                    switch (apiStatus)
                    {
                        case 200:
                            {
                                if (respond is LoginObject auth)
                                {
                                    if (auth.Data != null)
                                    {
                                        SetDataLogin(auth);

                                        StartActivity(new Intent(this, typeof(HomeActivity)));

                                        ToggleVisibility(false);
                                        FinishAffinity();
                                    }
                                }

                                break;
                            }
                        case 400:
                            {
                                if (respond is ErrorObject error)
                                {
                                    ToggleVisibility(false);
                                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), error.ErrorData.ErrorText, GetText(Resource.String.Lbl_Ok));
                                }

                                break;
                            }
                        default:
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
            catch (Exception exception)
            {
                ToggleVisibility(false);
                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), exception.Message, GetText(Resource.String.Lbl_Ok));
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void OnError(Object e)
        {
            try
            {
                Console.WriteLine(e);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public async void OnResult(Object result)
        {
            try
            {
                if (result is GetCredentialResponse response)
                {
                    Credential credential = response.Credential;
                    if (credential is CustomCredential customCredential)
                    {
                        if (customCredential.Type == GoogleIdTokenCredential.TypeGoogleIdTokenCredential)
                        {
                            GoogleIdTokenCredential googleIdTokenCredential = GoogleIdTokenCredential.CreateFrom(credential.Data);

                            if (googleIdTokenCredential != null)
                            {
                                string email = googleIdTokenCredential.Id;
                                string firstName = googleIdTokenCredential.GivenName;
                                string lastName = googleIdTokenCredential.FamilyName;
                                string token = googleIdTokenCredential.IdToken;
                                SetContentGoogle(token);
                            }
                        } 
                    }
                    else if (credential is PasswordCredential passwordCredential)
                    {
                        Methods.HideKeyboard(this);

                        ToggleVisibility(true);
                        await AuthApi(passwordCredential.Id, passwordCredential.Password);
                    }
                }
                else if (result is CreatePublicKeyCredentialResponse credentialResponse)
                {

                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region WoWonder

        //Event Click login using WoWonder
        private void WoWonderSignInButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(this, typeof(WoWonderLoginActivity)));
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public async void LoginWoWonder(string woWonderAccessToken)
        {
            try
            {
                ToggleVisibility(true);

                if (!string.IsNullOrEmpty(woWonderAccessToken))
                {
                    //Login Api 
                    var (apiStatus, respond) = await RequestsAsync.Auth.SocialLoginAsync(woWonderAccessToken, "wowonder", UserDetails.DeviceId);
                    switch (apiStatus)
                    {
                        case 200:
                            {
                                if (respond is LoginObject auth)
                                {
                                    SetDataLogin(auth);

                                    StartActivity(new Intent(this, typeof(HomeActivity)));
                                    FinishAffinity();
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
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #endregion

        #region Permissions && Result

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                // Logins Facebook
                MFbCallManager?.OnActivityResult(requestCode, (int)resultCode, data);
                base.OnActivityResult(requestCode, resultCode, data);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                switch (requestCode)
                {
                    case 16248 when grantResults.Length > 0 && grantResults[0] == Permission.Granted:
                        if (string.IsNullOrEmpty(UserDetails.DeviceId))
                            OneSignalNotification.Instance.RegisterNotificationDevice(this);
                        break;
                    case 16248:
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long)?.Show();
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Cross App Authentication

        protected void BuildClients()
        {
            try
            {
                GetPasswordOption getPasswordOption = new GetPasswordOption();

                GetCredentialRequest getCredRequest = new GetCredentialRequest.Builder()
                    .AddCredentialOption(getPasswordOption)
                    .Build();

                CredentialManager ??= ICredentialManager.Create(this);

                CredentialManager.GetCredentialAsync(this, getCredRequest, new CancellationSignal(), new HandlerExecutor(Looper.MainLooper), this);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        private async Task AuthApi(string email, string password)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    ToggleVisibility(false);
                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_CheckYourInternetConnection), GetText(Resource.String.Lbl_Ok));
                    return;
                }
                 
                Methods.HideKeyboard(this);
                var (apiStatus, respond) = await RequestsAsync.Auth.LoginAsync(email, password, UserDetails.DeviceId);
                if (apiStatus == 200 && respond is LoginObject auth)
                {
                    if (AppSettings.EnableSmartLockForPasswords)
                        CredentialManager?.CreateCredentialAsync(this, new CreatePasswordRequest(email, password),
                            new CancellationSignal(), ContextCompat.GetMainExecutor(this), this);

                    SetDataLogin(auth);
                    StartActivity(new Intent(this, typeof(HomeActivity)));
                    Finish();
                }
                else if (apiStatus == 200)
                {
                    if (respond is LoginTwoFactorObject auth2)
                    {
                        UserDetails.UserId = Convert.ToInt32(auth2.UserId);
                        Intent intent = new Intent(this, typeof(VerificationAccountActivity));
                        intent.PutExtra("Type", "TwoFactor");
                        StartActivity(intent);
                        Finish();
                    }
                }
                else if (apiStatus == 400)
                {
                    if (respond is ErrorObject error)
                    {
                        string errorText = error.Message;
                        long errorId = error.Code;
                        if (errorId == 1)
                            Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),
                                GetString(Resource.String.Lbl_Error_1), GetText(Resource.String.Lbl_Ok));
                        else if (errorId == 2)
                            Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),
                                GetString(Resource.String.Lbl_Error_2), GetText(Resource.String.Lbl_Ok));
                        else if (errorId == 3)
                            Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),
                                GetString(Resource.String.Lbl_Error_3), GetText(Resource.String.Lbl_Ok));
                        else if (errorId == 4)
                            Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),
                                GetString(Resource.String.Lbl_Error_4), GetText(Resource.String.Lbl_Ok));
                        else if (errorId == 5)
                            Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),
                                GetString(Resource.String.Lbl_Error_5), GetText(Resource.String.Lbl_Ok));
                        else
                            Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),
                                errorText, GetText(Resource.String.Lbl_Ok));
                    }

                    ToggleVisibility(false);
                }
                else if (apiStatus == 404)
                {
                    ToggleVisibility(false);
                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), respond.ToString(), GetText(Resource.String.Lbl_Ok));
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
         
    }
}