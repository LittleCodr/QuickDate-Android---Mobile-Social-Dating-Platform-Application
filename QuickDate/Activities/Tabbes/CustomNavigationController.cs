using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Com.Airbnb.Lottie;
using Q.Rorbin.Badgeview;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using System;
using System.Collections.Generic;
using FragmentManager = AndroidX.Fragment.App.FragmentManager;
using FragmentTransaction = AndroidX.Fragment.App.FragmentTransaction;

namespace QuickDate.Activities.Tabbes
{
    public class CustomNavigationController : Java.Lang.Object, View.IOnClickListener
    {
        private readonly Activity MainContext;

        private FrameLayout NotificationButton;
        private LinearLayout MainLayout;
        private LinearLayout HomeButton, ProfileButton, TrendButton, MessagesButton;
        public ImageView HomeImage, NotificationImage, ProfileImage, TrendImage, MessagesImage;
        private int PageNumber;


        public readonly List<AndroidX.Fragment.App.Fragment> FragmentListTab0 = new();
        public readonly List<AndroidX.Fragment.App.Fragment> FragmentListTab1 = new();
        public readonly List<AndroidX.Fragment.App.Fragment> FragmentListTab2 = new();
        public readonly List<AndroidX.Fragment.App.Fragment> FragmentListTab3 = new();
        public readonly List<AndroidX.Fragment.App.Fragment> FragmentListTab4 = new();

        private readonly HomeActivity Context;

        public CustomNavigationController(Activity activity)
        {
            try
            {
                MainContext = activity;

                if (activity is HomeActivity cont)
                    Context = cont;

                Initialize();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void Initialize()
        {
            try
            {
                MainLayout = MainContext.FindViewById<LinearLayout>(Resource.Id.llMain);

                HomeButton = MainContext.FindViewById<LinearLayout>(Resource.Id.llHome);
                NotificationButton = MainContext.FindViewById<FrameLayout>(Resource.Id.llNotification);
                ProfileButton = MainContext.FindViewById<LinearLayout>(Resource.Id.llProfile);
                TrendButton = MainContext.FindViewById<LinearLayout>(Resource.Id.llTrend);
                MessagesButton = MainContext.FindViewById<LinearLayout>(Resource.Id.llMessages);

                HomeImage = MainContext.FindViewById<ImageView>(Resource.Id.ivHome);
                NotificationImage = MainContext.FindViewById<ImageView>(Resource.Id.ivNotification);
                ProfileImage = MainContext.FindViewById<ImageView>(Resource.Id.ivProfile);
                TrendImage = MainContext.FindViewById<ImageView>(Resource.Id.ivTrend);
                MessagesImage = MainContext.FindViewById<ImageView>(Resource.Id.ivMessages);

                if (!AppSettings.ShowTrending)
                {
                    TrendButton.Visibility = ViewStates.Gone;
                    MainLayout.WeightSum = 4;
                }

                HomeButton.SetOnClickListener(this);
                TrendButton.SetOnClickListener(this);
                NotificationButton.SetOnClickListener(this);
                ProfileButton.SetOnClickListener(this);
                MessagesButton.SetOnClickListener(this);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnClick(View v)
        {
            try
            {
                switch (v.Id)
                {
                    case Resource.Id.llHome:
                        PageNumber = 0;
                        EnableNavigationButton(HomeImage, MainContext);
                        ShowFragment0();
                        Context?.TracksCounter?.CheckTracksCounter();
                        break;

                    case Resource.Id.llTrend:
                        PageNumber = 1;
                        EnableNavigationButton(TrendImage, MainContext);
                        ShowFragment1();
                        Context?.TracksCounter?.CheckTracksCounter();
                        AdsGoogle.Ad_AppOpenManager(Context);

                        Context?.InAppReview();

                        break;

                    case Resource.Id.llNotification:
                        PageNumber = 2;
                        EnableNavigationButton(NotificationImage, MainContext);
                        ShowNotificationBadge(false);
                        ShowFragment2();
                        Context?.TracksCounter?.CheckTracksCounter();
                        AdsGoogle.Ad_Interstitial(Context);
                        break;

                    case Resource.Id.llMessages:
                        PageNumber = 3;
                        EnableNavigationButton(MessagesImage, MainContext);
                        //NavigationTabBar.Model tabMessages = Models.FirstOrDefault(a => a.Title == Context.GetText(Resource.String.Lbl_messages));
                        HomeActivity.CountMessagesStatic = 0;
                        ShowMessagesBadge(false, 0);
                        ShowFragment3();
                        break;
                    case Resource.Id.llProfile:
                        PageNumber = 4;
                        EnableNavigationButton(ProfileImage, MainContext);
                        ShowFragment4();
                        AdsGoogle.Ad_RewardedInterstitial(Context);
                        break;

                    default:
                        PageNumber = 0;
                        EnableNavigationButton(HomeImage, MainContext);
                        ShowFragment0();
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void EnableNavigationButton(ImageView image, Activity activity)
        {
            DisableAllNavigationButton();
            image.Background = MainContext.GetDrawable(Resource.Drawable.shape_bg_bottom_navigation);
            if (image.Id == ProfileImage.Id)
            {
                ApiRequest.GetInfoData(MainContext, UserDetails.UserId.ToString()).ConfigureAwait(false);
                return;
            }
            image.SetColorFilter(Color.ParseColor(AppSettings.MainColor));

        }

        public void DisableAllNavigationButton()
        {
            HomeImage.Background = null;
            HomeImage.SetColorFilter(QuickDateTools.IsTabDark() ? Color.White : Color.ParseColor("#4D4D4D"));

            NotificationImage.Background = null;
            NotificationImage.SetColorFilter(QuickDateTools.IsTabDark() ? Color.White : Color.ParseColor("#4D4D4D"));

            ProfileImage.Background = null;
            //ProfileImage.SetColorFilter(QuickDateTools.IsTabDark() ? Color.White : Color.ParseColor("#4D4D4D"));

            TrendImage.Background = null;
            TrendImage.SetColorFilter(QuickDateTools.IsTabDark() ? Color.White : Color.ParseColor("#4D4D4D"));

            MessagesImage.Background = null;
            MessagesImage.SetColorFilter(QuickDateTools.IsTabDark() ? Color.White : Color.ParseColor("#4D4D4D"));

        }

        public void ShowNotificationBadge(bool showBadge)
        {
            try
            {
                LottieAnimationView animationView2 = MainContext.FindViewById<LottieAnimationView>(Resource.Id.animation_view2);

                if (showBadge)
                {
                    NotificationImage.SetImageDrawable(null);

                    animationView2.SetAnimation("NotificationLotti.json");
                    animationView2.PlayAnimation();
                }
                else
                {
                    animationView2.Progress = 0;
                    animationView2.CancelAnimation();
                    NotificationImage.SetImageResource(Resource.Drawable.icon_notification_vector);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private QBadgeView BadgeMessages;

        public void ShowMessagesBadge(bool showBadge, int count)
        {
            try
            {
                if (showBadge && count > 0)
                {
                    BadgeMessages = new QBadgeView(Context);
                    int gravity = (int)(GravityFlags.End | GravityFlags.Top);
                    BadgeMessages.BindTarget(MessagesImage);
                    BadgeMessages.SetBadgeNumber(count);
                    BadgeMessages.SetBadgeGravity(gravity);
                    BadgeMessages.SetBadgeBackgroundColor(Color.ParseColor(AppSettings.MainColor));
                    BadgeMessages.SetGravityOffset(0, true);
                }
                else
                {
                    BadgeMessages?.BindTarget(MessagesImage).Hide(true);
                    HomeActivity.CountMessagesStatic = 0;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public AndroidX.Fragment.App.Fragment GetSelectedTabBackStackFragment()
        {
            switch (PageNumber)
            {
                case 0:
                    {
                        var currentFragment = FragmentListTab0[FragmentListTab0.Count - 2];
                        if (currentFragment != null)
                            return currentFragment;
                        break;
                    }
                case 1:
                    {
                        var currentFragment = FragmentListTab1[FragmentListTab1.Count - 2];
                        if (currentFragment != null)
                            return currentFragment;
                        break;
                    }
                case 2:
                    {
                        var currentFragment = FragmentListTab2[FragmentListTab2.Count - 2];
                        if (currentFragment != null)
                            return currentFragment;
                        break;
                    }
                case 3:
                    {
                        var currentFragment = FragmentListTab3[FragmentListTab3.Count - 2];
                        if (currentFragment != null)
                            return currentFragment;
                        break;
                    }
                case 4:
                    {
                        var currentFragment = FragmentListTab4[FragmentListTab4.Count - 2];
                        if (currentFragment != null)
                            return currentFragment;
                        break;
                    }

                default:
                    return null;

            }

            return null;
        }

        public int GetCountFragment()
        {
            try
            {
                switch (PageNumber)
                {
                    case 0:
                        return FragmentListTab0.Count > 1 ? FragmentListTab0.Count : 0;
                    case 1:
                        return FragmentListTab1.Count > 1 ? FragmentListTab1.Count : 0;
                    case 2:
                        return FragmentListTab2.Count > 1 ? FragmentListTab2.Count : 0;
                    case 3:
                        return FragmentListTab3.Count > 1 ? FragmentListTab3.Count : 0;
                    case 4:
                        return FragmentListTab4.Count > 1 ? FragmentListTab4.Count : 0;
                    default:
                        return 0;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return 0;
            }
        }

        public static void HideFragmentFromList(List<AndroidX.Fragment.App.Fragment> fragmentList, FragmentTransaction ft)
        {
            try
            {
                if (fragmentList.Count < 0)
                    return;

                foreach (var fra in fragmentList)
                {
                    if (fra.IsVisible)
                        ft.Hide(fra);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void DisplayFragment(AndroidX.Fragment.App.Fragment newFragment)
        {
            try
            {
                FragmentTransaction ft = Context.SupportFragmentManager.BeginTransaction();

                HideFragmentFromList(FragmentListTab0, ft);
                HideFragmentFromList(FragmentListTab1, ft);
                HideFragmentFromList(FragmentListTab2, ft);
                HideFragmentFromList(FragmentListTab3, ft);
                HideFragmentFromList(FragmentListTab4, ft);

                switch (PageNumber)
                {
                    case 0:
                        {
                            if (!FragmentListTab0.Contains(newFragment))
                                FragmentListTab0.Add(newFragment);
                            break;
                        }
                    case 1:
                        {
                            if (!FragmentListTab1.Contains(newFragment))
                                FragmentListTab1.Add(newFragment);
                            break;
                        }
                    case 2:
                        {
                            if (!FragmentListTab2.Contains(newFragment))
                                FragmentListTab2.Add(newFragment);
                            break;
                        }
                    case 3:
                        {
                            if (!FragmentListTab3.Contains(newFragment))
                                FragmentListTab3.Add(newFragment);
                            break;
                        }
                    case 4:
                        {
                            if (!FragmentListTab4.Contains(newFragment))
                                FragmentListTab4.Add(newFragment);
                            break;
                        }
                }

                if (!newFragment.IsAdded)
                    ft.Add(Resource.Id.content, newFragment, newFragment.Id.ToString());

                //ft.SetCustomAnimations(Resource.Animation.fab_slide_in_from_right, Resource.Animation.fab_slide_in_from_left);
                //ft.Show(newFragment).AddToBackStack(null)?.Commit();
                ft.Show(newFragment)?.Commit();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void RemoveFragment(AndroidX.Fragment.App.Fragment oldFragment)
        {
            try
            {
                FragmentTransaction ft = Context.SupportFragmentManager.BeginTransaction();

                switch (PageNumber)
                {
                    case 0:
                        {
                            if (FragmentListTab0.Contains(oldFragment))
                                FragmentListTab0.Remove(oldFragment);
                            break;
                        }
                    case 1:
                        {
                            if (FragmentListTab1.Contains(oldFragment))
                                FragmentListTab1.Remove(oldFragment);
                            break;
                        }
                    case 2:
                        {
                            if (FragmentListTab2.Contains(oldFragment))
                                FragmentListTab2.Remove(oldFragment);
                            break;
                        }
                    case 3:
                        {
                            if (FragmentListTab3.Contains(oldFragment))
                                FragmentListTab3.Remove(oldFragment);
                            break;
                        }
                    case 4:
                        {
                            if (FragmentListTab4.Contains(oldFragment))
                                FragmentListTab4.Remove(oldFragment);
                            break;
                        }
                }


                HideFragmentFromList(FragmentListTab0, ft);
                HideFragmentFromList(FragmentListTab1, ft);
                HideFragmentFromList(FragmentListTab2, ft);
                HideFragmentFromList(FragmentListTab3, ft);
                HideFragmentFromList(FragmentListTab4, ft);

                if (oldFragment.IsAdded)
                    ft.Remove(oldFragment);

                switch (PageNumber)
                {
                    case 0:
                        {
                            var currentFragment = FragmentListTab0[FragmentListTab0.Count - 1];
                            ft.Show(currentFragment)?.Commit();
                            break;
                        }
                    case 1:
                        {
                            var currentFragment = FragmentListTab1[FragmentListTab1.Count - 1];
                            ft.Show(currentFragment)?.Commit();
                            break;
                        }
                    case 2:
                        {
                            var currentFragment = FragmentListTab2[FragmentListTab2.Count - 1];
                            ft.Show(currentFragment)?.Commit();
                            break;
                        }
                    case 3:
                        {
                            var currentFragment = FragmentListTab3[FragmentListTab3.Count - 1];
                            ft.Show(currentFragment)?.Commit();
                            break;
                        }
                    case 4:
                        {
                            var currentFragment = FragmentListTab4[FragmentListTab4.Count - 1];
                            ft.Show(currentFragment)?.Commit();
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnBackStackClickFragment()
        {
            try
            {
                switch (PageNumber)
                {
                    case 0 when FragmentListTab0.Count > 1:
                        {
                            var currentFragment = FragmentListTab0[FragmentListTab0.Count - 1];
                            if (currentFragment != null)
                                RemoveFragment(currentFragment);
                            break;
                        }
                    case 0:
                        Context.Finish();
                        break;
                    case 1 when FragmentListTab1.Count > 1:
                        {
                            var currentFragment = FragmentListTab1[FragmentListTab1.Count - 1];
                            if (currentFragment != null)
                                RemoveFragment(currentFragment);
                            break;
                        }
                    case 1:
                        Context.Finish();
                        break;
                    case 2 when FragmentListTab2.Count > 1:
                        {
                            var currentFragment = FragmentListTab2[FragmentListTab2.Count - 1];
                            if (currentFragment != null)
                                RemoveFragment(currentFragment);
                            break;
                        }
                    case 2:
                        Context.Finish();
                        break;
                    case 3 when FragmentListTab3.Count > 1:
                        {
                            var currentFragment = FragmentListTab3[FragmentListTab3.Count - 1];
                            if (currentFragment != null)
                                RemoveFragment(currentFragment);
                            break;
                        }
                    case 3:
                        Context.Finish();
                        break;
                    case 4 when FragmentListTab4.Count > 1:
                        {
                            var currentFragment = FragmentListTab4[FragmentListTab4.Count - 1];
                            if (currentFragment != null)
                                RemoveFragment(currentFragment);
                            break;
                        }
                    case 4:
                        Context.Finish();
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void ShowFragment0()
        {
            try
            {
                if (FragmentListTab0.Count <= 0)
                    return;
                var currentFragment = FragmentListTab0[FragmentListTab0.Count - 1];
                if (currentFragment != null)
                    DisplayFragment(currentFragment);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void ShowFragment1()
        {
            try
            {
                if (FragmentListTab1.Count <= 0) return;
                var currentFragment = FragmentListTab1[FragmentListTab1.Count - 1];
                if (currentFragment != null)
                    DisplayFragment(currentFragment);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void ShowFragment2()
        {
            try
            {
                if (FragmentListTab2.Count <= 0) return;
                var currentFragment = FragmentListTab2[FragmentListTab2.Count - 1];
                if (currentFragment != null)
                    DisplayFragment(currentFragment);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void ShowFragment3()
        {
            try
            {
                if (FragmentListTab3.Count <= 0) return;
                var currentFragment = FragmentListTab3[FragmentListTab3.Count - 1];
                if (currentFragment != null)
                    DisplayFragment(currentFragment);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void ShowFragment4()
        {
            try
            {
                if (FragmentListTab4.Count <= 0) return;
                var currentFragment = FragmentListTab4[FragmentListTab4.Count - 1];
                if (currentFragment != null)
                    DisplayFragment(currentFragment);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public static bool BringFragmentToTop(AndroidX.Fragment.App.Fragment tobeshown, FragmentManager fragmentManager, List<AndroidX.Fragment.App.Fragment> videoFrameLayoutFragments)
        {
            if (tobeshown != null)
            {
                FragmentTransaction fragmentTransaction = fragmentManager.BeginTransaction();


                foreach (var f in fragmentManager.Fragments)
                {
                    if (videoFrameLayoutFragments.Contains(f))
                    {
                        if (f == tobeshown)
                            fragmentTransaction.Show(f);
                        else
                            fragmentTransaction.Hide(f);
                    }

                }

                fragmentTransaction.Commit();

                return true;
            }
            else
            {
                FragmentTransaction fragmentTransaction = fragmentManager.BeginTransaction();

                foreach (var f in videoFrameLayoutFragments)
                {
                    fragmentTransaction.Hide(f);
                }

                fragmentTransaction.Commit();
            }

            return false;
        }
    }
}