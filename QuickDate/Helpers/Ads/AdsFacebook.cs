using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using Com.Facebook.Ads;
using Java.Util;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDateClient.Requests;
using Object = Java.Lang.Object;

namespace QuickDate.Helpers.Ads
{
    public static class AdsFacebook
    {
        private static int CountInterstitial;
        private static int CountRewarded;
        public static readonly List<NativeAd> MAdItems = new List<NativeAd>();
        public static NativeAdsManager MNativeAdsManager;

        #region Banner

        public static AdView InitAdView(Activity activity, LinearLayout adContainer, RecyclerView recyclerView)
        {
            try
            {
                if (QuickDateTools.GetStatusAds() && AppSettings.ShowFbBannerAds)
                {
                    AdView adView = new AdView(activity, AppSettings.AdsFbBannerKey, AdSize.BannerHeight50);
                    // Add the ad view to your activity layout
                    adContainer.AddView(adView);

                    // Request an ad
                    var build = adView.BuildLoadAdConfig()
                        ?.WithAdListener(new BannerAdListener(adContainer, recyclerView))
                        ?.Build();
                    adView.LoadAd(build);

                    return adView;
                }

                if (recyclerView != null)
                    Methods.SetMargin(recyclerView, 0, 0, 0, 0);
                return null;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }


        private class BannerAdListener : Object, IAdListener
        {
            private readonly RecyclerView MRecycler;
            private readonly LinearLayout AdContainer;
            public BannerAdListener(LinearLayout adContainer, RecyclerView recyclerView)
            {
                AdContainer = adContainer;
                MRecycler = recyclerView;
            }
            /// <summary>
            /// Ad clicked callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnAdClicked(IAd ad)
            {

            }

            /// <summary>
            /// Ad loaded callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnAdLoaded(IAd ad)
            {
                AdContainer.Visibility = ViewStates.Visible;
            }

            /// <summary>
            /// Ad error callback
            /// </summary>
            /// <param name="ad"></param>
            /// <param name="adError"></param>
            public void OnError(IAd ad, AdError adError)
            {
                try
                {
                    AdContainer.Visibility = ViewStates.Gone;
                    var error = adError.ErrorMessage;
                    Console.WriteLine(error);
                    if (MRecycler != null) Methods.SetMargin(MRecycler, 0, 0, 0, 0);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            /// <summary>
            /// Ad impression logged callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnLoggingImpression(IAd ad)
            {

            }
        }

        #endregion

        #region Interstitial

        public static InterstitialAd InitInterstitial(Activity activity)
        {
            try
            {
                if (QuickDateTools.GetStatusAds() && AppSettings.ShowFbInterstitialAds)
                {
                    if (CountInterstitial == AppSettings.ShowAdInterstitialCount)
                    {
                        CountInterstitial = 0;
                        var interstitialAd = new InterstitialAd(activity, AppSettings.AdsFbInterstitialKey);

                        var build = interstitialAd.BuildLoadAdConfig()
                            .WithAdListener(new MyInterstitialAdListener(activity, interstitialAd))
                            .WithCacheFlags(EnumSet.Of(CacheFlag.Video))
                            .Build();
                        interstitialAd.LoadAd(build);

                        return interstitialAd;
                    }

                    if (AppSettings.ShowAppLovinInterstitialAds)
                        AdsAppLovin.Ad_Interstitial(activity);

                    CountInterstitial++;
                }
                else
                {
                    if (AppSettings.ShowAppLovinInterstitialAds)
                        AdsAppLovin.Ad_Interstitial(activity);
                }
                return null;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        private class MyInterstitialAdListener : Object, IInterstitialAdListener, InterstitialAd.IInterstitialLoadAdConfig
        {
            private readonly InterstitialAd InterstitialAd;
            private readonly Activity Activity;
            public MyInterstitialAdListener(Activity activity, InterstitialAd interstitialAd)
            {
                Activity = activity;
                InterstitialAd = interstitialAd;
            }

            /// <summary>
            /// Ad clicked callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnAdClicked(IAd ad)
            {

            }

            /// <summary>
            /// Ad loaded callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnAdLoaded(IAd ad)
            {
                try
                {
                    // Show the ad
                    InterstitialAd?.Show();
                    Console.WriteLine(Activity);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            /// <summary>
            /// Ad error callback
            /// </summary>
            /// <param name="ad"></param>
            /// <param name="adError"></param>
            public void OnError(IAd ad, AdError adError)
            {
                try
                {
                    var error = adError.ErrorMessage;
                    Console.WriteLine(error);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            /// <summary>
            /// Ad impression logged callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnLoggingImpression(IAd ad)
            {

            }

            /// <summary>
            /// Interstitial dismissed callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnInterstitialDismissed(IAd ad)
            {

            }

            /// <summary>
            /// Interstitial ad displayed callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnInterstitialDisplayed(IAd ad)
            {

            }
        }

        #endregion

        #region RewardVideo

        public static RewardedVideoAd InitRewardVideo(Activity activity)
        {
            try
            {
                if (QuickDateTools.GetStatusAds() && AppSettings.ShowFbRewardVideoAds && CountRewarded == AppSettings.ShowAdRewardedVideoCount)
                {
                    CountRewarded = 0;

                    var rewardVideoAd = new RewardedVideoAd(activity, AppSettings.AdsFbRewardVideoKey);

                    // Create the rewarded ad data
                    RewardData rewardData = new RewardData(UserDetails.UserId.ToString(), "200");

                    var build = rewardVideoAd.BuildLoadAdConfig()
                        ?.WithAdListener(new MyRRewardVideoAdListener(activity, rewardVideoAd))
                        ?.WithRewardData(rewardData)
                        ?.WithFailOnCacheFailureEnabled(true)
                        ?.Build();
                    rewardVideoAd.LoadAd(build);

                    //rewardVideoAd.SetRewardData(new RewardData("YOUR_USER_ID", "YOUR_REWARD"));
                    return rewardVideoAd;
                }

                if (AppSettings.ShowAppLovinRewardAds)
                    AdsAppLovin.Ad_Rewarded(activity);
                CountRewarded++;

                return null;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        private class MyRRewardVideoAdListener : Object, IS2SRewardedVideoAdListener
        {
            private readonly RewardedVideoAd RewardVideoAd;
            private readonly Activity Activity;
            public MyRRewardVideoAdListener(Activity activity, RewardedVideoAd rewardVideoAd)
            {
                Activity = activity;
                RewardVideoAd = rewardVideoAd;
                Console.WriteLine(Activity);
            }

            /// <summary>
            /// Ad clicked callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnAdClicked(IAd ad)
            {

            }

            /// <summary>
            /// Ad loaded callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnAdLoaded(IAd ad)
            {
                try
                {
                    // Show the ad
                    RewardVideoAd?.Show();
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            /// <summary>
            /// Ad error callback
            /// </summary>
            /// <param name="ad"></param>
            /// <param name="adError"></param>
            public void OnError(IAd ad, AdError adError)
            {
                try
                {
                    var error = adError.ErrorMessage;
                    Console.WriteLine(error);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            /// <summary>
            /// Ad impression logged callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnLoggingImpression(IAd ad)
            {

            }

            /// <summary>
            /// Rewarded Video Closed
            /// </summary>
            public void OnRewardedVideoClosed()
            {

            }

            /// <summary>
            /// Rewarded Video View Complete
            /// </summary>
            public void OnRewardedVideoCompleted()
            {
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

            /// <summary>
            /// Reward Video Server Failed
            /// </summary>
            public void OnRewardServerFailed()
            {

            }

            /// <summary>
            /// Reward Video Server Succeeded
            /// </summary>
            public void OnRewardServerSuccess()
            {

            }
        }

        #endregion

        #region Native

        public static void InitNative(Activity activity, LinearLayout nativeAdLayout, NativeAd ad)
        {
            try
            {
                if (QuickDateTools.GetStatusAds() && AppSettings.ShowFbNativeAds)
                {
                    if (ad == null)
                    {
                        var nativeAd = new NativeAd(activity, AppSettings.AdsFbNativeKey);

                        // Initiate a request to load an ad. 
                        var build = nativeAd.BuildLoadAdConfig()
                            .WithAdListener(new NativeAdListener(activity, nativeAd, nativeAdLayout))
                            .Build();
                        nativeAd.LoadAd(build);
                    }
                    else
                    {
                        var holder = new AdHolder(nativeAdLayout);
                        LoadAd(activity, holder, ad, nativeAdLayout);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }


        private class NativeAdListener : Object, INativeAdListener
        {
            private readonly NativeAd NativeAd;
            private readonly LinearLayout NativeAdLayout;
            private readonly Activity Activity;
            public NativeAdListener(Activity activity, NativeAd nativeAd, LinearLayout nativeAdLayout)
            {
                Activity = activity;
                NativeAd = nativeAd;
                NativeAdLayout = nativeAdLayout;
            }

            /// <summary>
            /// Ad clicked callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnAdClicked(IAd ad)
            {

            }

            /// <summary>
            /// Ad loaded callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnAdLoaded(IAd ad)
            {
                try
                {
                    if (NativeAd == null || NativeAd != ad)
                    {
                        // Race condition, load() called again before last ad was displayed
                        return;
                    }

                    switch (NativeAdLayout)
                    {
                        case null:
                            return;
                        default:
                            Activity?.RunOnUiThread(() =>
                            {
                                try
                                {
                                    NativeAdLayout.Visibility = ViewStates.Visible;

                                    // Unregister last ad
                                    NativeAd.UnregisterView();

                                    //if (NativeAdChoicesContainer != null)
                                    //{
                                    //    //var adOptionsView = new NativeAdView(Activity, NativeAd, NativeAdLayout);
                                    //    NativeAdChoicesContainer.RemoveAllViews();
                                    //    NativeAdChoicesContainer.AddView(NativeAdLayout, 0);
                                    //}

                                    InflateAd(NativeAd, NativeAdLayout);
                                }
                                catch (Exception e)
                                {
                                    Methods.DisplayReportResultTrack(e);
                                }
                            });
                            break;
                    }
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            /// <summary>
            /// Ad error callback
            /// </summary>
            /// <param name="ad"></param>
            /// <param name="adError"></param>
            public void OnError(IAd ad, AdError adError)
            {
                try
                {
                    var error = adError.ErrorMessage;
                    Console.WriteLine(error);

                    if (NativeAdLayout != null)
                        NativeAdLayout.Visibility = ViewStates.Gone;
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            /// <summary>
            /// Ad impression logged callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnLoggingImpression(IAd ad)
            {

            }

            /// <summary>
            /// on Media Downloaded
            /// </summary>
            /// <param name="p0"></param>
            public void OnMediaDownloaded(IAd p0)
            {

            }

            //private LinearLayout NativeAdChoicesContainer;

            private void InflateAd(NativeAd nativeAd, View adView)
            {
                try
                {
                    // Create native UI using the ad metadata.
                    var holder = new AdHolder(adView);
                    //NativeAdChoicesContainer = holder.NativeAdChoicesContainer;

                    // Setting the Text
                    holder.NativeAdSocialContext.Text = nativeAd.AdSocialContext;
                    holder.NativeAdCallToAction.Text = nativeAd.AdCallToAction;
                    holder.NativeAdCallToAction.Visibility = nativeAd.HasCallToAction ? ViewStates.Visible : ViewStates.Invisible;
                    holder.NativeAdTitle.Text = nativeAd.AdvertiserName;
                    holder.NativeAdBody.Text = nativeAd.AdBodyText;
                    holder.SponsoredLabel.Text = Activity.GetText(Resource.String.sponsored);

                    // You can use the following to specify the clickable areas.
                    List<View> clickableViews = new List<View> { holder.NativeAdIcon, holder.NativeAdMedia, holder.NativeAdCallToAction };

                    nativeAd.RegisterViewForInteraction(NativeAdLayout, holder.NativeAdMedia, holder.NativeAdIcon, clickableViews);

                    // Optional: tag views
                    NativeAdBase.NativeComponentTag.TagView(holder.NativeAdIcon, NativeAdBase.NativeComponentTag.AdIcon);
                    NativeAdBase.NativeComponentTag.TagView(holder.NativeAdTitle, NativeAdBase.NativeComponentTag.AdTitle);
                    NativeAdBase.NativeComponentTag.TagView(holder.NativeAdBody, NativeAdBase.NativeComponentTag.AdBody);
                    NativeAdBase.NativeComponentTag.TagView(holder.NativeAdSocialContext, NativeAdBase.NativeComponentTag.AdSocialContext);
                    NativeAdBase.NativeComponentTag.TagView(holder.NativeAdCallToAction, NativeAdBase.NativeComponentTag.AdCallToAction);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public class MediaViewListener : Object, IMediaViewListener
            {
                public void OnComplete(MediaView p0)
                {

                }

                public void OnEnterFullscreen(MediaView p0)
                {

                }

                public void OnExitFullscreen(MediaView p0)
                {

                }

                public void OnFullscreenBackground(MediaView p0)
                {

                }

                public void OnFullscreenForeground(MediaView p0)
                {

                }

                public void OnPause(MediaView p0)
                {

                }

                public void OnPlay(MediaView p0)
                {

                }

                public void OnVolumeChange(MediaView p0, float p1)
                {

                }
            }
        }

        private static void LoadAd(Activity activity, AdHolder holder, NativeAd nativeAd, LinearLayout adView)
        {
            try
            {
                adView.Visibility = ViewStates.Visible;

                //if (holder.NativeAdChoicesContainer != null)
                //{
                //    //var adOptionsView = new AdOptionsView(activity, nativeAd, adView);
                //    //holder.NativeAdChoicesContainer.RemoveAllViews();
                //    //holder.NativeAdChoicesContainer.AddView(adView, 0);
                //}

                // Setting the Text
                holder.NativeAdSocialContext.Text = nativeAd.AdSocialContext;
                holder.NativeAdCallToAction.Text = nativeAd.AdCallToAction;
                holder.NativeAdCallToAction.Visibility = nativeAd.HasCallToAction ? ViewStates.Visible : ViewStates.Invisible;
                holder.NativeAdTitle.Text = nativeAd.AdvertiserName;
                holder.NativeAdBody.Text = nativeAd.AdBodyText;
                holder.SponsoredLabel.Text = activity.GetText(Resource.String.sponsored);

                // You can use the following to specify the clickable areas.
                List<View> clickableViews = new List<View> { holder.NativeAdIcon, holder.NativeAdMedia, holder.NativeAdCallToAction };

                nativeAd.RegisterViewForInteraction(adView, holder.NativeAdMedia, holder.NativeAdIcon, clickableViews);

                // Optional: tag views
                NativeAdBase.NativeComponentTag.TagView(holder.NativeAdIcon, NativeAdBase.NativeComponentTag.AdIcon);
                NativeAdBase.NativeComponentTag.TagView(holder.NativeAdTitle, NativeAdBase.NativeComponentTag.AdTitle);
                NativeAdBase.NativeComponentTag.TagView(holder.NativeAdBody, NativeAdBase.NativeComponentTag.AdBody);
                NativeAdBase.NativeComponentTag.TagView(holder.NativeAdSocialContext, NativeAdBase.NativeComponentTag.AdSocialContext);
                NativeAdBase.NativeComponentTag.TagView(holder.NativeAdCallToAction, NativeAdBase.NativeComponentTag.AdCallToAction);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private class AdHolder : RecyclerView.ViewHolder
        {
            public View NativeAdLayout { get; private set; }
            public MediaView NativeAdMedia { get; private set; }
            public MediaView NativeAdIcon { get; private set; }
            public TextView NativeAdTitle { get; private set; }
            public TextView NativeAdBody { get; private set; }
            public TextView NativeAdSocialContext { get; private set; }
            public TextView SponsoredLabel { get; private set; }
            public AppCompatButton NativeAdCallToAction { get; private set; }
            public LinearLayout NativeAdChoicesContainer { get; private set; }

            public AdHolder(View adLayout) : base(adLayout)
            {
                try
                {
                    NativeAdLayout = adLayout;

                    NativeAdMedia = adLayout.FindViewById<MediaView>(Resource.Id.native_ad_media);
                    NativeAdTitle = adLayout.FindViewById<TextView>(Resource.Id.native_ad_title);
                    NativeAdBody = adLayout.FindViewById<TextView>(Resource.Id.native_ad_body);
                    NativeAdSocialContext = adLayout.FindViewById<TextView>(Resource.Id.native_ad_social_context);
                    SponsoredLabel = adLayout.FindViewById<TextView>(Resource.Id.native_ad_sponsored_label);
                    NativeAdCallToAction = adLayout.FindViewById<AppCompatButton>(Resource.Id.native_ad_call_to_action);
                    NativeAdIcon = adLayout.FindViewById<MediaView>(Resource.Id.native_ad_icon);
                    NativeAdChoicesContainer = adLayout.FindViewById<LinearLayout>(Resource.Id.ad_choices_container);

                    NativeAdMedia?.SetListener(new NativeAdListener.MediaViewListener());
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }
        }

        public static void BindAdFb(Context context)
        {
            try
            {
                switch (QuickDateTools.GetStatusAds() && AppSettings.ShowFbNativeAds)
                {
                    case true when MAdItems?.Count == 0:
                        MNativeAdsManager = new NativeAdsManager(context, AppSettings.AdsFbNativeKey, 5);
                        MNativeAdsManager.LoadAds();
                        MNativeAdsManager.SetListener(new MyNativeAdsManagerListener());
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion
    }

    public class MyNativeAdsManagerListener : Object, NativeAdsManager.IListener
    {
        public void OnAdError(AdError p0)
        {

        }

        public void OnAdsLoaded()
        {
            try
            {
                NativeAd ad = AdsFacebook.MNativeAdsManager?.NextNativeAd();
                if (ad != null)
                {
                    AdsFacebook.MAdItems.Add(ad);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }

    public class InitializeFacebook
    {
        public static void Initialize(Context context)
        {
            try
            {
                if (AppSettings.ShowFbBannerAds || AppSettings.ShowFbInterstitialAds || AppSettings.ShowFbRewardVideoAds)
                {
                    if (!AudienceNetworkAds.IsInitialized(context))
                    {
                        if (BuildConfig.Debug)
                        {
                            AdSettings.TurnOnSDKDebugger(context);
                        }
                        //AdSettings.AddTestDevice(UserDetails.AdvertisingId);
                        AdSettings.SetIntegrationErrorMode(AdSettings.IntegrationErrorMode.IntegrationErrorCrashDebugMode);

                        AudienceNetworkAds.BuildInitSettings(context)
                            ?.WithInitListener(new MyInitializationListener(context))
                            ?.Initialize();
                    }

                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private class MyInitializationListener : Object, AudienceNetworkAds.IInitListener
        {
            private readonly Context Context;
            public MyInitializationListener(Context context)
            {
                Context = context;
            }

            public void OnInitialized(AudienceNetworkAds.IInitResult result)
            {
                try
                {
                    Console.WriteLine("AudienceNetwork Ads Initialization : " + result.Message);
                    AdsFacebook.BindAdFb(Context);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }
        }
    }
}