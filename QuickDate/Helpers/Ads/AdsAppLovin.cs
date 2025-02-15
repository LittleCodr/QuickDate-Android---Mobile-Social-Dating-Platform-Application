using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content.Res;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Com.Applovin.Adview;
using Com.Applovin.Mediation;
using Com.Applovin.Mediation.Ads;
using Com.Applovin.Sdk;
using Java.Util.Concurrent;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Utils;
using QuickDateClient.Requests;
using Object = Java.Lang.Object;

namespace QuickDate.Helpers.Ads
{
    public class AdsAppLovin
    {
        private static int CountInterstitial = 1;
        private static int CountRewarded = 1;

        #region Banner

        public static void InitBannerAd(Activity context, LinearLayout adContainer, RecyclerView mRecycler)
        {
            try
            {
                if (adContainer == null)
                    return;

                if (QuickDateTools.GetStatusAds() && AppSettings.ShowAppLovinBannerAds)
                {
                    //Remove previous ad view if present.
                    if (adContainer.ChildCount > 0)
                        adContainer.RemoveAllViews();

                    MaxAdView adView = new MaxAdView(AppSettings.AdsAppLovinBannerId, context);
                    adView.SetListener(new AdsAdViewListener(adView, adContainer, mRecycler));

                    int px = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 50, context.Resources.DisplayMetrics);
                    adView.LayoutParameters = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, px);

                    adContainer.AddView(adView);

                    // Load the ad
                    adView.LoadAd();
                    adView.StartAutoRefresh();
                }
                else
                {
                    adContainer.Visibility = ViewStates.Gone;
                    if (mRecycler != null) Methods.SetMargin(mRecycler, 0, 0, 0, 0);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private class AdsAdViewListener : Object, IMaxAdViewAdListener
        {
            private readonly LinearLayout AdContainer;
            private readonly RecyclerView MRecycler;
            private readonly MaxAdView MaxAdView;

            public AdsAdViewListener(MaxAdView adView, LinearLayout adContainer, RecyclerView mRecycler)
            {
                try
                {
                    MaxAdView = adView;
                    AdContainer = adContainer;
                    MRecycler = mRecycler;
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public void OnAdClicked(IMaxAd ad)
            {

            }

            public void OnAdDisplayFailed(IMaxAd ad, IMaxError p1)
            {
                try
                {
                    AdContainer.Visibility = ViewStates.Gone;
                    if (MRecycler != null) Methods.SetMargin(MRecycler, 0, 0, 0, 0);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public void OnAdDisplayed(IMaxAd ad)
            {

            }

            public void OnAdHidden(IMaxAd ad)
            {
                try
                {
                    AdContainer.Visibility = ViewStates.Gone;
                    if (MRecycler != null) Methods.SetMargin(MRecycler, 0, 0, 0, 0);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public void OnAdLoadFailed(string adUnitId, IMaxError error)
            {
                try
                {
                    AdContainer.Visibility = ViewStates.Gone;
                    if (MRecycler != null) Methods.SetMargin(MRecycler, 0, 0, 0, 0);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public void OnAdLoaded(IMaxAd ad)
            {
                try
                {
                    AdContainer.Visibility = ViewStates.Visible;

                    Resources r = Application.Context.Resources;
                    int px = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, MaxAdView.Height, r.DisplayMetrics);
                    if (MRecycler != null) Methods.SetMargin(MRecycler, 0, 0, 0, px);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public void OnAdCollapsed(IMaxAd ad)
            {

            }

            public void OnAdExpanded(IMaxAd ad)
            {

            }
        }

        #endregion

        #region Interstitial

        public static void Ad_Interstitial(Activity context)
        {
            try
            {
                if (QuickDateTools.GetStatusAds() && AppSettings.ShowAppLovinInterstitialAds)
                {
                    if (CountInterstitial == AppSettings.ShowAdInterstitialCount)
                    {
                        CountInterstitial = 1;
                        InitInterstitialAd(context);
                    }
                    else
                        CountInterstitial++;
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private static void InitInterstitialAd(Activity context)
        {
            try
            {
                MaxInterstitialAd interstitialAd = new MaxInterstitialAd(AppSettings.AdsAppLovinInterstitialId, context);
                interstitialAd.SetExtraParameter("container_view_ads", "true");
                interstitialAd.SetListener(new MyInterstitialMaxAdListener(context, interstitialAd));

                // Load the first ad
                interstitialAd.LoadAd();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private class MyInterstitialMaxAdListener : Object, IMaxAdListener
        {
            private readonly MaxInterstitialAd InterstitialAd;
            private readonly Activity Activity;
            private int RetryAttempt;

            public MyInterstitialMaxAdListener(Activity activity, MaxInterstitialAd interstitialAd)
            {
                try
                {
                    Activity = activity;
                    InterstitialAd = interstitialAd;
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public void OnAdClicked(IMaxAd maxAd)
            {

            }

            public void OnAdDisplayFailed(IMaxAd maxAd, IMaxError error)
            {
                try
                {
                    // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
                    InterstitialAd?.LoadAd();
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public void OnAdDisplayed(IMaxAd maxAd)
            {

            }

            public void OnAdHidden(IMaxAd maxAd)
            {
                try
                {
                    // Interstitial ad is hidden. Pre-load the next ad

                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public void OnAdLoadFailed(string adUnitId, IMaxError error)
            {
                try
                {
                    // Interstitial ad failed to load 
                    // AppLovin recommends that you retry with exponentially higher delays up to a maximum delay (in this case 64 seconds)

                    RetryAttempt++;
                    long delayMillis = TimeUnit.Seconds.ToMillis((long)Math.Pow(2, Math.Min(6, RetryAttempt)));
                    new Handler(Looper.MainLooper).PostDelayed(() =>
                    {
                        InterstitialAd?.LoadAd();
                    }, delayMillis);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public void OnAdLoaded(IMaxAd maxAd)
            {
                try
                {
                    // Interstitial ad is ready to be shown. interstitialAd.isReady() will now return 'true'
                    //if (InterstitialAd is { IsReady: true })
                    //    InterstitialAd.ShowAd();

                    // Reset retry attempt
                    RetryAttempt = 0;
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }
        }

        #endregion

        #region Rewarded

        public static void Ad_Rewarded(Activity context)
        {
            try
            {
                if (QuickDateTools.GetStatusAds() && AppSettings.ShowAppLovinInterstitialAds)
                {
                    if (CountRewarded == AppSettings.ShowAdRewardedVideoCount)
                    {
                        CountRewarded = 1;
                        InitRewardedAd(context);
                    }
                    else
                        CountRewarded++;
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private static void InitRewardedAd(Activity context)
        {
            try
            {
                MaxRewardedAd rewardedAd = MaxRewardedAd.GetInstance(AppSettings.AdsAppLovinRewardedId, context);
                rewardedAd.SetListener(new MyRewardedAdListener(context, rewardedAd));

                rewardedAd.LoadAd();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private class MyRewardedAdListener : Object, IMaxRewardedAdListener
        {
            private readonly MaxRewardedAd RewardedAd;
            private readonly Activity Activity;
            private int RetryAttempt;

            public MyRewardedAdListener(Activity activity, MaxRewardedAd rewardedAd)
            {
                try
                {
                    Activity = activity;
                    RewardedAd = rewardedAd;
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public void OnAdClicked(IMaxAd ad)
            {

            }

            public void OnAdDisplayFailed(IMaxAd ad, IMaxError p1)
            {
                try
                {
                    // Rewarded ad failed to display. We recommend loading the next ad
                    RewardedAd?.LoadAd();
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public void OnAdDisplayed(IMaxAd ad)
            {

            }

            public void OnAdHidden(IMaxAd ad)
            {
                // rewarded ad is hidden. Pre-load the next ad

            }

            public void OnAdLoadFailed(string adUnitId, IMaxError error)
            {
                try
                {
                    // Rewarded ad failed to load 
                    // We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds)

                    RetryAttempt++;
                    long delayMillis = TimeUnit.Seconds.ToMillis((long)Math.Pow(2, Math.Min(6, RetryAttempt)));
                    new Handler(Looper.MainLooper).PostDelayed(() =>
                    {
                        RewardedAd?.LoadAd();
                    }, delayMillis);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public void OnAdLoaded(IMaxAd ad)
            {
                try
                {
                    // Rewarded ad is ready to be shown. Rewarded.isReady() will now return 'true'
                    //if (RewardedAd is { IsReady: true })
                    //    RewardedAd.ShowAd();

                    // Reset retry attempt
                    RetryAttempt = 0;
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public void OnRewardedVideoCompleted(IMaxAd ad)
            {

            }

            public void OnRewardedVideoStarted(IMaxAd ad)
            {

            }

            public void OnUserRewarded(IMaxAd ad, IMaxReward reward)
            {
                // Rewarded ad was displayed and user should receive the reward
                try
                {
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

        #endregion

        public static void Initialize(Activity context)
        {
            try
            {
                if (AppSettings.ShowAppLovinBannerAds || AppSettings.ShowAppLovinInterstitialAds || AppSettings.ShowAppLovinRewardAds)
                {
                    AppLovinPrivacySettings.SetHasUserConsent(true, context);
                    //AppLovinPrivacySettings.SetIsAgeRestrictedUser(true, context);
                    AppLovinPrivacySettings.SetDoNotSell(true, context);

                    var initConfigBuilder = AppLovinSdkInitializationConfiguration.Builder(context.GetText(Resource.String.applovin_key), context);
                    initConfigBuilder.SetMediationProvider(AppLovinMediationProvider.Max);

                    List<string> adUnitIds = new List<string>
                    {
                        AppSettings.AdsAppLovinBannerId,
                        AppSettings.AdsAppLovinInterstitialId,
                        AppSettings.AdsAppLovinRewardedId
                    };
                    initConfigBuilder.SetAdUnitIds(adUnitIds);

                    var ad = AppLovinSdk.GetInstance(context);
                    if (ad != null)
                    {
                        //ad.Settings.TestDeviceAdvertisingIds = new List<string>() { Methods.GetAdvertisingId(context) };
                        ad.Settings.SetVerboseLogging(true);
                        ad.Settings.Muted = true;

                        //ad.ShowMediationDebugger();  

                        ad.Initialize(initConfigBuilder.Build(), new MyAppLovinSdkInitialization(context));
                    }

                    AppLovinPrivacySettings.SetHasUserConsent(true, context);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private class MyAppLovinSdkInitialization : Object, AppLovinSdk.ISdkInitializationListener
        {
            private readonly Activity Activity;

            public MyAppLovinSdkInitialization(Activity context)
            {
                Activity = context;
            }

            public void OnSdkInitialized(IAppLovinSdkConfiguration p0)
            {
                try
                {
                    // AppLovin SDK is initialized, start loading ads now or later if ad gate is reached

                    var instance = AppLovinSdk.GetInstance(Activity);
                    instance?.AdService?.LoadNextAd(AppLovinAdSize.Interstitial, new MyLoadNextListener(Activity));
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            private class MyLoadNextListener : Object, IAppLovinAdLoadListener, IAppLovinAdDisplayListener, IAppLovinAdClickListener, IAppLovinAdVideoPlaybackListener
            {
                private readonly Activity Context;
                public MyLoadNextListener(Activity context)
                {
                    Context = context;
                }

                public void AdReceived(IAppLovinAd ad)
                {
                    try
                    {
                        if (AppSettings.ShowAppLovinInterstitialAds)
                        {
                            IAppLovinInterstitialAdDialog interstitialAd = AppLovinInterstitialAd.Create(AppLovinSdk.GetInstance(Context), Context);

                            // Optional: Assign listeners
                            interstitialAd.SetAdDisplayListener(this);
                            interstitialAd.SetAdClickListener(this);
                            interstitialAd.SetAdVideoPlaybackListener(this);

                            interstitialAd.ShowAndRender(ad);
                        }
                    }
                    catch (Exception exception)
                    {
                        Methods.DisplayReportResultTrack(exception);
                    }
                }

                public void FailedToReceiveAd(int p0)
                {

                    // Look at AppLovinErrorCodes.java for list of error codes.

                }


                public void AdDisplayed(IAppLovinAd p0)
                {

                }

                public void AdHidden(IAppLovinAd p0)
                {

                }

                public void AdClicked(IAppLovinAd p0)
                {

                }

                public void VideoPlaybackBegan(IAppLovinAd p0)
                {

                }

                public void VideoPlaybackEnded(IAppLovinAd p0, double p1, bool p2)
                {

                }
            }

        }
    }
}