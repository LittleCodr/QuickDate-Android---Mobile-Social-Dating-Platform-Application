using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Anjo.Android.GoogleAds;
using Com.Applovin.Mediation;
using Com.Applovin.Mediation.Adapters;
using Com.Google.Android.Gms.Ads;
using Com.Google.Android.Gms.Ads.Admanager;
using Com.Google.Android.Gms.Ads.Appopen;
using Com.Google.Android.Gms.Ads.Initialization;
using Com.Google.Android.Gms.Ads.Interstitial;
using Com.Google.Android.Gms.Ads.Nativead;
using Com.Google.Android.Gms.Ads.Rewarded;
using Com.Google.Android.Gms.Ads.Rewardedinterstitial;
using Java.Lang;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Utils;
using QuickDateClient.Requests;
using Xamarin.Google.UserMesssagingPlatform;
using Exception = System.Exception;
using NativeAd = Com.Google.Android.Gms.Ads.Nativead.NativeAd;
using Object = Java.Lang.Object;

namespace QuickDate.Helpers.Ads
{
    public static class AdsGoogle
    {
        private static int CountInterstitial;
        private static int CountRewarded;
        private static int CountAppOpen;
        private static int CountRewardedInterstitial;
        public static readonly List<NativeAd> NativeAdsPool = new List<NativeAd>();

        #region Interstitial

        private class AdMobInterstitial : AnjoInterstitialLoadCallback
        {
            private Activity ActivityContext;

            public void Show(Activity context)
            {
                try
                {
                    ActivityContext = context;

                    var adRequest = new AdRequest.Builder();
                    if (AppSettings.ShowAppLovinInterstitialAds)
                    {
                        Bundle extras = new AppLovinExtras.Builder()
                            ?.SetMuteAudio(true)
                            ?.Build();

                        adRequest.AddNetworkExtrasBundle(Class.FromType(typeof(AppLovinMediationAdapter)), extras);
                    }

                    InterstitialAd.Load(context, AppSettings.AdInterstitialKey, adRequest.Build(), this);
                }
                catch (Exception exception)
                {
                    Methods.DisplayReportResultTrack(exception);
                }
            }

            public override void OnAdLoaded(InterstitialAd p0)
            {
                try
                {
                    p0.FullScreenContentCallback = new MyFullScreenContentCallback();
                    p0?.Show(ActivityContext);
                    base.OnAdLoaded(p0);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public override void OnAdFailedToLoad(LoadAdError p0)
            {
                Log.Debug("Google-Ads", "I_Ad AdMobInterstitial Load Failed: " + p0.Message);
                base.OnAdFailedToLoad(p0);
            }
        }

        public static void Ad_Interstitial(Activity context, bool openDir = false)
        {
            try
            {
                if (QuickDateTools.GetStatusAds() && AppSettings.ShowAdMobInterstitial)
                {
                    if (openDir)
                    {
                        AdMobInterstitial ads = new AdMobInterstitial();
                        ads.Show(context);
                        return;
                    }

                    if (CountInterstitial == AppSettings.ShowAdInterstitialCount)
                    {
                        CountInterstitial = 0;
                        AdMobInterstitial ads = new AdMobInterstitial();
                        ads.Show(context);
                        return;
                    }

                    Ad_AppOpenManager(context);

                    CountInterstitial++;
                }
                else
                {
                    Ad_AppOpenManager(context);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Native 

        public class AdMobNative : AdListener, NativeAd.IOnNativeAdLoadedListener
        {
            private TemplateView Template;
            private Activity Context;
            private AdLoader AdLoader;

            public void BindAdMobNative(Activity act)
            {
                try
                {
                    if (QuickDateTools.GetStatusAds() && AppSettings.ShowAdMobNative)
                    {
                        AdLoader.Builder builder = new AdLoader.Builder(act, AppSettings.AdAdMobNativeKey);
                        builder.ForNativeAd(this);

                        VideoOptions videoOptions = new VideoOptions.Builder()
                            .SetCustomControlsRequested(true)
                            .SetStartMuted(true)
                            .Build();

                        NativeAdOptions adOptions = new NativeAdOptions.Builder()
                            .SetMediaAspectRatio(IMediaAspectRatio.Portrait)
                            .SetVideoOptions(videoOptions)
                            .Build();

                        builder.WithNativeAdOptions(adOptions);

                        AdLoader = builder.WithAdListener(this).Build();
                        AdLoader.LoadAds(new AdRequest.Builder().Build(), 5);
                    }
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public void ShowAd(Activity context, TemplateView template = null)
            {
                try
                {
                    Context = context;

                    Template = template ?? Context.FindViewById<TemplateView>(Resource.Id.my_template);
                    if (Template != null)
                    {
                        var ad = NativeAdsPool?.FirstOrDefault();
                        if (ad != null && NativeAdsPool?.Count > 1 && AppSettings.ShowAdMobNative)
                        {
                            NativeTemplateStyle styles = new NativeTemplateStyle.Builder().Build();

                            if (Template.GetTemplateTypeName() == TemplateView.NativeContentAd)
                            {
                                Template.NativeContentAdView(ad);
                            }
                            else
                            {
                                Template.SetStyles(styles);
                                Template.SetNativeAd(ad);
                            }

                            Template.Visibility = ViewStates.Visible;

                            NativeAdsPool.Remove(ad);
                        }
                        else
                        {
                            Template.Visibility = ViewStates.Gone;

                            if (AppSettings.ShowAdMobNative)
                            {
                                BindAdMobNative(context);
                            }
                            else
                            {
                                Template.Visibility = ViewStates.Gone;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public void OnNativeAdLoaded(NativeAd ad)
            {
                try
                {
                    // some code that displays the ad.
                    if (AdLoader != null && AdLoader.IsLoading)
                    {
                        // The AdLoader is still loading ads.
                        // Expect more adLoaded or onAdFailedToLoad callbacks.
                        NativeAdsPool.Add(ad);
                    }
                    else
                    {
                        // The AdLoader has finished loading ads. 
                        if (Template != null)
                        {
                            NativeTemplateStyle styles = new NativeTemplateStyle.Builder().Build();

                            if (Template.GetTemplateTypeName() == TemplateView.NativeContentAd)
                            {
                                Template.NativeContentAdView(ad);
                            }
                            else
                            {
                                Template.SetStyles(styles);
                                Template.SetNativeAd(ad);
                            }

                            Template.Visibility = ViewStates.Visible;
                        }
                    }
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public override void OnAdFailedToLoad(LoadAdError p0)
            {
                try
                {
                    Log.Debug("Google-Ads", "I_Ad AdMobNative Load Failed: " + p0.Message);

                    if (Template != null)
                        Template.Visibility = ViewStates.Gone;
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }
        }

        public static void Ad_AdMobNative(Activity context, TemplateView template = null)
        {
            try
            {
                if (QuickDateTools.GetStatusAds() && AppSettings.ShowAdMobNative)
                {
                    AdMobNative ads = new AdMobNative();
                    ads.ShowAd(context, template);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Rewarded

        public class AdMobRewardedVideo : AnjoRewardedLoadCallback
        {
            private Activity Context;
            public void ShowAd(Activity context)
            {
                try
                {
                    Context = context;

                    var adRequest = new AdRequest.Builder();
                    if (AppSettings.ShowAppLovinRewardAds)
                    {
                        Bundle extras = new AppLovinExtras.Builder()
                            ?.SetMuteAudio(true)
                            ?.Build();

                        adRequest.AddNetworkExtrasBundle(Class.FromType(typeof(AppLovinMediationAdapter)), extras);
                    }

                    RewardedAd.Load(context, AppSettings.AdRewardVideoKey, adRequest.Build(), this);
                }
                catch (Exception exception)
                {
                    Methods.DisplayReportResultTrack(exception);
                }
            }

            public override void OnAdLoaded(RewardedAd p0)
            {
                try
                {
                    p0.FullScreenContentCallback = new MyFullScreenContentCallback();
                    p0?.Show(Context, new MyUserEarnedRewardListener(Context));
                    base.OnAdLoaded(p0);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public override void OnAdFailedToLoad(LoadAdError p0)
            {
                Log.Debug("Google-Ads", "I_Ad AdMobRewardedVideo Load Failed: " + p0.Message);
                base.OnAdFailedToLoad(p0);
            }
        }

        public static void Ad_RewardedVideo(Activity context)
        {
            try
            {
                if (QuickDateTools.GetStatusAds() && AppSettings.ShowAdMobRewardVideo)
                {
                    if (CountRewarded == AppSettings.ShowAdRewardedVideoCount)
                    {
                        CountRewarded = 0;
                        AdMobRewardedVideo ads = new AdMobRewardedVideo();
                        ads.ShowAd(context);
                        return;
                    }

                    if (AppSettings.ShowFbInterstitialAds)
                        AdsFacebook.InitInterstitial(context);
                    else if (AppSettings.ShowAppLovinInterstitialAds)
                        AdsAppLovin.Ad_Interstitial(context);

                    CountRewarded++;
                }
                else
                {
                    if (AppSettings.ShowFbInterstitialAds)
                        AdsFacebook.InitInterstitial(context);
                    else if (AppSettings.ShowAppLovinInterstitialAds)
                        AdsAppLovin.Ad_Interstitial(context);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Banner

        public static void InitAdView(AdView mAdView, RecyclerView mRecycler)
        {
            try
            {
                if (mAdView == null) return;

                if (QuickDateTools.GetStatusAds() && AppSettings.ShowAdMobBanner)
                {
                    mAdView.Visibility = ViewStates.Visible;
                    var adRequest = new AdRequest.Builder();
                    if (AppSettings.ShowAppLovinBannerAds)
                    {
                        Bundle extras = new AppLovinExtras.Builder()
                            ?.SetMuteAudio(true)
                            ?.Build();

                        adRequest.AddNetworkExtrasBundle(Class.FromType(typeof(AppLovinMediationAdapter)), extras);
                    }

                    mAdView.LoadAd(adRequest.Build());
                    mAdView.AdListener = new MyAdListener(mAdView, mRecycler);
                }
                else
                {
                    mAdView.Pause();
                    mAdView.Visibility = ViewStates.Gone;
                    if (mRecycler != null) Methods.SetMargin(mRecycler, 0, 0, 0, 0);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public static void LifecycleAdView(AdView mAdView, string lifecycle)
        {
            try
            {
                if (mAdView == null) return;

                if (QuickDateTools.GetStatusAds() && AppSettings.ShowAdMobBanner)
                {
                    switch (lifecycle)
                    {
                        case "Resume":
                            mAdView.Resume();
                            break;
                        case "Pause":
                            mAdView.Pause();
                            break;
                        case "Destroy":
                            mAdView.Destroy();
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public static AdView InitBannerAdView(Activity activity, LinearLayout adContainer, RecyclerView mRecycler, AdSize adSize = null)
        {
            try
            {
                if (QuickDateTools.GetStatusAds() && AppSettings.ShowAdMobBanner)
                {
                    AdView mAdView = new AdView(activity);

                    if (adSize != null)
                        mAdView.AdSize = adSize;
                    else
                        mAdView.AdSize = GetFullWidthAdaptiveSize(activity);

                    mAdView.AdUnitId = activity.GetText(Resource.String.banner_ad_unit_id);

                    var adRequest = new AdRequest.Builder();
                    mAdView.LoadAd(adRequest.Build());
                    mAdView.AdListener = new MyAdListener(mAdView, mRecycler);

                    // Add the ad view to your activity layout
                    adContainer.AddView(mAdView);

                    return mAdView;
                }

                if (mRecycler != null)
                    Methods.SetMargin(mRecycler, 0, 0, 0, 0);
                return null;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        private static AdSize GetFullWidthAdaptiveSize(Context context)
        {
            try
            {
                var display = context?.GetSystemService(Context.WindowService).JavaCast<IWindowManager>()?.DefaultDisplay;

                DisplayMetrics outMetrics = new DisplayMetrics();
                display.GetMetrics(outMetrics);

                float widthPixels = outMetrics.WidthPixels;
                float density = outMetrics.Density;

                int adWidth = (int)(widthPixels / density);
                return AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSize(context, adWidth);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return AdSize.Banner;
            }
        }

        private class MyAdListener : AdListener
        {
            private readonly AdView MAdView;
            private readonly RecyclerView MRecycler;
            public MyAdListener(AdView mAdView, RecyclerView mRecycler)
            {
                MAdView = mAdView;
                MRecycler = mRecycler;
            }

            public override void OnAdFailedToLoad(LoadAdError p0)
            {
                try
                {
                    Log.Debug("Google-Ads", "I_Ad AdView Load Failed: " + p0.Message);

                    MAdView.Visibility = ViewStates.Gone;
                    if (MRecycler != null) Methods.SetMargin(MRecycler, 0, 0, 0, 0);
                    base.OnAdFailedToLoad(p0);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }


            public override void OnAdLoaded()
            {
                try
                {
                    MAdView.Visibility = ViewStates.Visible;

                    Resources r = Application.Context.Resources;
                    int px = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, MAdView.AdSize.Height, r.DisplayMetrics);
                    if (MRecycler != null) Methods.SetMargin(MRecycler, 0, 0, 0, px);

                    base.OnAdLoaded();
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }
        }

        #endregion

        #region Manager

        public static void InitAdManagerAdView(AdManagerAdView mAdView)
        {
            try
            {
                if (mAdView == null) return;

                if (QuickDateTools.GetStatusAds() && AppSettings.ShowAdMobBanner)
                {
                    mAdView.Visibility = ViewStates.Visible;
                    var adRequest = new AdRequest.Builder();
                    if (AppSettings.ShowAppLovinBannerAds)
                    {
                        Bundle extras = new AppLovinExtras.Builder()
                            ?.SetMuteAudio(true)
                            ?.Build();

                        adRequest.AddNetworkExtrasBundle(Class.FromType(typeof(AppLovinMediationAdapter)), extras);
                    }

                    mAdView.AdListener = new MyAdManagerAdViewListener(mAdView);
                    mAdView.LoadAd(adRequest.Build());
                }
                else
                {
                    mAdView.Pause();
                    mAdView.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public static void LifecycleAdManagerAdView(AdManagerAdView mAdView, string lifecycle)
        {
            try
            {
                if (mAdView == null) return;

                if (QuickDateTools.GetStatusAds() && AppSettings.ShowAdMobBanner)
                {
                    switch (lifecycle)
                    {
                        case "Resume":
                            mAdView.Resume();
                            break;
                        case "Pause":
                            mAdView.Pause();
                            break;
                        case "Destroy":
                            mAdView.Destroy();
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private class MyAdManagerAdViewListener : AdListener
        {
            private readonly AdManagerAdView MAdView;
            public MyAdManagerAdViewListener(AdManagerAdView mAdView)
            {
                MAdView = mAdView;
            }

            public override void OnAdFailedToLoad(LoadAdError p0)
            {
                try
                {
                    Log.Debug("Google-Ads", "I_Ad AdManagerAdView Load Failed: " + p0.Message);

                    MAdView.Visibility = ViewStates.Gone;
                    base.OnAdFailedToLoad(p0);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public override void OnAdLoaded()
            {
                try
                {
                    MAdView.Visibility = ViewStates.Visible;
                    base.OnAdLoaded();
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }
        }

        #endregion

        #region AppOpen

        public static void Ad_AppOpenManager(Activity context)
        {
            try
            {
                if (QuickDateTools.GetStatusAds() && AppSettings.ShowAdMobAppOpen)
                {
                    if (CountAppOpen == AppSettings.ShowAdAppOpenCount)
                    {
                        CountAppOpen = 0;

                        AppOpenManager appOpenManager = new AppOpenManager(context);
                        appOpenManager.ShowAdIfAvailable();
                        return;
                    }

                    if (AppSettings.ShowFbInterstitialAds)
                        AdsFacebook.InitInterstitial(context);
                    else if (AppSettings.ShowAppLovinInterstitialAds)
                        AdsAppLovin.Ad_Interstitial(context);

                    CountAppOpen++;
                }
                else
                {
                    if (AppSettings.ShowFbInterstitialAds)
                        AdsFacebook.InitInterstitial(context);
                    else if (AppSettings.ShowAppLovinInterstitialAds)
                        AdsAppLovin.Ad_Interstitial(context);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private class AppOpenManager : AnjoAppOpenLoadCallback
        {
            private readonly Activity MostCurrentActivity;
            private static AppOpenAd Ad;

            public AppOpenManager(Activity context)
            {
                try
                {
                    MostCurrentActivity = context;
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public void ShowAdIfAvailable()
            {
                try
                {
                    var adRequest = new AdRequest.Builder();
                    if (AppSettings.ShowAppLovinInterstitialAds)
                    {
                        Bundle extras = new AppLovinExtras.Builder()
                            ?.SetMuteAudio(true)
                            ?.Build();

                        adRequest.AddNetworkExtrasBundle(Class.FromType(typeof(AppLovinMediationAdapter)), extras);
                    }

                    AppOpenAd.Load(MostCurrentActivity, AppSettings.AdAdMobAppOpenKey, adRequest.Build(), this);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public override void OnAdLoaded(AppOpenAd p0)
            {
                try
                {
                    base.OnAdLoaded(p0);

                    Ad = p0;
                    p0.FullScreenContentCallback = new MyFullScreenContentCallback();
                    Ad.Show(MostCurrentActivity);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public override void OnAdFailedToLoad(LoadAdError p0)
            {
                Log.Debug("Google-Ads", "I_Ad AppOpenManager Load Failed: " + p0.Message);
                base.OnAdFailedToLoad(p0);
            }

        }

        #endregion

        #region RewardedInterstitial

        public class AdMobRewardedInterstitial : AnjoRewardedInterstitialLoadCallback
        {
            private Activity Context;
            public void ShowAd(Activity context)
            {
                try
                {
                    Context = context;

                    var adRequest = new AdRequest.Builder();
                    if (AppSettings.ShowAppLovinRewardAds)
                    {
                        Bundle extras = new AppLovinExtras.Builder()
                            ?.SetMuteAudio(true)
                            ?.Build();

                        adRequest.AddNetworkExtrasBundle(Class.FromType(typeof(AppLovinMediationAdapter)), extras);
                    }

                    // Use an activity context to get the rewarded video instance. 
                    RewardedInterstitialAd.Load(context, AppSettings.AdRewardedInterstitialKey, adRequest.Build(), this);
                }
                catch (Exception exception)
                {
                    Methods.DisplayReportResultTrack(exception);
                }
            }

            public override void OnAdLoaded(RewardedInterstitialAd p0)
            {
                try
                {
                    p0.FullScreenContentCallback = new MyFullScreenContentCallback();
                    p0?.Show(Context, new MyUserEarnedRewardListener(Context));
                    base.OnAdLoaded(p0);
                }
                catch (Exception exception)
                {
                    Methods.DisplayReportResultTrack(exception);
                }
            }

            public override void OnAdFailedToLoad(LoadAdError p0)
            {
                Log.Debug("Google-Ads", "I_Ad AdMobRewardedInterstitial Load Failed: " + p0.Message);
                base.OnAdFailedToLoad(p0);
            }
        }

        public static void Ad_RewardedInterstitial(Activity context)
        {
            try
            {
                if (QuickDateTools.GetStatusAds() && AppSettings.ShowAdMobRewardedInterstitial)
                {
                    if (CountRewardedInterstitial == AppSettings.ShowAdRewardedVideoCount)
                    {
                        CountRewardedInterstitial = 0;
                        AdMobRewardedInterstitial ads = new AdMobRewardedInterstitial();
                        ads.ShowAd(context);
                        return;
                    }

                    if (AppSettings.ShowFbRewardVideoAds)
                        AdsFacebook.InitRewardVideo(context);
                    else if (AppSettings.ShowAppLovinRewardAds)
                        AdsAppLovin.Ad_Rewarded(context);

                    CountRewardedInterstitial++;
                }
                else
                {
                    if (AppSettings.ShowFbRewardVideoAds)
                        AdsFacebook.InitRewardVideo(context);
                    else if (AppSettings.ShowAppLovinRewardAds)
                        AdsAppLovin.Ad_Rewarded(context);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        private class MyUserEarnedRewardListener : Object, IOnUserEarnedRewardListener
        {
            private readonly Activity Activity;
            public MyUserEarnedRewardListener(Activity context)
            {
                Activity = context;
            }

            public void OnUserEarnedReward(IRewardItem rewardItem)
            {
                try
                {
                    // Handle the reward.
                    Console.WriteLine("The user earned the reward.");
                    int rewardAmount = rewardItem.Amount;
                    string rewardType = rewardItem.Type;

                    if (!AppSettings.RewardedAdvertisingSystem)
                        return;

                    if (!Methods.CheckConnectivity())
                        Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    else
                    {
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Auth.SetCreditAsync(ListUtils.SettingsSiteList?.CostAdmob?.ToString(), "", "AdMob") });
                        Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_PointsAdded), ToastLength.Short)?.Show();
                    }
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }
        }

        private class MyFullScreenContentCallback : FullScreenContentCallback
        {
            /// <summary>
            /// Called when a click is recorded for an ad
            /// </summary>
            public override void OnAdClicked()
            {
                base.OnAdClicked();
                Console.WriteLine("On Ad Clicked.");
            }

            /// <summary>
            /// Called when ad is dismissed
            /// Set the ad reference to null so you don't show the ad a second time.
            /// </summary>
            public override void OnAdDismissedFullScreenContent()
            {
                base.OnAdDismissedFullScreenContent();
                Console.WriteLine("On Ad Dismissed Full Screen Content.");
            }

            /// <summary>
            /// Called when ad fails to show
            /// </summary>
            /// <param name="p0"></param>
            public override void OnAdFailedToShowFullScreenContent(AdError p0)
            {
                base.OnAdFailedToShowFullScreenContent(p0);
                Console.WriteLine("On Ad Failed To Show Full Screen Content.");
            }

            /// <summary>
            /// Called when an impression is recorded for an ad.
            /// </summary>
            public override void OnAdImpression()
            {
                base.OnAdImpression();
                Console.WriteLine("On Ad Impression.");
            }

            /// <summary>
            /// Called when ad is shown
            /// </summary>
            public override void OnAdShowedFullScreenContent()
            {
                base.OnAdShowedFullScreenContent();
                Console.WriteLine("On Ad Showed FullScreen Content.");
            }
        }

        public static class InitializeAdsGoogle
        {
            public static void Initialize(Activity context)
            {
                context.RunOnUiThread(() =>
                {
                    try
                    {
                        if (AppSettings.ShowAdMobBanner || AppSettings.ShowAdMobInterstitial || AppSettings.ShowAdMobRewardVideo || AppSettings.ShowAdMobNative || AppSettings.ShowAdMobAppOpen || AppSettings.ShowAdMobRewardedInterstitial)
                        {
                            RequestConfiguration configuration = new RequestConfiguration.Builder().SetTestDeviceIds(new List<string>
                            {
                                //"5BCFF0AAE83AF424648A954038C71DE6", "A5E3E2068BD88202CBC281AD76984BEE" , AdRequest.DeviceIdEmulator
                                // UserDetails.AdvertisingId
                            }).Build();
                            MobileAds.RequestConfiguration = configuration;

                            ConsentDebugSettings debugSettings = new ConsentDebugSettings.Builder(context)
                                .AddTestDeviceHashedId("")
                                .Build();

                            // Set tag for under age of consent. false means users are not under age.
                            ConsentRequestParameters param = new ConsentRequestParameters.Builder()
                                .SetTagForUnderAgeOfConsent(false)
                                .SetAdMobAppId(context.GetText(Resource.String.admob_app_id))
                                .SetConsentDebugSettings(debugSettings)
                                .Build();

                            var consentInformation = UserMessagingPlatform.GetConsentInformation(context);
                            var requestParameters = new MyConsentRequestParameters(context);
                            consentInformation.RequestConsentInfoUpdate(context, param, requestParameters, requestParameters);
                            UserMessagingPlatform.ShowPrivacyOptionsForm(context, new MyConsentFormOnConsentFormDismissedListener(context));
                        }
                    }
                    catch (Exception e)
                    {
                        Methods.DisplayReportResultTrack(e);
                    }
                });
            }

            private class MyConsentFormOnConsentFormDismissedListener : Object, IConsentFormOnConsentFormDismissedListener
            {
                private readonly Activity Context;
                public MyConsentFormOnConsentFormDismissedListener(Activity context)
                {
                    Context = context;
                }

                public void OnConsentFormDismissed(FormError p0)
                {

                }
            }

            private class MyInitializationCompleteListener : Object, IOnInitializationCompleteListener
            {
                private readonly Activity Context;
                public MyInitializationCompleteListener(Activity context)
                {
                    Context = context;
                }

                public void OnInitializationComplete(IInitializationStatus p0)
                {
                    try
                    {
                        AdMobNative ads = new AdMobNative();
                        ads.BindAdMobNative(Context);
                    }
                    catch (Exception e)
                    {
                        Methods.DisplayReportResultTrack(e);
                    }
                }
            }

            private class MyConsentRequestParameters : Object, IConsentInformationOnConsentInfoUpdateSuccessListener, IConsentInformationOnConsentInfoUpdateFailureListener
            {
                private readonly Activity Context;
                public MyConsentRequestParameters(Activity context)
                {
                    Context = context;
                }

                public void OnConsentInfoUpdateSuccess()
                {
                    try
                    {
                        MobileAds.Initialize(Context, new MyInitializationCompleteListener(Context));
                    }
                    catch (Exception e)
                    {
                        Methods.DisplayReportResultTrack(e);
                    }
                }

                public void OnConsentInfoUpdateFailure(FormError loadAndShowError)
                {
                    try
                    {
                        if (loadAndShowError != null)
                        {
                            // Consent gathering failed.
                            Console.WriteLine("ErrorCodeData: " + loadAndShowError.ErrorCodeData() + "\nMessage: " + loadAndShowError.Message);
                            return;
                        }

                        // Consent has been gathered.
                        MobileAds.Initialize(Context, new MyInitializationCompleteListener(Context));
                    }
                    catch (Exception e)
                    {
                        Methods.DisplayReportResultTrack(e);
                    }
                }
            }

        }
    }
}