using AndroidX.AppCompat.App;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using System;
using System.Linq;

namespace QuickDate.Helpers.Controller
{
    public class TracksCounter
    {
        private readonly AppCompatActivity ActivityContext;
        private readonly HomeActivity GlobalContext;

        private static int CountClick;
        public TracksCounterEnum LastCounterEnum;
        public AdsGoogle.AdMobRewardedVideo RewardedVideoAd;
        private static bool AddImageDialog;

        public enum TracksCounterEnum
        {
            AdsInterstitial,
            AdsRewardedVideo,
            AddImage,
            UpgradePremium,
            AddCredit,
            AddPhoneNumber,
        }

        public TracksCounter(AppCompatActivity activity)
        {
            try
            {
                ActivityContext = activity;
                GlobalContext = HomeActivity.GetInstance();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }


        public void CheckTracksCounter()
        {
            try
            {
                CountClick += 1;
                var lastAvatar = ListUtils.SettingsSiteList?.UserDefaultAvatar?.Split('/').Last() ?? "d-avatar";

                var dataUser = ListUtils.MyUserInfo?.FirstOrDefault();
                if (dataUser != null)
                {
                    switch (CountClick)
                    {
                        case 3 when UserDetails.Avatar.Contains(lastAvatar):
                            LastCounterEnum = TracksCounterEnum.AddImage;
                            GlobalContext?.OpenAddPhotoFragment();
                            break;
                        case 3:
                            {
                                LastCounterEnum = TracksCounterEnum.AdsInterstitial;
                                AdsGoogle.Ad_Interstitial(ActivityContext);
                                break;
                            }
                        case 7 when !AppSettings.EnableAppFree && (dataUser.Balance == "0.00" || dataUser.Balance == "0.0" || dataUser.Balance == "0") && LastCounterEnum != TracksCounterEnum.AddCredit:
                            {
                                LastCounterEnum = TracksCounterEnum.AddCredit;

                                var window = new PopupController(ActivityContext);
                                window.DisplayCreditWindow();
                                break;
                            }
                        case 7:
                            {
                                LastCounterEnum = TracksCounterEnum.AdsRewardedVideo;
                                AdsGoogle.Ad_RewardedVideo(ActivityContext);
                                break;
                            }
                        case 10 when !AppSettings.ValidationEnabled:
                            return;
                        case 10:
                            {
                                if (dataUser.PhoneVerified == "0" && dataUser.Verified == "0" && LastCounterEnum != TracksCounterEnum.AddPhoneNumber)
                                {
                                    LastCounterEnum = TracksCounterEnum.AddPhoneNumber;

                                    var window = new PopupController(ActivityContext);
                                    window.DisplayAddPhoneNumber();
                                }
                                else if (UserDetails.Avatar.Contains(lastAvatar) && LastCounterEnum != TracksCounterEnum.AddImage && !AddImageDialog)
                                {
                                    AddImageDialog = true;
                                    LastCounterEnum = TracksCounterEnum.AddImage;
                                    GlobalContext?.OpenAddPhotoFragment();
                                }
                                else if (dataUser.VerifiedFinal != null && !dataUser.VerifiedFinal.Value)
                                {
                                    if (!AppSettings.EnableAppFree)
                                    {
                                        LastCounterEnum = TracksCounterEnum.UpgradePremium;
                                        if (dataUser.IsPro == "0")
                                        {
                                            var window = new PopupController(ActivityContext);
                                            window.DisplayPremiumWindow();
                                        }
                                    }
                                    else
                                    {
                                        LastCounterEnum = TracksCounterEnum.AdsInterstitial;
                                        AdsGoogle.Ad_RewardedInterstitial(ActivityContext);
                                    }
                                }
                                else
                                {
                                    LastCounterEnum = TracksCounterEnum.AdsInterstitial;
                                    AdsGoogle.Ad_RewardedVideo(ActivityContext);
                                }

                                CountClick = 0;
                                break;
                            }
                    }
                }
                else
                {
                    AdsGoogle.Ad_Interstitial(ActivityContext);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

    }
}