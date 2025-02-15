//###############################################################
// Author >> Elin Doughouz 
// Copyright (c) PixelPhoto 15/07/2018 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================
//For the accuracy of the icon and logo, please use this website " http://nsimage.brosteins.com " and add images according to size in folders " mipmap " 

using Android.Graphics;
using QuickDate.Helpers.Model;
using System.Collections.Generic;

namespace QuickDate
{
    internal static class AppSettings
    {
        /// <summary>
        /// Deep Links To App Content
        /// you should add your website without http in the analytic.xml file >> ../values/analytic.xml .. line 5
        /// <string name="ApplicationUrlWeb">quickdatescript.com</string>
        /// </summary> 
        public static readonly string TripleDesAppServiceProvider = "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";

        //Main Settings >>>>>
        //********************************************************* 
        public static string Version = "3.6";
        public static readonly string ApplicationName = "QuickDate";
        public static readonly string DatabaseName = "QuickDate";

        //Main Colors >>
        //*********************************************************
        public static readonly string MainColor = "#FF007F";
        public static Color TitleTextColor = Color.Black;
        public static Color TitleTextColorDark = Color.White;

        //Language Settings >> http://www.lingoes.net/en/translator/langcode.htm
        //*********************************************************
        public static bool FlowDirectionRightToLeft = true;
        public static string Lang = ""; //Default language ar

        public static readonly Dictionary<string, string> LanguageList = new Dictionary<string, string>() //#New 
        {
            {"en", "English"},
            {"ar", "Arabic"},
        };

        //Notification Settings >>
        //*********************************************************
        public static bool ShowNotification = true;
        public static string OneSignalAppId = "c6d8ecf6-e3b8-4c49-b208-07a23364a6ed";

        //Error Report Mode
        //*********************************************************
        public static readonly bool SetApisReportMode = false;

        //Add Animation Image User
        //*********************************************************
        public static readonly bool EnableAddAnimationImageUser = false;

        //Set Theme Full Screen App
        //*********************************************************
        public static readonly bool EnableFullScreenApp = false;

        //Social Logins >>
        //If you want login with facebook or google you should change id key in the analytic.xml file or AndroidManifest.xml
        //Facebook >> ../values/analytic.xml  
        //Google >> ../Properties/AndroidManifest.xml .. line 42
        //*********************************************************
        public static readonly bool EnableSmartLockForPasswords = false;

        public static readonly bool ShowFacebookLogin = true;
        public static readonly bool ShowGoogleLogin = true;
        public static readonly bool ShowWoWonderLogin = true;
        public static readonly bool ShowSocialLoginAtRegisterScreen = true;

        public static readonly string ClientId = "716215768781-1riglii0rihhc9gmp53qad69tt8o2e03.apps.googleusercontent.com";

        public static readonly string AppNameWoWonder = "WoWonder";
        public static readonly string WoWonderDomainUri = "https://demo.wowonder.com";
        public static readonly string WoWonderAppKey = "35bf23159ca898e246e5e84069f4deba1b81ee97-60b93b3942f269c7a29a1760199642ec-46595136";

        //AdMob >> Please add the code ads in the Here and analytic.xml 
        //*********************************************************
        public static readonly ShowAds ShowAds = ShowAds.AllUsers;

        public static readonly bool RewardedAdvertisingSystem = true;//#New 

        //Three times after entering the ad is displayed
        public static readonly int ShowAdInterstitialCount = 5;
        public static readonly int ShowAdRewardedVideoCount = 5;
        public static int ShowAdNativeCount = 40;
        public static readonly int ShowAdAppOpenCount = 3;

        public static readonly bool ShowAdMobBanner = true;
        public static readonly bool ShowAdMobInterstitial = true;
        public static readonly bool ShowAdMobRewardVideo = true;
        public static readonly bool ShowAdMobNative = true;
        public static readonly bool ShowAdMobAppOpen = true;
        public static readonly bool ShowAdMobRewardedInterstitial = true;

        public static readonly string AdInterstitialKey = "ca-app-pub-5135691635931982/6657648824";
        public static readonly string AdRewardVideoKey = "ca-app-pub-5135691635931982/7559666953";
        public static readonly string AdAdMobNativeKey = "ca-app-pub-5135691635931982/2342769069";
        public static readonly string AdAdMobAppOpenKey = "ca-app-pub-5135691635931982/7036343147";
        public static readonly string AdRewardedInterstitialKey = "ca-app-pub-5135691635931982/9662506481";

        //FaceBook Ads >> Please add the code ad in the Here and analytic.xml 
        //*********************************************************
        public static readonly bool ShowFbBannerAds = false;
        public static readonly bool ShowFbInterstitialAds = false;
        public static readonly bool ShowFbRewardVideoAds = false;
        public static readonly bool ShowFbNativeAds = false;

        //YOUR_PLACEMENT_ID
        public static readonly string AdsFbBannerKey = "250485588986218_554026418632132";
        public static readonly string AdsFbInterstitialKey = "250485588986218_554026125298828";
        public static readonly string AdsFbRewardVideoKey = "250485588986218_554072818627492";
        public static readonly string AdsFbNativeKey = "250485588986218_554706301897477";

        //Ads AppLovin >> Please add the code ad in the Here 
        //*********************************************************  
        public static readonly bool ShowAppLovinBannerAds = false;
        public static readonly bool ShowAppLovinInterstitialAds = false;
        public static readonly bool ShowAppLovinRewardAds = false;

        public static readonly string AdsAppLovinBannerId = "a474c0e01669a691";
        public static readonly string AdsAppLovinInterstitialId = "e4e6aa765134f661";
        public static readonly string AdsAppLovinRewardedId = "2acfe4b28e64de66";
        //********************************************************* 

        //Last_Messages Page >>
        ///********************************************************* 
        public static readonly bool RunSoundControl = true;
        public static readonly int RefreshAppAPiSeconds = 6000; // 6 Seconds

        public static readonly int MessageRequestSpeed = 3000; // 3 Seconds

        //Set Theme Tab
        //********************************************************* 
        public static TabTheme SetTabDarkTheme = TabTheme.Light;
        public static readonly BackgroundTheme SetBackgroundTheme = BackgroundTheme.Image;

        //Bypass Web Errors  
        //*********************************************************
        public static readonly bool TurnTrustFailureOnWebException = true;
        public static readonly bool TurnSecurityProtocolType3072On = true;

        //Trending 
        //*********************************************************
        public static readonly bool ShowTrending = true;

        public static readonly bool ShowFilterBasic = true;
        public static readonly bool ShowFilterLooks = true;
        public static readonly bool ShowFilterBackground = true;
        public static readonly bool ShowFilterLifestyle = true;
        public static readonly bool ShowFilterMore = true;

        /// <summary>
        /// On main full filter view screen, reset filter option will available only on the first page by default
        /// If you want to show the reset filter option for all the pages then set "ShowResetFilterForAllPages" as true
        /// </summary>
        public static readonly bool ShowResetFilterForAllPages = true;

        //*********************************************************
        public static readonly bool RegisterEnabled = true;
        public static readonly bool BlogsEnabled = true;
        public static readonly bool PeopleILikedEnabled = true;
        public static readonly bool PeopleIDislikedEnabled = true;
        public static readonly bool FavoriteEnabled = true;

        //Premium system
        public static bool PremiumSystemEnabled = true;

        //Phone Validation system
        public static readonly bool ValidationEnabled = true;

        public static readonly int AvatarSize = 60;
        public static readonly int ImageSize = 200;

        public static readonly bool ShowTextWithSpace = true;

        /// <summary>
        /// JustWhenRegister : You can't change type gender after registering an account
        /// </summary>
        public static readonly UpdateGenderSystem UpdateGenderSystem = UpdateGenderSystem.JustWhenRegister;

        /// <summary>
        /// if notv Enable Friend System ..
        /// you should comment this lines https://prnt.sc/1d2n56g on file notifcation_bar_tabs.xml
        /// you can find this file from  Resources/xml/notifcation_bar_tabs.xml
        /// </summary>
        public static readonly bool EnableFriendSystem = true;

        public static bool ShowWalkTroutPage = true;
        public static readonly bool EnableAppFree = false;

        //Payment System (ShowPaymentCardPage >> Paypal & Stripe ) (ShowLocalBankPage >> Local Bank ) 
        //*********************************************************

        public static readonly PaymentsSystem PaymentsSystem = PaymentsSystem.All;

        /// <summary>
        /// Paypal and google pay using Braintree Gateway https://www.braintreepayments.com/
        /// 
        /// Add info keys in Payment Methods : https://prnt.sc/1z5bffc - https://prnt.sc/1z5b0yj
        /// To find your merchant ID :  https://prnt.sc/1z59dy8
        ///
        /// Tokenization Keys : https://prnt.sc/1z59smv
        /// </summary>
        public static readonly bool ShowPaypal = true;
        public static readonly string MerchantAccountId = "test";

        public static readonly string SandboxTokenizationKey = "sandbox_kt2f6mdh_hf4ccmn4dfy45******";
        public static readonly string ProductionTokenizationKey = "production_t2wns2y2_dfy45******";

        public static readonly bool ShowCreditCard = true;
        public static readonly bool ShowBankTransfer = true;

        /// <summary>
        /// if you want this feature enabled go to Properties -> AndroidManefist.xml and remove comments from below code
        /// <uses-permission android:name="com.android.vending.BILLING" />
        /// </summary>
        public static readonly bool ShowInAppBilling = true;


        public static readonly bool ShowCashFree = true;
        /// <summary>
        /// Currencies : http://prntscr.com/u600ok
        /// </summary>
        public static readonly string CashFreeCurrency = "INR";

        /// <summary>
        /// If you want RazorPay you should change id key in the analytic.xml file
        /// razorpay_api_Key >> .. line 28 
        /// </summary>
        public static readonly bool ShowRazorPay = true;
        /// <summary>
        /// Currencies : https://razorpay.com/accept-international-payments
        /// </summary>
        public static readonly string RazorPayCurrency = "INR";

        public static readonly bool ShowAuthorizeNet = true;
        public static readonly bool ShowSecurionPay = true;
        public static readonly bool ShowIyziPay = true;
        public static readonly bool ShowPayStack = true;
        public static readonly bool ShowAamarPay = true;

        /// <summary>
        /// FlutterWave get Api Keys From https://app.flutterwave.com/dashboard/settings/apis/live
        /// </summary>
        public static readonly bool ShowFlutterWave = true;
        public static readonly string FlutterWaveCurrency = "NGN";
        public static readonly string FlutterWavePublicKey = "FLWPUBK_TEST-9c877b3110438191127e631c8*****";
        public static readonly string FlutterWaveEncryptionKey = "FLWSECK_TEST298f1****";

        //*********************************************************

        //Settings Page >>  
        //********************************************************* 
        public static readonly bool ShowSettingsAccount = true;
        public static readonly bool ShowSettingsSocialLinks = true;
        public static readonly bool ShowSettingsPassword = true;
        public static readonly bool ShowSettingsBlockedUsers = true;
        public static readonly bool ShowSettingsDeleteAccount = true;
        public static readonly bool ShowSettingsTwoFactor = true;
        public static readonly bool ShowSettingsManageSessions = true;
        public static readonly bool ShowSettingsWithdrawals = true;
        public static readonly bool ShowSettingsMyAffiliates = true;
        public static readonly bool ShowSettingsTransactions = true;

        /// <summary>
        /// if you want this feature enabled go to Properties -> AndroidManefist.xml and remove comments from below code
        /// <uses-permission android:name="android.permission.READ_CONTACTS" />
        /// <uses-permission android:name="android.permission.READ_PHONE_NUMBERS" />
        /// </summary>
        public static readonly bool InvitationSystem = true;
        /// <summary>
        /// If want to have limit on messages then set this variable as 'true'
        /// If you set the limit on messages then non pro user will able to send only 5 messages
        /// </summary>
        public static readonly bool ShouldHaveLimitOnMessages = false;
        public static int MaxMessageLimitForNonProUser = 5;
        //********************************************************* 

        public static readonly bool ShowSettingsRateApp = true;
        public static readonly int ShowRateAppCount = 5;

        public static readonly bool ShowSettingsUpdateManagerApp = false;

        public static readonly bool OpenVideoFromApp = true;
        public static readonly bool OpenImageFromApp = true;

        /// <summary>
        /// true => Only over 18 years old
        /// false => all 
        /// </summary>
        public static readonly bool IsUserYearsOld = true;

        //********************************************************* 
        public static readonly bool ShowLive = true;
        public static readonly string AppIdAgoraLive = "7f1cb06a3b4d4e84965ea6d3b3e16a8a";
        //*********************************************************
    }
}